using Prime.Procs;

namespace Affix.Core
{
    /// <summary>
    /// Defines a unique ability that can roll on Legendary items.
    /// Contains the Prime ability ID, trigger conditions, and item restrictions.
    /// </summary>
    public class LegendaryAbilityDefinition
    {
        /// <summary>
        /// Unique identifier for this legendary ability (e.g., "frozen_fury").
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The Prime ability ID to execute when triggered (e.g., "FrostNova").
        /// </summary>
        public string AbilityId { get; set; }

        /// <summary>
        /// Display name shown in tooltips (e.g., "Frozen Fury").
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Description shown in tooltips (e.g., "25% chance on hit to freeze nearby enemies").
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// What combat event triggers this ability.
        /// Maps to Prime's ProcTrigger enum.
        /// </summary>
        public ProcTrigger Trigger { get; set; }

        /// <summary>
        /// Chance to proc (0-1). 0 = always proc if conditions are met.
        /// </summary>
        public float ProcChance { get; set; }

        /// <summary>
        /// Minimum seconds between procs to prevent spam.
        /// </summary>
        public float InternalCooldown { get; set; }

        /// <summary>
        /// Target health threshold (0-1). Proc only if target HP below this.
        /// 0 = no threshold check.
        /// </summary>
        public float TargetHealthThreshold { get; set; }

        /// <summary>
        /// Owner health threshold (0-1). Proc only if owner HP below this.
        /// 0 = no threshold check.
        /// </summary>
        public float OwnerHealthThreshold { get; set; }

        /// <summary>
        /// Multiplier for ability damage (1.0 = normal, 0.5 = half damage).
        /// Use to balance powerful abilities that are guaranteed to proc.
        /// </summary>
        public float DamageMultiplier { get; set; } = 1.0f;

        /// <summary>
        /// Valid weapon types this ability can roll on.
        /// Null or empty = not valid for weapons.
        /// </summary>
        public WeaponType[] ValidWeaponTypes { get; set; }

        /// <summary>
        /// Valid armor slots this ability can roll on.
        /// Null or empty = not valid for armor.
        /// </summary>
        public ArmorSlot[] ValidArmorSlots { get; set; }

        /// <summary>
        /// Weight for random selection. Higher = more likely to be chosen.
        /// </summary>
        public int Weight { get; set; } = 100;

        /// <summary>
        /// Creates a Prime ItemProcConfig from this definition.
        /// </summary>
        public ItemProcConfig ToProcConfig()
        {
            return new ItemProcConfig
            {
                AbilityId = AbilityId,
                Trigger = Trigger,
                ProcChance = ProcChance,
                InternalCooldown = InternalCooldown,
                TargetHealthThreshold = TargetHealthThreshold,
                OwnerHealthThreshold = OwnerHealthThreshold,
                DamageMultiplier = DamageMultiplier,
                SkipResourceCost = true, // Legendary procs are always free
                DisplayName = DisplayName,
                Description = Description
            };
        }

        /// <summary>
        /// Checks if this ability is valid for a weapon type.
        /// </summary>
        public bool IsValidForWeapon(WeaponType weaponType)
        {
            if (ValidWeaponTypes == null || ValidWeaponTypes.Length == 0)
                return false;

            foreach (var valid in ValidWeaponTypes)
            {
                if (valid == WeaponType.Any || valid == weaponType)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if this ability is valid for an armor slot.
        /// </summary>
        public bool IsValidForArmor(ArmorSlot armorSlot)
        {
            if (ValidArmorSlots == null || ValidArmorSlots.Length == 0)
                return false;

            foreach (var valid in ValidArmorSlots)
            {
                if (valid == ArmorSlot.Any || valid == armorSlot)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a formatted tooltip string for this ability.
        /// </summary>
        public string GetTooltipText()
        {
            return $"<color=#FF8000>[Unique]</color> {DisplayName}\n{Description}";
        }
    }

    /// <summary>
    /// Weapon types for filtering which legendary abilities can roll on which weapons.
    /// </summary>
    public enum WeaponType
    {
        /// <summary>One-handed swords.</summary>
        Sword,
        /// <summary>One-handed and two-handed axes.</summary>
        Axe,
        /// <summary>Maces and blunt weapons.</summary>
        Mace,
        /// <summary>Spears and polearms.</summary>
        Spear,
        /// <summary>Knives and daggers.</summary>
        Knife,
        /// <summary>Bows.</summary>
        Bow,
        /// <summary>Crossbows.</summary>
        Crossbow,
        /// <summary>Staves and magic weapons.</summary>
        Staff,
        /// <summary>Two-handed swords and great weapons.</summary>
        TwoHanded,
        /// <summary>Shields (for block-based procs).</summary>
        Shield,
        /// <summary>Any weapon type.</summary>
        Any
    }

    /// <summary>
    /// Armor slots for filtering which legendary abilities can roll on which armor.
    /// </summary>
    public enum ArmorSlot
    {
        /// <summary>Helmets and head gear.</summary>
        Helmet,
        /// <summary>Chest armor.</summary>
        Chest,
        /// <summary>Leg armor.</summary>
        Legs,
        /// <summary>Capes and shoulder items.</summary>
        Cape,
        /// <summary>Utility slot items.</summary>
        Utility,
        /// <summary>Any armor slot.</summary>
        Any
    }
}
