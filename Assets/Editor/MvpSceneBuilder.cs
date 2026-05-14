using HollowStyleMVP.Boss;
using HollowStyleMVP.Combat;
using HollowStyleMVP.Core;
using HollowStyleMVP.Dialogue;
using HollowStyleMVP.Enemies;
using HollowStyleMVP.Interaction;
using HollowStyleMVP.Inventory;
using HollowStyleMVP.Items;
using HollowStyleMVP.Level;
using HollowStyleMVP.Player;
using HollowStyleMVP.Save;
using HollowStyleMVP.Shop;
using HollowStyleMVP.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HollowStyleMVP.EditorTools
{
    public static class MvpSceneBuilder
    {
        [MenuItem("Hollow Style MVP/Build TestRoom Scene")]
        [MenuItem("Tools/Hollow Style MVP/Build TestRoom Scene")]
        [MenuItem("Assets/Hollow Style MVP/Build TestRoom Scene")]
        [MenuItem("GameObject/Hollow Style MVP/Build TestRoom Scene", false, 49)]
        public static void BuildTestRoom()
        {
            EnsureFolders();
            DefaultConfigBuilder.CreateDefaultConfigAssets();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "TestRoom";
            CreateManagers();
            CreateEventSystem();
            CreateCamera();
            var player = CreatePlayerAt(new Vector2(-8, 0), "玩家");
            Box("Ground", new Vector2(0, -2), new Vector2(28, 1), Color.gray);
            Label("地面", new Vector2(0, -0.95f), 0.075f);
            Box("Left Wall", new Vector2(-14, 2), new Vector2(1, 8), Color.gray);
            Box("Right Wall", new Vector2(14, 2), new Vector2(1, 8), Color.gray);
            CreateHazard("Spikes", new Vector2(-5, -1.25f), new Vector2(2, 0.45f), "尖刺");
            CreateFragilePlatform(new Vector2(2, 0.5f));
            CreateEnemy(new Vector2(5, -1.25f));
            CreateBoss(new Vector2(10, -1.1f));
            CreateNpc(new Vector2(-2, -1.1f));
            CreateShop(new Vector2(-7, -1.1f));
            CreateSavePoint(new Vector2(-9.5f, -1.1f));
            CreatePickup(new Vector2(-0.8f, 0.2f), 3, "金币");
            CreatePickup(new Vector2(0.2f, 0.2f), 3, "金币");
            CreatePickup(new Vector2(1.0f, 0.2f), 3, "金币");
            CreatePortal(new Vector2(9.3f, -0.85f), "去TestRoom2", "TestRoom2");
            CreateHud();
            CreateTestLegend();
            InstantiateGameplayUi();

            Selection.activeGameObject = player;
            bool saved = EditorSceneManager.SaveScene(scene, "Assets/Scenes/TestRoom.unity", true);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            if (!saved)
            {
                EditorUtility.DisplayDialog("TestRoom 保存失败", "Unity 没有成功保存 Assets/Scenes/TestRoom.unity。请确认当前不在 Play Mode，然后重新执行生成菜单。", "OK");
            }
        }


        [MenuItem("Hollow Style MVP/Build TestRoom2 Scene")]
        [MenuItem("Tools/Hollow Style MVP/Build TestRoom2 Scene")]
        [MenuItem("Assets/Hollow Style MVP/Build TestRoom2 Scene")]
        [MenuItem("GameObject/Hollow Style MVP/Build TestRoom2 Scene", false, 49)]
        public static void BuildTestRoom2()
        {
            EnsureFolders();
            DefaultConfigBuilder.CreateDefaultConfigAssets();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "TestRoom2";
            CreateManagers();
            CreateEventSystem();
            CreateCamera();
            var player = CreatePlayerAt(new Vector2(-9, 0), "玩家出生点");
            Box("Room2 Ground", new Vector2(0, -2), new Vector2(28, 1), new Color(0.45f, 0.45f, 0.5f));
            Label("TestRoom2：Boss/宝箱/能力门/刷怪点/回程传送门", new Vector2(0, 4.6f), 0.08f);
            CreateBoss(new Vector2(6, -1.1f));
            CreateChest(new Vector2(-5.5f, -1.2f));
            CreateAbilityGate(new Vector2(1.5f, -1.05f));
            CreateSpawner(new Vector2(10.5f, -1.2f));
            CreatePortal(new Vector2(-9.8f, -0.85f), "回TestRoom", "TestRoom");
            CreateSavePoint(new Vector2(-8, -1.1f));
            CreateHud();
            CreateTestLegend();
            InstantiateGameplayUi();
            bool saved = EditorSceneManager.SaveScene(scene, "Assets/Scenes/TestRoom2.unity", true);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            if (!saved) EditorUtility.DisplayDialog("TestRoom2 保存失败", "Unity 没有成功保存 Assets/Scenes/TestRoom2.unity。", "OK");
            Selection.activeGameObject = player;
        }
        private static void CreateManagers()
        {
            new GameObject("GameManager").AddComponent<GameManager>();
            new GameObject("InventorySystem").AddComponent<InventorySystem>();
            new GameObject("SaveGameApplier").AddComponent<SaveGameApplier>();
            new GameObject("FeedbackManager").AddComponent<FeedbackManager>();
        }

        private static void CreateEventSystem()
        {
            var obj = new GameObject("EventSystem");
            obj.AddComponent<EventSystem>();
            obj.AddComponent<StandaloneInputModule>();
        }

        private static void CreateCamera()
        {
            var camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            var cam = camObj.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 6f;
            cam.backgroundColor = new Color(0.38f, 0.47f, 0.52f);
            camObj.transform.position = new Vector3(0, 1, -10);
            camObj.AddComponent<CameraShake>();
        }

        private static GameObject CreatePlayerAt(Vector2 position, string label)
        {
            var player = Box("Player", position, new Vector2(0.8f, 1.5f), new Color(0.2f, 0.65f, 1f));
            player.tag = "Player";
            var rb = player.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            player.AddComponent<Health>();
            player.AddComponent<Interactor>();
            player.AddComponent<PlayerAbilityController>();
            player.AddComponent<PlayerStatsController>();
            player.AddComponent<ComboCounter>();
            player.AddComponent<PlayerRespawnController>();
            var controller = player.AddComponent<PlayerController2D>();
            var ground = new GameObject("GroundCheck");
            ground.transform.SetParent(player.transform);
            ground.transform.localPosition = new Vector3(0, -0.82f, 0);
            var attack = Box("AttackHitbox", Vector2.zero, new Vector2(1.1f, 0.8f), Color.yellow);
            attack.transform.SetParent(player.transform);
            attack.transform.localPosition = new Vector3(0.8f, 0, 0);
            attack.GetComponent<SpriteRenderer>().enabled = false;
            attack.GetComponent<BoxCollider2D>().isTrigger = true;
            attack.AddComponent<DamageDealer>();
            attack.SetActive(false);
            var so = new SerializedObject(controller);
            so.FindProperty("config").objectReferenceValue = AssetDatabase.LoadAssetAtPath<PlayerConfig>(DefaultConfigBuilder.PlayerConfigPath);
            so.FindProperty("groundCheck").objectReferenceValue = ground.transform;
            so.FindProperty("attackHitbox").objectReferenceValue = attack.GetComponent<DamageDealer>();
            so.FindProperty("groundMask").intValue = -1;
            so.ApplyModifiedPropertiesWithoutUndo();
            Label(label, position + Vector2.up * 1.45f);
            return player;
        }

        private static void CreateEnemy(Vector2 pos)
        {
            var enemy = Box("Enemy", pos, new Vector2(1, 1), Color.red);
            enemy.AddComponent<Rigidbody2D>().freezeRotation = true;
            enemy.AddComponent<Health>();
            var enemyAi = enemy.AddComponent<EnemyAI>();
            var enemyContact = enemy.AddComponent<ContactDamage>();
            var enemyConfig = AssetDatabase.LoadAssetAtPath<EnemyConfig>(DefaultConfigBuilder.EnemyConfigPath);
            enemyAi.ApplyConfig(enemyConfig);
            CreateWorldHealthBar(enemy.transform, new Vector3(0, 0.9f, 0), 1.2f);
            Label("怪物", pos + Vector2.up * 1.05f);
        }

        private static void CreateBoss(Vector2 pos)
        {
            var boss = Box("Boss", pos, new Vector2(1.8f, 2f), new Color(0.55f, 0.1f, 0.8f));
            boss.AddComponent<Rigidbody2D>().freezeRotation = true;
            boss.AddComponent<Health>();
            var bossController = boss.AddComponent<BossController>();
            boss.AddComponent<ContactDamage>();
            var bossConfig = AssetDatabase.LoadAssetAtPath<BossConfig>(DefaultConfigBuilder.BossConfigPath);
            bossController.ApplyConfig(bossConfig);
            CreateWorldHealthBar(boss.transform, new Vector3(0, 1.35f, 0), 1.8f);
            Label("Boss", pos + Vector2.up * 1.55f);
        }

        private static void CreateNpc(Vector2 pos)
        {
            var npc = Box("Dialogue NPC", pos, new Vector2(0.8f, 1.4f), Color.green);
            npc.GetComponent<BoxCollider2D>().isTrigger = true;
            npc.AddComponent<DialogueNpc>();
            Label("对话NPC", pos + Vector2.up * 1.25f);
        }

        private static void CreateShop(Vector2 pos)
        {
            var shop = Box("Shop NPC", pos, new Vector2(0.9f, 1.4f), new Color(1f, 0.7f, 0.1f));
            shop.GetComponent<BoxCollider2D>().isTrigger = true;
            var shopNpc = shop.AddComponent<ShopNpc>();
            DefaultConfigBuilder.CreateDefaultConfigAssets();
            var item = AssetDatabase.LoadAssetAtPath<InventoryItem>(DefaultConfigBuilder.PotionPath);
            var serialized = new SerializedObject(shopNpc);
            serialized.FindProperty("shopName").stringValue = "测试商店";
            var items = serialized.FindProperty("items");
            items.arraySize = 1;
            items.GetArrayElementAtIndex(0).FindPropertyRelative("item").objectReferenceValue = item;
            items.GetArrayElementAtIndex(0).FindPropertyRelative("price").intValue = 3;
            items.GetArrayElementAtIndex(0).FindPropertyRelative("stock").intValue = 3;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            Label("商店", pos + Vector2.up * 1.25f);
        }


        private static void CreatePortal(Vector2 pos, string label, string sceneName)
        {
            var portal = Box(label, pos, new Vector2(1.6f, 2.4f), new Color(0.0f, 1f, 1f));
            portal.GetComponent<BoxCollider2D>().isTrigger = true;
            var teleporter = portal.AddComponent<Teleporter>();
            var so = new SerializedObject(teleporter);
            so.FindProperty("sceneName").stringValue = sceneName;
            so.ApplyModifiedPropertiesWithoutUndo();
            Label("传送门 " + label + "：靠近按E", pos + Vector2.up * 1.65f, 0.07f);
        }

        private static void CreateChest(Vector2 pos)
        {
            var chest = Box("Chest", pos, new Vector2(0.9f, 0.7f), new Color(0.65f, 0.38f, 0.12f));
            chest.GetComponent<BoxCollider2D>().isTrigger = true;
            chest.AddComponent<Chest>();
            Label("宝箱：按E", pos + Vector2.up * 0.8f, 0.06f);
        }

        private static void CreateAbilityGate(Vector2 pos)
        {
            var gate = Box("Dash Ability Gate", pos, new Vector2(0.45f, 2.2f), new Color(0.15f, 0.3f, 1f));
            gate.GetComponent<BoxCollider2D>().isTrigger = true;
            gate.AddComponent<AbilityGate>();
            Label("能力门：需冲刺", pos + Vector2.up * 1.3f, 0.055f);
        }

        private static void CreateSpawner(Vector2 pos)
        {
            var spawner = Box("Enemy Spawner", pos, new Vector2(0.6f, 0.6f), new Color(0.9f, 0.2f, 0.2f));
            var prefab = CreateEnemyPrefabForSpawner();
            var component = spawner.AddComponent<EnemySpawner>();
            var so = new SerializedObject(component);
            so.FindProperty("enemyPrefab").objectReferenceValue = prefab;
            so.FindProperty("respawnSeconds").floatValue = 5f;
            so.ApplyModifiedPropertiesWithoutUndo();
            Label("刷怪点", pos + Vector2.up * 0.75f, 0.06f);
        }

        private static GameObject CreateEnemyPrefabForSpawner()
        {
            const string path = "Assets/Prefabs/TestSpawnEnemy.prefab";
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null) return existing;
            var enemy = Box("SpawnEnemy", Vector2.zero, new Vector2(1, 1), Color.red);
            enemy.AddComponent<Rigidbody2D>().freezeRotation = true;
            enemy.AddComponent<Health>();
            enemy.AddComponent<EnemyAI>();
            enemy.AddComponent<ContactDamage>();
            var prefab = PrefabUtility.SaveAsPrefabAsset(enemy, path);
            Object.DestroyImmediate(enemy);
            return prefab;
        }

        private static void CreateWorldHealthBar(Transform parent, Vector3 localPosition, float width)
        {
            var canvasObj = new GameObject("World Health Bar");
            canvasObj.transform.SetParent(parent, false);
            canvasObj.transform.localPosition = localPosition;
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            var rect = canvasObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(width, 0.16f);
            canvasObj.AddComponent<GraphicRaycaster>();
            var bg = new GameObject("BG");
            bg.transform.SetParent(canvasObj.transform, false);
            var bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            bg.AddComponent<Image>().color = Color.black;
            var fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(canvasObj.transform, false);
            var fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = new Vector2(0.02f, 0.02f);
            fillRect.offsetMax = new Vector2(-0.02f, -0.02f);
            fillObj.AddComponent<Image>().color = Color.red;
            var slider = canvasObj.AddComponent<Slider>();
            slider.transition = Selectable.Transition.None;
            slider.targetGraphic = null;
            slider.fillRect = fillRect;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0;
            slider.maxValue = 1;
            slider.value = 1;
            var bar = canvasObj.AddComponent<WorldHealthBar>();
            var so = new SerializedObject(bar);
            so.FindProperty("slider").objectReferenceValue = slider;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
        private static void CreateSavePoint(Vector2 pos)
        {
            var save = Box("Save Point", pos, new Vector2(0.7f, 1.2f), Color.cyan);
            save.GetComponent<BoxCollider2D>().isTrigger = true;
            save.AddComponent<SavePoint>();
            Label("存档", pos + Vector2.up * 1.15f);
        }

        private static void CreatePickup(Vector2 pos, int coins, string label)
        {
            var pickup = Box("Coin Pickup", pos, new Vector2(0.35f, 0.35f), Color.yellow);
            pickup.GetComponent<BoxCollider2D>().isTrigger = true;
            var pickupItem = pickup.AddComponent<PickupItem>();
            var serialized = new SerializedObject(pickupItem);
            serialized.FindProperty("coins").intValue = coins;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            Label(label, pos + Vector2.up * 0.55f, 0.055f);
        }

        private static void CreateHazard(string name, Vector2 pos, Vector2 size, string label)
        {
            var hazard = Box(name, pos, size, Color.magenta);
            hazard.GetComponent<BoxCollider2D>().isTrigger = true;
            hazard.AddComponent<HazardDamage>();
            Label(label, pos + Vector2.up * 0.65f);
        }

        private static void CreateFragilePlatform(Vector2 pos)
        {
            var platform = Box("Fragile Platform", pos, new Vector2(3, 0.35f), new Color(0.8f, 0.45f, 0.2f));
            platform.AddComponent<FragilePlatform>();
            Label("脆弱平台", pos + Vector2.up * 0.6f);
        }

        private static void CreateHud()
        {
            var canvasObj = new GameObject("HUD Canvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();
            var hud = canvasObj.AddComponent<HudController>();

            var healthText = CreateText(canvasObj.transform, "Health Text", new Vector2(110, -30), "生命 5/5");
            var healthSlider = CreateScreenSlider(canvasObj.transform, "Health Slider", new Vector2(150, -68), new Color(0.85f, 0.1f, 0.1f));
            var energyText = CreateText(canvasObj.transform, "Energy Text", new Vector2(110, -105), "能量 100/100");
            var energySlider = CreateScreenSlider(canvasObj.transform, "Energy Slider", new Vector2(150, -143), new Color(0.1f, 0.45f, 1f));
            var coins = CreateText(canvasObj.transform, "Coins", new Vector2(110, -180), "金币 0");
            var combo = CreateText(canvasObj.transform, "Combo", new Vector2(110, -215), "连击 -");
            var ability = CreateText(canvasObj.transform, "Abilities", new Vector2(360, -30), "能力：二段跳[开] 冲刺[开]");
            var prompt = CreateText(canvasObj.transform, "Prompt", new Vector2(0, 60), "");
            prompt.alignment = TextAnchor.MiddleCenter;

            var so = new SerializedObject(hud);
            so.FindProperty("healthText").objectReferenceValue = healthText;
            so.FindProperty("healthSlider").objectReferenceValue = healthSlider;
            so.FindProperty("energyText").objectReferenceValue = energyText;
            so.FindProperty("energySlider").objectReferenceValue = energySlider;
            so.FindProperty("coinText").objectReferenceValue = coins;
            so.FindProperty("comboText").objectReferenceValue = combo;
            so.FindProperty("abilityText").objectReferenceValue = ability;
            so.FindProperty("promptText").objectReferenceValue = prompt;
            so.ApplyModifiedPropertiesWithoutUndo();
            CreateDeathUI(canvasObj.transform);
        }

        private static void CreateDeathUI(Transform parent)
        {
            var panel = new GameObject("Death Panel");
            panel.transform.SetParent(parent, false);
            var rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = panel.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.72f);
            var textObj = new GameObject("Death Text");
            textObj.transform.SetParent(panel.transform, false);
            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);
            textRect.sizeDelta = new Vector2(600, 180);
            var label = textObj.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 48;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            label.text = "死亡";
            var controller = parent.gameObject.AddComponent<DeathUIController>();
            var so = new SerializedObject(controller);
            so.FindProperty("panel").objectReferenceValue = panel;
            so.FindProperty("messageText").objectReferenceValue = label;
            so.ApplyModifiedPropertiesWithoutUndo();
            panel.SetActive(false);
        }


        private static void CreateTestLegend()
        {
            var canvasObj = new GameObject("Test Legend Canvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();

            var panel = new GameObject("Test Legend Panel");
            panel.transform.SetParent(canvasObj.transform, false);
            var rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-24f, -24f);
            rect.sizeDelta = new Vector2(520f, 270f);
            var image = panel.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.62f);

            var textObj = new GameObject("Legend Text");
            textObj.transform.SetParent(panel.transform, false);
            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(18f, 14f);
            textRect.offsetMax = new Vector2(-18f, -14f);
            var label = textObj.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 22;
            label.color = Color.white;
            label.alignment = TextAnchor.UpperLeft;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.text = "测试说明\n" +
                "玩家：A/D移动 W/空格跳跃 Shift冲刺 J攻击\n" +
                "交互：靠近 NPC/商店/存档点按 E\n" +
                "背包：I 打开，Esc 关闭\n" +
                "金币：靠近自动拾取\n" +
                "尖刺/怪物/Boss：测试受伤与战斗\n" +
                "详细用例见 TESTING_GUIDE.md";
        }
        private static void InstantiateGameplayUi()
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/InventoryUI.prefab") == null)
                MvpUiBuilder.BuildUiPrefabs();
            InstantiatePrefab("Assets/Prefabs/UI/InventoryUI.prefab");
            InstantiatePrefab("Assets/Prefabs/UI/DialogueUI.prefab");
            InstantiatePrefab("Assets/Prefabs/UI/ShopUI.prefab");
        }

        private static void InstantiatePrefab(string path)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) return;
            PrefabUtility.InstantiatePrefab(prefab);
        }


        private static Slider CreateScreenSlider(Transform parent, string name, Vector2 anchored, Color fillColor)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);
            var rect = root.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.anchoredPosition = anchored;
            rect.sizeDelta = new Vector2(260, 18);
            var bg = new GameObject("BG");
            bg.transform.SetParent(root.transform, false);
            var bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            bg.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.6f);
            var fill = new GameObject("Fill");
            fill.transform.SetParent(root.transform, false);
            var fillRect = fill.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = new Vector2(2, 2);
            fillRect.offsetMax = new Vector2(-2, -2);
            fill.AddComponent<Image>().color = fillColor;
            var slider = root.AddComponent<Slider>();
            slider.transition = Selectable.Transition.None;
            slider.targetGraphic = null;
            slider.fillRect = fillRect;
            slider.minValue = 0;
            slider.maxValue = 1;
            slider.value = 1;
            return slider;
        }
        private static Text CreateText(Transform parent, string name, Vector2 anchored, string text)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.anchoredPosition = anchored;
            rect.sizeDelta = new Vector2(360, 40);
            var label = obj.AddComponent<Text>();
            label.text = text;
            label.fontSize = 24;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.color = Color.white;
            return label;
        }

        private static GameObject Box(string name, Vector2 position, Vector2 size, Color color)
        {
            var obj = new GameObject(name);
            obj.transform.position = position;
            var sprite = obj.AddComponent<SpriteRenderer>();
            sprite.sprite = MakeSprite();
            sprite.color = color;
            obj.transform.localScale = new Vector3(size.x, size.y, 1);
            obj.AddComponent<BoxCollider2D>();
            return obj;
        }

        private static void Label(string text, Vector2 position, float size = 0.075f)
        {
            var obj = new GameObject("Label - " + text);
            obj.transform.position = position;
            var label = obj.AddComponent<TextMesh>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.text = text;
            label.fontSize = 32;
            label.characterSize = size;
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.color = Color.white;
            var renderer = obj.GetComponent<MeshRenderer>();
            renderer.sortingOrder = 50;
            renderer.sharedMaterial = label.font.material;
        }

        private static Sprite MakeSprite()
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        }

        private static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Scenes")) AssetDatabase.CreateFolder("Assets", "Scenes");
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs")) AssetDatabase.CreateFolder("Assets", "Prefabs");
            if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects")) AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
            if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects/Items")) AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Items");
        }
    }
}













