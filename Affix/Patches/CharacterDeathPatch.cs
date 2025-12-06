using System.Collections.Generic;
using Affix.Core;
using HarmonyLib;
using UnityEngine;
using AffixDropTable = Affix.DropTables.DropTable;

namespace Affix.Patches
{
    /// <summary>
    /// Patches creature death to handle custom drop tables with affixes.
    /// </summary>
    [HarmonyPatch(typeof(CharacterDrop), nameof(CharacterDrop.GenerateDropList))]
    public static class CharacterDrop_GenerateDropList_Patch
    {
        /// <summary>
        /// After vanilla drops are generated, add our custom affix drops.
        /// </summary>
        static void Postfix(CharacterDrop __instance, ref List<KeyValuePair<GameObject, int>> __result)
        {
            if (__instance == null) return;

            // Get the creature name
            var creatureName = GetCreatureName(__instance);
            if (string.IsNullOrEmpty(creatureName)) return;

            // Check if we have a custom drop table for this creature
            if (!AffixDropTable.Has(creatureName))
            {
                Plugin.Log?.LogDebug($"No custom drop table for: {creatureName}");
                return;
            }

            Plugin.Log?.LogInfo($"Rolling custom drops for: {creatureName}");

            // Get player count for scaling
            var playerCount = Player.GetAllPlayers()?.Count ?? 1;

            // Roll our custom drops
            var drops = AffixDropTable.Roll(creatureName, playerCount);

            foreach (var drop in drops)
            {
                var prefab = ZNetScene.instance?.GetPrefab(drop.PrefabName);
                if (prefab == null)
                {
                    Plugin.Log?.LogWarning($"Could not find prefab for drop: {drop.PrefabName}");
                    continue;
                }

                // If this drop has affixes, we need to handle it specially
                // We'll use a marker system to apply affixes when the item spawns
                if (drop.HasAffixes)
                {
                    // Store the pending affix data for when the item spawns
                    AffixSpawnQueue.Enqueue(drop.PrefabName, drop.Rarity);
                    Plugin.Log?.LogDebug($"Queued affix spawn: {drop.PrefabName} ({drop.Rarity})");
                }

                // Add to vanilla drop list
                __result.Add(new KeyValuePair<GameObject, int>(prefab, drop.Count));
            }
        }

        private static string GetCreatureName(CharacterDrop characterDrop)
        {
            if (characterDrop == null) return null;

            // Try to get from Character component
            if (characterDrop.TryGetComponent<Character>(out var character))
            {
                // Use the prefab name, not the localized name
                var prefabName = Utils.GetPrefabName(character.gameObject);
                if (!string.IsNullOrEmpty(prefabName))
                    return prefabName;
            }

            // Fallback to gameobject name
            return characterDrop.gameObject.name.Replace("(Clone)", "").Trim();
        }
    }

    /// <summary>
    /// Patches item spawn to apply queued affixes.
    /// </summary>
    [HarmonyPatch(typeof(ItemDrop), nameof(ItemDrop.Awake))]
    public static class ItemDrop_Awake_Patch
    {
        static void Postfix(ItemDrop __instance)
        {
            if (__instance == null || __instance.m_itemData == null) return;

            var prefabName = Utils.GetPrefabName(__instance.gameObject);

            // Check if this item has pending affixes from drop table
            if (AffixSpawnQueue.TryDequeue(prefabName, out var rarity))
            {
                Plugin.Log?.LogDebug($"Applying queued affixes to {prefabName} ({rarity})");
                AffixItemSpawner.ApplyAffixes(__instance.m_itemData, rarity);
            }
        }
    }

    /// <summary>
    /// Patches ItemDrop.Start to add VFX to any item with affixes in the world.
    /// This handles items dropped from inventory, spawned, or from creature deaths.
    /// </summary>
    [HarmonyPatch(typeof(ItemDrop), nameof(ItemDrop.Start))]
    public static class ItemDrop_Start_Patch
    {
        static void Postfix(ItemDrop __instance)
        {
            if (__instance == null || __instance.m_itemData == null) return;

            // Check if item has affixes
            if (!AffixData.HasAffixes(__instance.m_itemData)) return;

            var rarity = AffixData.GetRarity(__instance.m_itemData);

            // Skip common items (no VFX)
            if (rarity == Rarity.Common) return;

            // Add rarity VFX
            DropVFXHelper.AddRarityEffect(__instance, rarity);
        }
    }

    /// <summary>
    /// Queue for pending affix applications.
    /// Used to pass affix data between drop generation and item spawn.
    /// </summary>
    public static class AffixSpawnQueue
    {
        private static readonly System.Collections.Generic.Queue<(string prefab, Rarity rarity)> _queue = new();

        public static void Enqueue(string prefabName, Rarity rarity)
        {
            _queue.Enqueue((prefabName, rarity));
        }

        public static bool TryDequeue(string prefabName, out Rarity rarity)
        {
            rarity = Rarity.Common;

            if (_queue.Count == 0)
                return false;

            // Check if the next item matches
            var peek = _queue.Peek();
            if (peek.prefab != prefabName)
                return false;

            var item = _queue.Dequeue();
            rarity = item.rarity;
            return true;
        }

        public static void Clear()
        {
            _queue.Clear();
        }
    }
}
