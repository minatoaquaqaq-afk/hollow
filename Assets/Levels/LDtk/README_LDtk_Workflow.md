# LDtk Import Workflow

The MVP project keeps LDtk optional so Unity can compile offline.

Install LDtkToUnity after the project compiles:

## Option A: Unity Package Manager / OpenUPM

1. Open `Edit > Project Settings > Package Manager`.
2. Add scoped registry:
   - Name: `OpenUPM`
   - URL: `https://package.openupm.com`
   - Scope: `com.cammin.ldtkunity`
3. Open `Window > Package Manager`.
4. Add package by name: `com.cammin.ldtkunity`.

## Option B: OpenUPM CLI

```bash
openupm add com.cammin.ldtkunity
```

Recommended LDtk entity identifiers:

- `PlayerSpawn`
- `EnemySpawn`
- `BossSpawn`
- `Npc`
- `Shop`
- `SavePoint`
- `Door`

After importing an LDtk level:

1. Drag the generated LDtk level prefab/root into a scene.
2. Add `LdtkLevelBootstrapper` to the imported root.
3. Assign gameplay prefabs.
4. Use the component context menu: `Build Gameplay Objects From LDtk Root`.

`LdtkLevelBootstrapper` does not hard-reference LDtk classes, so it works before and after installing the plugin.
