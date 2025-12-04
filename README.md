# Affix

Random affixes and rarity tiers for Valheim gear. Adds ARPG-style loot with procedural stat modifiers.

## Features

- **Rarity Tiers**: Common, Uncommon, Rare, Epic, and Legendary items with distinct colors
- **Random Affixes**: Procedurally generated stat modifiers on weapons and armor
- **Drop Tables**: Configurable drop tables for creatures with rarity-weighted loot
- **Tooltip Integration**: Affixes display in item tooltips via Veneer UI
- **Stat Modifiers**: Uses Prime for applying stat changes when equipment is equipped

## Rarity System

| Rarity | Color | Affixes | Drop Weight |
|--------|-------|---------|-------------|
| Common | White | 0-2 | 60% |
| Uncommon | Green | 3-4 | 25% |
| Rare | Blue | 5 | 10% |
| Epic | Purple | 6 | 4% |
| Legendary | Orange | 6 + unique | 1% |

## Console Commands

Requires [Munin](https://github.com/Slatyo/Valheim-Munin) (optional dependency):

```
munin affix inspect    - Inspect affixes on held item
munin affix list       - List all registered affixes
munin affix test [rarity] - Apply random affixes to held weapon
munin affix clear      - Remove all affixes from held weapon
munin affix rarity     - Show rarity tier configuration
```

## Dependencies

- [BepInEx](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/)
- [Jotunn](https://valheim.thunderstore.io/package/ValheimModding/Jotunn/)
- [Prime](https://github.com/Slatyo/Valheim-Prime) - Combat engine for stat modifiers
- [Veneer](https://github.com/Slatyo/Valheim-Veneer) - UI framework for tooltip display
- [Munin](https://github.com/Slatyo/Valheim-Munin) (optional) - Console commands

## For Mod Developers

### Registering Custom Affixes

```csharp
AffixRegistry.Register(new AffixDefinition
{
    Id = "my_affix",
    Name = "My Affix",
    Description = "+{0} to Something",
    PrimeStat = "my_stat",
    Type = ModifierType.Flat,
    MinValue = 5,
    MaxValue = 20,
    AllowedTypes = ItemType.Weapon | ItemType.Armor
});
```

### Custom Drop Tables

```csharp
DropTable.Register("MyCreature", new DropTableConfig
{
    Entries = new List<DropEntry>
    {
        new DropEntry
        {
            PrefabName = "SwordIron",
            DropChance = 0.1f,
            MinRarity = Rarity.Uncommon
        }
    }
});
```

## Installation

1. Install dependencies (BepInEx, Jotunn, Prime, Veneer)
2. Download and extract to `BepInEx/plugins/Affix/`

## License

MIT License
