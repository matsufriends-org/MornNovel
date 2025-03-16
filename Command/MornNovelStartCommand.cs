using System.Collections.Generic;
using Arbor;
using Cysharp.Threading.Tasks;
using MornBeat;
using MornTransition;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace MornNovel
{
    public class MornNovelStartCommand : MornNovelCommandBase
    {
        public override string Tips => "ノベルを開始する";
        [SerializeField]
        [Label("BGM")] private MornBeatMemoSo _beatMemo;
        [SerializeField] private StateLink _nextState;
        [Inject] private MornNovelSettings _settings;
        [Inject] private MornTransitionCtrl _transitionCtrl;
        [Inject] private MornBeatControllerMono _beatController;

        public override async void OnStateBegin()
        {
            // 自分以外のシーンが存在したらエリアノベル判定
            if (SceneManager.sceneCount > 1)
            {
                Transition(_nextState);
                return;
            }

            var list = new List<UniTask>();
            var ct = CancellationTokenOnEnd;
            if (_beatMemo != null)
            {
                list.Add(_beatController.StartAsync(
                    new MornBeatStartInfo()
                    {
                        BeatMemo = _beatMemo,
                        FadeDuration = _settings.BgmChangeSec,
                        Ct = ct,
                    }));
            }
            else
            {
                list.Add(_beatController.StopBeatAsync(_settings.BgmChangeSec, ct));
            }

            list.Add(_transitionCtrl.ClearAsync(ct: ct));
            await UniTask.WhenAll(list).SuppressCancellationThrow();
            Transition(_nextState);
        }
    }
}