using UnityEngine;

namespace MornNovel
{
    [CreateAssetMenu(fileName = nameof(MornNovelPoseSo), menuName = "Morn/" + nameof(MornNovelPoseSo))]
    public sealed class MornNovelPoseSo : ScriptableObject
    {
        public MornNovelTalkerSo Talker;
        [SpritePreview] public Sprite EyeOpen;
        [SpritePreview] public Sprite EyeClose;
        public Sprite Preview => EyeOpen;
    }
}