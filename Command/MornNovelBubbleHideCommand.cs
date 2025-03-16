﻿using System;
using Arbor;
using UnityEngine;

namespace MornNovel
{
    [Serializable]
    public class MornNovelBubbleHideCommand : MornNovelCommandBase
    {
        public override string Tips => "フキダシを消す";
        [SerializeField] private StateLink _nextState;

        public override async void OnStateBegin()
        {
            var controller = FindFirstObjectByType<MornNovelControllerMono>();
            await controller.BubbleHideAsync();
            Transition(_nextState);
        }
    }
}