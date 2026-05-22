# Hollow Style Roguelike MVP

Unity version: 2023.2.20f1

Project path: `G:\HollowStyleMVP`

## 当前保留功能

- Roguelike run loop: seed-based dungeon layout, room node graph, start/combat/treasure/shop/boss room types, room enter/clear events.
- Room gameplay: room controller, directional doors, door locking, encounter spawning, clear rewards.
- Combat: health, damage dealer, contact damage, combat stats, hit effects, damage popups.
- Player: top-down movement, dash, melee attack, projectile shooting, projectile modifiers, stats and ability unlock hooks.
- Enemies: walker enemy config, AI, spawner, drop table support.
- Items and inventory: coins, pickup magnet, inventory items, equipment/charm stat modifiers, projectile upgrade items.
- Shop: shop NPC/config/controller/items are preserved for roguelike shop rooms.
- Dialogue: dialogue assets, NPC, and controller are preserved for room NPCs and run flavor.
- UI and feedback: HUD, inventory, shop/dialogue windows, pause/death UI, world health bar, camera shake and feedback manager.
- Art/tools: art preparation and UI prefab builders remain for the retained shop/dialogue/inventory UI.

## 已移除的旧 MVP 内容

- Save system and save points.
- LDtk bridge, scene doors, teleporters, platformer hazards, moving/fragile platforms, ability gates, map exploration state.
- Old metroidvania Boss state machine/config assets.
- Feature roadmap assets and old testing guide/matrix.
- Old complete test-flow scene builders and auto-rebuild hooks.

## 保留目录速览

- `Assets/Scripts/Roguelike`: procedural run, dungeon rooms, doors, rewards, projectiles.
- `Assets/Scripts/Combat`: shared damage/health/stat foundations.
- `Assets/Scripts/Player`: player controller, abilities, stats, combo hooks.
- `Assets/Scripts/Enemies`: enemy AI/config/spawning.
- `Assets/Scripts/Inventory`, `Assets/Scripts/Items`: pickups, drops, equipment/charm upgrades.
- `Assets/Scripts/Shop`, `Assets/Scripts/Dialogue`: intentionally kept per current design.
- `Assets/Scripts/UI`, `Assets/Scripts/Core`, `Assets/Scripts/Interaction`: runtime glue and screens.
