using System.Collections.Generic;
using System.Text.RegularExpressions;
using HollowStyleMVP.Combat;
using HollowStyleMVP.Core;
using HollowStyleMVP.Dialogue;
using HollowStyleMVP.Enemies;
using HollowStyleMVP.Interaction;
using HollowStyleMVP.Inventory;
using HollowStyleMVP.Items;
using HollowStyleMVP.Player;
using HollowStyleMVP.Shop;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HollowStyleMVP.Level
{
    public class TestSceneRoomSetup : MonoBehaviour
    {
        private const string RootName = "Runtime Testscene Content";
        private const string BossArtPrefabPath = "Assets/Art/Mosters/BossPrefabTemplate.prefab";
        private static readonly HashSet<int> clearedCombatRooms = new HashSet<int>();
        private static readonly List<Health> livingLocks = new List<Health>();
        private static bool currentRoomLocked;
        private static bool treasureChestOpened;
        private static bool victoryShown;
        private static int currentRoomNumber;
        private static int configuredSceneHandle = -1;

        public static bool IsCurrentRoomLocked => currentRoomLocked;

        public static void Apply(string sceneName, bool forceRebuild = false)
        {
            if (!TryGetRoomNumber(sceneName, out int roomNumber)) return;
            EnsureVisibleFightHud();
            int sceneHandle = UnityEngine.SceneManagement.SceneManager.GetActiveScene().handle;
            if (!forceRebuild && configuredSceneHandle == sceneHandle && GameObject.Find(RootName) != null) return;

            configuredSceneHandle = sceneHandle;
            currentRoomNumber = roomNumber;
            livingLocks.Clear();
            currentRoomLocked = false;
            SetSceneDoorsLocked(false);
            EnsurePlayerBoundaryLimiter();

            var oldRoot = GameObject.Find(RootName);
            if (oldRoot != null) Destroy(oldRoot);
            CleanupCopiedRoomContent();

            var root = new GameObject(RootName);
            switch (roomNumber)
            {
                case 1:
                    CreateRoomMarker(root.transform, "TESTSCENE 1\n初始房", new Vector3(0f, 2.2f, -3f), Color.white);
                    CreateStartDialogueNpc(root.transform, new Vector3(-1.6f, 0.65f, 0f));
                    GameEvents.RaiseSceneMessage("初始房：向右进入宝箱房");
                    break;
                case 2:
                    CreateRoomMarker(root.transform, "TESTSCENE 2\n宝箱房", new Vector3(0f, 2.2f, -3f), Color.cyan);
                    CreateChest(root.transform, new Vector3(-1.35f, -1.05f, 0f), treasureChestOpened);
                    GameEvents.RaiseSceneMessage(treasureChestOpened ? "宝箱房：宝箱已领取" : "宝箱房：按 E 打开宝箱");
                    break;
                case 3:
                    CreateRoomMarker(root.transform, "TESTSCENE 3\n战斗房", new Vector3(0f, 2.2f, -3f), Color.yellow);
                    if (clearedCombatRooms.Contains(roomNumber))
                    {
                        CreateRoomMarker(root.transform, "已清理", new Vector3(0f, 0.35f, -3f), Color.green, 34, 0.08f);
                        GameEvents.RaiseSceneMessage("战斗房已清理：出口已开启");
                        break;
                    }

                    LockRoom("战斗房：击败所有怪物后开门");
                    RegisterLockEnemy(CreateEnemy(root.transform, new Vector3(-1.8f, 0.7f, 0f), "战斗怪 A", Color.red, 8, 1, 4));
                    RegisterLockEnemy(CreateEnemy(root.transform, new Vector3(1.8f, -0.5f, 0f), "战斗怪 B", new Color(1f, 0.45f, 0.1f), 8, 1, 4));
                    break;
                case 4:
                    CreateRoomMarker(root.transform, "TESTSCENE 4\n商店房", new Vector3(0f, 2.2f, -3f), Color.green);
                    CreateShop(root.transform, new Vector3(0f, 0.6f, 0f));
                    GameEvents.RaiseSceneMessage("商店房：按 E 打开商店");
                    break;
                case 5:
                    CreateRoomMarker(root.transform, "TESTSCENE 5\nBoss房", new Vector3(0f, 2.2f, -3f), Color.magenta);
                    if (clearedCombatRooms.Contains(roomNumber))
                    {
                        CreateRoomMarker(root.transform, "Boss已击败", new Vector3(0f, 0.35f, -3f), Color.green, 34, 0.08f);
                        GameEvents.RaiseSceneMessage("Boss房已清理：出口已开启");
                        break;
                    }

                    LockRoom("Boss房：击败 Boss 后开门");
                    RegisterLockEnemy(CreateBossEnemy(root.transform, new Vector3(0f, 0.5f, 0f)));
                    break;
            }
        }

        private static void EnsureVisibleFightHud()
        {
            if (FindAnyObjectByType<RuntimeVisibleFightHud>() != null) return;

            var obj = new GameObject(nameof(RuntimeVisibleFightHud));
            DontDestroyOnLoad(obj);
            obj.AddComponent<RuntimeVisibleFightHud>();
        }

        private static void LockRoom(string message)
        {
            currentRoomLocked = true;
            SetSceneDoorsLocked(true);
            GameEvents.RaiseSceneMessage(message);
        }

        private static void RegisterLockEnemy(Health health)
        {
            if (health == null) return;
            livingLocks.Add(health);
            health.Died += () =>
            {
                livingLocks.Remove(health);
                if (livingLocks.Count > 0) return;
                clearedCombatRooms.Add(currentRoomNumber);
                currentRoomLocked = false;
                SetSceneDoorsLocked(false);
                if (currentRoomNumber == 5)
                {
                    ShowVictoryScreen();
                    return;
                }

                GameEvents.RaiseSceneMessage("房间已解锁");
            };
        }

        private static void ShowVictoryScreen()
        {
            if (victoryShown) return;
            victoryShown = true;
            GameEvents.RaiseSceneMessage("Boss已击败，恭喜通关");

            var obj = new GameObject("Runtime Victory Screen");
            Object.DontDestroyOnLoad(obj);
            obj.AddComponent<RuntimeVictoryScreen>();
        }

        public static void ResetRunState()
        {
            clearedCombatRooms.Clear();
            livingLocks.Clear();
            currentRoomLocked = false;
            treasureChestOpened = false;
            victoryShown = false;
            configuredSceneHandle = -1;
            Time.timeScale = 1f;
        }

        private static void SetSceneDoorsLocked(bool locked)
        {
            var doors = FindObjectsByType<SceneDoor>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var door in doors)
                if (door != null)
                    door.SetLocked(locked);
        }

        private static void CleanupCopiedRoomContent()
        {
            foreach (var chest in FindObjectsByType<Chest>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
                if (chest != null)
                    Destroy(chest.gameObject);

            foreach (var shop in FindObjectsByType<ShopNpc>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
                if (shop != null)
                    Destroy(shop.gameObject);

            foreach (var enemy in FindObjectsByType<EnemyAI>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
                if (enemy != null)
                    Destroy(enemy.gameObject);

            DestroySceneObject("Stage1 Closed Chest");
            DestroySceneObject("Stage1 Enemy A");
            DestroySceneObject("Stage1 Enemy B");
        }

        private static void CreateChest(Transform parent, Vector3 position, bool opened)
        {
            var chest = CreateSpriteObject("Treasure Chest", parent, position, opened ? new Color(0.35f, 0.75f, 1f) : new Color(0.95f, 0.66f, 0.18f), new Vector3(0.7f, 0.48f, 1f));
            var collider = chest.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(2.2f, 1.8f);
            chest.AddComponent<RuntimeTreasureChest>().Configure(opened);
            CreateRoomMarker(parent, opened ? "已领取" : "宝箱\nE", position + new Vector3(0f, 0.65f, -0.2f), opened ? Color.cyan : Color.yellow, 30, 0.065f);
        }

        private static void CreateStartDialogueNpc(Transform parent, Vector3 position)
        {
            var npc = CreateSpriteObject("Start Dialogue NPC", parent, position, new Color(0.55f, 0.95f, 1f), new Vector3(0.72f, 0.95f, 1f));
            var collider = npc.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(1.4f, 1.6f);
            npc.AddComponent<RuntimeStartDialogueNpc>().Configure(Resources.Load<DialogueAsset>("StartRoomDialogue"));
            CreateRoomMarker(parent, "引导员\nE", position + new Vector3(0f, 0.9f, -0.2f), Color.cyan, 30, 0.065f);
        }

        private static void DestroySceneObject(string objectName)
        {
            var obj = GameObject.Find(objectName);
            if (obj != null) Destroy(obj);
        }

        private static Health CreateEnemy(Transform parent, Vector3 position, string name, Color color, int maxHealth, int contactDamage, int dropCoins, bool boss = false)
        {
            var enemy = CreateSpriteObject(name, parent, position, color, boss ? new Vector3(1.45f, 1.45f, 1f) : new Vector3(0.8f, 0.8f, 1f));
            enemy.AddComponent<Rigidbody2D>();
            enemy.AddComponent<CircleCollider2D>();
            var health = enemy.AddComponent<Health>();
            var stats = enemy.AddComponent<CombatStats>();
            var contact = enemy.AddComponent<ContactDamage>();
            enemy.AddComponent<EnemyAI>();
            enemy.AddComponent<RoomBoundaryLimiter>();
            var drop = enemy.AddComponent<CoinDropOnDeath>();

            health.Configure(maxHealth, 0.18f, true);
            stats.SetBase(maxHealth, 1, contactDamage, boss ? 1 : 0, 0.05f, 0f, 1.5f);
            contact.Configure(contactDamage, boss ? 8f : 6f);
            drop.Configure(dropCoins);

            if (boss) enemy.AddComponent<BossHealthHud>().Bind(health);
            return health;
        }

        private static Health CreateBossEnemy(Transform parent, Vector3 position)
        {
            var prefab = LoadBossArtPrefab();
            if (prefab == null)
                return CreateEnemy(parent, position, "Test Boss", Color.magenta, 35, 2, 12, true);

            var enemy = Instantiate(prefab, position, Quaternion.identity, parent);
            enemy.name = "Test Boss";
            enemy.AddComponent<Rigidbody2D>();
            var collider = enemy.AddComponent<CircleCollider2D>();
            collider.radius = 0.7f;
            var health = enemy.AddComponent<Health>();
            var stats = enemy.AddComponent<CombatStats>();
            var contact = enemy.AddComponent<ContactDamage>();
            enemy.AddComponent<EnemyAI>();
            enemy.AddComponent<RoomBoundaryLimiter>();
            var drop = enemy.AddComponent<CoinDropOnDeath>();

            health.Configure(35, 0.18f, true);
            stats.SetBase(35, 1, 2, 1, 0.05f, 0f, 1.5f);
            contact.Configure(2, 8f);
            drop.Configure(12);
            enemy.AddComponent<BossHealthHud>().Bind(health);
            return health;
        }

        private static GameObject LoadBossArtPrefab()
        {
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<GameObject>(BossArtPrefabPath);
#else
            return null;
#endif
        }

        private static void CreateShop(Transform parent, Vector3 position)
        {
            var shop = CreateSpriteObject("Shop NPC", parent, position, new Color(0.25f, 0.95f, 0.55f), new Vector3(0.75f, 1f, 1f));
            var collider = shop.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(1.2f, 1.4f);
            shop.AddComponent<RuntimeShopNpc>().Configure();
            CreateRoomMarker(parent, "商店\nE", position + new Vector3(0f, 0.9f, -0.2f), Color.green, 30, 0.065f);
        }

        private static void EnsurePlayerBoundaryLimiter()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null || player.GetComponent<RoomBoundaryLimiter>() != null) return;
            player.AddComponent<RoomBoundaryLimiter>();
        }

        private static GameObject CreateSpriteObject(string name, Transform parent, Vector3 position, Color color, Vector3 scale)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent);
            obj.transform.position = position;
            obj.transform.localScale = scale;
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = MakeSprite();
            renderer.color = color;
            renderer.sortingOrder = 20;
            return obj;
        }

        private static void CreateRoomMarker(Transform parent, string text, Vector3 position, Color color, int fontSize = 42, float characterSize = 0.09f)
        {
            var obj = new GameObject(text.Replace('\n', ' '));
            obj.transform.SetParent(parent);
            obj.transform.position = position;
            var mesh = obj.AddComponent<TextMesh>();
            mesh.text = text;
            mesh.anchor = TextAnchor.MiddleCenter;
            mesh.alignment = TextAlignment.Center;
            mesh.fontSize = fontSize;
            mesh.characterSize = characterSize;
            mesh.color = color;
            mesh.GetComponent<MeshRenderer>().sortingOrder = 1001;
        }

        private static Sprite MakeSprite()
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        }

        private static bool TryGetRoomNumber(string sceneName, out int roomNumber)
        {
            roomNumber = 0;
            if (sceneName == "TestRoom")
            {
                roomNumber = 1;
                return true;
            }

            var match = Regex.Match(sceneName, @"^TestRoom(\d+)$");
            return match.Success && int.TryParse(match.Groups[1].Value, out roomNumber);
        }

        public static void MarkTreasureChestOpened()
        {
            treasureChestOpened = true;
        }
    }

    public class CoinDropOnDeath : MonoBehaviour
    {
        private int coins = 1;
        private Health health;

        public void Configure(int newCoins)
        {
            coins = Mathf.Max(0, newCoins);
        }

        private void Awake()
        {
            health = GetComponent<Health>();
            if (health != null) health.Died += Drop;
        }

        private void OnDestroy()
        {
            if (health != null) health.Died -= Drop;
        }

        private void Drop()
        {
            if (coins <= 0) return;
            var obj = new GameObject("Dropped Coins");
            obj.transform.position = transform.position;
            obj.transform.localScale = Vector3.one * 0.35f;
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = TestSceneRoomSetupSprite.Sprite;
            renderer.color = Color.yellow;
            renderer.sortingOrder = 40;
            var collider = obj.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            obj.AddComponent<PickupItem>().Configure(coins, null, 1);
        }
    }

    public class RuntimeTreasureChest : MonoBehaviour
    {
        private const int RewardCoins = 12;
        private bool opened;
        private SpriteRenderer spriteRenderer;

        public void Configure(bool alreadyOpened)
        {
            opened = alreadyOpened;
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (opened || !Input.GetKeyDown(KeyCode.E)) return;

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;
            if (Vector2.Distance(transform.position, player.transform.position) > 2.0f) return;

            Open();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (opened || !other.CompareTag("Player") || !Input.GetKeyDown(KeyCode.E)) return;
            Open();
        }

        private void Open()
        {
            opened = true;
            TestSceneRoomSetup.MarkTreasureChestOpened();
            if (InventorySystem.Instance != null)
                InventorySystem.Instance.AddCoins(RewardCoins);

            if (spriteRenderer != null)
                spriteRenderer.color = new Color(0.35f, 0.75f, 1f);

            GameEvents.RaiseSceneMessage($"+{RewardCoins} 金币，宝箱已打开");
        }
    }

    public class RuntimeStartDialogueNpc : MonoBehaviour, IInteractable
    {
        private DialogueAsset dialogue;
        private readonly string[] fallbackLines =
        {
            "欢迎来到测试流程。向右走可以进入宝箱房。",
            "宝箱房之后是战斗房，清理怪物后房门才会重新开启。",
            "继续往右是商店房，最后是 Boss 房。准备好就出发吧。"
        };

        public string Prompt => "按 E 对话";

        public void Configure(DialogueAsset newDialogue)
        {
            dialogue = newDialogue;
        }

        public void Interact()
        {
            OpenDialogue();
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.E) || UiModalState.HasOpenModal) return;

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;
            if (Vector2.Distance(transform.position, player.transform.position) > 1.8f) return;

            OpenDialogue();
        }

        private void OpenDialogue()
        {
            if (dialogue != null) DialogueController.Instance?.Open(dialogue);
            else DialogueController.Instance?.OpenLines("引导员", fallbackLines);
        }
    }

    public class RuntimeShopNpc : MonoBehaviour, IInteractable
    {
        private ShopItem[] items;

        public string Prompt => "按 E 打开商店";

        public void Configure()
        {
            items = new[]
            {
                new ShopItem { item = CreateItem("test_potion_runtime", "测试药水", ItemType.Consumable, 3, 2), price = 3, stock = 5 },
                new ShopItem { item = CreateEquipmentItem("shop_health_badge_runtime", "生命徽章", EquipmentSlot.Accessory, 1, new StatModifier { maxHealth = 5 }, "装备后最大生命 +5。"), price = 1, stock = 1 },
                new ShopItem { item = CreateEquipmentItem("shop_attack_blade_runtime", "攻击刀柄", EquipmentSlot.Weapon, 1, new StatModifier { attackPower = 1 }, "装备后攻击力 +1。"), price = 1, stock = 1 },
                new ShopItem { item = CreateEquipmentItem("shop_defense_plate_runtime", "防御甲片", EquipmentSlot.Armor, 1, new StatModifier { defense = 1 }, "装备后防御力 +1。"), price = 1, stock = 1 },
                new ShopItem { item = CreateSkillItem("shop_skill_push_runtime", "震退脉冲", "击退身旁一定半径敌人，造成 1 点伤害，消耗 20 能量。"), price = 1, stock = 1 },
                new ShopItem { item = CreateSkillItem("shop_skill_heal_runtime", "生命回流", "回复 5 HP，消耗 50 能量。"), price = 1, stock = 1 },
                new ShopItem { item = CreateSkillItem("shop_skill_charged_shot_runtime", "蓄能弹", "长按蓄力 2 秒后释放一个 10 伤害子弹，消耗 30 能量。"), price = 1, stock = 1 }
            };
        }

        public void Interact()
        {
            if (items == null) Configure();
            ShopController.Instance?.Open("TestScene 商店", items);
        }

        private static InventoryItem CreateItem(string id, string displayName, ItemType type, int price, int heal)
        {
            var item = ScriptableObject.CreateInstance<InventoryItem>();
            item.id = id;
            item.displayName = displayName;
            item.type = type;
            item.price = price;
            item.healAmount = heal;
            return item;
        }

        private static InventoryItem CreateEquipmentItem(string id, string displayName, EquipmentSlot slot, int price, StatModifier modifier, string description)
        {
            var item = CreateItem(id, displayName, ItemType.Equipment, price, 0);
            item.equipmentSlot = slot;
            item.statModifier = modifier;
            item.description = description;
            return item;
        }

        private static InventoryItem CreateSkillItem(string id, string displayName, string description)
        {
            var item = CreateItem(id, displayName, ItemType.Ability, 1, 0);
            item.description = description;
            return item;
        }
    }

    public class BossHealthHud : MonoBehaviour
    {
        private Health target;

        public void Bind(Health newTarget)
        {
            target = newTarget;
        }

        private void OnGUI()
        {
            if (target == null || target.IsDead) return;

            const float width = 420f;
            const float height = 24f;
            float x = (Screen.width - width) * 0.5f;
            var rect = new Rect(x, 24f, width, height);
            var frame = HollowStyleMVP.UI.FightHudSkin.LoadTexture("BossHpBar_Frame.png");
            var fill = HollowStyleMVP.UI.FightHudSkin.LoadTexture("BossHpBar_Fill.png");
            float ratio = Mathf.Clamp01((float)target.CurrentHealth / target.MaxHealth);
            var oldColor = GUI.color;
            if (frame != null && fill != null)
            {
                var frameRect = new Rect((Screen.width - 719f * 0.58f) * 0.5f, 28f, 719f * 0.58f, 32f * 0.58f);
                var fillRect = new Rect(frameRect.x + 11f * 0.58f, frameRect.y + 7f * 0.58f, 697f * 0.58f, 18f * 0.58f);
                GUI.BeginGroup(new Rect(fillRect.x, fillRect.y, fillRect.width * ratio, fillRect.height));
                GUI.DrawTexture(new Rect(0f, 0f, fillRect.width, fillRect.height), fill, ScaleMode.StretchToFill, true);
                GUI.EndGroup();
                GUI.DrawTexture(frameRect, frame, ScaleMode.StretchToFill, true);
            }
            else
            {
                GUI.Box(new Rect(x - 2f, 22f, width + 4f, height + 20f), "BOSS");
                GUI.Box(rect, string.Empty);
                GUI.color = Color.magenta;
                GUI.DrawTexture(new Rect(x + 2f, 26f, (width - 4f) * ratio, height - 4f), Texture2D.whiteTexture);
                GUI.color = Color.white;
            }
            GUI.color = Color.white;
            GUI.Label(new Rect(x, 24f, width, height), $"{target.CurrentHealth} / {target.MaxHealth}");
            GUI.color = oldColor;
        }
    }

    public class RuntimeVisibleFightHud : MonoBehaviour
    {
        private Health playerHealth;
        private PlayerStatsController playerEnergy;
        private CombatStats playerStats;
        private GUIStyle titleStyle;
        private GUIStyle valueStyle;
        private GUIStyle smallStyle;

        private void Awake()
        {
            hideFlags = HideFlags.DontSave;
        }

        private void Update()
        {
            if (playerHealth != null && playerEnergy != null && playerStats != null) return;

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            player.TryGetComponent(out playerHealth);
            player.TryGetComponent(out playerEnergy);
            player.TryGetComponent(out playerStats);
        }

        private void OnGUI()
        {
            EnsureStyles();

            int hp = playerHealth != null ? playerHealth.CurrentHealth : 0;
            int maxHp = playerHealth != null ? playerHealth.MaxHealth : 0;
            int energy = playerEnergy != null ? playerEnergy.CurrentEnergy : 0;
            int maxEnergy = playerEnergy != null ? playerEnergy.MaxEnergy : 0;
            int attack = playerStats != null ? playerStats.Snapshot.attackPower : 0;
            int coins = InventorySystem.Instance != null ? InventorySystem.Instance.Coins : 0;

            DrawPanel(new Rect(16f, 16f, 380f, 136f), new Color(0f, 0f, 0f, 0.78f), new Color(0.1f, 0.9f, 1f, 1f));
            GUI.Label(new Rect(30f, 22f, 340f, 30f), "FIGHT HUD", titleStyle);
            DrawValueRow(36f, "HP", $"{hp}/{maxHp}", new Color(1f, 0.08f, 0.16f, 1f));
            DrawValueRow(72f, "EN", $"{energy}/{maxEnergy}", new Color(0.1f, 0.9f, 1f, 1f));
            DrawValueRow(108f, "ATK / CR", $"{attack} / {coins}", new Color(1f, 0.78f, 0.18f, 1f));

            float stripWidth = Mathf.Min(520f, Screen.width - 40f);
            var strip = new Rect((Screen.width - stripWidth) * 0.5f, Screen.height - 92f, stripWidth, 72f);
            DrawPanel(strip, new Color(0f, 0f, 0f, 0.68f), new Color(1f, 0.1f, 0.9f, 1f));
            GUI.Label(new Rect(strip.x + 16f, strip.y + 12f, strip.width - 32f, 24f), "WEAPONS: GUN  DAGGER  BOMB", valueStyle);
            GUI.Label(new Rect(strip.x + 16f, strip.y + 42f, strip.width - 32f, 20f), "ESC / top-right button pauses", smallStyle);
        }

        private void DrawValueRow(float y, string label, string value, Color accent)
        {
            var oldColor = GUI.color;
            GUI.color = accent;
            GUI.DrawTexture(new Rect(30f, y + 10f, 5f, 22f), Texture2D.whiteTexture);
            GUI.color = oldColor;
            GUI.Label(new Rect(44f, y, 120f, 36f), label, smallStyle);
            GUI.Label(new Rect(160f, y, 200f, 36f), value, valueStyle);
        }

        private void DrawPanel(Rect rect, Color background, Color accent)
        {
            var oldColor = GUI.color;
            GUI.color = background;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = accent;
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 3f), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.yMax - 3f, rect.width, 3f), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y, 3f, rect.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMax - 3f, rect.y, 3f, rect.height), Texture2D.whiteTexture);
            GUI.color = oldColor;
        }

        private void EnsureStyles()
        {
            if (titleStyle != null) return;

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 22,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };
            valueStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 22,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };
            smallStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.72f, 0.98f, 1f, 1f) }
            };
        }
    }

    public class RuntimeVictoryScreen : MonoBehaviour
    {
        private void Awake()
        {
            Time.timeScale = 0f;
            UiModalState.Open();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R)) Restart();
            if (Input.GetKeyDown(KeyCode.Escape)) BackToTitle();
        }

        private void OnGUI()
        {
            var previousColor = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.78f);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = previousColor;

            float width = Mathf.Min(620f, Screen.width - 80f);
            float height = 260f;
            var rect = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);

            GUI.Box(rect, string.Empty);
            GUIStyle title = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 44,
                normal = { textColor = Color.yellow }
            };
            GUIStyle body = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 22,
                normal = { textColor = Color.white }
            };

            GUI.Label(new Rect(rect.x, rect.y + 34f, rect.width, 60f), "VICTORY", title);
            GUI.Label(new Rect(rect.x + 30f, rect.y + 104f, rect.width - 60f, 70f), "Boss defeated. All 5 testscenes cleared.", body);

            if (GUI.Button(new Rect(rect.x + 100f, rect.y + 190f, 170f, 42f), "Restart (R)"))
                Restart();
            if (GUI.Button(new Rect(rect.x + rect.width - 270f, rect.y + 190f, 170f, 42f), "Title (Esc)"))
                BackToTitle();
        }

        private void Restart()
        {
            Close();
            TestSceneRoomSetup.ResetRunState();
            SceneManager.LoadScene("TestRoom");
        }

        private void BackToTitle()
        {
            Close();
            TestSceneRoomSetup.ResetRunState();
            SceneManager.LoadScene("MainMenu");
        }

        private void Close()
        {
            UiModalState.Close();
            Time.timeScale = 1f;
            Destroy(gameObject);
        }
    }

    internal static class TestSceneRoomSetupSprite
    {
        private static Sprite sprite;
        public static Sprite Sprite
        {
            get
            {
                if (sprite != null) return sprite;
                var tex = new Texture2D(1, 1);
                tex.SetPixel(0, 0, Color.white);
                tex.Apply();
                sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
                return sprite;
            }
        }
    }
}
