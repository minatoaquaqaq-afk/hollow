using System.Text.RegularExpressions;
using HollowStyleMVP.Combat;
using HollowStyleMVP.Core;
using HollowStyleMVP.Inventory;
using HollowStyleMVP.Level;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HollowStyleMVP.UI
{
    public class RoguelikeRoomHud : MonoBehaviour
    {
        private const int LastRoom = 5;
        private const float BossWarningDuration = 2.4f;

        private static readonly Color Backdrop = new Color(0.005f, 0.014f, 0.032f, 0.84f);
        private static readonly Color Cyan = new Color(0.12f, 0.92f, 1f, 1f);
        private static readonly Color CyanDim = new Color(0.05f, 0.45f, 0.58f, 0.72f);
        private static readonly Color Red = new Color(1f, 0.08f, 0.16f, 1f);
        private static readonly Color Purple = new Color(0.76f, 0.18f, 1f, 1f);
        private static readonly Color Gold = new Color(1f, 0.78f, 0.18f, 1f);

        private Health playerHealth;
        private CombatStats playerStats;
        private int coins;
        private int roomNumber;
        private float bossWarningUntil;
        private GUIStyle smallStyle;
        private GUIStyle valueStyle;
        private GUIStyle titleStyle;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            EnsureExists();
        }

        public static void EnsureExists()
        {
            if (FindAnyObjectByType<RoguelikeRoomHud>() != null) return;

            var obj = new GameObject(nameof(RoguelikeRoomHud));
            DontDestroyOnLoad(obj);
            obj.AddComponent<RoguelikeRoomHud>();
        }

        public static void StyleShopPanel(GameObject panel)
        {
            if (panel == null) return;

            var rect = panel.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = new Vector2(720f, 500f);
            }

            var image = panel.GetComponent<Image>() ?? panel.AddComponent<Image>();
            image.color = new Color(0.004f, 0.015f, 0.033f, 0.90f);
            AddOrUpdateOutline(panel, Cyan, new Vector2(2f, -2f));
        }

        public static void StyleShopRow(Button row)
        {
            if (row == null) return;

            var image = row.GetComponent<Image>() ?? row.gameObject.AddComponent<Image>();
            image.color = new Color(0.02f, 0.08f, 0.12f, 0.72f);
            AddOrUpdateOutline(row.gameObject, CyanDim, new Vector2(1f, -1f));

            var label = row.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.color = Cyan;
                label.fontSize = Mathf.Max(label.fontSize, 18);
                label.alignment = TextAnchor.MiddleLeft;
            }
        }

        public static void StyleDialoguePanel(GameObject panel)
        {
            if (panel == null) return;

            var rect = panel.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.5f, 0f);
                rect.anchorMax = new Vector2(0.5f, 0f);
                rect.pivot = new Vector2(0.5f, 0f);
                rect.anchoredPosition = new Vector2(0f, 28f);
                rect.sizeDelta = new Vector2(920f, 150f);
            }

            var image = panel.GetComponent<Image>() ?? panel.AddComponent<Image>();
            image.color = new Color(0.004f, 0.012f, 0.028f, 0.92f);
            AddOrUpdateOutline(panel, Cyan, new Vector2(2f, -2f));
        }

        private static void AddOrUpdateOutline(GameObject obj, Color color, Vector2 distance)
        {
            var outline = obj.GetComponent<Outline>() ?? obj.AddComponent<Outline>();
            outline.effectColor = color;
            outline.effectDistance = distance;
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            GameEvents.CoinsChanged += OnCoinsChanged;
            BindPlayer();
            RefreshRoom(SceneManager.GetActiveScene().name);
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            GameEvents.CoinsChanged -= OnCoinsChanged;
            UnbindPlayer();
        }

        private void Update()
        {
            if (playerHealth == null || playerStats == null) BindPlayer();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            BindPlayer();
            RefreshRoom(scene.name);
            if (roomNumber == 5) bossWarningUntil = Time.unscaledTime + BossWarningDuration;
        }

        private void BindPlayer()
        {
            UnbindPlayer();

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            player.TryGetComponent(out playerHealth);
            player.TryGetComponent(out playerStats);

            if (InventorySystem.Instance != null) coins = InventorySystem.Instance.Coins;
        }

        private void UnbindPlayer()
        {
            playerHealth = null;
            playerStats = null;
        }

        private void OnCoinsChanged(int value)
        {
            coins = value;
        }

        private void RefreshRoom(string sceneName)
        {
            roomNumber = GetRoomNumber(sceneName);
        }

        private void OnGUI()
        {
            if (roomNumber <= 0) return;
            EnsureStyles();

            DrawTopHud();
            DrawLockedDoorPressure();
            DrawChestPrompt();
            DrawBossWarning();
        }

        private void EnsureStyles()
        {
            if (smallStyle != null) return;

            smallStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.65f, 0.98f, 1f, 1f) }
            };
            valueStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };
            titleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 42,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Red }
            };
        }

        private void DrawTopHud()
        {
            const float margin = 18f;
            float width = Mathf.Min(760f, Screen.width - margin * 2f);
            var hud = new Rect(margin, margin, width, 58f);
            DrawPanel(hud, Cyan);

            int hp = playerHealth != null ? playerHealth.CurrentHealth : 0;
            int maxHp = playerHealth != null ? playerHealth.MaxHealth : 0;
            int attack = playerStats != null ? playerStats.Snapshot.attackPower : 0;

            float cell = hud.width / 4f;
            DrawHudCell(new Rect(hud.x, hud.y + 8f, cell, 42f), "HP", $"{hp}/{maxHp}", Red);
            DrawHudCell(new Rect(hud.x + cell, hud.y + 8f, cell, 42f), "ATK", attack.ToString(), Purple);
            DrawHudCell(new Rect(hud.x + cell * 2f, hud.y + 8f, cell, 42f), "CR", coins.ToString(), Gold);
            DrawHudCell(new Rect(hud.x + cell * 3f, hud.y + 8f, cell, 42f), "ROOM", RoomName(roomNumber), Cyan);
        }

        private void DrawHudCell(Rect rect, string label, string value, Color accent)
        {
            var oldColor = GUI.color;
            GUI.color = accent;
            GUI.DrawTexture(new Rect(rect.x + 10f, rect.y + 3f, rect.width - 20f, 2f), Texture2D.whiteTexture);
            GUI.color = oldColor;
            GUI.Label(new Rect(rect.x, rect.y + 2f, rect.width, 16f), label, smallStyle);
            GUI.Label(new Rect(rect.x, rect.y + 18f, rect.width, 24f), value, valueStyle);
        }

        private void DrawLockedDoorPressure()
        {
            if (!TestSceneRoomSetup.IsCurrentRoomLocked) return;

            float pulse = 0.55f + Mathf.Sin(Time.unscaledTime * 8f) * 0.18f;
            var color = new Color(Red.r, Red.g, Red.b, pulse);
            DrawEdgeAlert(new Rect(0f, 0f, Screen.width, 10f), color);
            DrawEdgeAlert(new Rect(0f, Screen.height - 10f, Screen.width, 10f), color);
            DrawEdgeAlert(new Rect(0f, 0f, 10f, Screen.height), color);
            DrawEdgeAlert(new Rect(Screen.width - 10f, 0f, 10f, Screen.height), color);
        }

        private void DrawEdgeAlert(Rect rect, Color color)
        {
            var oldColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = oldColor;
        }

        private void DrawChestPrompt()
        {
            if (roomNumber != 2) return;

            var chest = GameObject.Find("Treasure Chest");
            if (chest == null || Camera.main == null) return;

            Vector3 screen = Camera.main.WorldToScreenPoint(chest.transform.position + new Vector3(0f, 0.88f, 0f));
            if (screen.z < 0f) return;

            var rect = new Rect(screen.x - 92f, Screen.height - screen.y - 42f, 184f, 34f);
            DrawPanel(rect, Gold);
            GUI.Label(rect, "E  OPEN CACHE", valueStyle);
        }

        private void DrawBossWarning()
        {
            float remaining = bossWarningUntil - Time.unscaledTime;
            if (remaining <= 0f) return;

            float alpha = Mathf.Clamp01(remaining / BossWarningDuration);
            var oldColor = GUI.color;
            GUI.color = new Color(Red.r, Red.g, Red.b, 0.22f * alpha);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = oldColor;

            var rect = new Rect((Screen.width - 520f) * 0.5f, Screen.height * 0.24f, 520f, 82f);
            DrawPanel(rect, Red);
            GUI.Label(rect, "BOSS SIGNAL DETECTED", titleStyle);
        }

        private void DrawPanel(Rect rect, Color accent)
        {
            var oldColor = GUI.color;
            GUI.color = Backdrop;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = accent;
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 2f), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.yMax - 2f, rect.width, 2f), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y, 2f, rect.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMax - 2f, rect.y, 2f, rect.height), Texture2D.whiteTexture);
            GUI.color = oldColor;
        }

        private static int GetRoomNumber(string sceneName)
        {
            if (sceneName == "TestRoom") return 1;

            var match = Regex.Match(sceneName, @"^TestRoom(\d+)$");
            return match.Success && int.TryParse(match.Groups[1].Value, out int value) ? value : 0;
        }

        private static string RoomName(int value)
        {
            return value switch
            {
                1 => "START",
                2 => "CHEST",
                3 => "COMBAT",
                4 => "SHOP",
                5 => "BOSS",
                _ => $"{value}/{LastRoom}"
            };
        }
    }
}
