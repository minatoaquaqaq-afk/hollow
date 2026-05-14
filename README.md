# Hollow Style MVP

Unity version: 2023.2.20f1

Project path: `G:\HollowStyleMVP`

## Quick Test Path

1. Wait until Unity Console has no red errors.
2. Use one of these menus:
   - `Assets > Hollow Style MVP > Rebuild Complete Test Flow`
   - `GameObject > Hollow Style MVP > Rebuild Complete Test Flow`
   - `Hollow Style MVP > Rebuild Complete Test Flow`
3. Open `Assets/Scenes/MainMenu.unity`.
4. Press Play.
5. Click `开始游戏`.
6. Test features in `TestRoom` by following the Chinese labels above each block.

See `TESTING_GUIDE.md` for the full Chinese test path, feature list, controls, configuration options, and test cases.

## MVP Content

- Main menu: new game, continue, quit
- Player: move, variable jump, double jump, dash, attack, death/respawn, PlayerConfig asset
- Interaction: dialogue, shop, save point
- UI: HUD, inventory, dialogue panel, shop panel
- Combat: damage, health, knockback, contact damage
- Enemy and Boss prototypes
- Items: auto pickup coins, shop item purchase, inventory display
- Level components: spikes, fragile platform, LDtk bootstrap bridge

## LDtk

LDtk is optional so the project can compile offline. Read:

`Assets/Levels/LDtk/README_LDtk_Workflow.md`

## Config Assets

Run Hollow Style MVP > Create Default Config Assets or Rebuild Complete Test Flow. Then inspect:

- Assets/ScriptableObjects/Player/DefaultPlayerConfig.asset
- Assets/ScriptableObjects/Enemies/WalkerEnemyConfig.asset
- Assets/ScriptableObjects/Bosses/TestBossConfig.asset
- Assets/ScriptableObjects/Progression/MvpFeatureRoadmap.asset

