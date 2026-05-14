# Hollow Style MVP 测试指南


## 配置资产生成

如果 `Assets/ScriptableObjects/Player` 下没有看到玩家配置，请先执行：

- `Hollow Style MVP > Create Default Config Assets`
- 或 `Hollow Style MVP > Rebuild Complete Test Flow`

生成后应看到：

- `Assets/ScriptableObjects/Player/DefaultPlayerConfig.asset`
- `Assets/ScriptableObjects/Enemies/WalkerEnemyConfig.asset`
- `Assets/ScriptableObjects/Bosses/TestBossConfig.asset`
- `Assets/ScriptableObjects/Items/TestPotion.asset`
- `Assets/ScriptableObjects/Shops/TestShopConfig.asset`
- `Assets/ScriptableObjects/Audio/DefaultAudioFeedbackConfig.asset`
- `Assets/ScriptableObjects/Progression/MvpFeatureRoadmap.asset`

玩家移动、跳跃、二段跳、冲刺、攻击、生命、能量等主要参数从 `DefaultPlayerConfig.asset` 配置。

## 一、测试路径

1. Unity 等待编译完成，Console 没有红色报错。
2. 点击顶部菜单之一：
   - `Hollow Style MVP > Rebuild Complete Test Flow`
   - 或 `Assets > Hollow Style MVP > Rebuild Complete Test Flow`
   - 或 `GameObject > Hollow Style MVP > Rebuild Complete Test Flow`
3. 打开场景：`Assets/Scenes/MainMenu.unity`。
4. 点击 Play。
5. 在开始界面点击 `开始游戏`。
6. 进入 `TestRoom`。
7. 按场景中每个中文标签逐项测试功能。

如果只想单独重建：

1. `Build UI Prefabs`
2. `Build MainMenu Scene`
3. `Build TestRoom Scene`
4. `Add MVP Scenes To Build Settings`

## 二、已开发功能清单

### 开始与存档

- 开始界面：开始游戏、继续、退出。
- 新游戏：删除旧存档并进入 `TestRoom`。
- 继续游戏：读取存档场景、玩家位置、生命、金币。
- 存档点：靠近按 E 保存。

### 玩家

- A/D 或 Horizontal 轴左右移动。
- W 或 Space 跳跃。
- 二段跳。
- Left Shift 冲刺。
- J 普通攻击。
- 受伤、击退、短暂无敌。

### 战斗

- 攻击 Hitbox。
- 通用生命值组件。
- 受伤扣血。
- 接触伤害。
- 怪物死亡销毁。

### 怪物

- 怪物检测玩家。
- 追击玩家。
- 接触玩家造成伤害。
- 被 J 攻击可受伤死亡。

### Boss

- Boss 检测玩家。
- 追击玩家。
- 跳跃攻击原型。
- 近战攻击原型。
- 被 J 攻击可受伤死亡。

### 交互

- E 键交互。
- 交互提示文字。
- NPC 对话。
- 商店打开。
- 存档点保存。

### UI

- HUD 金币显示。
- 交互提示显示。
- 背包 UI。
- 对话 UI。
- 商店 UI。
- 开始界面 UI。

### 物品与背包

- 金币自动吸附拾取。
- 金币数量刷新。
- 背包打开/关闭。
- 商店购买物品后加入背包。

### 关卡组件

- 基础地面碰撞。
- 尖刺伤害。
- 脆弱平台：踩上延迟消失，稍后恢复。
- LDtk 桥接脚本：`LdtkLevelBootstrapper`。

## 三、按钮操作

### 开始界面

- 鼠标点击 `开始游戏`：进入 TestRoom。
- 鼠标点击 `继续`：有存档时读取存档进入场景。
- 鼠标点击 `退出`：退出游戏，Editor 中不会真正关闭 Unity。

### 场景内

- A / D：左右移动。
- W / Space：跳跃。
- W / Space 在空中再按一次：二段跳。
- Left Shift：冲刺。
- J：攻击。
- E：和 NPC / 商店 / 存档点交互。
- I：打开/关闭背包。
- Esc：关闭商店、关闭对话、关闭背包；没有 UI 时切换暂停。
- 鼠标点击商店商品：购买。

## 四、可配置内容

### PlayerController2D

- `moveSpeed`：移动速度。
- `jumpForce`：跳跃力度。
- `maxAirJumps`：空中额外跳跃次数，当前为 1，即二段跳。
- `dashSpeed`：冲刺速度。
- `dashDuration`：冲刺持续时间。
- `dashCooldown`：冲刺冷却。
- `groundMask`：地面检测层。
- `attackHitbox`：攻击判定盒。
- `attackTime`：攻击判定持续时间。

### Health

- `maxHealth`：最大生命值。
- `invulnerableSeconds`：受伤后无敌时间。
- `flashRenderer`：受伤闪色对象。

### DamageDealer / ContactDamage

