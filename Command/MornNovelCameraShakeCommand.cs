using System;
using Arbor;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace MornNovel
{
    public class MornNovelCameraShakeCommand : MornNovelCommandBase
    {
        public override string Tips => "カメラを揺らす";
        [SerializeField] private CinemachineImpulseSource _source;
        [SerializeField] private StateLink _onComplete;

        public override async void OnStateBegin()
        {
            base.OnStateBegin();
            if (_source != null)
            {
                var source = Instantiate(_source);
                source.GenerateImpulse();
                await UniTask.Delay(
                    TimeSpan.FromSeconds(source.ImpulseDefinition.ImpulseDuration),
                    cancellationToken: CancellationTokenOnEnd);
                Destroy(source.gameObject);
            }

            Transition(_onComplete);
        }
    }
}