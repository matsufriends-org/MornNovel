using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace MornNovel._Morn.MornNovel.Editor
{
    public static class AddressableDependencyAssigner
    {
        [MenuItem("Tools/Addressableの依存アセットのアドレスを登録")]
        public static void AutoAssignDependencies()
        {
            
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("AddressableAssetSettings not found!");
                return;
            }

            HashSet<string> addressableAssets = new HashSet<string>();

            // 既存のAddressableアセットを取得
            foreach (var group in settings.groups)
            {
                foreach (var entry in group.entries)
                {
                    addressableAssets.Add(entry.AssetPath);
                }
            }

            // 依存関係を解析して、Addressableでないものを追加
            List<string> assetsToAdd = new List<string>();

            foreach (string assetPath in addressableAssets)
            {
                string[] dependencies = AssetDatabase.GetDependencies(assetPath, true);
                foreach (string dep in dependencies)
                {
                    if (!addressableAssets.Contains(dep) && !AssetDatabase.IsValidFolder(dep))
                    {
                        assetsToAdd.Add(dep);
                    }
                }
            }

            // Addressableに追加
            foreach (string asset in assetsToAdd)
            {
                var guid = AssetDatabase.AssetPathToGUID(asset);
                var entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
                Debug.Log($"Added to Addressables: {asset}");
            }

            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, null, true);
            AssetDatabase.SaveAssets();
            Debug.Log("Addressable dependency assignment completed.");
        }
    }
}