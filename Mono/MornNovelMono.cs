using UnityEngine;
using VContainer;

namespace MornNovel
{
    public sealed class MornNovelMono : MonoBehaviour
    {
        // Addressableのパスを指定
        public string Key => $"Novel/{name}";
        [Inject] private MornNovelControllerMono _novelController;
        [Inject] private MornNovelService _novelManager;

        private void Awake()
        {
            foreach (var audioSource in GetComponentsInChildren<AudioSource>())
            {
                audioSource.outputAudioMixerGroup = _novelController.AudioMixerGroup;
            }
        }

        public bool IsNovelRead()
        {
            return _novelManager.IsNovelRead(Key);
        }

        public void SetNovelRead()
        {
            _novelManager.SetNovelRead(Key);
        }
    }
}