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

Requires [Munin](https://thunderstore.io/c/valheim/p/Slatyo/Munin/) (optional dependency):

```
munin affix inspect    - Inspect affixes on held item
munin affix list       - List all registered affixes
munin affix test [rarity] - Apply random affixes to held weapon
munin affix clear      - Remove all affixes from held weapon
munin affix rarity     - Show rarity tier configuration
```

## Dependencies

- BepInEx
- Jotunn
- Prime - Combat engine for stat modifiers
- Veneer - UI framework for tooltip display
- Munin (optional) - Console commands

## Installation

Install via Thunderstore Mod Manager or manually extract to `BepInEx/plugins/`
