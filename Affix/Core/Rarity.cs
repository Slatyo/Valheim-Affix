namespace Affix.Core
{
    /// <summary>
    /// Rarity tiers for items with affixes.
    /// </summary>
    public enum Rarity
    {
        Common,      // White - 0-2 affixes
        Uncommon,    // Green - 3-4 affixes
        Rare,        // Blue - 5 affixes
        Epic,        // Purple - 6 affixes
        Legendary    // Orange - 6 affixes + unique effect
    }

    /// <summary>
    /// Configuration for a rarity tier.
    /// </summary>
    public class RarityConfig
    {
        /// <summary>
        /// Minimum number of affixes for this rarity.
        /// </summary>
        public int MinAffixes { get; set; }

        /// <summary>
        /// Maximum number of affixes for this rarity.
        /// </summary>
        public int MaxAffixes { get; set; }

        /// <summary>
        /// Whether items of this rarity can have unique effects (Legendary only by default).
        /// </summary>
        public bool HasUniqueEffect { get; set; }

        /// <summary>
        /// Weight for drop chance calculations. Higher = more common.
        /// </summary>
        public float DropWeight { get; set; }

        /// <summary>
        /// Display color for this rarity (hex format without #).
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Localized display name key.
        /// </summary>
        public string DisplayName { get; set; }
    }
}
