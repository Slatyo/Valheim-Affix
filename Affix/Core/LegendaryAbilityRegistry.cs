using System.Collections.Generic;
using System.Linq;
using Prime.Procs;

namespace Affix.Core
{
    /// <summary>
    /// Registry of all legendary abilities that can roll on items.
    /// Provides methods for getting random abilities filtered by weapon/armor type.
    /// </summary>
    public class LegendaryAbilityRegistry
    {
        private static LegendaryAbilityRegistry _instance;

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static LegendaryAbilityRegistry Instance => _instance ??= new LegendaryAbilityRegistry();

        private readonly Dictionary<string, LegendaryAbilityDefinition> _abilities = new();
        private readonly System.Random _random = new();
        private bool _initialized;

        private LegendaryAbilityRegistry()
        {
        }

        /// <summary>
        /// Initialize the registry with default legendary abilities.
        /// </summary>
        public void Initialize()
        {
            if (_initialized) return;

            RegisterMeleeAbilities();
            RegisterRangedAbilities();
            RegisterMagicAbilities();
            RegisterShieldAbilities();
            RegisterArmorAbilities();
            RegisterUniversalAbilities();

            _initialized = true;
            Plugin.Log?.LogInfo($"[Affix] LegendaryAbilityRegistry initialized with {_abilities.Count} abilities");
        }

        /// <summary>
        /// Register a legendary ability definition.
        /// </summary>
        public void Register(LegendaryAbilityDefinition definition)
        {
            if (definition == null || string.IsNullOrEmpty(definition.Id))
            {
                Plugin.Log?.LogWarning("[Affix] Attempted to register null or invalid legendary ability");
                return;
            }

            _abilities[definition.Id] = definition;
            Plugin.Log?.LogDebug($"[Affix] Registered legendary ability: {definition.Id}");
        }

        /// <summary>
        /// Get a legendary ability by ID.
        /// </summary>
        public LegendaryAbilityDefinition Get(string id)
        {
            return _abilities.TryGetValue(id, out var def) ? def : null;
        }

        /// <summary>
        /// Get all registered legendary abilities.
        /// </summary>
        public IEnumerable<LegendaryAbilityDefinition> GetAll()
        {
            return _abilities.Values;
        }

        /// <summary>
        /// Get a random legendary ability valid for a weapon type.
        /// </summary>
        public LegendaryAbilityDefinition GetRandomForWeapon(WeaponType weaponType)
        {
            var candidates = _abilities.Values
                .Where(a => a.IsValidForWeapon(weaponType))
                .ToList();

            return SelectWeighted(candidates);
        }

        /// <summary>
        /// Get a random legendary ability valid for an armor slot.
        /// </summary>
        public LegendaryAbilityDefinition GetRandomForArmor(ArmorSlot armorSlot)
        {
            var candidates = _abilities.Values
                .Where(a => a.IsValidForArmor(armorSlot))
                .ToList();

            return SelectWeighted(candidates);
        }

        /// <summary>
        /// Get a random legendary ability for an item based on its type.
        /// </summary>
        public LegendaryAbilityDefinition GetRandomForItem(ItemDrop.ItemData item)
        {
            if (item?.m_shared == null) return null;

            var weaponType = GetWeaponType(item);
            if (weaponType != null)
                return GetRandomForWeapon(weaponType.Value);

            var armorSlot = GetArmorSlot(item);
            if (armorSlot != null)
                return GetRandomForArmor(armorSlot.Value);

            return null;
        }

        /// <summary>
        /// Determine the weapon type for an item.
        /// </summary>
        public static WeaponType? GetWeaponType(ItemDrop.ItemData item)
        {
            if (item?.m_shared == null) return null;

            var itemType = item.m_shared.m_itemType;
            var skillType = item.m_shared.m_skillType;

            // Shields
            if (itemType == ItemDrop.ItemData.ItemType.Shield)
                return WeaponType.Shield;

            // Check if it's a weapon
            if (itemType != ItemDrop.ItemData.ItemType.OneHandedWeapon &&
                itemType != ItemDrop.ItemData.ItemType.TwoHandedWeapon &&
                itemType != ItemDrop.ItemData.ItemType.TwoHandedWeaponLeft &&
                itemType != ItemDrop.ItemData.ItemType.Bow)
                return null;

            // Map by skill type
            return skillType switch
            {
                Skills.SkillType.Swords => WeaponType.Sword,
                Skills.SkillType.Axes => WeaponType.Axe,
                Skills.SkillType.Clubs => WeaponType.Mace,
                Skills.SkillType.Polearms => WeaponType.Spear,
                Skills.SkillType.Knives => WeaponType.Knife,
                Skills.SkillType.Bows => WeaponType.Bow,
                Skills.SkillType.Crossbows => WeaponType.Crossbow,
                Skills.SkillType.ElementalMagic => WeaponType.Staff,
                Skills.SkillType.BloodMagic => WeaponType.Staff,
                _ => itemType == ItemDrop.ItemData.ItemType.TwoHandedWeapon ? WeaponType.TwoHanded : WeaponType.Sword
            };
        }

