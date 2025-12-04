using System.Collections.Generic;
using System.Linq;
using Affix.Core;
using Munin;

namespace Affix.Commands
{
    /// <summary>
    /// Console commands for Affix via Munin.
    /// Usage: munin affix [command]
    /// </summary>
    public static class AffixCommands
    {
        public static void Register()
        {
            Command.RegisterMany("affix",
                new CommandConfig
                {
                    Name = "inspect",
                    Description = "Inspect affixes on held item",
                    Permission = PermissionLevel.Anyone,
                    Handler = args =>
                    {
                        var player = Player.m_localPlayer;
                        if (player == null)
                            return CommandResult.Error("No player found");

                        // Get currently equipped right-hand item
                        var item = player.GetCurrentWeapon();
                        if (item == null)
                        {
                            // Try inventory selected item
                            item = player.GetInventory()?.GetEquippedItems()?.FirstOrDefault();
                        }

                        if (item == null)
                            return CommandResult.Error("No item equipped. Equip a weapon first.");

                        return InspectItem(item);
                    }
                },
                new CommandConfig
                {
                    Name = "list",
                    Description = "List all registered affixes",
                    Permission = PermissionLevel.Anyone,
                    Handler = args =>
                    {
                        var affixes = AffixRegistry.GetAll().ToList();
                        if (affixes.Count == 0)
                            return CommandResult.Info("No affixes registered");

                        var lines = new List<string>
                        {
                            $"<color=#{ChatColor.Gold}>Registered Affixes ({affixes.Count})</color>"
                        };

                        foreach (var affix in affixes.OrderBy(a => a.Id))
                        {
                            var types = affix.AllowedTypes.ToString();
                            lines.Add($"  {affix.Name} ({affix.Id}) - {types}");
                        }

                        return CommandResult.Info(string.Join("\n", lines));
                    }
                },
                new CommandConfig
                {
                    Name = "test",
                    Description = "Apply random affixes to held weapon",
                    Permission = PermissionLevel.Admin,
                    Handler = args =>
                    {
                        var player = Player.m_localPlayer;
                        if (player == null)
                            return CommandResult.Error("No player found");

                        var item = player.GetCurrentWeapon();
                        if (item == null)
                            return CommandResult.Error("No weapon equipped");

                        // Parse rarity if provided
                        var rarity = Rarity.Rare;
                        if (args.Count > 0 && System.Enum.TryParse<Rarity>(args.Get(0), true, out var parsed))
                        {
                            rarity = parsed;
                        }

                        // Apply affixes
                        AffixItemSpawner.ApplyAffixes(item, rarity);

                        return InspectItem(item);
                    }
                },
                new CommandConfig
                {
                    Name = "clear",
                    Description = "Remove all affixes from held weapon",
                    Permission = PermissionLevel.Admin,
                    Handler = args =>
                    {
                        var player = Player.m_localPlayer;
                        if (player == null)
                            return CommandResult.Error("No player found");

                        var item = player.GetCurrentWeapon();
                        if (item == null)
                            return CommandResult.Error("No weapon equipped");

                        if (!AffixData.HasAffixes(item))
                            return CommandResult.Info("Item has no affixes");

                        AffixData.ClearAffixes(item);
                        return CommandResult.Success("Affixes cleared from item");
                    }
                },
                new CommandConfig
                {
                    Name = "rarity",
                    Description = "Show rarity tier configuration",
                    Permission = PermissionLevel.Anyone,
                    Handler = args =>
                    {
                        var lines = new List<string>
                        {
                            $"<color=#{ChatColor.Gold}>Rarity Tiers</color>"
                        };

                        foreach (Rarity r in System.Enum.GetValues(typeof(Rarity)))
                        {
                            var config = RarityManager.GetConfig(r);
                            var color = RarityManager.GetColorHex(r).TrimStart('#');
                            lines.Add($"  <color=#{color}>{r}</color>: {config.MinAffixes}-{config.MaxAffixes} affixes, {config.DropWeight}% weight");
                        }

                        return CommandResult.Info(string.Join("\n", lines));
                    }
                }
            );

            Plugin.Log?.LogInfo("Affix commands registered with Munin");
        }

        public static void Unregister()
        {
            Command.UnregisterMod("affix");
        }

        private static CommandResult InspectItem(ItemDrop.ItemData item)
        {
            var lines = new List<string>();

            var itemName = Localization.instance?.Localize(item.m_shared.m_name) ?? item.m_shared.m_name;
            lines.Add($"<color=#{ChatColor.Gold}>{itemName}</color>");

            if (!AffixData.HasAffixes(item))
            {
                lines.Add("  No affixes on this item");
                lines.Add($"  m_customData keys: {(item.m_customData?.Count ?? 0)}");
                if (item.m_customData != null)
                {
                    foreach (var kvp in item.m_customData)
                    {
                        lines.Add($"    {kvp.Key}: {kvp.Value}");
                    }
                }
                return CommandResult.Info(string.Join("\n", lines));
            }

            var rarity = AffixData.GetRarity(item);
            var rarityColor = RarityManager.GetColorHex(rarity).TrimStart('#');
            lines.Add($"  Rarity: <color=#{rarityColor}>{rarity}</color>");

            var affixes = AffixData.GetAffixes(item);
            lines.Add($"  Affixes ({affixes.Count}):");

            foreach (var affix in affixes)
            {
                lines.Add($"    - {affix.Definition?.Name ?? "?"}: {affix.GetFormattedDescription()}");
            }

            // Show raw data for debugging
            lines.Add($"  Raw data:");
            if (item.m_customData != null)
            {
                foreach (var kvp in item.m_customData.Where(k => k.Key.StartsWith("affix")))
                {
                    lines.Add($"    {kvp.Key}: {kvp.Value}");
                }
            }

            return CommandResult.Info(string.Join("\n", lines));
        }
    }
}
