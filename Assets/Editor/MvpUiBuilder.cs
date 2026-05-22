using HollowStyleMVP.Core;
using HollowStyleMVP.Dialogue;
using HollowStyleMVP.Shop;
using HollowStyleMVP.UI;

using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HollowStyleMVP.EditorTools
{
    public static class MvpUiBuilder
    {
        [MenuItem("Hollow Style MVP/Build UI Prefabs")]
        [MenuItem("Tools/Hollow Style MVP/Build UI Prefabs")]
        [MenuItem("Assets/Hollow Style MVP/Build UI Prefabs")]
        [MenuItem("GameObject/Hollow Style MVP/Build UI Prefabs", false, 49)]
        public static void BuildUiPrefabs()
        {
            EnsureFolders();
            MvpArtBuilder.PrepareArtAssets();
            SavePrefab(CreateInventoryUi(), "Assets/Prefabs/UI/InventoryUI.prefab");
            SavePrefab(CreateDialogueUi(), "Assets/Prefabs/UI/DialogueUI.prefab");
            SavePrefab(CreateShopUi(), "Assets/Prefabs/UI/ShopUI.prefab");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Hollow Style MVP/Build MainMenu Scene")]
        [MenuItem("Tools/Hollow Style MVP/Build MainMenu Scene")]
        [MenuItem("Assets/Hollow Style MVP/Build MainMenu Scene")]
        [MenuItem("GameObject/Hollow Style MVP/Build MainMenu Scene", false, 49)]
        public static void BuildMainMenuScene()
        {
            EnsureFolders();
            MvpArtBuilder.PrepareArtAssets();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "MainMenu";

            new GameObject("GameManager").AddComponent<GameManager>();
            CreateEventSystem();
            var camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            camObj.AddComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            camObj.transform.position = new Vector3(0, 0, -10);

            var canvasObj = CreateCanvas("MainMenu Canvas");
            var root = CreatePanel(canvasObj.transform, "Menu Panel", new Color(0.06f, 0.07f, 0.09f, 1f), Vector2.zero, new Vector2(520, 520));
            CreateText(root.transform, "Title", "Hollow Style MVP", 42, new Vector2(0, 175), new Vector2(460, 70));
            CreateText(root.transform, "Hint", "测试路径：点击开始游戏进入 TestRoom", 20, new Vector2(0, 125), new Vector2(460, 40));

            var controller = canvasObj.AddComponent<MainMenuController>();
            var newButton = CreateButton(root.transform, "New Game Button", "开始游戏", new Vector2(0, 55));
            var continueButton = CreateButton(root.transform, "Continue Button", "继续", new Vector2(0, -10));
            var quitButton = CreateButton(root.transform, "Quit Button", "退出", new Vector2(0, -75));
            CreateText(root.transform, "Keys", "进入场景后：WASD/方向键移动 Shift冲刺 J攻击 E交互 I背包 Esc关闭", 17, new Vector2(0, -155), new Vector2(470, 70));

            UnityEventTools.AddPersistentListener(newButton.onClick, controller.NewGame);
            UnityEventTools.AddPersistentListener(continueButton.onClick, controller.ContinueGame);
            UnityEventTools.AddPersistentListener(quitButton.onClick, controller.Quit);

            var so = new SerializedObject(controller);
            so.FindProperty("continueButton").objectReferenceValue = continueButton;
            so.ApplyModifiedPropertiesWithoutUndo();

            bool saved = EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity", true);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            if (!saved)
            {
                EditorUtility.DisplayDialog("MainMenu 保存失败", "Unity 没有成功保存 Assets/Scenes/MainMenu.unity。请确认当前不在 Play Mode，然后重新执行生成菜单。", "OK");
                return;
            }
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        }


        private static void CreateEventSystem()
        {
            var obj = new GameObject("EventSystem");
            obj.AddComponent<EventSystem>();
            obj.AddComponent<StandaloneInputModule>();
        }
        private static GameObject CreateInventoryUi()
        {
            var canvasObj = CreateCanvas("InventoryUI");
            var panel = CreatePanel(canvasObj.transform, "Inventory Panel", new Color(0.08f, 0.09f, 0.1f, 0.96f), Vector2.zero, new Vector2(620, 460));
            CreateText(panel.transform, "Title", "背包", 34, new Vector2(0, 180), new Vector2(520, 60));
            var coins = CreateText(panel.transform, "Coins", "Geo 0", 22, new Vector2(-210, 130), new Vector2(180, 40));
            var content = CreatePanel(panel.transform, "Content", new Color(0.13f, 0.14f, 0.16f, 1f), new Vector2(0, -35), new Vector2(520, 260));
            var layout = content.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(18, 18, 16, 16);
            layout.spacing = 8;
            layout.childControlHeight = false;
            layout.childForceExpandHeight = false;
            var row = CreateText(content.transform, "Row Template", "物品 x1", 20, Vector2.zero, new Vector2(460, 32));
            row.gameObject.SetActive(false);

            var controller = canvasObj.AddComponent<InventoryUIController>();
            var so = new SerializedObject(controller);
            so.FindProperty("panel").objectReferenceValue = panel;
            so.FindProperty("contentRoot").objectReferenceValue = content.transform;
            so.FindProperty("rowPrefab").objectReferenceValue = row;
            so.FindProperty("coinsText").objectReferenceValue = coins;
            so.ApplyModifiedPropertiesWithoutUndo();
            panel.SetActive(false);
            return canvasObj;
        }

        private static GameObject CreateDialogueUi()
        {
            var canvasObj = CreateCanvas("DialogueUI");
            var panel = CreatePanel(canvasObj.transform, "Dialogue Panel", new Color(0.01f, 0.018f, 0.024f, 0.88f), new Vector2(0, -410), new Vector2(900, 120));
            var speaker = CreateText(panel.transform, "Speaker", "NPC", 20, new Vector2(-360, 34), new Vector2(220, 30));
            var body = CreateText(panel.transform, "Body", "Dialogue text", 20, new Vector2(0, -20), new Vector2(790, 60));
            body.alignment = TextAnchor.UpperLeft;

            var controller = canvasObj.AddComponent<DialogueController>();
            var so = new SerializedObject(controller);
            so.FindProperty("panel").objectReferenceValue = panel;
            so.FindProperty("speakerText").objectReferenceValue = speaker;
            so.FindProperty("bodyText").objectReferenceValue = body;
            so.ApplyModifiedPropertiesWithoutUndo();
            panel.SetActive(false);
            return canvasObj;
        }

        private static GameObject CreateShopUi()
        {
            var canvasObj = CreateCanvas("ShopUI");
            var panel = CreatePanel(canvasObj.transform, "Shop Panel", new Color(0.09f, 0.075f, 0.05f, 0.97f), Vector2.zero, new Vector2(650, 480));
            var title = CreateText(panel.transform, "Title", "商店", 34, new Vector2(0, 180), new Vector2(520, 60));
            var list = CreatePanel(panel.transform, "List", new Color(0.15f, 0.13f, 0.09f, 1f), new Vector2(0, -30), new Vector2(520, 300));
            var layout = list.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 16, 16);
            layout.spacing = 10;
            layout.childControlHeight = false;
            layout.childForceExpandHeight = false;
            var row = CreateButton(list.transform, "Shop Row Template", "商品 - 5G x1", Vector2.zero);
            row.gameObject.SetActive(false);

            var controller = canvasObj.AddComponent<ShopController>();
            var so = new SerializedObject(controller);
            so.FindProperty("panel").objectReferenceValue = panel;
            so.FindProperty("titleText").objectReferenceValue = title;
            so.FindProperty("listRoot").objectReferenceValue = list.transform;
            so.FindProperty("rowPrefab").objectReferenceValue = row;
            so.ApplyModifiedPropertiesWithoutUndo();
            panel.SetActive(false);
            return canvasObj;
        }

        private static GameObject CreateCanvas(string name)
        {
            var canvasObj = new GameObject(name);
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();
            return canvasObj;
        }

        private static GameObject CreatePanel(Transform parent, string name, Color color, Vector2 anchored, Vector2 size)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchored;
            rect.sizeDelta = size;
            var image = obj.AddComponent<Image>();
            image.color = color;
            var panelSprite = MvpArtBuilder.Sprite(MvpArtBuilder.K("ui_box.png"));
            bool usePlainRuntimePanel = name == "Inventory Panel" || name == "Dialogue Panel" || name == "Shop Panel";
            if (!usePlainRuntimePanel && panelSprite != null) image.sprite = panelSprite;
            return obj;
        }

        private static Text CreateText(Transform parent, string name, string text, int size, Vector2 anchored, Vector2 boxSize)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchored;
            rect.sizeDelta = boxSize;
            var label = obj.AddComponent<Text>();
            label.text = text;
            label.fontSize = size;
            label.font = MvpArtBuilder.UiFont();
            label.color = Color.white;
            label.alignment = TextAnchor.MiddleCenter;
            return label;
        }

        private static Button CreateButton(Transform parent, string name, string text, Vector2 anchored)
        {
            var obj = CreatePanel(parent, name, new Color(0.18f, 0.21f, 0.24f, 1f), anchored, new Vector2(300, 48));
            var image = obj.GetComponent<Image>();
            var buttonSprite = MvpArtBuilder.Sprite(MvpArtBuilder.K("ui_button.png"));
            if (image != null && buttonSprite != null) image.sprite = buttonSprite;
            var button = obj.AddComponent<Button>();
            var label = CreateText(obj.transform, "Label", text, 22, Vector2.zero, new Vector2(280, 40));
            button.targetGraphic = obj.GetComponent<Image>();
            return button;
        }

        private static void SavePrefab(GameObject obj, string path)
        {
            PrefabUtility.SaveAsPrefabAsset(obj, path);
            Object.DestroyImmediate(obj);
        }

        private static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs")) AssetDatabase.CreateFolder("Assets", "Prefabs");
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs/UI")) AssetDatabase.CreateFolder("Assets/Prefabs", "UI");
            if (!AssetDatabase.IsValidFolder("Assets/Scenes")) AssetDatabase.CreateFolder("Assets", "Scenes");
        }
    }
}
