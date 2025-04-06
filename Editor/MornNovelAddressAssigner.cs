using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace MornNovel
{
    public static class MornNovelAddressAssigner
    {
        [MenuItem("Tools/MornNovelアドレス設定")]
        private static void SetAddressables()
        {
            var global = MornNovelGlobal.I;
            if (global == null)
            {
                MornNovelGlobal.LogError($"{nameof(MornNovelGlobal)} が見つかりません。");
                return;
            }

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                MornNovelGlobal.LogError("Addressable Asset Settings が見つかりません。");
                return;
            }

            // Main グループを取得または作成
            var groupName = global.AddressGroupName;
            var ignoreAddressPrefix = global.IgnoreAddressPrefix;
            var labelTag = global.AddressLabelTag;
            var group = settings.FindGroup(groupName);
            
            if (group == null)
            {
                group = settings.CreateGroup(groupName, false, false, false, null);
                MornNovelGlobal.Log($"グループ {groupName} を作成しました。");
            }

            // プレハブを検索
            var guids = AssetDatabase.FindAssets("t:Prefab");
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (prefab == null || prefab.GetComponent<MornNovelMono>() == null)
                {
                    continue;
                }

                var address = assetPath;
                if (!string.IsNullOrEmpty(ignoreAddressPrefix) && address.StartsWith(ignoreAddressPrefix))
                {
                    address = address[ignoreAddressPrefix.Length..];
                }

                if (address.EndsWith(".prefab"))
                {
                    address = address[..^7];
                }

                // 既に設定されている場合はスキップ
                var entry = settings.FindAssetEntry(guid);
                if (entry == null
                    || entry.parentGroup != group
                    || entry.address != address
                    || entry.labels.Count != 1
                    || !entry.labels.Contains(labelTag))
                {
                    entry = settings.CreateOrMoveEntry(guid, group);
                    entry.address = address;
                    entry.SetLabel(labelTag, true, true);
                    MornNovelGlobal.Log($"アドレス {address} を設定しました。");
                    
                    //　依存しているアセットのアドレスを設定
                    var dependencies = AssetDatabase.GetDependencies(assetPath, true);
                    foreach (var dependency in dependencies)
                    {
                        if (dependency == assetPath)
                        {
                            continue;
                        }

                        var depGuid = AssetDatabase.AssetPathToGUID(dependency);
                        var depEntry = settings.FindAssetEntry(depGuid);
                        if (depEntry == null)
                        {
                            depEntry = settings.CreateOrMoveEntry(depGuid, group);
                            depEntry.address = dependency;
                            depEntry.SetLabel(labelTag, true, true);
                            MornNovelGlobal.Log($"アドレス {dependency} を設定しました。");
                        }
                    }
                }
            }

            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, null, true);
            AssetDatabase.SaveAssets();
            MornNovelGlobal.Log("MornNovelMono の Addressable 設定が完了しました。");
        }
    }
}