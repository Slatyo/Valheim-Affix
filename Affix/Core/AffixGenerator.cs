using System;
using System.Collections.Generic;
using System.Linq;

namespace Affix.Core
{
    /// <summary>
    /// Generates affixes for items.
    /// </summary>
    public static class AffixGenerator
    {
        private static readonly System.Random _random = new();

        /// <summary>
        /// Generates affixes and unique ability for an item.
        /// </summary>
        /// <param name="item">The item to generate affixes for.</param>
        /// <param name="rarity">The rarity tier for the item.</param>
        public static void GenerateForItem(ItemDrop.ItemData item, Rarity rarity)
        {
            if (item == null) return;

            var itemType = AffixData.GetAffixItemType(item);
            var affixes = GenerateAffixes(itemType, rarity);

            // Set affixes
            AffixData.SetAffixes(item, rarity, affixes);

            // For Legendary items, also roll a unique ability
            if (rarity == Rarity.Legendary)
            {
                var uniqueAbility = LegendaryAbilityRegistry.Instance.GetRandomForItem(item);
                if (uniqueAbility != null)
                {
                    AffixData.SetUniqueAbility(item, uniqueAbility);
                    Plugin.Log?.LogDebug($"[Affix] Rolled legendary ability '{uniqueAbility.Id}' for {item.m_shared?.m_name}");
                }
                else
                {
                    Plugin.Log?.LogWarning($"[Affix] No valid legendary ability found for {item.m_shared?.m_name}");
                }
            }
        }

        /// <summary>
        /// Generates a set of affixes for an item.
        /// </summary>
        /// <param name="itemType">The type of item to generate affixes for.</param>
        /// <param name="rarity">The rarity tier for the item.</param>
        /// <returns>List of rolled affixes.</returns>
        public static List<RolledAffix> GenerateAffixes(ItemType itemType, Rarity rarity)
        {
            var result = new List<RolledAffix>();
            var affixCount = RarityManager.RollAffixCount(rarity);

            if (affixCount == 0)
                return result;

            // Get available affixes for this item type
            var availableAffixes = AffixRegistry.GetForItemType(itemType).ToList();
            if (availableAffixes.Count == 0)
            {
                Plugin.Log?.LogWarning($"No affixes available for item type {itemType}");
                return result;
            }

            var usedAffixIds = new HashSet<string>();
            var excludedIds = new HashSet<string>();

            for (int i = 0; i < affixCount && availableAffixes.Count > 0; i++)
            {
                // Filter out used and excluded affixes
                var candidates = availableAffixes
                    .Where(a => !usedAffixIds.Contains(a.Id) && !excludedIds.Contains(a.Id))
                    .ToList();

                if (candidates.Count == 0)
                    break;

                // Weighted random selection
                var selected = SelectWeighted(candidates);
                if (selected == null)
                    break;

                // Roll value
                var rolledAffix = RollAffix(selected, rarity);
                result.Add(rolledAffix);

                // Mark as used and add exclusions
                usedAffixIds.Add(selected.Id);
                if (selected.Exclusive != null)
                {
                    foreach (var excludeId in selected.Exclusive)
                        excludedIds.Add(excludeId);
                }
            }

            return result;
        }

        /// <summary>
        /// Rolls a single affix with a value appropriate for the rarity.
        /// </summary>
        public static RolledAffix RollAffix(AffixDefinition definition, Rarity rarity)
        {
            var minValue = definition.GetMinValue(rarity);
            var maxValue = definition.GetMaxValue(rarity);

            // Roll a value between min and max
            var value = minValue + (float)_random.NextDouble() * (maxValue - minValue);

            // Round based on modifier type
            if (definition.Type == ModifierType.Flat)
            {
                value = (float)Math.Round(value);
            }
            else
            {
                // Round to 2 decimal places for percent/multiplicative
                value = (float)Math.Round(value, 2);
            }

            return new RolledAffix
            {
                Definition = definition,
                Value = value,
                Tier = rarity
            };
        }

        /// <summary>
        /// Selects an affix using weighted random selection.
        /// </summary>
        private static AffixDefinition SelectWeighted(List<AffixDefinition> candidates)
        {
            if (candidates == null || candidates.Count == 0)
                return null;

            var totalWeight = candidates.Sum(a => a.Weight);
            var roll = (float)_random.NextDouble() * totalWeight;
            var cumulative = 0f;

            foreach (var candidate in candidates)
            {
                cumulative += candidate.Weight;
                if (roll <= cumulative)
                    return candidate;
            }

            return candidates[candidates.Count - 1];
        }
    }
}
