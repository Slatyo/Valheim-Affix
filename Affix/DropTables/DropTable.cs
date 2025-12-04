using System.Collections.Generic;
using System.Linq;
using Affix.Core;
using UnityEngine;

namespace Affix.DropTables
{
    /// <summary>
    /// Manages drop table registration and rolling.
    /// </summary>
    public static class DropTable
    {
        private static readonly Dictionary<string, DropTableConfig> _tables = new();

        /// <summary>
        /// Registers a drop table for a creature/source.
        /// </summary>
        public static void Register(string source, DropTableConfig config)
        {
            if (string.IsNullOrEmpty(source) || config == null)
            {
                Plugin.Log?.LogWarning("Attempted to register null drop table");
                return;
            }

            _tables[source] = config;
            Plugin.Log?.LogDebug($"Registered drop table: {source} with {config.Entries?.Count ?? 0} entries");
        }

        /// <summary>
        /// Gets a registered drop table by source name.
        /// </summary>
        public static DropTableConfig Get(string source)
        {
            return _tables.TryGetValue(source, out var table) ? table : null;
        }

        /// <summary>
        /// Checks if a drop table exists for the given source.
        /// </summary>
        public static bool Has(string source)
        {
            return _tables.ContainsKey(source);
        }

        /// <summary>
        /// Rolls drops from a table.
        /// </summary>
        /// <param name="source">The drop table source name.</param>
        /// <param name="playerCount">Number of players (for bonus drops).</param>
        /// <returns>List of drop results with prefab names, counts, and optional affix data.</returns>
        public static List<DropResult> Roll(string source, int playerCount = 1)
        {
            var results = new List<DropResult>();

            if (!_tables.TryGetValue(source, out var table))
            {
                Plugin.Log?.LogDebug($"No drop table found for: {source}");
                return results;
            }

            // Get all entries including inherited
            var allEntries = GetAllEntries(table);
            if (allEntries.Count == 0)
                return results;

            // Calculate drop count
            var bonusDrops = table.BonusDropsPerPlayer * (playerCount - 1);
            var maxDrops = table.MaxDrops + Mathf.Max(0, bonusDrops);
            var dropCount = Random.Range(table.MinDrops, maxDrops + 1);

            Plugin.Log?.LogDebug($"Rolling {dropCount} drops from {source}");

            // Roll each drop
            for (int i = 0; i < dropCount; i++)
            {
                foreach (var entry in allEntries)
                {
                    if (Random.value > entry.Chance)
                        continue;

                    var result = RollEntry(entry);
                    if (result != null)
                        results.Add(result);
                }
            }

            return results;
        }

        /// <summary>
        /// Rolls a single drop entry.
        /// </summary>
        private static DropResult RollEntry(DropEntry entry)
        {
            var result = new DropResult
            {
                PrefabName = entry.Item,
                Count = Random.Range(entry.Min, entry.Max + 1)
            };

            // Roll rarity if specified
            if (entry.Rarity != null)
            {
                result.HasAffixes = true;
                result.Rarity = entry.Rarity.Type switch
                {
                    RarityRollType.Fixed => entry.Rarity.Rarity,
                    RarityRollType.Minimum => RarityManager.RollRarity(entry.Rarity.Rarity),
                    _ => RarityManager.RollRarity()
                };

                Plugin.Log?.LogDebug($"Rolled {result.PrefabName} with rarity {result.Rarity}");
            }

            return result;
        }

        /// <summary>
        /// Gets all entries including inherited tables.
        /// </summary>
        private static List<DropEntry> GetAllEntries(DropTableConfig table)
        {
            var entries = new List<DropEntry>();

            // Add inherited entries first
            if (!string.IsNullOrEmpty(table.Inherits) && _tables.TryGetValue(table.Inherits, out var parent))
            {
                entries.AddRange(GetAllEntries(parent));
            }

            // Add this table's entries
            if (table.Entries != null)
            {
                entries.AddRange(table.Entries);
            }

            return entries;
        }

        /// <summary>
        /// Clears all registered drop tables.
        /// </summary>
        public static void Clear()
        {
            _tables.Clear();
        }
    }

    /// <summary>
    /// Result of rolling a drop table entry.
    /// </summary>
    public class DropResult
    {
        /// <summary>
        /// The prefab name to spawn.
        /// </summary>
        public string PrefabName { get; set; }

        /// <summary>
        /// Stack count.
        /// </summary>
        public int Count { get; set; } = 1;

        /// <summary>
        /// Whether this item should have affixes generated.
        /// </summary>
        public bool HasAffixes { get; set; }

        /// <summary>
        /// The rolled rarity (if HasAffixes is true).
        /// </summary>
        public Rarity Rarity { get; set; }
    }
}
