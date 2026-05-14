using HollowStyleMVP.Boss;
using HollowStyleMVP.Config;
using HollowStyleMVP.Enemies;
using HollowStyleMVP.Inventory;
using HollowStyleMVP.Items;
using HollowStyleMVP.Player;
using HollowStyleMVP.Progression;
using HollowStyleMVP.Shop;
using UnityEditor;
using UnityEngine;

namespace HollowStyleMVP.EditorTools
{
    public static class DefaultConfigBuilder
    {
        public const string PlayerConfigPath = "Assets/ScriptableObjects/Player/DefaultPlayerConfig.asset";
        public const string EnemyConfigPath = "Assets/ScriptableObjects/Enemies/WalkerEnemyConfig.asset";
        public const string BossConfigPath = "Assets/ScriptableObjects/Bosses/TestBossConfig.asset";
        public const string PotionPath = "Assets/ScriptableObjects/Items/TestPotion.asset";
        public const string ShopConfigPath = "Assets/ScriptableObjects/Shops/TestShopConfig.asset";
        public const string AudioConfigPath = "Assets/ScriptableObjects/Audio/DefaultAudioFeedbackConfig.asset";
        public const string RoadmapPath = "Assets/ScriptableObjects/Progression/MvpFeatureRoadmap.asset";

