using System.IO;
using HollowStyleMVP.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace HollowStyleMVP.EditorTools
{
    [InitializeOnLoad]
    public static class RoguelikeStage1SceneApplier
    {
        private const string FlagPath = "Assets/Editor/ApplyRoguelikeStage1.flag";
        private const string ScenePath = "Assets/Scenes/TestRoom.unity";
        private const string UiSliceRoot = "Assets/Art/RoguelikeUI/Sliced/stage1_run_ui_doors_toasts_minimap/stage1_run_ui_doors_toasts_minimap_";
        private const string DoorRoot = "Assets/Art/ImageGen/ArcadeDistrict/Doors/arcade_gate_";
        private const string PlayerSliceRoot = "Assets/Art/RoguelikeUI/Sliced/player_animation_sheet/player_animation_sheet_";
        private const string EnemySliceRoot = "Assets/Art/RoguelikeUI/Sliced/enemy_animation_sheet/enemy_animation_sheet_";
        private const string ChestSliceRoot = "Assets/Art/RoguelikeUI/Sliced/chest_loot_sheet/chest_loot_sheet_";

        static RoguelikeStage1SceneApplier()
        {
            EditorApplication.delayCall += TryApplyFromFlag;
        }

        [MenuItem("Hollow Style MVP/Apply Stage 1 Roguelike Art")]
        public static void ApplyMenu()
        {
            Apply();
        }

        private static void TryApplyFromFlag()
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), FlagPath);
            if (!File.Exists(fullPath)) return;
            if (EditorApplication.isCompiling)
            {
                EditorApplication.delayCall += TryApplyFromFlag;
                return;
            }
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.isPlaying = false;
                EditorApplication.delayCall += TryApplyFromFlag;
                return;
            }

            File.Delete(fullPath);
            Apply();
        }

        private static void Apply()
        {
            AssetDatabase.Refresh();
            ImportArt("Assets/Art/RoguelikeUI");

            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            RemoveIfExists("Stage1 Roguelike Art");
            HideIfExists("HUD Canvas");

            var root = new GameObject("Stage1 Roguelike Art");
            CreateWorldDoors(root.transform);
            CreatePreviewGameplayObjects(root.transform);
            ApplyPlayerSprite();
            CreateStage1Canvas(root.transform);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("Applied Stage 1 roguelike art to TestRoom.");
        }

        private static void ImportArt(string path)
        {
            foreach (string asset in AssetDatabase.FindAssets("t:Texture2D", new[] { path }))
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(asset);
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            }
        }

        private static void CreateWorldDoors(Transform root)
        {
            var doorRoot = new GameObject("Doors").transform;
            doorRoot.SetParent(root);
            CreateWorldSprite("North Locked Boss Door", Door("north"), new Vector3(0f, 3.62f, -1f), new Vector3(0.36f, 0.36f, 1f), doorRoot, 80);
            CreateWorldSprite("South Open Entrance Door", Door("south"), new Vector3(0f, -3.62f, -1f), new Vector3(0.36f, 0.36f, 1f), doorRoot, 80);
            CreateWorldSprite("West Locked Battle Door", Door("west"), new Vector3(-7.25f, 0f, -1f), new Vector3(0.38f, 0.38f, 1f), doorRoot, 80);
            CreateWorldSprite("East Open Shop Door", Door("east"), new Vector3(7.25f, 0f, -1f), new Vector3(0.38f, 0.38f, 1f), doorRoot, 80);
        }

        private static void CreatePreviewGameplayObjects(Transform root)
        {
            var objectRoot = new GameObject("Preview Gameplay Sprites").transform;
            objectRoot.SetParent(root);
            CreateWorldSprite("Preview Chest Closed", Chest(4), new Vector3(-1.35f, -1.05f, -1f), Vector3.one * 0.52f, objectRoot, 90);
            CreateWorldSprite("Preview Enemy Idle A", Enemy(1), new Vector3(1.35f, 0.65f, -1f), Vector3.one * 0.48f, objectRoot, 90);
            CreateWorldSprite("Preview Enemy Idle B", Enemy(2), new Vector3(2.45f, -1.1f, -1f), Vector3.one * 0.48f, objectRoot, 90);
        }

        private static void ApplyPlayerSprite()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;
            var renderer = player.GetComponentInChildren<SpriteRenderer>();
            if (renderer == null) renderer = player.AddComponent<SpriteRenderer>();
            renderer.sprite = Player(1);
            renderer.sortingOrder = 95;
            player.transform.localScale = Vector3.one * 0.72f;
        }

        private static void CreateStage1Canvas(Transform root)
        {
            var canvasObj = new GameObject("Stage1 HUD Canvas");
            canvasObj.transform.SetParent(root);
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200;
            canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObj.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();

            CreateDoorToast(canvasObj.transform, "Locked Door Toast", Ui(49), "房门已锁", "需要清房才能开启", new Vector2(-705f, -215f), new Color(1f, 0.18f, 0.72f));
            CreateDoorToast(canvasObj.transform, "Open Door Toast", Ui(57), "出口开启", "继续前进吧！", new Vector2(-705f, -325f), new Color(0.05f, 0.86f, 1f));
            CreatePrompt(canvasObj.transform);
            CreateMinimap(canvasObj.transform);
        }

        private static void CreateDoorToast(Transform parent, string name, Sprite panelSprite, string title, string body, Vector2 pos, Color titleColor)
        {
            var panel = CreateImage(parent, name, panelSprite, new Vector2(330f, 94f), pos, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f));
            CreateText(panel.transform, "Title", title, 30, new Vector2(22f, 18f), new Vector2(250f, 32f), titleColor, TextAnchor.MiddleLeft);
            CreateText(panel.transform, "Body", body, 18, new Vector2(22f, -18f), new Vector2(250f, 28f), Color.white, TextAnchor.MiddleLeft);
        }

        private static void CreatePrompt(Transform parent)
        {
            var panel = CreateImage(parent, "Stage1 Toast Prompt", Ui(58), new Vector2(465f, 88f), new Vector2(0f, -405f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            CreateText(panel.transform, "Message", "进入战斗房", 30, Vector2.zero, new Vector2(390f, 42f), new Color(0.7f, 1f, 1f), TextAnchor.MiddleCenter);
        }

        private static void CreateMinimap(Transform parent)
        {
            var panel = CreateImage(parent, "Stage1 Minimap", Ui(41), new Vector2(285f, 245f), new Vector2(-32f, -142f), new Vector2(1f, 1f), new Vector2(1f, 1f));
            AddMiniRoom(panel.transform, Ui(44), Ui(61), new Vector2(0f, -54f), 60f);
            AddMiniRoom(panel.transform, Ui(42), Ui(62), new Vector2(0f, 20f), 54f);
            AddMiniRoom(panel.transform, Ui(43), Ui(63), new Vector2(-74f, -54f), 54f);
            AddMiniRoom(panel.transform, Ui(43), Ui(64), new Vector2(74f, -54f), 54f);
            AddMiniRoom(panel.transform, Ui(43), Ui(65), new Vector2(0f, 94f), 54f);
            CreateImage(panel.transform, "Map Line North", Ui(25), new Vector2(16f, 40f), new Vector2(0f, 57f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            CreateImage(panel.transform, "Map Line South", Ui(25), new Vector2(16f, 40f), new Vector2(0f, -18f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            CreateImage(panel.transform, "Map Line West", Ui(21), new Vector2(40f, 16f), new Vector2(-37f, -54f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            CreateImage(panel.transform, "Map Line East", Ui(21), new Vector2(40f, 16f), new Vector2(37f, -54f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        }

        private static void AddMiniRoom(Transform parent, Sprite room, Sprite icon, Vector2 pos, float size)
        {
            var tile = CreateImage(parent, "Room", room, new Vector2(size, size), pos, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            CreateImage(tile.transform, "Icon", icon, new Vector2(size * 0.55f, size * 0.55f), Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        }

        private static Sprite Ui(int index) => LoadSprite(UiSliceRoot, index);
        private static Sprite Door(string direction) => AssetDatabase.LoadAssetAtPath<Sprite>($"{DoorRoot}{direction}.png");
        private static Sprite Player(int index) => LoadSprite(PlayerSliceRoot, index);
        private static Sprite Enemy(int index) => LoadSprite(EnemySliceRoot, index);
        private static Sprite Chest(int index) => LoadSprite(ChestSliceRoot, index);

        private static Sprite LoadSprite(string root, int index)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>($"{root}{index:000}.png");
        }

        private static GameObject CreateWorldSprite(string name, Sprite sprite, Vector3 position, Vector3 scale, Transform parent, int sortingOrder)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent);
            obj.transform.position = position;
            obj.transform.localScale = scale;
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = sortingOrder;
            return obj;
        }

        private static GameObject CreateImage(Transform parent, string name, Sprite sprite, Vector2 size, Vector2 anchoredPosition, Vector2 anchorMin, Vector2 anchorMax)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
            var image = obj.AddComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
            return obj;
        }

        private static Text CreateText(Transform parent, string name, string text, int size, Vector2 anchoredPosition, Vector2 boxSize, Color color, TextAnchor alignment)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = boxSize;
            var label = obj.AddComponent<Text>();
            label.text = text.Length > 15 ? text.Substring(0, 15) : text;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = size;
            label.color = color;
            label.alignment = alignment;
            label.raycastTarget = false;
            return label;
        }

        private static void HideIfExists(string name)
        {
            var obj = GameObject.Find(name);
            if (obj != null) obj.SetActive(false);
        }

        private static void RemoveIfExists(string name)
        {
            var obj = GameObject.Find(name);
            if (obj != null) Object.DestroyImmediate(obj);
        }
    }
}
