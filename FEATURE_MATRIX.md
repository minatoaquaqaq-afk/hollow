# Hollow Style MVP 功能完成度矩阵

状态说明：

- 已做：当前测试流程中可直接验证。
- 部分：已有脚本/配置骨架或原型，需要继续打磨成正式功能。
- 待做：尚未实现，只在路线图中记录。

## 一、核心玩法

| 功能 | 状态 | 位置/说明 |
| --- | --- | --- |
| 玩家基础移动 WASD | 已做 | `PlayerController2D` |
| 手柄左摇杆 | 部分 | 使用 Unity Horizontal 轴，正式 InputSystem Action 待做 |
| 跳跃、二段跳 | 已做 | `PlayerController2D` + `PlayerAbilityController` |
| 可变跳跃高度 | 已做 | 松开 W/Space 后削减上升速度 |
| 地面/空中冲刺、冷却 | 已做 | `PlayerConfig` 控制参数 |
| 冲刺无敌帧 | 部分 | 配置字段已有，未接入 Health 无敌覆盖 |
| 普通攻击 | 已做 | `DamageDealer` + 攻击 Hitbox |
| 空中攻击 | 部分 | 普通攻击空中也可触发，独立动画/判定待做 |
| 下劈 | 部分 | `PlayerAbility.DownStrike` 已定义，判定待做 |
| 连击 | 部分 | `PlayerConfig.comboCount` 已定义，连击状态机待做 |
| 受伤、击退、短暂无敌 | 已做 | `Health` |
| 死亡与复活 | 部分 | `PlayerRespawnController` 原型 |
| 技能解锁 | 部分 | `PlayerAbilityController` |
| 类银河城能力门 | 部分 | `AbilityGate` |

## 二、角色系统

| 功能 | 状态 | 位置/说明 |
| --- | --- | --- |
| 生命值 | 已做 | `Health` |
| 能量值 | 部分 | `PlayerConfig.maxEnergy` 字段已有，运行时消耗待做 |
| 金币/货币 | 已做 | `InventorySystem` |
| 属性系统 | 部分 | `PlayerConfig` 中攻击、防御、移速、跳跃力 |
| 动画状态机参数 | 部分 | Animator 参数已写入，正式 Controller/Clip 待做 |
| 帧动画加载管理 | 待做 | 需要导入 Sprite Sheet 和 Animator Override 方案 |
| 存档读取位置/能力/背包/剧情 | 部分 | `SaveData` 字段已扩展，背包/剧情完整恢复待做 |

## 三、战斗系统

| 功能 | 状态 | 位置/说明 |
| --- | --- | --- |
| 近战攻击判定 | 已做 | `DamageDealer` |
| 伤害盒/受击盒 | 部分 | Collider + Health，正式 Hurtbox/Hitbox 分层待做 |
| 敌我阵营判定 | 部分 | `targetTag`，正式 Faction 组件待做 |
| 击退、硬直、无敌时间 | 部分 | 击退/无敌已做，硬直待做 |
| 暴击、元素、异常状态 | 待做 | 建议后续 `DamageInfo` 结构化 |
| 攻击/受击/命中特效 | 待做 | 资源和对象池待做 |
| 音效反馈和屏幕震动 | 部分 | `FeedbackManager` + `AudioFeedbackConfig` 骨架 |

## 四、怪物系统

| 功能 | 状态 | 位置/说明 |
| --- | --- | --- |
| 巡逻/发现/追击/攻击/返回 | 部分 | `EnemyAI` 有巡逻追击，返回和攻击动作待细化 |
| 飞行/远程/跳跃/盾牌模板 | 部分 | `EnemyArchetype` 已定义，具体 AI 待做 |
| 怪物血条 | 部分 | `WorldHealthBar` 骨架 |
| 受击表现 | 部分 | Health 闪色/击退，闪白材质待做 |
| 死亡动画 | 待做 | 当前死亡销毁 |
| 掉落 | 部分 | `DropTable` 配置，EnemyAI 掉落接口已有 |
| 刷怪点与刷新 | 部分 | `EnemySpawner` |
| 怪物配置表 | 部分 | `EnemyConfig` |

