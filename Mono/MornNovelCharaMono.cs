using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace MornNovel
{
    public sealed class MornNovelCharaMono : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Transform _parentX;
        [SerializeField] private Transform _parentY;
        [SerializeField] [ReadOnly] private MornNovelPoseSo _pose;
        [SerializeField] [ReadOnly] private float _nextChangeTime;
        [SerializeField] [ReadOnly] private bool _isEyeOpen;
        [Inject] private MornNovelSettings _novelSettings;
        private CancellationTokenSource _ctsT = new();
        private CancellationTokenSource _ctsAlpha = new();
        private CancellationTokenSource _ctsX = new();
        private CancellationTokenSource _ctsY = new();

        private void Awake()
        {
            HideAsync(isImmediate: true).Forget();
            SetUnfocus(true);
        }

        private void Update()
        {
            if (_pose == null)
            {
                return;
            }

            if (Time.time > _nextChangeTime)
            {
                _isEyeOpen = !_isEyeOpen;
                _renderer.sprite = _isEyeOpen ? _pose.EyeOpen : _pose.EyeClose;
                var range = _isEyeOpen ? _novelSettings.EyeOpenRange : _novelSettings.EyeCloseRange;
                _nextChangeTime = Time.time + Random.Range(range.x, range.y);
            }
        }

        public void DecreaseOrderInLayer()
        {
            _renderer.sortingOrder--;
        }

        public void ResetOrderInLayer()
        {
            _renderer.sortingOrder = 0;
        }

        public async UniTask SetPositionXAsync(float talkerPosition, CancellationToken ct = default)
        {
            _ctsT?.Cancel();
            _ctsT = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var pos = transform.localPosition;
            pos.x = talkerPosition * _novelSettings.PositionScale;
            await transform.DOLocalMove(pos, _novelSettings.AnimDuration, _ctsT.Token);
        }

        public void SetPositionX(float talkerPosition)
        {
            var pos = transform.localPosition;
            pos.x = talkerPosition * _novelSettings.PositionScale;
            transform.localPosition = pos;
        }

        public void SetFlipX(bool flipX)
        {
            _renderer.flipX = flipX;
        }

        public void SetPose(MornNovelPoseSo pose)
        {
            _pose = pose;
            _isEyeOpen = true;
            _renderer.sprite = pose.EyeOpen;
            var range = _novelSettings.EyeOpenRange;
            _nextChangeTime = Time.time + Random.Range(range.x, range.y);
        }

        public async UniTask SpawnAsync(MornNovelCharaMoveType moveType = MornNovelCharaMoveType.ToInner,
            CancellationToken ct = default)
        {
            _ctsX?.Cancel();
            _ctsX = CancellationTokenSource.CreateLinkedTokenSource(ct);
            _ctsAlpha?.Cancel();
            _ctsAlpha = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var localFromPos = _parentX.localPosition;
            var dif = _novelSettings.SpawnDifX;
            localFromPos.x = moveType switch
            {
                MornNovelCharaMoveType.ToOuter => transform.localPosition.x > 0 ? -dif : dif,
                MornNovelCharaMoveType.ToInner => transform.localPosition.x > 0 ? dif : -dif,
                MornNovelCharaMoveType.ToLeft => dif,
                MornNovelCharaMoveType.ToRight => -dif,
                MornNovelCharaMoveType.Slide => 0,
                _ => throw new ArgumentOutOfRangeException(nameof(moveType), moveType, null),
            };
            _parentX.localPosition = localFromPos;
            var taskA = _parentX.DOLocalMoveX(0, _novelSettings.AnimDuration, _ctsX.Token);
            var taskB = _renderer.DOFade(1, _novelSettings.AnimDuration, _ctsAlpha.Token);
            await UniTask.WhenAll(taskA, taskB).SuppressCancellationThrow();
        }

        public async UniTask HideAsync(MornNovelCharaMoveType moveType = MornNovelCharaMoveType.ToOuter,
            bool isImmediate = false, CancellationToken ct = default)
        {
            _ctsX?.Cancel();
            _ctsAlpha?.Cancel();
            var dif = _novelSettings.SpawnDifX;
            var aimX = moveType switch
            {
                MornNovelCharaMoveType.ToOuter => transform.localPosition.x > 0 ? dif : -dif,
                MornNovelCharaMoveType.ToInner => transform.localPosition.x > 0 ? -dif : dif,
                MornNovelCharaMoveType.ToLeft => -dif,
                MornNovelCharaMoveType.ToRight => dif,
                _ => throw new ArgumentOutOfRangeException(nameof(moveType), moveType, null),
            };
            if (isImmediate)
            {
                _ctsX = null;
                _ctsAlpha = null;
                _parentX.localPosition = new Vector3(aimX, _parentX.localPosition.y, _parentX.localPosition.z);
                _renderer.color = new Color(1, 1, 1, 0);
            }
            else
            {
                _ctsX = CancellationTokenSource.CreateLinkedTokenSource(ct);
                _ctsAlpha = CancellationTokenSource.CreateLinkedTokenSource(ct);
                var taskA = _parentX.DOLocalMoveX(aimX, _novelSettings.AnimDuration, _ctsX.Token);
                var taskB = _renderer.DOFade(0, _novelSettings.AnimDuration, _ctsAlpha.Token);
                await UniTask.WhenAll(taskA, taskB).SuppressCancellationThrow();
            }
        }

        public void Focus(CancellationToken ct = default)
        {
            _ctsY?.Cancel();
            _ctsY = CancellationTokenSource.CreateLinkedTokenSource(ct);
            _parentY.DOLocalMoveY(_novelSettings.HeightFocus, _novelSettings.AnimDuration, _ctsY.Token).Forget();
        }

        public void SetUnfocus(bool isImmediate = false, CancellationToken ct = default)
        {
            _ctsY?.Cancel();
            if (isImmediate)
            {
                _ctsY = null;
                _parentY.SetLocalY(_novelSettings.HeightUnfocus);
            }
            else
            {
                _ctsY = CancellationTokenSource.CreateLinkedTokenSource(ct);
                _parentY.DOLocalMoveY(_novelSettings.HeightUnfocus, _novelSettings.AnimDuration, _ctsY.Token).Forget();
            }
        }
    }
}