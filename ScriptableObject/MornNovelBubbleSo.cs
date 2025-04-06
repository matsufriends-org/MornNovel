using UnityEngine;

namespace MornNovel
{
    [CreateAssetMenu(fileName = nameof(MornNovelBubbleSo), menuName = "Morn/" + nameof(MornNovelBubbleSo))]
    public sealed class MornNovelBubbleSo : ScriptableObject
    {
        public Vector2 NamePosition;
        public Vector2 BubblePosition;
        [SpritePreview] public Sprite SpeechBubble;
        [SpritePreview] public Sprite SpeechBubbleEdge;
        public Sprite Preview => SpeechBubble;
    }
}