        /// <summary>
        /// Determine the armor slot for an item.
        /// </summary>
        public static ArmorSlot? GetArmorSlot(ItemDrop.ItemData item)
        {
            if (item?.m_shared == null) return null;

            return item.m_shared.m_itemType switch
            {
                ItemDrop.ItemData.ItemType.Helmet => ArmorSlot.Helmet,
                ItemDrop.ItemData.ItemType.Chest => ArmorSlot.Chest,
                ItemDrop.ItemData.ItemType.Legs => ArmorSlot.Legs,
                ItemDrop.ItemData.ItemType.Shoulder => ArmorSlot.Cape,
                ItemDrop.ItemData.ItemType.Utility => ArmorSlot.Utility,
                _ => null
            };
        }

        private LegendaryAbilityDefinition SelectWeighted(List<LegendaryAbilityDefinition> candidates)
        {
            if (candidates == null || candidates.Count == 0)
                return null;

            var totalWeight = candidates.Sum(a => a.Weight);
            var roll = _random.NextDouble() * totalWeight;
            var cumulative = 0.0;

            foreach (var candidate in candidates)
            {
                cumulative += candidate.Weight;
                if (roll <= cumulative)
                    return candidate;
            }

            return candidates[candidates.Count - 1];
        }

        // ==================== MELEE WEAPON ABILITIES ====================

        private void RegisterMeleeAbilities()
        {
            Register(new LegendaryAbilityDefinition
            {
                Id = "frozen_fury",
                AbilityId = "FrostNova",
                DisplayName = "Frozen Fury",
                Description = "25% chance on hit to freeze nearby enemies",
                Trigger = ProcTrigger.OnHit,
                ProcChance = 0.25f,
                InternalCooldown = 10f,
                ValidWeaponTypes = new[] { WeaponType.Sword, WeaponType.Axe, WeaponType.Mace, WeaponType.TwoHanded },
                Weight = 100
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "executioner",
                AbilityId = "Execute",
                DisplayName = "Executioner",
                Description = "Massive bonus damage when target below 30% health",
                Trigger = ProcTrigger.OnHit,
                ProcChance = 1.0f,
                TargetHealthThreshold = 0.3f,
                InternalCooldown = 5f,
                ValidWeaponTypes = new[] { WeaponType.Sword, WeaponType.Axe, WeaponType.TwoHanded },
                Weight = 80
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "bladestorm",
                AbilityId = "Whirlwind",
                DisplayName = "Bladestorm",
                Description = "15% chance on hit to unleash a whirlwind attack",
                Trigger = ProcTrigger.OnHit,
                ProcChance = 0.15f,
                InternalCooldown = 12f,
                ValidWeaponTypes = new[] { WeaponType.Sword, WeaponType.Axe, WeaponType.TwoHanded },
                Weight = 90
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "earthshatter",
                AbilityId = "GroundSlam",
                DisplayName = "Earthshatter",
                Description = "Critical hits cause a ground slam damaging nearby enemies",
                Trigger = ProcTrigger.OnCrit,
                ProcChance = 1.0f,
                InternalCooldown = 15f,
                DamageMultiplier = 0.75f,
                ValidWeaponTypes = new[] { WeaponType.Mace, WeaponType.TwoHanded },
                Weight = 70
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "berserkers_rage",
                AbilityId = "BerserkerRage",
                DisplayName = "Berserker's Rage",
                Description = "When below 25% health, enter a powerful berserk state",
                Trigger = ProcTrigger.OnLowHealth,
                ProcChance = 1.0f,
                OwnerHealthThreshold = 0.25f,
                InternalCooldown = 60f,
                ValidWeaponTypes = new[] { WeaponType.Axe, WeaponType.TwoHanded },
                Weight = 60
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "venomous_edge",
                AbilityId = "PoisonSpit",
                DisplayName = "Venomous Edge",
                Description = "20% chance on hit to poison the target",
                Trigger = ProcTrigger.OnHit,
                ProcChance = 0.20f,
                InternalCooldown = 8f,
                ValidWeaponTypes = new[] { WeaponType.Knife, WeaponType.Spear },
                Weight = 100
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "shadow_strike",
                AbilityId = "ShadowStrike",
                DisplayName = "Shadow Strike",
                Description = "Critical hits deal bonus shadow damage",
                Trigger = ProcTrigger.OnCrit,
                ProcChance = 1.0f,
                InternalCooldown = 6f,
                ValidWeaponTypes = new[] { WeaponType.Knife },
                Weight = 90
            });
        }

