using UnityEngine;
using VContainer;

namespace MornNovel
{
    public sealed class MornNovelMono : MonoBehaviour
    {
        // 暫定
        public string ReadKey
        {
            get
            {
                var objectName = gameObject.name;
                // (Clone)がついている場合は除去
                if (objectName.EndsWith("(Clone)"))
                {
                    objectName = objectName.Substring(0, objectName.Length - 7);
                }

                return $"{objectName}_Read";
            }
        }
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
            var address = new MornNovelAddress(ReadKey);
            return _novelManager.IsNovelRead(address);
        }

        public void SetNovelRead()
        {
            var address = new MornNovelAddress(ReadKey);
            _novelManager.SetNovelRead(address);
        }
    }
}