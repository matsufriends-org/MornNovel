using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;
using VContainer.Unity;

namespace MornNovel
{
    internal sealed class MornNovelAutoLoaderMono : MonoBehaviour
    {
        [SerializeField] private string _debugNovelKey;
        [Inject] private MornNovelService _novelService;
        [Inject] private IObjectResolver _resolver;

        private async void Awake()
        {
            var novelAddress = string.IsNullOrEmpty(_novelService.CurrentNovelAddress) ? _debugNovelKey
                : _novelService.CurrentNovelAddress;
            var handle = Addressables.LoadAssetAsync<GameObject>(novelAddress);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
            {
                var result = handle.Result.TryGetComponent<MornNovelMono>(out var prefab);
                if (result)
                {
                    _resolver.Instantiate(prefab, transform);
                }
                else
                {
                    Debug.Log($"NovelMono {novelAddress} is exists, but not MornNovelMono");
                }

                Addressables.Release(handle);
            }
            else
            {
                Debug.LogError($"Failed to load asset: {novelAddress}");
            }
        }

        public void SetDebugNovel(string novelKey)
        {
            _debugNovelKey = novelKey;
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(MornNovelAutoLoaderMono))]
    internal sealed class MornNovelAutoLoaderEditor : UnityEditor.Editor
    {
        private Vector2 _scrollPos;
        private readonly List<MornNovelMono> _novels = new();

        private void OnEnable()
        {
            _novels.Clear();
            var guids = AssetDatabase.FindAssets("t:Prefab");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab.TryGetComponent<MornNovelMono>(out var novel))
                {
                    _novels.Add(novel);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            using (var scroll = new EditorGUILayout.ScrollViewScope(_scrollPos))
            {
                foreach (var novel in _novels)
                {
                    if (GUILayout.Button(novel.name))
                    {
                        var loader = (MornNovelAutoLoaderMono)target;
                        loader.SetDebugNovel(novel.Key);
                        EditorUtility.SetDirty(target);
                    }
                }

                _scrollPos = scroll.scrollPosition;
            }
        }
    }
#endif
}