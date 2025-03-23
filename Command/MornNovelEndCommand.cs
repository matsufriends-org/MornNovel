using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MornBeat;
using MornScene;
using MornTransition;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace MornNovel
{
    public class MornNovelEndCommand : MornNovelCommandBase
    {
        private enum NovelEndTransitionType
        {
            他シーンへ遷移,
            シーンを閉じる,
            ノベルを読み込む,
        }

        public override string Tips => "ノベルを終了する";
        [SerializeField, Label("BGMを止めるか")]
        private bool _isStopBgm = true;
        [SerializeField, Label("終了時の処理")]
        private NovelEndTransitionType _endTransitionType;
        [SerializeField, ShowIf(nameof(IsChangeSceneOrChangeNovel)), Label("遷移時のトランジション")]
        private MornTransitionType _transitionType;
        [SerializeField, ShowIf(nameof(IsChangeScene)), Label("遷移先のシーン")]
        private MornSceneObject _scene;
        [SerializeField, ShowIf(nameof(IsChangeNovel)), Label("遷移先のノベル")] 
        private MornNovelAddress _address;
        [Inject] private MornTransitionCtrl _transitionCtrl;
        [Inject] private MornBeatControllerMono _beatController;
        [Inject] private MornNovelSettings _settings;
        [Inject] private MornNovelService _novelManager;
        public bool IsChangeScene => _endTransitionType == NovelEndTransitionType.他シーンへ遷移;
        private bool IsCloseScene => _endTransitionType == NovelEndTransitionType.シーンを閉じる;
        private bool IsChangeNovel => _endTransitionType == NovelEndTransitionType.ノベルを読み込む;
        private bool IsChangeSceneOrChangeNovel => IsChangeScene || IsChangeNovel;

        public override async void OnStateBegin()
        {
            var novel = FindFirstObjectByType<MornNovelMono>();
            var controller = FindFirstObjectByType<MornNovelControllerMono>();
            novel.SetNovelRead();
            var taskList = new List<UniTask>();
            var ct = CancellationTokenOnEnd;
            if (_isStopBgm)
            {
                taskList.Add(_beatController.StopBeatAsync(_settings.BgmChangeSec, ct));
            }

            if (_novelManager.Debug) _endTransitionType = NovelEndTransitionType.シーンを閉じる;

            if (IsChangeScene || IsChangeNovel)
            {
                taskList.Add(_transitionCtrl.FillAsync(_transitionType, ct: ct));
            }

            if (IsCloseScene)
            {
                taskList.Add(controller.RemoveAllAsync(ct));
            }

            await UniTask.WhenAll(taskList);
            if (IsChangeScene)
            {
                SceneManager.LoadScene(_scene);
            }

            if (IsChangeNovel)
            {
                _novelManager.SetNovelAddress(_address);
                SceneManager.LoadScene(MornNovelGlobal.I.NovelScene);
            }

            if (IsCloseScene)
            {
                SceneManager.UnloadSceneAsync(gameObject.scene).WithCancellation(ct).Forget();
            }
            
            _novelManager.NovelEndOnNext();
        }
    }
}