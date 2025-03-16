using System;
using UnityEngine;

namespace MornNovel
{
    [Serializable]
    public struct MornNovelAddress
    {
        [SerializeField] private string _address;
        public string Address => _address;

        public MornNovelAddress(string address)
        {
            _address = address;
        }

        public bool IsNullOrEmpty()
        {
            return string.IsNullOrEmpty(_address);
        }
    }
}