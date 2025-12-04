namespace Affix.Core
{
    /// <summary>
    /// Registers all default affixes that map to Prime stats.
    /// </summary>
    public static class DefaultAffixes
    {
        public static void RegisterAll()
        {
            RegisterOffenseAffixes();
            RegisterDefenseAffixes();
            RegisterUtilityAffixes();
        }

        private static void RegisterOffenseAffixes()
        {
            // Physical Damage
            AffixRegistry.Register(new AffixDefinition
            {
                Id = "affix_physical_damage",
                Name = "Keen",
                Description = "+{0} Physical Damage",
                PrimeStat = "PhysicalDamage",
                Type = ModifierType.Flat,
                TierValues = new[] { 2f, 5f, 8f, 12f, 15f },
                AllowedTypes = ItemType.Weapon,
                Weight = 100,
                IsPrefix = true
            });

            // Fire Damage
            AffixRegistry.Register(new AffixDefinition
            {
                Id = "affix_fire_damage",
                Name = "Blazing",
                Description = "+{0} Fire Damage",
                PrimeStat = "FireDamage",
                Type = ModifierType.Flat,
                TierValues = new[] { 3f, 6f, 10f, 13f, 15f },
                AllowedTypes = ItemType.Weapon,
                Weight = 100,
                IsPrefix = true
            });

            // Frost Damage
            AffixRegistry.Register(new AffixDefinition
            {
                Id = "affix_frost_damage",
                Name = "Frozen",
                Description = "+{0} Frost Damage",
                PrimeStat = "FrostDamage",
                Type = ModifierType.Flat,
                TierValues = new[] { 3f, 6f, 10f, 13f, 15f },
                AllowedTypes = ItemType.Weapon,
                Weight = 100,
                IsPrefix = true
            });

            // Lightning Damage
            AffixRegistry.Register(new AffixDefinition
            {
                Id = "affix_lightning_damage",
                Name = "Shocking",
                Description = "+{0} Lightning Damage",
                PrimeStat = "LightningDamage",
                Type = ModifierType.Flat,
                TierValues = new[] { 3f, 6f, 10f, 13f, 15f },
                AllowedTypes = ItemType.Weapon,
                Weight = 100,
                IsPrefix = true
            });

            // Poison Damage
            AffixRegistry.Register(new AffixDefinition
            {
                Id = "affix_poison_damage",
                Name = "Venomous",
                Description = "+{0} Poison Damage",
                PrimeStat = "PoisonDamage",
                Type = ModifierType.Flat,
                TierValues = new[] { 3f, 6f, 10f, 13f, 15f },
                AllowedTypes = ItemType.Weapon,
                Weight = 100,
                IsPrefix = true
            });

            // Spirit Damage
            AffixRegistry.Register(new AffixDefinition
            {
                Id = "affix_spirit_damage",
                Name = "Holy",
                Description = "+{0} Spirit Damage",
                PrimeStat = "SpiritDamage",
                Type = ModifierType.Flat,
                TierValues = new[] { 3f, 6f, 10f, 13f, 15f },
                AllowedTypes = ItemType.Weapon,
                Weight = 80, // Slightly rarer
                IsPrefix = true
            });

            // Attack Speed
            AffixRegistry.Register(new AffixDefinition
            {
                Id = "affix_attack_speed",
                Name = "Swift",
                Description = "+{0}% Attack Speed",
                PrimeStat = "AttackSpeed",
                Type = ModifierType.Percent,
                TierValues = new[] { 0.03f, 0.06f, 0.10f, 0.13f, 0.15f },
                AllowedTypes = ItemType.Weapon,
                Weight = 80,
                IsPrefix = true
            });

            // Crit Chance
            AffixRegistry.Register(new AffixDefinition
            {
                Id = "affix_crit_chance",
                Name = "Precise",
                Description = "+{0}% Crit Chance",
                PrimeStat = "CritChance",
                Type = ModifierType.Flat, // CritChance is stored as 0-1 but displayed as %
                TierValues = new[] { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f },
                AllowedTypes = ItemType.Weapon,
                Weight = 70,
                IsPrefix = true
            });

            // Crit Damage
            AffixRegistry.Register(new AffixDefinition
            {
                Id = "affix_crit_damage",
                Name = "Deadly",
                Description = "+{0}% Crit Damage",
                PrimeStat = "CritDamage",
                Type = ModifierType.Percent,
                TierValues = new[] { 0.10f, 0.20f, 0.30f, 0.40f, 0.50f },
                AllowedTypes = ItemType.Weapon,
                Weight = 70,
                IsPrefix = false // Suffix
            });
        }

        private static void RegisterDefenseAffixes()
        {
            // Max Health
            AffixRegistry.Register(new AffixDefinition
            {
                Id = "affix_max_health",
                Name = "Vital",
                Description = "+{0} Health",
                PrimeStat = "MaxHealth",
                Type = ModifierType.Flat,
                TierValues = new[] { 5f, 15f, 30f, 45f, 50f },
                AllowedTypes = ItemType.Armor | ItemType.Accessory,
                Weight = 100,
                IsPrefix = true
            });

            // Armor
            AffixRegistry.Register(new AffixDefinition
            {
                Id = "affix_armor",
                Name = "Sturdy",
                Description = "+{0} Armor",
                PrimeStat = "Armor",
                Type = ModifierType.Flat,
                TierValues = new[] { 2f, 5f, 10f, 15f, 20f },
                AllowedTypes = ItemType.Armor | ItemType.Shield,
                Weight = 100,
                IsPrefix = true
            });

            // Max Stamina
            AffixRegistry.Register(new AffixDefinition
            {
                Id = "affix_max_stamina",
                Name = "Enduring",
                Description = "+{0} Stamina",
                PrimeStat = "MaxStamina",
                Type = ModifierType.Flat,
                TierValues = new[] { 5f, 10f, 20f, 30f, 40f },
                AllowedTypes = ItemType.Armor | ItemType.Accessory,
                Weight = 90,
                IsPrefix = true
            });

            // Fire Resist
            AffixRegistry.Register(new AffixDefinition
            {
                Id = "affix_fire_resist",
                Name = "Fireproof",
                Description = "+{0}% Fire Resist",
                PrimeStat = "FireResist",
                Type = ModifierType.Flat,
                TierValues = new[] { 0.05f, 0.10f, 0.15f, 0.20f, 0.25f },
                AllowedTypes = ItemType.Armor | ItemType.Accessory | ItemType.Shield,
                Weight = 80,
                IsPrefix = false
            });

            // Frost Resist
            AffixRegistry.Register(new AffixDefinition
            {
                Id = "affix_frost_resist",
                Name = "Insulated",
                Description = "+{0}% Frost Resist",
                PrimeStat = "FrostResist",
                Type = ModifierType.Flat,
                TierValues = new[] { 0.05f, 0.10f, 0.15f, 0.20f, 0.25f },
                AllowedTypes = ItemType.Armor | ItemType.Accessory | ItemType.Shield,
                Weight = 80,
                IsPrefix = false
            });

            // Lightning Resist
            AffixRegistry.Register(new AffixDefinition
            {
                Id = "affix_lightning_resist",
                Name = "Grounded",
                Description = "+{0}% Lightning Resist",
                PrimeStat = "LightningResist",
                Type = ModifierType.Flat,
                TierValues = new[] { 0.05f, 0.10f, 0.15f, 0.20f, 0.25f },
                AllowedTypes = ItemType.Armor | ItemType.Accessory | ItemType.Shield,
                Weight = 80,
                IsPrefix = false
            });

            // Poison Resist
            AffixRegistry.Register(new AffixDefinition
            {
                Id = "affix_poison_resist",
                Name = "Antitoxin",
                Description = "+{0}% Poison Resist",
                PrimeStat = "PoisonResist",
                Type = ModifierType.Flat,
                TierValues = new[] { 0.05f, 0.10f, 0.15f, 0.20f, 0.25f },
                AllowedTypes = ItemType.Armor | ItemType.Accessory | ItemType.Shield,
                Weight = 80,
                IsPrefix = false
            });

            // Spirit Resist
            AffixRegistry.Register(new AffixDefinition
            {
                Id = "affix_spirit_resist",
                Name = "Warding",
                Description = "+{0}% Spirit Resist",
                PrimeStat = "SpiritResist",
                Type = ModifierType.Flat,
                TierValues = new[] { 0.05f, 0.10f, 0.15f, 0.20f, 0.25f },
                AllowedTypes = ItemType.Armor | ItemType.Accessory | ItemType.Shield,
                Weight = 70,
                IsPrefix = false
            });
        }

        private static void RegisterUtilityAffixes()
        {
            // Move Speed
            AffixRegistry.Register(new AffixDefinition
            {
                Id = "affix_move_speed",
                Name = "Fleet",
                Description = "+{0}% Move Speed",
                PrimeStat = "MoveSpeed",
                Type = ModifierType.Percent,
                TierValues = new[] { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f },
                AllowedTypes = ItemType.Armor | ItemType.Accessory,
                Weight = 60,
                IsPrefix = true
            });

            // Carry Weight
            AffixRegistry.Register(new AffixDefinition
            {
                Id = "affix_carry_weight",
                Name = "Burdened",
                Description = "+{0} Carry Weight",
                PrimeStat = "CarryWeight",
                Type = ModifierType.Flat,
                TierValues = new[] { 25f, 50f, 75f, 100f, 150f },
                AllowedTypes = ItemType.Armor | ItemType.Accessory,
                Weight = 50,
                IsPrefix = false
            });

            // Cooldown Reduction
            AffixRegistry.Register(new AffixDefinition
            {
                Id = "affix_cooldown_reduction",
                Name = "Focused",
                Description = "+{0}% Cooldown Reduction",
                PrimeStat = "CooldownReduction",
                Type = ModifierType.Flat, // CDR is stored as 0-0.75
                TierValues = new[] { 0.03f, 0.06f, 0.10f, 0.13f, 0.15f },
                AllowedTypes = ItemType.Armor | ItemType.Accessory,
                Weight = 50,
                IsPrefix = false
            });
        }
    }
}
