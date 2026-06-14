using HollowStyleMVP.Combat;
using HollowStyleMVP.Core;
using HollowStyleMVP.Inventory;
using HollowStyleMVP.Player;
using UnityEngine;
using UnityEngine.UI;

namespace HollowStyleMVP.UI
{
    public class HudController : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Text healthText;
        [SerializeField] private Slider energySlider;
        [SerializeField] private Text energyText;
        [SerializeField] private Text coinText;
        [SerializeField] private Text comboText;
        [SerializeField] private Text abilityText;
        [SerializeField] private Text promptText;

        private readonly System.Collections.Generic.Dictionary<PlayerAbility, bool> abilities = new System.Collections.Generic.Dictionary<PlayerAbility, bool>();

        private void OnEnable()
        {
            GameEvents.CoinsChanged += UpdateCoins;
            GameEvents.SceneMessage += UpdatePrompt;
            GameEvents.PlayerEnergyChanged += UpdateEnergy;
            GameEvents.ComboChanged += UpdateCombo;
            GameEvents.AbilityChanged += UpdateAbility;
        }

        private void OnDisable()
        {
            GameEvents.CoinsChanged -= UpdateCoins;
            GameEvents.SceneMessage -= UpdatePrompt;
            GameEvents.PlayerEnergyChanged -= UpdateEnergy;
            GameEvents.ComboChanged -= UpdateCombo;
            GameEvents.AbilityChanged -= UpdateAbility;
        }