- `damage`：伤害值。
- `knockback`：击退力度。
- `targetTag`：目标 Tag。为空时按 Health 组件识别。

### EnemyAI

- `patrolSpeed`：巡逻速度。
- `chaseSpeed`：追击速度。
- `detectRange`：发现玩家距离。
- `attackRange`：停止追击/攻击距离。
- `dropPrefab`：死亡掉落物。
- `patrolPoints`：巡逻点数组。

### BossController

- `detectRange`：发现玩家距离。
- `moveSpeed`：追击速度。
- `attackRange`：近战距离。
- `actionCooldown`：行为切换冷却。
- `leapForce`：跳跃攻击力度。
- `slashHitbox`：斩击判定。

### ShopNpc / ShopController

- `shopName`：商店名称。
- `items`：商品数组。
- 每个商品可配置 `item`、`price`、`stock`。

### InventoryItem

- `id`：存档/背包识别 ID。
- `displayName`：显示名称。
- `type`：物品类型。
- `price`：默认价格。
- `description`：描述。

### DialogueNpc / DialogueAsset

- `dialogue`：对话资产。
- `fallbackSpeaker`：未配置对话资产时的默认说话人。
- `fallbackLines`：默认测试对白。

### 关卡组件

- `HazardDamage.damage`：尖刺伤害。
- `FragilePlatform.breakDelay`：平台碎裂延迟。
- `FragilePlatform.restoreDelay`：平台恢复延迟。
- `MovingPlatform.a / b / speed`：移动平台端点和速度。
- `PendulumHazard.angle / speed`：摆锤角度和速度。

### LDtk

- `LdtkLevelBootstrapper` 可配置玩家、怪物、Boss、NPC、商店、存档点 Prefab。
- LDtk 实体推荐命名：`PlayerSpawn`、`EnemySpawn`、`BossSpawn`、`Npc`、`Shop`、`SavePoint`、`Door`。

## 五、逐项测试用例

### TC-001 开始界面进入测试场景

步骤：
1. 打开 `Assets/Scenes/MainMenu.unity`。
2. 点击 Play。
3. 点击 `开始游戏`。

预期：
- 切换到 `TestRoom`。
- 看到蓝色玩家方块和中文标签。

### TC-002 玩家移动

步骤：
1. 在 TestRoom 按 A。
2. 按 D。

预期：
- 玩家向左/向右移动。
- 玩家不会穿过地面或墙体。

### TC-003 跳跃与二段跳

步骤：
1. 按 W 或 Space。
2. 在空中再次按 W 或 Space。

预期：
- 第一次按键跳起。
- 第二次按键触发二段跳。
- 落地后可再次二段跳。

### TC-004 冲刺

步骤：
1. 按 D 面向右。
2. 按 Left Shift。

预期：
- 玩家快速向右冲刺一小段。
- 冲刺后需要短冷却才能再次冲刺。

### TC-005 自动拾取金币

步骤：
1. 移动到黄色金币附近。

预期：
- 金币靠近玩家并被自动拾取。
- 左上金币数增加。

### TC-006 背包 UI

步骤：
1. 按 I 打开背包。
2. 再按 I 或 Esc。

预期：
- 背包 UI 打开。
- 显示当前金币。
- 再次按键关闭。

### TC-007 NPC 对话

步骤：
1. 靠近绿色 NPC。
2. 按 E。
3. 按 E 或 Space 切换下一句。
4. 按 Esc 关闭。

预期：
- 出现对话框。
- 文本逐字显示。
- 可以进入下一句。
- Esc 可以关闭。

### TC-008 商店购买

步骤：
1. 先拾取金币。
2. 靠近黄色商店 NPC。
3. 按 E 打开商店。
4. 点击商品。
5. 按 I 打开背包。

预期：
- 商店 UI 出现测试商品。
- 点击商品后金币减少。
- 背包里出现 `测试药水`。
- Esc 关闭商店。

### TC-009 存档与继续

步骤：
1. 靠近青色存档点。
2. 按 E 保存。
3. 停止 Play。
4. 从 MainMenu 再次 Play。
5. 点击 `继续`。

预期：
- 继续按钮可点击。
- 进入 TestRoom。
- 玩家位置和金币恢复到存档时状态。

### TC-010 尖刺伤害

步骤：
1. 移动到紫色尖刺上。

预期：
- 玩家受伤并被击退。
- 短时间内不会连续疯狂扣血。

### TC-011 脆弱平台

步骤：
1. 跳到棕色脆弱平台上。
2. 等待一小会。

预期：
- 平台延迟消失。
- 过一段时间恢复。

### TC-012 怪物战斗

步骤：
1. 靠近红色怪物。
2. 按 J 攻击。
3. 观察怪物行为。

预期：
- 怪物会追击玩家。
- 接触玩家会造成伤害。
- J 攻击可让怪物受伤，生命归零后消失。

### TC-013 Boss 原型

步骤：
1. 靠近紫色 Boss。
2. 观察 Boss 行为。
3. 按 J 攻击。

