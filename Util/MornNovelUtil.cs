using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MornNovel
{
    public static class MornNovelUtil
    {
#if UNITY_EDITOR

        private static bool? _showDescription;
        public static bool ShowDescription
        {
            get => _showDescription ??= EditorPrefs.GetBool($"{nameof(MornNovel)}_{nameof(ShowDescription)}", true);
            set
            {
                _showDescription = value;
                EditorPrefs.SetBool($"{nameof(MornNovel)}_{nameof(ShowDescription)}", value);
            }
        }

        [MenuItem("Tools/MornNovel/説明欄を表示")]
        private static void SetShowDescription()
        {
            ShowDescription = true;
        }

        [MenuItem("Tools/MornNovel/説明欄を非表示")]
        private static void SetHideDescription()
        {
            ShowDescription = false;
        }
#endif

        public async static UniTask DOText(string context, Action<string> setText,
            Func<(AudioClip clip, float interval)> getMessageClip, Action<AudioClip> playSe,
            Action<bool> showWaitInputIcon, bool isWaitInput, Func<bool> submitFunc, CancellationToken ct = default)
        {
            setText("");
            await UniTask.Delay(TimeSpan.FromSeconds(MornNovelGlobal.I.MessageOffset), cancellationToken: ct);
            var sb = new StringBuilder();
            var nextSeTime = 0f;
            foreach (var c in context)
            {
                sb.Append(c);
                setText(sb.ToString());
                if (Time.time >= nextSeTime)
                {
                    var (clip, interval) = getMessageClip();
                    if (clip != null)
                    {
                        playSe(clip);
                        nextSeTime = Time.time + interval;
                    }
                }

                var waitInterval = c == '\n' ? MornNovelGlobal.I.CharReturnInterval : MornNovelGlobal.I.CharInterval;
                if (await WaitSecondsReturnSkipped(waitInterval, submitFunc, ct))
                {
                    break;
                }
            }

            setText(context);
            if (isWaitInput)
            {
                showWaitInputIcon(true);
                while (!submitFunc())
                {
                    await UniTask.Yield(ct);
                }

                playSe(MornNovelGlobal.I.SubmitClip);
                // 次Fへ入力を渡さないために1F待機
                await UniTask.Yield(ct);
                showWaitInputIcon(false);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(MornNovelGlobal.I.MessageOffset), cancellationToken: ct);
        }

        private async static UniTask<bool> WaitSecondsReturnSkipped(float seconds, Func<bool> submitFunc,
            CancellationToken ct = default)
        {
            var elapsedTime = 0f;
            while (elapsedTime < seconds)
            {
                elapsedTime += Time.deltaTime;
                if (submitFunc())
                {
                    // 次Fへ入力を渡さないために1F待機
                    await UniTask.Yield(ct);
                    return true;
                }

                await UniTask.Yield(ct);
            }

            return false;
        }

        public async static UniTask DOLocalMove(this Transform target, Vector3 endValue, float duration,
            CancellationToken ct = default)
        {
            if (target != null)
            {
                await DOAsync(
                    target.localPosition,
                    endValue,
                    duration,
                    Vector3.Lerp,
                    x => target.localPosition = x,
                    ct);
            }
        }

        public async static UniTask DOLocalMoveX(this Transform target, float endValue, float duration,
            CancellationToken ct = default)
        {
            if (target != null)
            {
                await DOAsync(target.localPosition.x, endValue, duration, Mathf.Lerp, target.SetLocalX, ct);
            }
        }

        public async static UniTask DOLocalMoveY(this Transform target, float endValue, float duration,
            CancellationToken ct = default)
        {
            if (target != null)
            {
                await DOAsync(target.localPosition.y, endValue, duration, Mathf.Lerp, target.SetLocalY, ct);
            }
        }

        public async static UniTask DOFade(this CanvasGroup target, float endValue, float duration,
            CancellationToken ct = default)
        {
            if (target != null)
            {
                await DOAsync(target.alpha, endValue, duration, Mathf.Lerp, x => target.alpha = x, ct);
            }
        }

        public async static UniTask DOFade(this Image target, float endValue, float duration,
            CancellationToken ct = default)
        {
            if (target)
            {
                await DOAsync(target.color.a, endValue, duration, Mathf.Lerp, x => SetAlpha(target, x), ct);
            }
        }

        public static async UniTask DoMaterialFloat(this Image target, string propertyName, float endValue,
            float duration, CancellationToken ct = default)
        {
            if (target)
                await DOAsync(
                    target.material.GetFloat(propertyName),
                    endValue,
                    duration,
                    Mathf.Lerp,
                    x => target.material.SetFloat(propertyName, x),
                    ct);
        }

        public async static UniTask DOFade(this SpriteRenderer target, float endValue, float duration,
            CancellationToken ct = default)
        {
            if (target)
            {
                await DOAsync(target.color.a, endValue, duration, Mathf.Lerp, x => SetAlpha(target, x), ct);
            }
        }

        private async static UniTask DOAsync<T>(T startValue, T endValue, float duration, Func<T, T, float, T> rateFunc,
            Action<T> onUpdateValue, CancellationToken ct = default)
        {
            // TODO Easingを設定できるように
            var elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                var rate = elapsedTime / duration;
                // OutQuad
                rate = 1 - (1 - rate) * (1 - rate);

                // EaseOutBack
                // const float c1 = 1.70158f;
                // const float c3 = c1 + 1;
                // rate -= 1;
                // rate = rate * rate * ((c3 + 1) * rate + c1) + 1;
                var value = rateFunc(startValue, endValue, rate);
                onUpdateValue.Invoke(value);
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(cancellationToken: ct);
            }

            onUpdateValue.Invoke(endValue);
        }

        public static void SetAlpha(this Image target, float alpha)
        {
            var color = target.color;
            color.a = alpha;
            target.color = color;
        }

        public static void SetAlpha(this SpriteRenderer target, float alpha)
        {
            var color = target.color;
            color.a = alpha;
            target.color = color;
        }

        public static void SetLocalX(this Transform target, float value)
        {
            var pos = target.localPosition;
            pos.x = value;
            target.localPosition = pos;
        }

        public static void SetLocalY(this Transform target, float value)
        {
            var pos = target.localPosition;
            pos.y = value;
            target.localPosition = pos;
        }
    }
}