using System.Collections.Generic;
using System.Linq;

namespace Affix.Core
{
    /// <summary>
    /// Registry for all available affixes.
    /// </summary>
    public static class AffixRegistry
    {
        private static readonly Dictionary<string, AffixDefinition> _affixes = new();

        /// <summary>
        /// Number of registered affixes.
        /// </summary>
        public static int Count => _affixes.Count;

        /// <summary>
        /// Registers a new affix definition.
        /// </summary>
        public static void Register(AffixDefinition affix)
        {
            if (affix == null || string.IsNullOrEmpty(affix.Id))
            {
                Plugin.Log?.LogWarning("Attempted to register null or invalid affix");
                return;
            }

            if (_affixes.ContainsKey(affix.Id))
            {
                Plugin.Log?.LogWarning($"Affix '{affix.Id}' already registered, overwriting");
            }

            _affixes[affix.Id] = affix;
            Plugin.Log?.LogDebug($"Registered affix: {affix.Id} ({affix.Name})");
        }

        /// <summary>
        /// Gets an affix definition by ID.
        /// </summary>
        public static AffixDefinition Get(string id)
        {
            return _affixes.TryGetValue(id, out var affix) ? affix : null;
        }

        /// <summary>
        /// Gets all registered affix definitions.
        /// </summary>
        public static IEnumerable<AffixDefinition> GetAll()
        {
            return _affixes.Values;
        }

        /// <summary>
        /// Gets all affixes valid for a specific item type.
        /// </summary>
        public static IEnumerable<AffixDefinition> GetForItemType(ItemType itemType)
        {
            return _affixes.Values.Where(a => (a.AllowedTypes & itemType) != 0);
        }

        /// <summary>
        /// Gets all affixes valid for an item type, excluding specified affix IDs.
        /// </summary>
        public static IEnumerable<AffixDefinition> GetForItemType(ItemType itemType, IEnumerable<string> excludeIds)
        {
            var excludeSet = new HashSet<string>(excludeIds);
            return _affixes.Values
                .Where(a => (a.AllowedTypes & itemType) != 0)
                .Where(a => !excludeSet.Contains(a.Id));
        }

        /// <summary>
        /// Clears all registered affixes.
        /// </summary>
        public static void Clear()
        {
            _affixes.Clear();
        }
    }
}
