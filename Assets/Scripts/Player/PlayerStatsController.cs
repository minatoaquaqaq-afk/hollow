using HollowStyleMVP.Core;
using UnityEngine;

namespace HollowStyleMVP.Player
{
    [RequireComponent(typeof(HollowStyleMVP.Combat.CombatStats))]
    public class PlayerStatsController : MonoBehaviour
    {
        [SerializeField] private int maxEnergy = 100;
        [SerializeField] private int currentEnergy = 100;
        [SerializeField] private float energyRegenPerSecond = 12f;
        [SerializeField] private int dashEnergyCost = 8;
        private float regenBank;
        private HollowStyleMVP.Combat.CombatStats combatStats;
        private HollowStyleMVP.Combat.Health health;

        public int CurrentEnergy => currentEnergy;
        public int MaxEnergy => maxEnergy;

        private void Awake()
        {
            combatStats = GetComponent<HollowStyleMVP.Combat.CombatStats>();
            health = GetComponent<HollowStyleMVP.Combat.Health>();
        }

        private void OnEnable()
        {
            if (combatStats != null) combatStats.StatsChanged += RefreshFromStats;
        }

        private void OnDisable()
        {
            if (combatStats != null) combatStats.StatsChanged -= RefreshFromStats;
        }

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
            return TrySpendEnergy(dashEnergyCost);
        }

        public bool TrySpendEnergy(int amount)
        {
            amount = Mathf.Max(0, amount);
            if (currentEnergy < amount) return false;
            currentEnergy -= amount;
            GameEvents.RaisePlayerEnergyChanged(currentEnergy, maxEnergy);
            return true;
        }

        private void RefreshFromStats()
        {
            if (combatStats == null) return;
            var snapshot = combatStats.Snapshot;
            int oldMax = maxEnergy;
            maxEnergy = snapshot.maxEnergy;
            if (oldMax != maxEnergy) currentEnergy = Mathf.Clamp(currentEnergy + (maxEnergy - oldMax), 0, maxEnergy);
            if (health != null) health.Configure(snapshot.maxHealth, 0.35f, false);
            GameEvents.RaisePlayerEnergyChanged(currentEnergy, maxEnergy);
        }
    }
}
