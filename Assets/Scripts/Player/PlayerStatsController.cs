using HollowStyleMVP.Core;
using UnityEngine;

namespace HollowStyleMVP.Player
{
    public class PlayerStatsController : MonoBehaviour
    {
        [SerializeField] private int maxEnergy = 100;
        [SerializeField] private int currentEnergy = 100;
        [SerializeField] private float energyRegenPerSecond = 12f;
        [SerializeField] private int dashEnergyCost = 8;
        private float regenBank;

        public int CurrentEnergy => currentEnergy;
        public int MaxEnergy => maxEnergy;

        private void Start() => GameEvents.RaisePlayerEnergyChanged(currentEnergy, maxEnergy);

        private void Update()
        {
            if (currentEnergy >= maxEnergy) return;
            regenBank += energyRegenPerSecond * Time.deltaTime;
            if (regenBank < 1f) return;
            int gain = Mathf.FloorToInt(regenBank);
            regenBank -= gain;
            currentEnergy = Mathf.Min(maxEnergy, currentEnergy + gain);
            GameEvents.RaisePlayerEnergyChanged(currentEnergy, maxEnergy);
        }

        public void ApplyConfig(PlayerConfig config)
        {
            if (config == null) return;
            maxEnergy = Mathf.Max(1, config.maxEnergy);
            currentEnergy = maxEnergy;
            GameEvents.RaisePlayerEnergyChanged(currentEnergy, maxEnergy);
        }

        public bool TrySpendDashEnergy()
        {
            if (currentEnergy < dashEnergyCost) return false;
            currentEnergy -= dashEnergyCost;
            GameEvents.RaisePlayerEnergyChanged(currentEnergy, maxEnergy);
            return true;
        }
    }
}
