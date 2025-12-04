using System;
using System.Collections.Generic;
using System.Linq;

namespace Affix.Core
{
    /// <summary>
    /// Manages rarity tier configurations.
    /// </summary>
    public static class RarityManager
    {
        private static readonly Dictionary<Rarity, RarityConfig> _configs = new();
        private static float _totalWeight;
        private static bool _initialized;

        /// <summary>
        /// Gets the configuration for a specific rarity.
        /// </summary>
        public static RarityConfig GetConfig(Rarity rarity)
        {
            EnsureInitialized();
            return _configs.TryGetValue(rarity, out var config) ? config : _configs[Rarity.Common];
        }

        /// <summary>
        /// Sets the configuration for a specific rarity.
        /// </summary>
        public static void SetConfig(Rarity rarity, RarityConfig config)
        {
            _configs[rarity] = config;
            RecalculateWeights();
        }

        /// <summary>
        /// Rolls a random rarity based on configured weights.
        /// </summary>
        public static Rarity RollRarity()
        {
            EnsureInitialized();

            var roll = UnityEngine.Random.value * _totalWeight;
            var cumulative = 0f;

            foreach (var kvp in _configs)
            {
                cumulative += kvp.Value.DropWeight;
                if (roll <= cumulative)
                    return kvp.Key;
            }

            return Rarity.Common;
        }

        /// <summary>
        /// Rolls a random rarity with a minimum tier.
        /// </summary>
        public static Rarity RollRarity(Rarity minimum)
        {
            EnsureInitialized();

            var eligibleRarities = _configs
                .Where(kvp => kvp.Key >= minimum)
                .ToList();

            if (eligibleRarities.Count == 0)
                return minimum;

            var totalWeight = eligibleRarities.Sum(kvp => kvp.Value.DropWeight);
            var roll = UnityEngine.Random.value * totalWeight;
            var cumulative = 0f;

            foreach (var kvp in eligibleRarities)
            {
                cumulative += kvp.Value.DropWeight;
                if (roll <= cumulative)
                    return kvp.Key;
            }

            return minimum;
        }

        /// <summary>
        /// Gets the number of affixes to roll for a given rarity.
        /// </summary>
        public static int RollAffixCount(Rarity rarity)
        {
            var config = GetConfig(rarity);
            if (config.MinAffixes == config.MaxAffixes)
                return config.MinAffixes;

            return UnityEngine.Random.Range(config.MinAffixes, config.MaxAffixes + 1);
        }

        /// <summary>
        /// Gets the display color for a rarity as a Unity Color.
        /// </summary>
        public static UnityEngine.Color GetColor(Rarity rarity)
        {
            var config = GetConfig(rarity);
            if (UnityEngine.ColorUtility.TryParseHtmlString("#" + config.Color, out var color))
                return color;
            return UnityEngine.Color.white;
        }

        /// <summary>
        /// Gets the hex color string for a rarity (with #).
        /// </summary>
        public static string GetColorHex(Rarity rarity)
        {
            return "#" + GetConfig(rarity).Color;
        }

        /// <summary>
        /// Formats text with rarity color tags.
        /// </summary>
        public static string Colorize(string text, Rarity rarity)
        {
            return $"<color={GetColorHex(rarity)}>{text}</color>";
        }

        private static void EnsureInitialized()
        {
            if (_initialized) return;
            InitializeDefaults();
            _initialized = true;
        }

        private static void InitializeDefaults()
        {
            _configs[Rarity.Common] = new RarityConfig
            {
                MinAffixes = 0,
                MaxAffixes = 2,
                HasUniqueEffect = false,
                DropWeight = 60f,
                Color = "FFFFFF",
                DisplayName = "$affix_rarity_common"
            };

            _configs[Rarity.Uncommon] = new RarityConfig
            {
                MinAffixes = 3,
                MaxAffixes = 4,
                HasUniqueEffect = false,
                DropWeight = 25f,
                Color = "1EFF00",
                DisplayName = "$affix_rarity_uncommon"
            };

            _configs[Rarity.Rare] = new RarityConfig
            {
                MinAffixes = 5,
                MaxAffixes = 5,
                HasUniqueEffect = false,
                DropWeight = 10f,
                Color = "0070DD",
                DisplayName = "$affix_rarity_rare"
            };

            _configs[Rarity.Epic] = new RarityConfig
            {
                MinAffixes = 6,
                MaxAffixes = 6,
                HasUniqueEffect = false,
                DropWeight = 4f,
                Color = "A335EE",
                DisplayName = "$affix_rarity_epic"
            };

            _configs[Rarity.Legendary] = new RarityConfig
            {
                MinAffixes = 6,
                MaxAffixes = 6,
                HasUniqueEffect = true,
                DropWeight = 1f,
                Color = "FF8000",
                DisplayName = "$affix_rarity_legendary"
            };

            RecalculateWeights();
        }

        private static void RecalculateWeights()
        {
            _totalWeight = _configs.Values.Sum(c => c.DropWeight);
        }
    }
}
