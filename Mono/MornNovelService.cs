using System;
using UniRx;

namespace MornNovel
{
    public sealed class MornNovelService
    {
        private readonly Func<MornNovelAddress, bool> _isNovelRead;
        private readonly Action<MornNovelAddress> _onNovelRead;
        private readonly Func<bool> _getInput;

        public bool Debug;
        public IObservable<Unit> OnNovelEnd => _onNovelEnd;
        private readonly Subject<Unit> _onNovelEnd = new();
        public MornNovelAddress CurrentNovelAddress { get; private set; }
        public MornNovelMono CurrentNovelPrefab { get; private set; }

        public MornNovelService(Func<MornNovelAddress, bool> novelRead, Action<MornNovelAddress> onNovelRead, Func<bool> getInput)
        {
            _isNovelRead = novelRead;
            _onNovelRead = onNovelRead;
            _getInput = getInput;
        }

        public void NovelEndOnNext()
        {
            _onNovelEnd.OnNext(Unit.Default);
        }

        public bool IsNovelRead(MornNovelAddress address)
        {
            return _isNovelRead(address);
        }

        public void SetNovelRead(MornNovelAddress address)
        {
            _onNovelRead(address);
        }

        public bool Input()
        {
            return _getInput();
        }
        
        public void SetNovelPrefab(MornNovelMono prefab)
        {
            CurrentNovelPrefab = prefab;
        }

        public void SetNovelAddress(MornNovelAddress novelAddress)
        {
            CurrentNovelAddress = novelAddress;
        }
    }
}