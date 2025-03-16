using System;

namespace MornNovel
{
    public sealed class MornNovelService
    {
        private readonly Func<MornNovelAddress, bool> _isGetNovelReadGetNovelRead;
        private readonly Action<MornNovelAddress> _onNovelRead;
        private readonly Func<bool> _getInput;
        public MornNovelAddress CurrentNovelAddress { get; private set; }

        public MornNovelService(Func<MornNovelAddress, bool> getNovelRead, Action<MornNovelAddress> onNovelRead, Func<bool> getInput)
        {
            _isGetNovelReadGetNovelRead = getNovelRead;
            _onNovelRead = onNovelRead;
            _getInput = getInput;
        }

        public bool IsNovelRead(MornNovelAddress address)
        {
            return _isGetNovelReadGetNovelRead(address);
        }

        public void SetNovelRead(MornNovelAddress address)
        {
            _onNovelRead(address);
        }

        public bool Input()
        {
            return _getInput();
        }

        public void SetNovelAddress(MornNovelAddress novelAddress)
        {
            CurrentNovelAddress = novelAddress;
        }
    }
}