using System;
using System.Collections.Generic;
using System.Linq;

namespace Affix.Core
{
    /// <summary>
    /// Keys used for storing affix data in item m_customData.
    /// </summary>
    public static class AffixDataKeys
    {
        public const string Rarity = "affix_rarity";
        public const string Affixes = "affix_data";
        public const string UniqueEffect = "affix_unique";
    }

    /// <summary>
    /// Handles reading and writing affix data to items.
    /// </summary>
    public static class AffixData
    {
        /// <summary>
        /// Checks if an item has affix data.
        /// </summary>
        public static bool HasAffixes(ItemDrop.ItemData item)
        {
            if (item?.m_customData == null) return false;
            return item.m_customData.ContainsKey(AffixDataKeys.Rarity);
        }

        /// <summary>
        /// Gets the rarity of an item with affixes.
        /// </summary>
        public static Rarity GetRarity(ItemDrop.ItemData item)
        {
            if (item?.m_customData == null) return Rarity.Common;

            if (item.m_customData.TryGetValue(AffixDataKeys.Rarity, out var rarityStr))
            {
                if (Enum.TryParse<Rarity>(rarityStr, out var rarity))
                    return rarity;
            }

            return Rarity.Common;
        }

        /// <summary>
        /// Gets all affixes on an item.
        /// </summary>
        public static List<RolledAffix> GetAffixes(ItemDrop.ItemData item)
        {
            var result = new List<RolledAffix>();
            if (item?.m_customData == null) return result;

            var rarity = GetRarity(item);

            if (item.m_customData.TryGetValue(AffixDataKeys.Affixes, out var affixData))
            {
                if (string.IsNullOrEmpty(affixData)) return result;

                var parts = affixData.Split(',');
                foreach (var part in parts)
                {
                    var affix = RolledAffix.Deserialize(part.Trim(), rarity);
                    if (affix != null)
                        result.Add(affix);
                }
            }

            return result;
        }

        /// <summary>
        /// Sets the rarity and affixes on an item.
        /// </summary>
        public static void SetAffixes(ItemDrop.ItemData item, Rarity rarity, List<RolledAffix> affixes)
        {
            if (item == null) return;

            item.m_customData ??= new Dictionary<string, string>();

            // Store rarity
            item.m_customData[AffixDataKeys.Rarity] = rarity.ToString();

            // Store affixes
            if (affixes != null && affixes.Count > 0)
            {
                var affixStrings = affixes.Select(a => a.Serialize());
                item.m_customData[AffixDataKeys.Affixes] = string.Join(",", affixStrings);
            }
            else
            {
                item.m_customData.Remove(AffixDataKeys.Affixes);
            }
        }

        /// <summary>
        /// Clears all affix data from an item.
        /// </summary>
        public static void ClearAffixes(ItemDrop.ItemData item)
        {
            if (item?.m_customData == null) return;

            item.m_customData.Remove(AffixDataKeys.Rarity);
            item.m_customData.Remove(AffixDataKeys.Affixes);
            item.m_customData.Remove(AffixDataKeys.UniqueEffect);
        }

        /// <summary>
        /// Gets the item type for affix filtering based on Valheim's ItemType.
        /// </summary>
        public static ItemType GetAffixItemType(ItemDrop.ItemData item)
        {
            if (item?.m_shared == null) return ItemType.None;

            switch (item.m_shared.m_itemType)
            {
                case ItemDrop.ItemData.ItemType.OneHandedWeapon:
                case ItemDrop.ItemData.ItemType.TwoHandedWeapon:
                case ItemDrop.ItemData.ItemType.TwoHandedWeaponLeft:
                case ItemDrop.ItemData.ItemType.Bow:
                    return ItemType.Weapon;

                case ItemDrop.ItemData.ItemType.Helmet:
                case ItemDrop.ItemData.ItemType.Chest:
                case ItemDrop.ItemData.ItemType.Legs:
                case ItemDrop.ItemData.ItemType.Shoulder:
                    return ItemType.Armor;

                case ItemDrop.ItemData.ItemType.Shield:
                    return ItemType.Shield;

                case ItemDrop.ItemData.ItemType.Tool:
                    return ItemType.Tool;

                case ItemDrop.ItemData.ItemType.Utility:
                    return ItemType.Accessory;

                default:
                    return ItemType.None;
            }
        }

        /// <summary>
        /// Generates a display name for an item with affixes.
        /// Format: "[Prefix] ItemName [of Suffix]"
        /// </summary>
        public static string GenerateDisplayName(string baseName, List<RolledAffix> affixes)
        {
            if (affixes == null || affixes.Count == 0)
                return baseName;

            var prefix = affixes.FirstOrDefault(a => a.Definition?.IsPrefix == true);
            var suffix = affixes.FirstOrDefault(a => a.Definition?.IsPrefix == false);

            var name = baseName;

            if (prefix != null)
                name = $"{prefix.Definition.Name} {name}";

            if (suffix != null)
                name = $"{name} of {suffix.Definition.Name}";

            return name;
        }
    }
}