        private void Start()
        {
            RoguelikeRoomHud.EnsureExists();
            EnsureCanvasVisible();
            ApplyFightHudSkin();
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && player.TryGetComponent<Health>(out var health))
            {
                health.Changed += UpdateHealth;
                UpdateHealth(health.CurrentHealth, health.MaxHealth);
            }
            UpdateCombo(0);
            RebuildAbilityText();
        }

        private void EnsureCanvasVisible()
        {
            transform.localScale = Vector3.one;

            var canvas = GetComponent<Canvas>();
            if (canvas == null) return;

            canvas.enabled = true;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;
        }

        private void UpdateHealth(int current, int max)
        {
            if (healthSlider != null)
            {
                healthSlider.maxValue = max;
                healthSlider.value = current;
            }
            if (healthText != null) healthText.text = $"{current} / {max}";
        }

        private void UpdateEnergy(int current, int max)
        {
            if (energySlider != null)
            {
                energySlider.maxValue = max;
                energySlider.value = current;
            }
            if (energyText != null) energyText.text = $"{current} / {max}";
        }

        private void UpdateCoins(int coins)
        {
            if (coinText != null) coinText.text = $"CR {coins}";
        }

        private void UpdateCombo(int combo)
        {
            if (comboText != null) comboText.text = combo > 1 ? $"COMBO\n{combo}" : "COMBO\n0";
        }

        private void UpdateAbility(PlayerAbility ability, bool unlocked)
        {
            abilities[ability] = unlocked;
            RebuildAbilityText();
        }

        private void RebuildAbilityText()
        {
            if (abilityText == null) return;
            bool dash = abilities.TryGetValue(PlayerAbility.Dash, out bool d) && d;
            bool doubleJump = abilities.TryGetValue(PlayerAbility.DoubleJump, out bool j) && j;
            bool downStrike = abilities.TryGetValue(PlayerAbility.DownStrike, out bool down) && down;
            bool ranged = abilities.TryGetValue(PlayerAbility.RangedAttack, out bool r) && r;
            abilityText.text = string.Empty;
        }

        private void ApplyFightHudSkin()
        {
            SkinSlider(healthSlider, "Health Backplate", "HpBar_Frame.png", "HpBar_Fill.png", new Vector2(422f, 55f), new Vector2(402f, 21f));
            SkinSlider(energySlider, "Energy Backplate", "EnergyBar_Frame.png", "EnergyBar_Fill.png", new Vector2(423f, 22f), new Vector2(414f, 9f));
            EnsureCoinText();
            CreateWeaponStrip();
            CreateSettingButton();
        }

        private void SkinSlider(Slider slider, string backplateName, string frameFile, string fillFile, Vector2 frameSize, Vector2 fillSize)
        {
            if (slider == null) return;

            var frame = FightHudSkin.LoadSprite(frameFile);
            var fill = FightHudSkin.LoadSprite(fillFile);
            if (frame == null || fill == null) return;

            var sliderRect = slider.GetComponent<RectTransform>();
            if (sliderRect != null) sliderRect.sizeDelta = frameSize;

            var backplate = transform.Find(backplateName);
            var backplateImage = backplate != null ? backplate.GetComponent<Image>() : null;
            if (backplateImage != null)
            {
                backplateImage.sprite = frame;
                backplateImage.color = Color.white;
                backplateImage.preserveAspect = true;
                if (backplate is RectTransform backplateRect) backplateRect.sizeDelta = frameSize;
            }

            var fillImage = slider.fillRect != null ? slider.fillRect.GetComponent<Image>() : null;
            if (fillImage == null) return;

            fillImage.sprite = fill;
            fillImage.color = Color.white;
            fillImage.preserveAspect = true;
            fillImage.type = Image.Type.Simple;

            slider.fillRect.anchorMin = new Vector2(0f, 0.5f);
            slider.fillRect.anchorMax = new Vector2(1f, 0.5f);
            slider.fillRect.pivot = new Vector2(0.5f, 0.5f);
            slider.fillRect.anchoredPosition = Vector2.zero;
            slider.fillRect.sizeDelta = new Vector2(-(frameSize.x - fillSize.x), fillSize.y);
        }

        private void EnsureCoinText()
        {
            if (coinText == null)
            {
                var obj = new GameObject("Fight Coin Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
                obj.transform.SetParent(transform, false);
                var rect = obj.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(0f, 1f);
                rect.pivot = new Vector2(0f, 0.5f);
                rect.anchoredPosition = new Vector2(485f, -116f);
                rect.sizeDelta = new Vector2(120f, 28f);
                coinText = obj.GetComponent<Text>();
            }

            coinText.alignment = TextAnchor.MiddleLeft;
            coinText.color = new Color(1f, 0.78f, 0.18f, 1f);
            coinText.fontSize = 18;
            coinText.fontStyle = FontStyle.Bold;
            coinText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            coinText.raycastTarget = false;
            UpdateCoins(InventorySystem.Instance != null ? InventorySystem.Instance.Coins : 0);
        }

        private void CreateWeaponStrip()
        {
            if (transform.Find("Fight Weapon Strip") != null) return;

            var frame = FightHudSkin.LoadSprite("weapon_frame.png");
            if (frame == null) return;

            var root = CreateImage("Fight Weapon Strip", transform, frame, new Vector2(570f, 114f));
            var rect = root.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, 18f);

            string[] icons = { "weapon_gun.png", "weapon_dagger.png", "weapon_bomb.png" };
            for (int i = 0; i < icons.Length; i++)
            {
                var icon = FightHudSkin.LoadSprite(icons[i]);
                if (icon == null) continue;

                var child = CreateImage($"Fight Weapon Icon {i + 1}", root.transform, icon, new Vector2(58f, 58f));
                var iconRect = child.GetComponent<RectTransform>();
                iconRect.anchorMin = new Vector2(0f, 0.5f);
                iconRect.anchorMax = new Vector2(0f, 0.5f);
                iconRect.pivot = new Vector2(0.5f, 0.5f);
                iconRect.anchoredPosition = new Vector2(68f + i * 84f, 0f);
            }
        }

        private void CreateSettingButton()
        {
            if (transform.Find("Fight Setting Button") != null) return;

            var bg = FightHudSkin.LoadSprite("SettingBtn_Bg.png");
            var frame = FightHudSkin.LoadSprite("SettingBtn_Frame.png");
            if (bg == null || frame == null) return;

            var root = CreateImage("Fight Setting Button", transform, bg, new Vector2(149f, 50f));
            var rect = root.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-18f, -18f);

            var button = root.AddComponent<Button>();
            button.onClick.AddListener(() => GameManager.Instance?.SetPaused(!(GameManager.Instance?.IsPaused ?? false)));

            var frameObj = CreateImage("Frame", root.transform, frame, new Vector2(149f, 50f));
            StretchToParent(frameObj.GetComponent<RectTransform>());

            var labelObj = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            labelObj.transform.SetParent(root.transform, false);
            var labelRect = labelObj.GetComponent<RectTransform>();
            StretchToParent(labelRect);
            var label = labelObj.GetComponent<Text>();
            label.text = "ESC";
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            label.fontSize = 18;
            label.fontStyle = FontStyle.Bold;
            label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            label.raycastTarget = false;
        }

        private static GameObject CreateImage(string name, Transform parent, Sprite sprite, Vector2 size)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            obj.transform.SetParent(parent, false);
            var rect = obj.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            var image = obj.GetComponent<Image>();
            image.sprite = sprite;
            image.color = Color.white;
            image.preserveAspect = true;
            return obj;
        }

        private static void StretchToParent(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static string OnOff(bool value) => value ? "开" : "关";

        private void UpdatePrompt(string message)
        {
            if (promptText != null) promptText.text = message;
        }
    }
}
