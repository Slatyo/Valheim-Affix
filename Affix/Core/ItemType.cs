using System;

namespace Affix.Core
{
    /// <summary>
    /// Item types that can have affixes.
    /// </summary>
    [Flags]
    public enum ItemType
    {
        None = 0,
        Weapon = 1 << 0,
        Armor = 1 << 1,
        Accessory = 1 << 2,
        Shield = 1 << 3,
        Tool = 1 << 4,
        All = Weapon | Armor | Accessory | Shield | Tool
    }

    /// <summary>
    /// Modifier type for affix values.
    /// </summary>
    public enum ModifierType
    {
        /// <summary>
        /// Flat value added to base stat (e.g., +10 damage).
        /// </summary>
        Flat,

        /// <summary>
        /// Percentage of base stat (e.g., +10% = 0.10).
        /// </summary>
        Percent,

        /// <summary>
        /// Multiplier applied after other calculations (e.g., 1.1x).
        /// </summary>
        Multiplicative
    }
}
