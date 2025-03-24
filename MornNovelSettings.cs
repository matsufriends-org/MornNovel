using MornSound;
using UnityEngine;

namespace MornNovel
{
    [CreateAssetMenu(fileName = nameof(MornNovelSettings), menuName = "Novel/" + nameof(MornNovelSettings))]
    public sealed class MornNovelSettings : ScriptableObject
    {
        [Header("音")]
        [Label("フェードタイプ")] public MornSoundVolumeType FadeVolumeType;
        [Label("ノベル決定音")] public AudioClip SubmitClip;
        [Header("フェード系")]
        [Label("BGMフェード時間")] public float BgmChangeSec = 1f;
        [Label("背景フェード時間")] public float BackgroundFadeSec = 1f;

        [Header("トランジション")] [Label("背景歪みトランジションMaterial")]
        public Material DistortTransitionMaterial;

        [Label("背景歪みトランジション時間")] public float DistortTransitionSec = 1f;
        [Header("Eye")]
        public Vector2 EyeCloseRange;
        public Vector2 EyeOpenRange;
        [Header("キャラクター")]
        [Label("移動距離")] public float SpawnDifX = 0.5f;
        [Label("移動時間")] public float AnimDuration = 0.5f;
        [Label("フォーカス時の高さ")] public float HeightFocus = -0.8f;
        [Label("非フォーカス時の高さ")] public float HeightUnfocus = -1.2f;
        public float PositionScale = 11;
        [Header("メッセージ")]
        [Label("文字再生待機時間")] public float Offset = 0.1f;
        [Label("1文字送り時間")] public float CharInterval = 0.05f;
        [Label("改行時間")] public float CharReturnInterval = 0.1f;
    }
}