        // ==================== RANGED WEAPON ABILITIES ====================

        private void RegisterRangedAbilities()
        {
            Register(new LegendaryAbilityDefinition
            {
                Id = "storms_reach",
                AbilityId = "ChainLightning",
                DisplayName = "Storm's Reach",
                Description = "20% chance on hit to chain lightning to nearby enemies",
                Trigger = ProcTrigger.OnHit,
                ProcChance = 0.20f,
                InternalCooldown = 12f,
                ValidWeaponTypes = new[] { WeaponType.Bow, WeaponType.Crossbow },
                Weight = 100
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "venomous_shot",
                AbilityId = "PoisonArrow",
                DisplayName = "Venomous Shot",
                Description = "30% chance on hit to apply deadly poison",
                Trigger = ProcTrigger.OnHit,
                ProcChance = 0.30f,
                InternalCooldown = 8f,
                ValidWeaponTypes = new[] { WeaponType.Bow, WeaponType.Crossbow },
                Weight = 100
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "piercing_strike",
                AbilityId = "PowerShot",
                DisplayName = "Piercing Strike",
                Description = "Critical hits fire a devastating power shot",
                Trigger = ProcTrigger.OnCrit,
                ProcChance = 1.0f,
                InternalCooldown = 10f,
                DamageMultiplier = 0.6f,
                ValidWeaponTypes = new[] { WeaponType.Bow, WeaponType.Crossbow },
                Weight = 80
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "predators_eye",
                AbilityId = "HuntersMark",
                DisplayName = "Predator's Eye",
                Description = "15% chance on hit to mark target for increased damage",
                Trigger = ProcTrigger.OnHit,
                ProcChance = 0.15f,
                InternalCooldown = 20f,
                ValidWeaponTypes = new[] { WeaponType.Bow, WeaponType.Crossbow },
                Weight = 70
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "evasive_shot",
                AbilityId = "EvasiveRoll",
                DisplayName = "Evasive Shot",
                Description = "When hit, 20% chance to evade and gain movement speed",
                Trigger = ProcTrigger.OnHitTaken,
                ProcChance = 0.20f,
                InternalCooldown = 8f,
                ValidWeaponTypes = new[] { WeaponType.Bow, WeaponType.Crossbow },
                Weight = 60
            });
        }

        // ==================== MAGIC WEAPON ABILITIES ====================

        private void RegisterMagicAbilities()
        {
            Register(new LegendaryAbilityDefinition
            {
                Id = "inferno",
                AbilityId = "Fireball",
                DisplayName = "Inferno",
                Description = "25% chance on hit to launch a fireball",
                Trigger = ProcTrigger.OnHit,
                ProcChance = 0.25f,
                InternalCooldown = 8f,
                ValidWeaponTypes = new[] { WeaponType.Staff },
                Weight = 100
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "meteor_strike",
                AbilityId = "Meteor",
                DisplayName = "Meteor Strike",
                Description = "Critical hits call down a devastating meteor",
                Trigger = ProcTrigger.OnCrit,
                ProcChance = 1.0f,
                InternalCooldown = 15f,
                DamageMultiplier = 0.5f,
                ValidWeaponTypes = new[] { WeaponType.Staff },
                Weight = 60
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "glacial_spike",
                AbilityId = "IceSpikes",
                DisplayName = "Glacial Spike",
                Description = "20% chance on hit to impale enemies with ice",
                Trigger = ProcTrigger.OnHit,
                ProcChance = 0.20f,
                InternalCooldown = 10f,
                ValidWeaponTypes = new[] { WeaponType.Staff },
                Weight = 90
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "thunder_strike",
                AbilityId = "LightningBolt",
                DisplayName = "Thunder Strike",
                Description = "30% chance on hit to strike with lightning",
                Trigger = ProcTrigger.OnHit,
                ProcChance = 0.30f,
                InternalCooldown = 6f,
                ValidWeaponTypes = new[] { WeaponType.Staff },
                Weight = 100
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "arcane_chain",
                AbilityId = "ChainLightning",
                DisplayName = "Arcane Chain",
                Description = "15% chance on hit to chain arcane energy between enemies",
                Trigger = ProcTrigger.OnHit,
                ProcChance = 0.15f,
                InternalCooldown = 10f,
                ValidWeaponTypes = new[] { WeaponType.Staff },
                Weight = 80
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "mana_barrier",
                AbilityId = "ManaShield",
                DisplayName = "Mana Barrier",
                Description = "When hit, activate a protective mana barrier",
                Trigger = ProcTrigger.OnHitTaken,
                ProcChance = 0f,
                InternalCooldown = 30f,
                ValidWeaponTypes = new[] { WeaponType.Staff },
                Weight = 50
            });
        }

