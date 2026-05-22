using HollowStyleMVP.Combat;
using HollowStyleMVP.Config;
using HollowStyleMVP.Enemies;
using HollowStyleMVP.Inventory;
using HollowStyleMVP.Items;
using HollowStyleMVP.Player;
using HollowStyleMVP.Roguelike;
using HollowStyleMVP.Shop;
using UnityEditor;
using UnityEngine;

namespace HollowStyleMVP.EditorTools
{
    public static class DefaultConfigBuilder
    {
        public const string PlayerConfigPath = "Assets/ScriptableObjects/Player/DefaultPlayerConfig.asset";
        public const string EnemyConfigPath = "Assets/ScriptableObjects/Enemies/WalkerEnemyConfig.asset";
        public const string PotionPath = "Assets/ScriptableObjects/Items/TestPotion.asset";
        public const string MaterialPath = "Assets/ScriptableObjects/Items/TestOreMaterial.asset";
        public const string CharmPath = "Assets/ScriptableObjects/Items/TestCritCharm.asset";
        public const string WeaponPath = "Assets/ScriptableObjects/Items/TestBoneNail.asset";
        public const string ArmorPath = "Assets/ScriptableObjects/Items/TestShellArmor.asset";
        public const string AbilityItemPath = "Assets/ScriptableObjects/Items/TestDownStrikeSkill.asset";
        public const string EnemyDropPath = "Assets/ScriptableObjects/Items/WalkerDropTable.asset";
        public const string ShopConfigPath = "Assets/ScriptableObjects/Shops/TestShopConfig.asset";
        public const string AudioConfigPath = "Assets/ScriptableObjects/Audio/DefaultAudioFeedbackConfig.asset";

        [MenuItem("Hollow Style MVP/Create Roguelike Config Assets")]
        [MenuItem("Assets/Hollow Style MVP/Create Roguelike Config Assets")]
        public static void CreateDefaultConfigAssets()
        {
            EnsureFolders();

            var player = GetOrCreate<PlayerConfig>(PlayerConfigPath);
            player.maxHealth = 250;
            player.maxEnergy = 120;
            player.startCoins = 20;
            player.attackPower = 1;
            player.defense = 0;
            player.critChance = 0.08f;
            player.critResistance = 0.02f;
            player.critDamageMultiplier = 1.5f;
            player.moveSpeed = 5.8f;
            player.startWithDoubleJump = true;
            player.startWithDash = true;
            player.startWithDownStrike = true;
            player.maxAirJumps = 1;
            player.dashSpeed = 12.5f;
            player.dashDuration = 0.12f;
            player.dashCooldown = 0.34f;
            player.attackTime = 0.12f;
            player.attackKnockback = 6f;
            EditorUtility.SetDirty(player);

            var potion = ConfigureItem(PotionPath, "test_potion", "测试药水", ItemType.Consumable, 3, "拾取或购买后立即回复 1 点生命。", heal: 1);
            var material = ConfigureItem(MaterialPath, "test_ore", "测试矿石", ItemType.Material, 2, "房间奖励和掉落材料。");
            var charm = ConfigureItem(CharmPath, "crit_charm", "裂光护符", ItemType.Charm, 8, "自动装备，提升暴击率。", autoEquip: true);
            charm.charmCost = 1;
            charm.statModifier = new StatModifier { critChance = 0.18f, attackPower = 1 };
            charm.projectileModifier = new ProjectileModifier { damageBonus = 1, speedBonus = 1.5f, fireDelayMultiplier = 0.85f };
            EditorUtility.SetDirty(charm);

            var weapon = ConfigureItem(WeaponPath, "bone_nail", "骨钉", ItemType.Equipment, 10, "自动装备，提升攻击力。", autoEquip: true);
            weapon.equipmentSlot = EquipmentSlot.Weapon;
            weapon.statModifier = new StatModifier { attackPower = 2, critChance = 0.05f };
            weapon.projectileModifier = new ProjectileModifier { damageBonus = 1, rangeBonus = 1.5f };
            EditorUtility.SetDirty(weapon);

            var armor = ConfigureItem(ArmorPath, "shell_armor", "甲壳护甲", ItemType.Equipment, 9, "自动装备，提升防御和抗暴。", autoEquip: true);
            armor.equipmentSlot = EquipmentSlot.Armor;
            armor.statModifier = new StatModifier { defense = 1, critResistance = 0.12f, maxHealth = 1 };
            EditorUtility.SetDirty(armor);

            var skill = ConfigureItem(AbilityItemPath, "down_strike_skill", "下劈技巧", ItemType.Ability, 6, "拾取后解锁下劈。");
            skill.unlockAbilityOnPickup = true;
            skill.abilityToUnlock = PlayerAbility.DownStrike;
            EditorUtility.SetDirty(skill);

            var enemyDrop = GetOrCreate<DropTable>(EnemyDropPath);
            enemyDrop.drops = new[]
            {
                new DropEntry { kind = DropKind.Coins, chance = 1f, minAmount = 2, maxAmount = 5, color = Color.yellow },
                new DropEntry { kind = DropKind.Heal, chance = 0.25f, minAmount = 1, maxAmount = 1, color = Color.green },
                new DropEntry { kind = DropKind.Material, item = material, chance = 0.55f, minAmount = 1, maxAmount = 2, color = new Color(0.6f, 0.8f, 1f) }
            };
            EditorUtility.SetDirty(enemyDrop);

            var enemy = GetOrCreate<EnemyConfig>(EnemyConfigPath);
            enemy.archetype = EnemyArchetype.Walker;
            enemy.maxHealth = 3;
            enemy.contactDamage = 1;
            enemy.patrolSpeed = 2f;
            enemy.chaseSpeed = 4f;
            enemy.detectRange = 7f;
            enemy.attackRange = 1.2f;
            enemy.knockback = 5f;
            enemy.dropTable = enemyDrop;
            EditorUtility.SetDirty(enemy);

            var shop = GetOrCreate<ShopConfig>(ShopConfigPath);
            shop.shopName = "测试商店";
            shop.savePurchasedState = false;
            shop.items = new[]
            {
                new ShopItem { item = potion, price = 3, stock = 5 },
                new ShopItem { item = weapon, price = 10, stock = 1 },
                new ShopItem { item = armor, price = 9, stock = 1 },
                new ShopItem { item = charm, price = 8, stock = 1 },
                new ShopItem { item = skill, price = 6, stock = 1 }
            };
            EditorUtility.SetDirty(shop);

            var audio = GetOrCreate<AudioFeedbackConfig>(AudioConfigPath);
            EditorUtility.SetDirty(audio);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static InventoryItem ConfigureItem(string path, string id, string name, ItemType type, int price, string desc, int heal = 0, bool autoEquip = false)
        {
            var item = GetOrCreate<InventoryItem>(path);
            item.id = id;
            item.displayName = name;
            item.type = type;
            item.price = price;
            item.description = desc;
            item.healAmount = heal;
            item.autoEquipForTesting = autoEquip;
            EditorUtility.SetDirty(item);
            return item;
        }

        private static T GetOrCreate<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void EnsureFolders()
        {
            Ensure("Assets", "ScriptableObjects");
            Ensure("Assets/ScriptableObjects", "Player");
            Ensure("Assets/ScriptableObjects", "Enemies");
            Ensure("Assets/ScriptableObjects", "Items");
            Ensure("Assets/ScriptableObjects", "Shops");
            Ensure("Assets/ScriptableObjects", "Audio");
        }

        private static void Ensure(string parent, string child)
        {
            string path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(parent, child);
        }
    }
}