## 五、Boss 系统

| 功能 | 状态 | 位置/说明 |
| --- | --- | --- |
| 行为树或状态机 | 部分 | `BossController` 状态机原型 |
| 多阶段 | 部分 | `BossConfig.phaseTwoHealthPercent` 字段，切阶段逻辑待做 |
| Boss 血条 UI | 部分 | `WorldHealthBar` 可复用，专用 Boss UI 待做 |
| Boss 技能 | 部分 | 近战/跳跃已有，远程/冲锋/召唤/范围待做 |
| 受击、硬直、阶段切换 | 部分 | 受击已有，硬直/切阶段待做 |
| 锁场 | 部分 | `BossArenaController` |
| 死亡演出与奖励 | 部分 | `BossConfig.rewardPrefab` 字段，演出待做 |
| 击败状态存档 | 部分 | `SaveData.defeatedBossIds` 字段，写入点待做 |

## 六、交互系统

| 功能 | 状态 | 位置/说明 |
| --- | --- | --- |
| E 键交互 | 已做 | `Interactor` |
| NPC 对话 | 已做 | `DialogueNpc` |
| 商店 E 打开/Esc 关闭 | 已做 | `ShopNpc` + `ShopController` |
| 宝箱 | 部分 | `Chest` |
| 门、机关、拉杆、传送点 | 部分 | `SceneDoor`、`Lever`、`Teleporter` |
| 可拾取物品 | 已做 | `PickupItem` |
| 场景提示 UI | 已做 | `HudController` |
| 交互优先级 | 待做 | 当前取最后进入触发器的对象 |

## 七、背包与物品

| 功能 | 状态 | 位置/说明 |
| --- | --- | --- |
| I 键背包 | 已做 | `InventoryUIController` |
| 自动拾取 | 已做 | `PickupItem` |
| 物品分类 | 部分 | `ItemType` |
| 使用物品 | 待做 | 需要 `IUsableItem` 或效果表 |
| 装备/护符 | 待做 | 建议独立 Equipment/Charm 系统 |
| 商店购买联动 | 已做 | `ShopController` + `InventorySystem` |
| 掉落物吸附 | 已做 | `PickupItem` |

## 八、商店系统

| 功能 | 状态 | 位置/说明 |
| --- | --- | --- |
| 商品列表/价格/库存 | 已做 | `ShopItem` |
| 购买限制 | 部分 | 库存限制已有，能力/剧情限制待做 |
| 货币检查 | 已做 | `SpendCoins` |
| 购买后加入背包/解锁能力 | 部分 | 加背包已做，解锁能力待接 |
| 已购买状态保存 | 部分 | `SaveData.purchasedShopItemIds` 字段 |
| 商店 NPC 对话入口 | 待做 | 当前直接开商店 |
| 选择/确认/取消 | 部分 | 点击购买/Esc 取消，确认弹窗待做 |

## 九、对话与剧情

| 功能 | 状态 | 位置/说明 |
| --- | --- | --- |
| E 触发对话 | 已做 | `DialogueNpc` |
| 多句对白 | 已做 | `DialogueController` |
| 头像/名字/打字机 | 部分 | 名字/打字机已做，头像待做 |
| 对话分支 | 待做 | 需要 DialogueNode |
| 剧情变量 | 部分 | `SaveData.storyFlags` 字段 |
| 任务系统 | 待做 | 后续 QuestConfig |
| 对话后触发事件 | 待做 | 后续 UnityEvent/剧情命令 |

## 十、关卡与地图

| 功能 | 状态 | 位置/说明 |
| --- | --- | --- |
| LDtk 插件接入 | 部分 | 文档和 `LdtkLevelBootstrapper`，插件需手动安装 |
| Tilemap 导入 | 待做 | 依赖真实 LDtk 文件和插件 |
| 碰撞/装饰/背景层 | 待做 | 依赖真实地图 |
| 出生/传送/敌人出生点 | 部分 | `LdtkLevelBootstrapper` 命名桥接 |
| 场景切换门 | 部分 | `SceneDoor` |
| 房间系统 | 待做 | 需 RoomDefinition |
| 小地图/大地图 | 待做 | UI 未做 |
| 地图探索迷雾 | 部分 | `MapExplorationState` 骨架 |
| 存档点/休息点 | 部分 | 存档点已做，休息恢复规则待做 |

