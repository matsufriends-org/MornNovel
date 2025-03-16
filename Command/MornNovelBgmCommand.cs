using System;
using Arbor;
using MornBeat;
using UnityEngine;
using VContainer;

namespace MornNovel
{
    [Serializable]
    public class MornNovelBgmCommand : MornNovelCommandBase
    {
        public override string Tips => "BGMを再生する(nullで停止)";
        [SerializeField] private MornBeatMemoSo _beatMemo;
        [SerializeField] private StateLink _onComplete;
        [Inject] private MornBeatControllerMono _beatController;
        [Inject] private MornNovelSettings _novelSettings;

        public override async void OnStateBegin()
        {
            var ct = CancellationTokenOnEnd;
            if (_beatMemo != null)
            {
                await _beatController.StartAsync(
                    new MornBeatStartInfo
                    {
                        BeatMemo = _beatMemo,
                        FadeDuration = _novelSettings.BgmChangeSec,
                        Ct = ct,
                    });
            }
            else
            {
                await _beatController.StopBeatAsync(_novelSettings.BgmChangeSec, ct);
            }

            Transition(_onComplete);
        }
    }
}