        [MenuItem("Hollow Style MVP/Create Default Config Assets")]
        [MenuItem("Assets/Hollow Style MVP/Create Default Config Assets")]
        public static void CreateDefaultConfigAssets()
        {
            EnsureFolders();
            var player = GetOrCreate<PlayerConfig>(PlayerConfigPath);
            player.maxHealth = 5;
            player.maxEnergy = 100;
            player.startCoins = 0;
            player.attackPower = 1;
            player.defense = 0;
            player.moveSpeed = 8f;
            player.jumpForce = 15f;
            player.variableJumpCutMultiplier = 0.5f;
            player.startWithDoubleJump = true;
            player.startWithDash = true;
            player.startWithDownStrike = false;
            player.startWithRangedAttack = false;
            player.maxAirJumps = 1;
            player.dashSpeed = 18f;
            player.dashDuration = 0.16f;
            player.dashCooldown = 0.45f;
            player.dashInvulnerableSeconds = 0.08f;
            player.attackTime = 0.14f;
            player.attackKnockback = 6f;
            player.comboCount = 1;
            player.respawnDelay = 1.2f;
            EditorUtility.SetDirty(player);

            var enemy = GetOrCreate<EnemyConfig>(EnemyConfigPath);
            enemy.archetype = EnemyArchetype.Walker;
            enemy.maxHealth = 3;
            enemy.contactDamage = 1;
            enemy.patrolSpeed = 2f;
            enemy.chaseSpeed = 4f;
            enemy.detectRange = 7f;
            enemy.attackRange = 1.2f;
            enemy.knockback = 5f;
            EditorUtility.SetDirty(enemy);

            var boss = GetOrCreate<BossConfig>(BossConfigPath);
            boss.maxHealth = 20;
            boss.contactDamage = 2;
            boss.detectRange = 12f;
            boss.moveSpeed = 3.4f;
            boss.attackRange = 2f;
            boss.actionCooldown = 1.3f;
            boss.leapForce = 12f;
            boss.phaseTwoHealthPercent = 50;
            EditorUtility.SetDirty(boss);

            var potion = GetOrCreate<InventoryItem>(PotionPath);
            potion.id = "test_potion";
            potion.displayName = "测试药水";
            potion.type = ItemType.Consumable;
            potion.price = 3;
            potion.description = "用于测试商店购买与背包显示。";
            EditorUtility.SetDirty(potion);

            var shop = GetOrCreate<ShopConfig>(ShopConfigPath);
            shop.shopName = "测试商店";
            shop.savePurchasedState = true;
            shop.items = new[] { new ShopItem { item = potion, price = 3, stock = 3 } };
            EditorUtility.SetDirty(shop);

            var audio = GetOrCreate<AudioFeedbackConfig>(AudioConfigPath);
            EditorUtility.SetDirty(audio);

            var roadmap = GetOrCreate<FeatureRoadmap>(RoadmapPath);
            roadmap.entries = RoadmapEntries();
            EditorUtility.SetDirty(roadmap);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static T GetOrCreate<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static FeatureEntry[] RoadmapEntries()
        {
            return new[]
            {
                Entry("核心玩法", "WASD移动、跳跃、二段跳、冲刺、普通攻击、受伤击退、死亡复活", FeatureStatus.Partial, "已接入 PlayerConfig；下劈、远程攻击、连击表现仍需深化。"),
                Entry("角色系统", "生命、能量、金币、属性配置、能力解锁、动画参数", FeatureStatus.Partial, "配置和动画参数已留好；正式 Animator Controller 和帧动画资源待接入。"),
                Entry("战斗系统", "近战判定、伤害/受击、阵营、击退、硬直、特效/音效/震屏", FeatureStatus.Partial, "基础伤害和击退已完成；暴击、元素、异常状态、完整反馈待扩展。"),
                Entry("怪物系统", "巡逻/发现/追击/攻击/返回、血条、掉落、配置表、刷怪点", FeatureStatus.Partial, "Walker 原型和 EnemyConfig 已有；飞行/远程/盾牌模板待做。"),
                Entry("Boss系统", "状态机/行为树、多阶段、血条、锁场、奖励、存档", FeatureStatus.Partial, "状态机、BossConfig、锁场骨架已加；行为树和演出待做。"),
                Entry("交互系统", "E交互、对话、商店、宝箱、门、拉杆、传送、提示、优先级", FeatureStatus.Partial, "交互接口已统一；优先级排序还需完善。"),
                Entry("背包与物品", "背包、自动拾取、分类、使用物品、装备/护符、商店联动", FeatureStatus.Partial, "背包与购买已通；使用物品和装备系统待做。"),
                Entry("商店系统", "商品、价格、库存、购买限制、保存购买状态、确认取消", FeatureStatus.Partial, "商品配置已加；确认弹窗和购买状态存档待完善。"),
                Entry("对话剧情", "多句对白、头像名字、分支、剧情变量、事件触发", FeatureStatus.Partial, "多句和名字已做；头像、分支、任务与剧情事件待做。"),
                Entry("关卡地图", "LDtk、Tilemap、碰撞/装饰/背景、出生/传送/刷怪、小地图、迷雾", FeatureStatus.Partial, "LDtk桥接和说明已加；真实地图文件和地图UI待接入。"),
                Entry("UI系统", "开始、HUD、暂停、背包、商店、对话、Boss血条、地图、死亡、存档提示", FeatureStatus.Partial, "主要 UI 原型可生成；地图/死亡/存档提示待美化。"),
                Entry("存档系统", "新游戏、继续、位置、血量、金币、能力、背包、Boss/宝箱/商店/剧情/地图", FeatureStatus.Partial, "基础存档已做；能力/背包/世界状态字段已规划需完整写入读取。"),
                Entry("音频反馈", "BGM、区域音乐、音效、UI音、环境音、震屏、慢动作、粒子", FeatureStatus.Planned, "AudioFeedbackConfig 和 FeedbackManager 已加，资源与触发点待接。")
            };
        }

        private static FeatureEntry Entry(string module, string feature, FeatureStatus status, string note)
        {
            return new FeatureEntry { module = module, feature = feature, status = status, note = note };
        }

        private static void EnsureFolders()
        {
            Ensure("Assets", "ScriptableObjects");
            Ensure("Assets/ScriptableObjects", "Player");
            Ensure("Assets/ScriptableObjects", "Enemies");
            Ensure("Assets/ScriptableObjects", "Bosses");
            Ensure("Assets/ScriptableObjects", "Items");
            Ensure("Assets/ScriptableObjects", "Shops");
            Ensure("Assets/ScriptableObjects", "Audio");
            Ensure("Assets/ScriptableObjects", "Progression");
        }

        private static void Ensure(string parent, string child)
        {
            string path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(parent, child);
        }
    }
}
