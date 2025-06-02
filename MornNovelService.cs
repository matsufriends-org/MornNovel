using System;
using UniRx;

namespace MornNovel
{
    public sealed class MornNovelService
    {
        private readonly Func<MornNovelAddress, bool> _isNovelRead;
        private readonly Func<bool> _getInput;
        public bool Debug;
        private readonly Subject<MornNovelAddress> _onNovelStart = new();
        private readonly Subject<MornNovelAddress> _onNovelSet = new();
        private readonly Subject<MornNovelAddress> _onNovelEnd = new();
        public IObservable<MornNovelAddress> OnNovelStart => _onNovelStart;
        public IObservable<MornNovelAddress> OnNovelSet => _onNovelSet;
        public IObservable<MornNovelAddress> OnNovelEnd => _onNovelEnd;
        public MornNovelAddress CurrentNovelAddress { get; private set; }
        public MornNovelSetType NovelSetType { get; private set; }
        public MornNovelMono CurrentNovelPrefab { get; private set; }

        public MornNovelService(Func<MornNovelAddress, bool> novelRead, Func<bool> getInput)
        {
            _isNovelRead = novelRead;
            _getInput = getInput;
        }

        public void AtNovelStart(MornNovelAddress address)
        {
            _onNovelStart.OnNext(address);
        }

        public void AtNovelEnd(MornNovelAddress address)
        {
            _onNovelEnd.OnNext(address);
        }

        public bool IsNovelRead(MornNovelAddress address)
        {
            return _isNovelRead(address);
        }

        public bool Input()
        {
            return _getInput();
        }

        public void SetNovelPrefab(MornNovelMono prefab)
        {
            CurrentNovelPrefab = prefab;
        }

        public void SetNovelAddress(MornNovelAddress novelAddress, MornNovelSetType novelSetType)
        {
            CurrentNovelAddress = novelAddress;
            NovelSetType = novelSetType;
        }
    }
}