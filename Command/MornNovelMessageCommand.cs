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
            try
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

                await MornNovelUtil.DOText(
                    GetText(),
                    controller.SetMessage,
                    () =>
                    {
                        var clip = _talker.Clips[UnityEngine.Random.Range(0, _talker.Clips.Length)];
                        return (clip, _talker.ClipLength);
                    },
                    _novelController.PlayOneShot,
                    controller.SetWaitInputIcon,
                    true,
                    () => _novelManager.Input(),
                    ct);
                Transition(_stateLink);
            }
            catch (OperationCanceledException)
            {
            }
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