﻿using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MornBeat;
using MornScene;
using MornSound;
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
            ノベルシーンだけ消す,
            次のノベルをこのまま読み込む,
            次のノベルへトランジション,
        }

        public override string Tips => "ノベルを終了する";
        [SerializeField, Label("BGMを止めるか")]
        private bool _isStopBgm = true;
        [SerializeField, Label("終了時の処理")]
        private NovelEndTransitionType _endTransitionType;
        [SerializeField, ShowIf(nameof(IsNeedTransition)), Label("遷移時のトランジション")]
        private MornTransitionType _transitionType;
        [SerializeField, ShowIf(nameof(IsChangeScene)), Label("遷移先のシーン")]
        private MornSceneObject _scene;
        [SerializeField, ShowIf(nameof(IsChangeNovel)), Label("遷移先のノベル")] 
        private MornNovelAddress _address;
        [Inject] private MornTransitionCtrl _transitionCtrl;
        [Inject] private MornBeatControllerMono _beatController;
        [Inject] private MornNovelSettings _settings;
        [Inject] private MornNovelService _novelManager;
        [Inject] private MornSoundVolumeCore _volume;
        public bool IsChangeScene => _endTransitionType == NovelEndTransitionType.他シーンへ遷移;
        private bool IsCloseScene => _endTransitionType == NovelEndTransitionType.ノベルシーンだけ消す;
        private bool IsChangeNovel => _endTransitionType == NovelEndTransitionType.次のノベルをこのまま読み込む ||
                                     _endTransitionType == NovelEndTransitionType.次のノベルへトランジション;
        private bool IsNeedTransition => IsChangeScene || _endTransitionType == NovelEndTransitionType.次のノベルへトランジション;

        public override async void OnStateBegin()
        {
            var novel = FindFirstObjectByType<MornNovelMono>();
            var controller = FindFirstObjectByType<MornNovelControllerMono>();
            novel.SetNovelRead();
            if (!_novelManager.CurrentNovelAddress.IsNullOrEmpty())
            {
                _novelManager.SetNovelRead(_novelManager.CurrentNovelAddress);
            }
            var taskList = new List<UniTask>();
            var ct = CancellationTokenOnEnd;
            if (_isStopBgm)
            {
                taskList.Add(_beatController.StopBeatAsync(_settings.BgmChangeSec, ct));
            }

            if (_novelManager.Debug) _endTransitionType = NovelEndTransitionType.ノベルシーンだけ消す;

            if (IsNeedTransition)
            {
                taskList.Add(_transitionCtrl.FillAsync(_transitionType, ct));
                taskList.Add(_volume.FadeAsync(new MornSoundVolumeFadeInfo
                {
                    SoundVolumeType = _settings.FadeVolumeType,
                    Duration = _settings.BgmChangeSec,
                    IsFadeIn = false,
                    CancellationToken = ct,
                }));
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