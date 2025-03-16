using System;
using System.Text;
using System.Threading;
using Arbor;
using Cysharp.Threading.Tasks;
using MornLocalize;
using UnityEngine;
using VContainer;

namespace MornNovel
{
    [Serializable]
    public class MornNovelMessageCommand : MornNovelCommandBase
    {
        public override string Tips => "メッセージを表示する";
        [SerializeField] [ViewableSearch] private MornNovelTalkerSo _talker;
        [SerializeField] [ViewableSearch] private MornNovelBubbleSo _overrideBubble;
        [SerializeField] [Label("セリフ")] private MornLocalizeString _localizeString;
        [SerializeField] private StateLink _stateLink;
        [Inject] private MornLocalizeCore _localizeCore;
        [Inject] private MornNovelControllerMono _novelController;
        [Inject] private MornNovelSettings _novelSettings;
        [Inject] private MornNovelService _novelManager;
        public override Color? CommandColor => _talker == null ? null : _talker.CommandColor;

        public string GetText()
        {
            // TODO: ローカライズ
            return _localizeString.Get(_localizeCore?.CurrentLanguage ?? "jp");
        }

        public MornLocalizeString GetLocalizeString()
        {
            return _localizeString;
        }

        public string GetTalkerName()
        {
            return _talker.GetText(_localizeCore?.CurrentLanguage ?? "jp");
        }

        public override async void OnStateBegin()
        {
            var ct = CancellationTokenOnEnd;
            var controller = FindFirstObjectByType<MornNovelControllerMono>();
            controller.SetBubble(_overrideBubble, _talker);
            controller.SetWaitInputIcon(false);
            controller.SetMessage("");
            if (!_talker.IsMulti)
            {
                controller.SetFocus(_talker);
                controller.AllDecreaseOrderInLayer();
                controller.GetChara(_talker)?.ResetOrderInLayer();
            }
            else
            {
                controller.AllFocus();
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_novelSettings.Offset), cancellationToken: ct);
            var context = GetText();
            var sb = new StringBuilder();
            var nextSeTime = 0f;
            foreach (var c in context)
            {
                sb.Append(c);
                controller.SetMessage(sb.ToString());
                if (Time.time >= nextSeTime)
                {
                    if (_talker.Clips.Length > 0)
                    {
                        var clip = _talker.Clips[UnityEngine.Random.Range(0, _talker.Clips.Length)];
                        _novelController.PlayOneShot(clip);
                    }
                    nextSeTime = Time.time + _talker.ClipLength;
                }

                if (await WaitSecondsReturnSkipped(
                    c == '\n' ? _novelSettings.CharReturnInterval : _novelSettings.CharInterval,
                    CancellationTokenOnEnd))
                {
                    break;
                }
            }

            controller.SetMessage(context);
            controller.SetWaitInputIcon(true);
            while (!_novelManager.Input())
            {
                await UniTask.Yield(ct);
            }

            _novelController.PlayOneShot(_novelSettings.SubmitClip);
            // 次Fへ入力を渡さないために1F待機
            await UniTask.Yield(ct);
            controller.SetWaitInputIcon(false);
            await UniTask.Delay(TimeSpan.FromSeconds(_novelSettings.Offset), cancellationToken: ct);
            Transition(_stateLink);
        }

        private async UniTask<bool> WaitSecondsReturnSkipped(float seconds, CancellationToken ct = default)
        {
            var elapsedTime = 0f;
            while (elapsedTime < seconds)
            {
                elapsedTime += Time.deltaTime;
                if (_novelManager.Input())
                {
                    // 次Fへ入力を渡さないために1F待機
                    await UniTask.Yield(ct);
                    return true;
                }

                await UniTask.Yield(ct);
            }

            return false;
        }
    }
}