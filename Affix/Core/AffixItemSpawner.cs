using System.Collections.Generic;
using Affix.DropTables;
using UnityEngine;

namespace Affix.Core
{
    /// <summary>
    /// Handles spawning items with affixes applied.
    /// </summary>
    public static class AffixItemSpawner
    {
        /// <summary>
        /// Spawns a drop result as a world item with affixes applied.
        /// </summary>
        public static GameObject SpawnDrop(DropResult drop, Vector3 position)
        {
            if (drop == null || string.IsNullOrEmpty(drop.PrefabName))
                return null;

            // Get the prefab
            var prefab = ZNetScene.instance?.GetPrefab(drop.PrefabName);
            if (prefab == null)
            {
                Plugin.Log?.LogWarning($"Could not find prefab: {drop.PrefabName}");
                return null;
            }

            // Spawn the item
            var spawned = Object.Instantiate(prefab, position, Quaternion.identity);
            if (spawned == null)
                return null;

            // Get ItemDrop component
            if (!spawned.TryGetComponent<ItemDrop>(out var itemDrop))
            {
                Plugin.Log?.LogWarning($"Spawned object has no ItemDrop: {drop.PrefabName}");
                return spawned;
            }

            // Set stack size
            if (drop.Count > 1)
            {
                itemDrop.m_itemData.m_stack = drop.Count;
            }

            // Apply affixes if this drop should have them
            // VFX is added automatically via ItemDrop.Start patch
            if (drop.HasAffixes)
            {
                ApplyAffixes(itemDrop.m_itemData, drop.Rarity);
            }

            return spawned;
        }

        /// <summary>
        /// Applies random affixes to an item based on rarity.
        /// Also rolls a unique legendary ability for Legendary items.
        /// </summary>
        public static void ApplyAffixes(ItemDrop.ItemData item, Rarity rarity)
        {
            if (item == null) return;

            var itemType = AffixData.GetAffixItemType(item);
            if (itemType == ItemType.None)
            {
                Plugin.Log?.LogDebug($"Item type {item.m_shared?.m_itemType} cannot have affixes");
                return;
            }

            // Use GenerateForItem which handles both affixes and legendary abilities
            AffixGenerator.GenerateForItem(item, rarity);

            // Log results
            var affixes = AffixData.GetAffixes(item);
            Plugin.Log?.LogInfo($"Applied {affixes.Count} affixes ({rarity}) to {item.m_shared?.m_name}");

            foreach (var affix in affixes)
            {
                Plugin.Log?.LogDebug($"  - {affix.Definition.Name}: {affix.GetFormattedDescription()}");
            }

            // Log legendary ability if present
            var uniqueAbility = AffixData.GetUniqueAbility(item);
            if (uniqueAbility != null)
            {
                Plugin.Log?.LogInfo($"  Unique ability: {uniqueAbility.DisplayName} ({uniqueAbility.AbilityId})");
            }
        }

        /// <summary>
        /// Spawns all drop results from a list.
        /// </summary>
        public static List<GameObject> SpawnDrops(List<DropResult> drops, Vector3 position, float spreadRadius = 0.5f)
        {
            var spawned = new List<GameObject>();

            foreach (var drop in drops)
            {
                // Add some random spread
                var offset = Random.insideUnitCircle * spreadRadius;
                var spawnPos = position + new Vector3(offset.x, 0.5f, offset.y);

                var obj = SpawnDrop(drop, spawnPos);
                if (obj != null)
                    spawned.Add(obj);
            }

            return spawned;
        }
    }
}