        // ==================== SHIELD ABILITIES ====================

        private void RegisterShieldAbilities()
        {
            Register(new LegendaryAbilityDefinition
            {
                Id = "crushing_counter",
                AbilityId = "ShieldBash",
                DisplayName = "Crushing Counter",
                Description = "30% chance on block to stun the attacker",
                Trigger = ProcTrigger.OnBlock,
                ProcChance = 0.30f,
                InternalCooldown = 6f,
                ValidWeaponTypes = new[] { WeaponType.Shield },
                Weight = 100
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "frozen_barrier",
                AbilityId = "FrostNova",
                DisplayName = "Frozen Barrier",
                Description = "Blocking freezes nearby attackers",
                Trigger = ProcTrigger.OnBlock,
                ProcChance = 0f,
                InternalCooldown = 12f,
                ValidWeaponTypes = new[] { WeaponType.Shield },
                Weight = 80
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "bastions_stand",
                AbilityId = "Fortify",
                DisplayName = "Bastion's Stand",
                Description = "Blocking greatly increases your defenses",
                Trigger = ProcTrigger.OnBlock,
                ProcChance = 0f,
                InternalCooldown = 20f,
                ValidWeaponTypes = new[] { WeaponType.Shield },
                Weight = 70
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "thunderous_reprisal",
                AbilityId = "LightningStrike",
                DisplayName = "Thunderous Reprisal",
                Description = "25% chance on block to strike back with lightning",
                Trigger = ProcTrigger.OnBlock,
                ProcChance = 0.25f,
                InternalCooldown = 8f,
                ValidWeaponTypes = new[] { WeaponType.Shield },
                Weight = 90
            });
        }

        // ==================== ARMOR ABILITIES ====================

