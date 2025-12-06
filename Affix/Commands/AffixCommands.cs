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
                    Description = "Inspect affixes: inspect [slot]. Slots: weapon, helmet, chest, legs, cape, shield",
                    Permission = PermissionLevel.Anyone,
                    Handler = args =>
                    {
                        var player = Player.m_localPlayer;
                        if (player == null)
                            return CommandResult.Error("No player found");

                        var item = GetEquippedItem(player, args.Count > 0 ? args.Get(0) : null);
                        if (item == null)
                            return CommandResult.Error("No item equipped. Use: inspect [slot]");

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
                    Description = "Apply affixes: test [rarity] [slot]. Slots: weapon, helmet, chest, legs, cape, shield",
                    Permission = PermissionLevel.Admin,
                    Handler = args =>
                    {
                        var player = Player.m_localPlayer;
                        if (player == null)
                            return CommandResult.Error("No player found");

                        // Parse arguments - can be in any order
                        var rarity = Rarity.Rare;
                        string slotName = null;

                        for (int i = 0; i < args.Count; i++)
                        {
                            var arg = args.Get(i).ToLowerInvariant();

                            // Check if it's a rarity
                            if (System.Enum.TryParse<Rarity>(arg, true, out var parsedRarity))
                            {
                                rarity = parsedRarity;
                            }
                            // Check if it's a slot name
                            else if (arg == "weapon" || arg == "helmet" || arg == "chest" ||
                                     arg == "legs" || arg == "cape" || arg == "shield")
                            {
                                slotName = arg;
                            }
                        }

                        // Get item based on slot
                        ItemDrop.ItemData item = null;
                        if (slotName == null || slotName == "weapon")
                        {
                            item = player.GetCurrentWeapon();
                            if (item == null && slotName == "weapon")
                                return CommandResult.Error("No weapon equipped");
                        }

                        if (item == null && slotName != null)
                        {
                            var inventory = player.GetInventory();
                            var equipped = inventory?.GetEquippedItems();
                            if (equipped != null)
                            {
                                foreach (var eq in equipped)
                                {
                                    var itemType = eq.m_shared?.m_itemType;
                                    bool match = slotName switch
                                    {
                                        "helmet" => itemType == ItemDrop.ItemData.ItemType.Helmet,
                                        "chest" => itemType == ItemDrop.ItemData.ItemType.Chest,
                                        "legs" => itemType == ItemDrop.ItemData.ItemType.Legs,
                                        "cape" => itemType == ItemDrop.ItemData.ItemType.Shoulder,
                                        "shield" => itemType == ItemDrop.ItemData.ItemType.Shield,
                                        _ => false
                                    };
                                    if (match)
                                    {
                                        item = eq;
                                        break;
                                    }
                                }
                            }
                        }

                        if (item == null)
                        {
                            if (slotName != null)
                                return CommandResult.Error($"No {slotName} equipped");
                            return CommandResult.Error("No weapon equipped. Use: test [rarity] [slot]");
                        }

                        // Apply affixes
                        AffixItemSpawner.ApplyAffixes(item, rarity);

                        return InspectItem(item);
                    }
                },
                new CommandConfig
                {
                    Name = "clear",
                    Description = "Clear affixes: clear [slot]. Slots: weapon, helmet, chest, legs, cape, shield",
                    Permission = PermissionLevel.Admin,
                    Handler = args =>
                    {
                        var player = Player.m_localPlayer;
                        if (player == null)
                            return CommandResult.Error("No player found");

                        var item = GetEquippedItem(player, args.Count > 0 ? args.Get(0) : null);
                        if (item == null)
                            return CommandResult.Error("No item equipped. Use: clear [slot]");

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
                },
                new CommandConfig
                {
                    Name = "legendary",
                    Description = "List all legendary abilities [filter]",
                    Permission = PermissionLevel.Anyone,
                    Handler = args =>
                    {
                        var filter = args.Count > 0 ? args.Get(0).ToLowerInvariant() : null;
                        var abilities = LegendaryAbilityRegistry.Instance.GetAll();

                        if (!string.IsNullOrEmpty(filter))
                        {
                            abilities = abilities.Where(a =>
                                a.Id.ToLowerInvariant().Contains(filter) ||
                                a.DisplayName.ToLowerInvariant().Contains(filter) ||
                                a.AbilityId.ToLowerInvariant().Contains(filter));
                        }

                        var list = abilities.ToList();
                        if (list.Count == 0)
                            return CommandResult.Info("No legendary abilities found");

                        var lines = new List<string>
                        {
                            $"<color=#FF8000>Legendary Abilities ({list.Count})</color>"
                        };

                        foreach (var ability in list.OrderBy(a => a.Id))
                        {
                            var weaponTypes = ability.ValidWeaponTypes != null ? string.Join(", ", ability.ValidWeaponTypes) : "-";
                            var armorSlots = ability.ValidArmorSlots != null ? string.Join(", ", ability.ValidArmorSlots) : "-";
                            lines.Add($"  <color=#FFCC00>{ability.DisplayName}</color> ({ability.Id})");
                            lines.Add($"    {ability.Description}");
                            lines.Add($"    Ability: {ability.AbilityId}, Trigger: {ability.Trigger}");
                            if (ability.ValidWeaponTypes != null && ability.ValidWeaponTypes.Length > 0)
                                lines.Add($"    Weapons: {weaponTypes}");
                            if (ability.ValidArmorSlots != null && ability.ValidArmorSlots.Length > 0)
                                lines.Add($"    Armor: {armorSlots}");
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

        /// <summary>
        /// Gets an equipped item by slot name.
        /// </summary>
        private static ItemDrop.ItemData GetEquippedItem(Player player, string slotName)
        {
            if (player == null) return null;

            slotName = slotName?.ToLowerInvariant();

            // Default to weapon if no slot specified
            if (string.IsNullOrEmpty(slotName) || slotName == "weapon")
            {
                return player.GetCurrentWeapon();
            }

            var inventory = player.GetInventory();
            var equipped = inventory?.GetEquippedItems();
            if (equipped == null) return null;

            foreach (var item in equipped)
            {
                var itemType = item.m_shared?.m_itemType;
                bool match = slotName switch
                {
                    "helmet" => itemType == ItemDrop.ItemData.ItemType.Helmet,
                    "chest" => itemType == ItemDrop.ItemData.ItemType.Chest,
                    "legs" => itemType == ItemDrop.ItemData.ItemType.Legs,
                    "cape" => itemType == ItemDrop.ItemData.ItemType.Shoulder,
                    "shield" => itemType == ItemDrop.ItemData.ItemType.Shield,
                    _ => false
                };
                if (match) return item;
            }

            return null;
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

            // Show unique ability if present
            var uniqueAbility = AffixData.GetUniqueAbility(item);
            if (uniqueAbility != null)
            {
                lines.Add($"  <color=#FF8000>Unique Ability:</color>");
                lines.Add($"    {uniqueAbility.DisplayName}: {uniqueAbility.Description}");
                lines.Add($"    Ability: {uniqueAbility.AbilityId}, Trigger: {uniqueAbility.Trigger}");
                lines.Add($"    Proc: {(uniqueAbility.ProcChance > 0 ? $"{uniqueAbility.ProcChance * 100:F0}%" : "Always")}, CD: {uniqueAbility.InternalCooldown}s");
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