## 十二、UI 系统

| 功能 | 状态 | 位置/说明 |
| --- | --- | --- |
| 开始界面 | 已做 | `MvpUiBuilder` 生成 |
| HUD 生命/能量/金币/当前道具 | 部分 | 金币/提示已做，生命 Slider 字段有，能量/道具待做 |
| 暂停菜单 | 部分 | `PauseMenuController` 骨架 |
| 背包/商店/对话 UI | 已做 | 生成器 + Controller |
| Boss 血条 | 部分 | `WorldHealthBar` 可复用 |
| 地图 UI | 待做 | 未实现 |
| 死亡界面 | 待做 | 只有复活逻辑 |
| 存档提示 | 待做 | 保存后 toast 待做 |

## 十三、存档系统

| 功能 | 状态 | 位置/说明 |
| --- | --- | --- |
| 新游戏/继续 | 已做 | `GameManager` |
| 保存位置/血量/金币 | 已做 | `SavePoint` |
| 保存能力/背包 | 部分 | 字段和写入已有，完整物品定义恢复待做 |
| Boss/宝箱/商店/剧情/地图状态 | 部分 | 字段已有，具体触发写入待做 |
| 手动存档 | 已做 | `SavePoint` |
| 自动保存 | 待做 | 未实现 |
| 多存档槽 | 待做 | 当前单存档 |

## 十四、音频与反馈

| 功能 | 状态 | 位置/说明 |
| --- | --- | --- |
| BGM/区域音乐 | 部分 | `AudioFeedbackConfig` 字段 |
| 攻击/跳跃/受击/死亡/UI/环境音 | 部分 | 配置字段，触发点待接 |
| 屏幕震动 | 待做 | 建议 Cinemachine Impulse |
| 时间暂停/慢动作命中 | 部分 | `FeedbackManager.HitStop` |
| 粒子特效 | 待做 | 资源和对象池待做 |

## 本轮补充记录

- 玩家生命 UI：已补，`HudController` + 场景生成器生命 Text/Slider。
- 能量 UI：已补，`PlayerStatsController` + `HudController`。
- 金币 UI：已补，`InventorySystem` 事件驱动。
- 伤害跳字：已补，`DamagePopup`。
- 敌人血条 UI：已补，`WorldHealthBar`，TestRoom/TestRoom2 生成器会挂到怪物/Boss。
- 冲刺无效：已修，支持 LeftShift / RightShift / LeftControl，并避免旧存档清空默认冲刺能力。
- 连击数 UI：已补，`ComboCounter` + `HudController`。
- 能力系统 UI：已补，HUD 显示二段跳/冲刺/下劈/远程开关。
- TestRoom2：已补，一键流程生成 `Assets/Scenes/TestRoom2.unity`。
- TestRoom 传送门：已补，右侧 `去TestRoom2` 按 E。
- TestRoom2 Boss：已补。
- TestRoom2 宝箱：已补。
- TestRoom2 能力门：已补。
- TestRoom2 传送点：已补，`回TestRoom` 按 E。
- TestRoom2 刷怪点：已补，`EnemySpawner`。
- 音频没有声音：已补默认程序音，不依赖外部音频资源。
- 攻击特效：已补，攻击黄色粒子，命中红色粒子。

## 本轮补充记录 2

- HP=0 死亡状态：已补，`PlayerRespawnController` 禁用控制，显示 `DeathUIController`，延迟复活。
- 攻击/受击震屏：已补，`CameraShake` 挂到 Main Camera，`Health.Damage` 触发震屏。
- 传送门可见性：已调整，TestRoom 右侧 x=9.3 大青色门，TestRoom2 左侧 x=-9.8 大青色门，标签写明靠近按 E。
