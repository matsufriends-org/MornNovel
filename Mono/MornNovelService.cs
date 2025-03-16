using System;

namespace MornNovel
{
    public sealed class MornNovelService
    {
        private readonly Func<string, bool> _isGetNovelReadGetNovelRead;
        private readonly Action<string> _onNovelRead;
        private readonly Func<bool> _getInput;
        public string CurrentNovelAddress { get; private set; }

        public MornNovelService(Func<string, bool> getNovelRead, Action<string> onNovelRead, Func<bool> getInput)
        {
            _isGetNovelReadGetNovelRead = getNovelRead;
            _onNovelRead = onNovelRead;
            _getInput = getInput;
        }

        public bool IsNovelRead(string key)
        {
            return _isGetNovelReadGetNovelRead(key);
        }

        public void SetNovelRead(string key)
        {
            _onNovelRead(key);
        }

        public bool Input()
        {
            return _getInput();
        }

        public void SetNovelAddress(string novelAddress)
        {
            CurrentNovelAddress = novelAddress;
        }
    }
}