        private void RegisterArmorAbilities()
        {
            // Chest Armor
            Register(new LegendaryAbilityDefinition
            {
                Id = "retribution",
                AbilityId = "FrostNova",
                DisplayName = "Retribution",
                Description = "When struck, freeze nearby attackers",
                Trigger = ProcTrigger.OnHitTaken,
                ProcChance = 0f,
                InternalCooldown = 8f,
                ValidArmorSlots = new[] { ArmorSlot.Chest },
                Weight = 100
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "arcane_barrier",
                AbilityId = "ManaShield",
                DisplayName = "Arcane Barrier",
                Description = "When struck, gain a protective mana shield",
                Trigger = ProcTrigger.OnHitTaken,
                ProcChance = 0f,
                InternalCooldown = 30f,
                ValidArmorSlots = new[] { ArmorSlot.Chest },
                Weight = 70
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "divine_protection",
                AbilityId = "DivineShield",
                DisplayName = "Divine Protection",
                Description = "When below 50% health, gain divine armor",
                Trigger = ProcTrigger.OnHitTaken,
                ProcChance = 1.0f,
                OwnerHealthThreshold = 0.5f,
                InternalCooldown = 25f,
                ValidArmorSlots = new[] { ArmorSlot.Chest },
                Weight = 60
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "iron_fortress",
                AbilityId = "IronSkin",
                DisplayName = "Iron Fortress",
                Description = "When struck, temporarily harden your skin",
                Trigger = ProcTrigger.OnHitTaken,
                ProcChance = 0.20f,
                InternalCooldown = 15f,
                ValidArmorSlots = new[] { ArmorSlot.Chest },
                Weight = 80
            });

            // Helmet
            Register(new LegendaryAbilityDefinition
            {
                Id = "killer_instinct",
                AbilityId = "BattleFocus",
                DisplayName = "Killer Instinct",
                Description = "On kill, gain increased critical chance",
                Trigger = ProcTrigger.OnKill,
                ProcChance = 1.0f,
                InternalCooldown = 10f,
                ValidArmorSlots = new[] { ArmorSlot.Helmet },
                Weight = 100
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "adrenaline_rush",
                AbilityId = "SecondWind",
                DisplayName = "Adrenaline Rush",
                Description = "When below 30% health, recover stamina rapidly",
                Trigger = ProcTrigger.OnLowHealth,
                ProcChance = 1.0f,
                OwnerHealthThreshold = 0.3f,
                InternalCooldown = 45f,
                ValidArmorSlots = new[] { ArmorSlot.Helmet },
                Weight = 70
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "warcry",
                AbilityId = "WarCry",
                DisplayName = "War Cry",
                Description = "On kill, buff yourself and nearby allies",
                Trigger = ProcTrigger.OnKill,
                ProcChance = 0.5f,
                InternalCooldown = 15f,
                ValidArmorSlots = new[] { ArmorSlot.Helmet },
                Weight = 60
            });

            // Legs
            Register(new LegendaryAbilityDefinition
            {
                Id = "windrunner",
                AbilityId = "Sprint",
                DisplayName = "Windrunner",
                Description = "On kill, gain a burst of movement speed",
                Trigger = ProcTrigger.OnKill,
                ProcChance = 1.0f,
                InternalCooldown = 15f,
                ValidArmorSlots = new[] { ArmorSlot.Legs },
                Weight = 100
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "lightning_reflexes",
                AbilityId = "EvasiveRoll",
                DisplayName = "Lightning Reflexes",
                Description = "20% chance when hit to dodge incoming damage",
                Trigger = ProcTrigger.OnHitTaken,
                ProcChance = 0.20f,
                InternalCooldown = 8f,
                ValidArmorSlots = new[] { ArmorSlot.Legs },
                Weight = 80
            });

            // Cape
            Register(new LegendaryAbilityDefinition
            {
                Id = "shadow_cloak",
                AbilityId = "EvasiveRoll",
                DisplayName = "Shadow Cloak",
                Description = "15% chance when hit to vanish and evade",
                Trigger = ProcTrigger.OnHitTaken,
                ProcChance = 0.15f,
                InternalCooldown = 10f,
                ValidArmorSlots = new[] { ArmorSlot.Cape },
                Weight = 90
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "flowing_winds",
                AbilityId = "Sprint",
                DisplayName = "Flowing Winds",
                Description = "Gain movement speed after defeating enemies",
                Trigger = ProcTrigger.OnKill,
                ProcChance = 0.75f,
                InternalCooldown = 20f,
                ValidArmorSlots = new[] { ArmorSlot.Cape },
                Weight = 70
            });
        }

        // ==================== UNIVERSAL ABILITIES ====================

        private void RegisterUniversalAbilities()
        {
            Register(new LegendaryAbilityDefinition
            {
                Id = "soul_harvest",
                AbilityId = "GladiatorsGlory",
                DisplayName = "Soul Harvest",
                Description = "On kill, restore health and gain damage",
                Trigger = ProcTrigger.OnKill,
                ProcChance = 1.0f,
                InternalCooldown = 3f,
                ValidWeaponTypes = new[] { WeaponType.Any },
                Weight = 80
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "soul_siphon",
                AbilityId = "SpiritDrain",
                DisplayName = "Soul Siphon",
                Description = "10% chance on hit to drain life from enemies",
                Trigger = ProcTrigger.OnHit,
                ProcChance = 0.10f,
                InternalCooldown = 12f,
                ValidWeaponTypes = new[] { WeaponType.Any },
                Weight = 60
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "holy_ground",
                AbilityId = "Consecration",
                DisplayName = "Holy Ground",
                Description = "15% chance on hit to consecrate the ground",
                Trigger = ProcTrigger.OnHit,
                ProcChance = 0.15f,
                InternalCooldown = 15f,
                ValidWeaponTypes = new[] { WeaponType.Any },
                Weight = 50
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "bloodlust",
                AbilityId = "Bloodlust",
                DisplayName = "Bloodlust",
                Description = "On kill, enter a bloodthirsty frenzy",
                Trigger = ProcTrigger.OnKill,
                ProcChance = 0.50f,
                InternalCooldown = 20f,
                ValidWeaponTypes = new[] { WeaponType.Any },
                ValidArmorSlots = new[] { ArmorSlot.Any },
                Weight = 40
            });

            Register(new LegendaryAbilityDefinition
            {
                Id = "life_tap",
                AbilityId = "LifeTap",
                DisplayName = "Life Tap",
                Description = "10% chance on hit to convert damage to health",
                Trigger = ProcTrigger.OnHit,
                ProcChance = 0.10f,
                InternalCooldown = 8f,
                ValidWeaponTypes = new[] { WeaponType.Any },
                Weight = 70
            });
        }
    }
}
