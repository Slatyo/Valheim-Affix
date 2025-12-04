using System.Collections.Generic;
using System.Linq;
using Affix.Core;
using HarmonyLib;
using Prime.Core;
using Prime.Modifiers;

namespace Affix.Patches
{
    /// <summary>
    /// Patches equipment changes to apply/remove affix stats via Prime.
    /// </summary>
    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.EquipItem))]
    public static class Humanoid_EquipItem_Patch
    {
        static void Postfix(Humanoid __instance, ItemDrop.ItemData item, bool __result)
        {
            // Only process if equip was successful
            if (!__result || item == null) return;

            // Only process for players
            if (!(__instance is Player player)) return;

            // Check if item has affixes
            if (!AffixData.HasAffixes(item)) return;

            Plugin.Log?.LogDebug($"Applying affix stats from: {item.m_shared?.m_name}");
            AffixStatManager.ApplyAffixStats(player, item);
        }
    }

    /// <summary>
    /// Patches unequip to remove affix stats.
    /// </summary>
    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.UnequipItem))]
    public static class Humanoid_UnequipItem_Patch
    {
        static void Prefix(Humanoid __instance, ItemDrop.ItemData item)
        {
            if (item == null) return;

            // Only process for players
            if (!(__instance is Player player)) return;

            // Check if item has affixes
            if (!AffixData.HasAffixes(item)) return;

            Plugin.Log?.LogDebug($"Removing affix stats from: {item.m_shared?.m_name}");
            AffixStatManager.RemoveAffixStats(player, item);
        }
    }

    /// <summary>
    /// Manages applying and removing affix stats via Prime.
    /// </summary>
    public static class AffixStatManager
    {
        /// <summary>
        /// Applies all affix stats from an item to a player via Prime modifiers.
        /// </summary>
        public static void ApplyAffixStats(Player player, ItemDrop.ItemData item)
        {
            if (player == null || item == null) return;

            var affixes = AffixData.GetAffixes(item);
            if (affixes.Count == 0) return;

            var itemId = GetItemId(item);
            var container = EntityManager.Instance.GetOrCreate(player);

            foreach (var affix in affixes)
            {
                if (affix.Definition == null) continue;

                var modifierId = $"affix_{itemId}_{affix.Definition.Id}";

                // Convert affix modifier type to Prime modifier type
                var primeType = affix.Definition.Type switch
                {
                    Core.ModifierType.Flat => Prime.Modifiers.ModifierType.Flat,
                    Core.ModifierType.Percent => Prime.Modifiers.ModifierType.Percent,
                    Core.ModifierType.Multiplicative => Prime.Modifiers.ModifierType.Multiply,
                    _ => Prime.Modifiers.ModifierType.Flat
                };

                var modifier = new Modifier(modifierId, affix.Definition.PrimeStat, primeType, affix.Value)
                {
                    Source = $"Affix_{itemId}",
                    Order = ModifierOrder.Affix
                };

                container.AddModifier(modifier);
                Plugin.Log?.LogDebug($"  Added modifier: {modifierId} = {affix.Value} ({primeType})");
            }
        }

        /// <summary>
        /// Removes all affix stats from an item from a player.
        /// </summary>
        public static void RemoveAffixStats(Player player, ItemDrop.ItemData item)
        {
            if (player == null || item == null) return;

            var itemId = GetItemId(item);
            var source = $"Affix_{itemId}";

            var container = EntityManager.Instance.Get(player);
            if (container == null) return;

            // Remove all modifiers from this item's affix source
            var removed = container.RemoveModifiersFromSource(source);
            Plugin.Log?.LogDebug($"Removed {removed} affix modifiers from source: {source}");
        }

        /// <summary>
        /// Gets a unique identifier for an item instance.
        /// </summary>
        private static string GetItemId(ItemDrop.ItemData item)
        {
            // Use the item's unique ID if available, otherwise fall back to hash
            if (item.m_customData != null && item.m_customData.TryGetValue("affix_uid", out var uid))
                return uid;

            // Generate and store a UID
            uid = System.Guid.NewGuid().ToString("N").Substring(0, 8);
            item.m_customData ??= new Dictionary<string, string>();
            item.m_customData["affix_uid"] = uid;
            return uid;
        }
    }
}
