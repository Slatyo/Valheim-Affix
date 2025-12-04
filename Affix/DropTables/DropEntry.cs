namespace Affix.DropTables
{
    /// <summary>
    /// Defines how rarity is determined for a dropped item.
    /// </summary>
    public enum RarityRollType
    {
        /// <summary>
        /// Use weighted random selection based on RarityManager config.
        /// </summary>
        Weighted,

        /// <summary>
        /// Fixed rarity, always drops at the specified tier.
        /// </summary>
        Fixed,

        /// <summary>
        /// Weighted random but with a minimum rarity tier.
        /// </summary>
        Minimum
    }

    /// <summary>
    /// Configuration for rarity rolling on a drop entry.
    /// </summary>
    public class RarityRoll
    {
        public RarityRollType Type { get; set; } = RarityRollType.Weighted;
        public Core.Rarity Rarity { get; set; } = Core.Rarity.Common;

        /// <summary>
        /// Use standard weighted rarity roll.
        /// </summary>
        public static RarityRoll Weighted => new() { Type = RarityRollType.Weighted };

        /// <summary>
        /// Drop at a fixed rarity.
        /// </summary>
        public static RarityRoll Fixed(Core.Rarity rarity) => new() { Type = RarityRollType.Fixed, Rarity = rarity };

        /// <summary>
        /// Roll rarity with a minimum tier.
        /// </summary>
        public static RarityRoll Minimum(Core.Rarity minimum) => new() { Type = RarityRollType.Minimum, Rarity = minimum };
    }

    /// <summary>
    /// A single entry in a drop table.
    /// </summary>
    public class DropEntry
    {
        /// <summary>
        /// Item prefab name to drop (e.g., "SwordSilver").
        /// Use special tokens like "$WEAPON" for random item types.
        /// </summary>
        public string Item { get; set; }

        /// <summary>
        /// Chance to drop (0.0 to 1.0).
        /// </summary>
        public float Chance { get; set; } = 1f;

        /// <summary>
        /// Minimum stack size.
        /// </summary>
        public int Min { get; set; } = 1;

        /// <summary>
        /// Maximum stack size.
        /// </summary>
        public int Max { get; set; } = 1;

        /// <summary>
        /// How to roll rarity for this item. Null means no affixes.
        /// </summary>
        public RarityRoll Rarity { get; set; }

        /// <summary>
        /// Creates a drop entry for a specific item.
        /// </summary>
        public DropEntry(string item, float chance = 1f, int min = 1, int max = 1, RarityRoll rarity = null)
        {
            Item = item;
            Chance = chance;
            Min = min;
            Max = max;
            Rarity = rarity;
        }

        /// <summary>
        /// Default constructor for serialization.
        /// </summary>
        public DropEntry() { }
    }
}
