using System;
using Arbor;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MornNovel
{
    public class MornNovelCameraShakeCommand : MornNovelCommandBase
    {
        public override string Tips => "カメラを揺らす";
        [SerializeField] private CinemachineImpulseSource _source;
        [SerializeField] private StateLink _onComplete;
        public float ShakeStrength = 1f;
        public int ShakeCount = 5;
        public float ShakeInterval = 0.05f;

        public override async void OnStateBegin()
        {
            base.OnStateBegin();
            if (_source != null)
            {
                var source = Instantiate(_source);
                await ShakeAsync();
                Destroy(source.gameObject);
            }

            Transition(_onComplete);
        }

        private async UniTask ShakeAsync()
        {
            for (var i = 0; i < ShakeCount; i++)
            {
                var randomDir = Random.insideUnitCircle.normalized;
                var shakeVector = new Vector3(randomDir.x, randomDir.y, 0f) * ShakeStrength;

                var impulse = _source.ImpulseDefinition;
                impulse.ImpulseDuration = ShakeInterval;
                _source.ImpulseDefinition = impulse;
                
                _source.GenerateImpulse(shakeVector);
                await UniTask.Delay(TimeSpan.FromSeconds(ShakeInterval), cancellationToken: CancellationTokenOnEnd);
            }
        }
    }
}