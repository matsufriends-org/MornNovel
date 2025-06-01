using UnityEngine;

namespace MornNovel
{
    [CreateAssetMenu(fileName = nameof(MornNovelCameraShakeSettings), menuName = nameof(MornNovelCameraShakeSettings))]
    public sealed class MornNovelCameraShakeSettings : ScriptableObject
    {
        public float ShakeStrength = 1f;
        public int ShakeCount = 5;
        public float ShakeInterval = 0.05f;
    }
}