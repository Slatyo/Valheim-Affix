namespace Affix.Core
{
    /// <summary>
    /// Defines an affix that can roll on items.
    /// Affixes map directly to Prime stats.
    /// </summary>
    public class AffixDefinition
    {
        /// <summary>
        /// Unique identifier for this affix (e.g., "affix_fire_damage").
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Display name prefix/suffix (e.g., "Blazing").
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description format string (e.g., "+{0} Fire Damage").
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The Prime stat this affix modifies (e.g., "FireDamage").
        /// </summary>
        public string PrimeStat { get; set; }

        /// <summary>
        /// How this affix modifies the stat.
        /// </summary>
        public ModifierType Type { get; set; } = ModifierType.Flat;

        /// <summary>
        /// Minimum value per rarity tier (indexed by Rarity enum).
        /// [0]=Common, [1]=Uncommon, [2]=Rare, [3]=Epic, [4]=Legendary
        /// </summary>
        public float[] TierValues { get; set; }

        /// <summary>
        /// Item types this affix can appear on.
        /// </summary>
        public ItemType AllowedTypes { get; set; } = ItemType.All;

        /// <summary>
        /// Weight for random selection. Higher = more likely.
        /// </summary>
        public float Weight { get; set; } = 100f;

        /// <summary>
        /// Affix IDs that cannot appear alongside this one.
        /// </summary>
        public string[] Exclusive { get; set; }

        /// <summary>
        /// Whether this is a prefix (true) or suffix (false).
        /// </summary>
        public bool IsPrefix { get; set; } = true;

        /// <summary>
        /// Gets the minimum value for a given rarity tier.
        /// </summary>
        public float GetMinValue(Rarity rarity)
        {
            var index = (int)rarity;
            if (TierValues == null || index >= TierValues.Length)
                return 0f;
            return TierValues[index];
        }

        /// <summary>
        /// Gets the maximum value for a given rarity tier.
        /// For the highest tier in TierValues, uses the value directly.
        /// For lower tiers, max is the next tier's min value.
        /// </summary>
        public float GetMaxValue(Rarity rarity)
        {
            var index = (int)rarity;
            if (TierValues == null || index >= TierValues.Length)
                return 0f;

            // If we're at the highest defined tier, return that value
            if (index >= TierValues.Length - 1)
                return TierValues[TierValues.Length - 1];

            // Otherwise max is the next tier's min
            return TierValues[index + 1];
        }
    }
}
