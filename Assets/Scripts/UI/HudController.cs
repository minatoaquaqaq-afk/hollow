using HollowStyleMVP.Combat;
using HollowStyleMVP.Core;
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
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && player.TryGetComponent<Health>(out var health))
            {
                health.Changed += UpdateHealth;
                UpdateHealth(health.CurrentHealth, health.MaxHealth);
            }
            UpdateCombo(0);
            RebuildAbilityText();
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
            if (coinText != null) coinText.text = $"{coins}";
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

        private static string OnOff(bool value) => value ? "开" : "关";

        private void UpdatePrompt(string message)
        {
            if (promptText != null) promptText.text = message;
        }
    }
}