预期：
- Boss 会追击、跳跃或近战。
- 接触/攻击玩家会造成伤害。
- 玩家攻击可让 Boss 受伤，生命归零后进入死亡状态。

### TC-014 Build Settings

步骤：
1. 执行 `Add MVP Scenes To Build Settings`。
2. 打开 `File > Build Settings`。

预期：
- `MainMenu` 和 `TestRoom` 都在 Scenes In Build 中。
- `MainMenu` 位于第一项。

## 六、当前已知限制

- 角色和敌人目前是占位方块，不是正式美术。
- Boss 行为是原型状态机，不是完整行为树资源编辑器。
- 地图还没有真实 LDtk 地图文件，只提供导入桥接流程。
- 背包目前展示物品数量，不包含装备/使用物品逻辑。
- 怪物掉落接口已留好，但测试怪物暂未绑定掉落 Prefab。


## 新增测试：UI、TestRoom2、音频与特效

### TC-015 玩家生命 UI

步骤：
1. 进入 TestRoom。
2. 碰到尖刺或怪物。

预期：
- 左上角 `生命 current/max` 更新。
- 红色生命条同步减少。
- 出现伤害跳字。

### TC-016 能量 UI 与冲刺

步骤：
1. 按 `LeftShift`、`RightShift` 或 `LeftControl`。
2. 连续冲刺几次。

预期：
- 玩家向面朝方向冲刺。
- 蓝色能量条减少。
- 能量会自动恢复。
- 能量不足时冲刺不会触发。

### TC-017 金币 UI

步骤：
1. 靠近金币。

预期：
- 金币自动吸附并消失。
- 左上角 `金币` 数值增加。

### TC-018 连击数 UI

步骤：
1. 靠近怪物或 Boss。
2. 连续按 J 命中。

预期：
- 命中后显示 `连击 xN`。
- 一段时间不命中后恢复 `连击 -`。

### TC-019 能力系统 UI

步骤：
1. 进入 TestRoom。
2. 观察左上角能力行。

预期：
- 显示二段跳、冲刺、下劈、远程能力开关。
- 默认二段跳和冲刺为开。

### TC-020 敌人血条 UI

步骤：
1. 找到红色怪物或 Boss。
2. 按 J 攻击。

预期：
- 敌人/Boss 头顶血条减少。

### TC-021 攻击特效与音频

步骤：
1. 按 J。
2. 命中敌人。
3. 按 W/Space 跳跃，按 Shift/Ctrl 冲刺。

预期：
- 攻击时出现黄色粒子。
- 命中时出现红色粒子和跳字。
- 跳跃、冲刺、攻击、命中、死亡有默认电子提示音。

### TC-022 TestRoom 到 TestRoom2

步骤：
1. 在 TestRoom 向右走到青色传送门 `去TestRoom2`。
2. 靠近按 E。

预期：
- 切换到 TestRoom2。
- TestRoom2 中有 Boss、宝箱、能力门、刷怪点、回 TestRoom 传送门。

### TC-023 TestRoom2 内容

步骤：
1. 在 TestRoom2 按 E 打开宝箱。
2. 尝试穿过能力门。
3. 观察刷怪点。
4. 靠近 `回TestRoom` 传送门按 E。

预期：
- 宝箱给金币/物品。
- 能力门检测冲刺能力，默认可通过并关闭门。
- 刷怪点生成怪物，怪物死亡后过几秒再刷。
- 传送回 TestRoom。

### TC-024 HP=0 死亡状态与复活

步骤：
1. 反复碰尖刺、怪物或 Boss，直到生命为 0。

预期：
- 出现 `死亡` UI。
- 玩家控制暂时禁用。
- 倒计时后回到出生点/复活点。
- 生命恢复满值，死亡 UI 隐藏。

### TC-025 攻击/受击震屏反馈

步骤：
1. 靠近怪物或 Boss。
2. 按 J 命中，或让玩家受伤。

预期：
- 命中/受伤时相机有短暂震动。
- 同时出现伤害跳字、命中特效和音效。

### TC-026 找传送门

步骤：
1. 在 TestRoom 从出生点向右走。
2. 找到右侧大青色竖门，文字为 `传送门 去TestRoom2：靠近按E`。
3. 靠近按 E。

预期：
- 进入 TestRoom2。
- TestRoom2 左侧有大青色 `回TestRoom` 传送门。

### TC-027 交互后移动恢复

步骤：
1. 靠近对话 NPC，按 E 打开对话。
2. 按 E/空格读完，或按 Esc 关闭。
3. 立刻按 A/D 移动。
4. 靠近商店，按 E 打开，按 Esc 关闭，再按 A/D。
5. 按 I 打开背包，按 I 或 Esc 关闭，再按 A/D。

预期：
- 每个 UI 关闭后玩家都能立刻移动。
- Time.timeScale 不会停在 0。
- E 交互不会因为 UI 状态卡死。
