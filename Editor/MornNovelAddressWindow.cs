using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace MornNovel
{
    internal sealed class MornNovelAddressWindow : EditorWindow
    {
        private SerializedProperty _addressProperty;
        private List<string> _allAddress = new();
        private List<string> _filteredAddress = new();
        private readonly HashSet<string> _headerHashSet = new();
        private string _searchText;
        private Vector2 _scrollPos;

        public static void ShowWindow(SerializedProperty addressProperty)
        {
            var window = GetWindow<MornNovelAddressWindow>(nameof(MornNovelAddressWindow));
            window.Initialize(addressProperty);
            window.Show();
        }

        private void Initialize(SerializedProperty property)
        {
            _addressProperty = property;
            _allAddress.Clear();
            _filteredAddress.Clear();
            _headerHashSet.Clear();
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                MornNovelGlobal.LogError("Addressable Asset Settings が見つかりません。");
                return;
            }

            var guids = AssetDatabase.FindAssets("t:Prefab");
            foreach (var guid in guids)
            {
                var entry = settings.FindAssetEntry(guid);
                if (entry == null)
                {
                    continue;
                }

                _allAddress.Add(entry.address);
            }

            _allAddress = _allAddress.OrderBy(obj => obj.Split('/').Length).ToList();
            _filteredAddress.AddRange(_allAddress);
        }

        private void OnGUI()
        {
            var cachedText = _searchText;
            _searchText = EditorGUILayout.TextField("Search", _searchText);
            if (cachedText != _searchText)
            {
                _filteredAddress.Clear();
                _filteredAddress.AddRange(_allAddress.Where(obj => obj.Contains(_searchText)));
            }

            using var scroll = new EditorGUILayout.ScrollViewScope(_scrollPos);
            _headerHashSet.Clear();
            foreach (var address in _filteredAddress)
            {
                var addressSplit = address.Split('/');
                _headerHashSet.RemoveWhere(header => !addressSplit.Contains(header));
                EditorGUI.indentLevel = _headerHashSet.Count - 1;
                for (var i = 0; i < addressSplit.Length - 1; i++)
                {
                    var header = addressSplit[i];
                    if (_headerHashSet.Add(header))
                    {
                        EditorGUI.indentLevel = _headerHashSet.Count - 1;
                        EditorGUILayout.LabelField(header, EditorStyles.boldLabel);
                    }
                }

                using (new GUILayout.HorizontalScope())
                {
                    var cachedBackgroundColor = GUI.backgroundColor;
                    // propertyからアドレスを取得

                    // アドレスが一致している場合は背景色を変更
                    if (_addressProperty.stringValue == address)
                    {
                        GUI.backgroundColor = Color.cyan;
                    }

                    // ボタンもIndent
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Space(20 * _headerHashSet.Count);
                        if (GUILayout.Button(address))
                        {
                            _addressProperty.stringValue = address;
                            _addressProperty.serializedObject.ApplyModifiedProperties();
                            Close();
                        }
                    }

                    GUI.backgroundColor = cachedBackgroundColor;
                }
            }

            _scrollPos = scroll.scrollPosition;
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                Close();
            }
        }

        private void OnEnable()
        {
            wantsMouseMove = true;
        }

        private void Update()
        {
            if (focusedWindow != this)
            {
                Close();
            }
        }
    }
}