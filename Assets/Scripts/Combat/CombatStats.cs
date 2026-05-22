using System;
using UnityEngine;

namespace HollowStyleMVP.Combat
{
    [Serializable]
    public struct StatModifier
    {
        public int maxHealth;
        public int maxEnergy;
        public int attackPower;
        public int defense;
        [Range(0f, 1f)] public float critChance;
        [Range(0f, 1f)] public float critResistance;
        public float critDamageBonus;
        public float moveSpeedBonus;
        public float jumpForceBonus;
    }

    [Serializable]
    public struct CombatStatSnapshot
    {
        public int maxHealth;
        public int maxEnergy;
        public int attackPower;
        public int defense;
        public float critChance;
        public float critResistance;
        public float critDamageMultiplier;
        public float moveSpeedBonus;
        public float jumpForceBonus;
    }

    public class CombatStats : MonoBehaviour
    {
        [Header("Base Stats")]
        [SerializeField] private int maxHealth = 5;
        [SerializeField] private int maxEnergy = 100;
        [SerializeField] private int attackPower = 1;
        [SerializeField] private int defense;
        [Range(0f, 1f)] [SerializeField] private float critChance = 0.05f;
        [Range(0f, 1f)] [SerializeField] private float critResistance;
        [SerializeField] private float critDamageMultiplier = 1.5f;
        [SerializeField] private float moveSpeedBonus;
        [SerializeField] private float jumpForceBonus;

        private StatModifier equipmentBonus;
        private StatModifier charmBonus;

        public event Action StatsChanged;

        public CombatStatSnapshot Snapshot
        {
            get
            {
                return new CombatStatSnapshot
                {
                    maxHealth = Mathf.Max(1, maxHealth + equipmentBonus.maxHealth + charmBonus.maxHealth),
                    maxEnergy = Mathf.Max(1, maxEnergy + equipmentBonus.maxEnergy + charmBonus.maxEnergy),
                    attackPower = Mathf.Max(0, attackPower + equipmentBonus.attackPower + charmBonus.attackPower),
                    defense = Mathf.Max(0, defense + equipmentBonus.defense + charmBonus.defense),
                    critChance = Mathf.Clamp01(critChance + equipmentBonus.critChance + charmBonus.critChance),
                    critResistance = Mathf.Clamp01(critResistance + equipmentBonus.critResistance + charmBonus.critResistance),
                    critDamageMultiplier = Mathf.Max(1f, critDamageMultiplier + equipmentBonus.critDamageBonus + charmBonus.critDamageBonus),
                    moveSpeedBonus = moveSpeedBonus + equipmentBonus.moveSpeedBonus + charmBonus.moveSpeedBonus,
                    jumpForceBonus = jumpForceBonus + equipmentBonus.jumpForceBonus + charmBonus.jumpForceBonus
                };
            }
        }

        public void SetBase(int health, int energy, int attack, int armor, float crit, float antiCrit, float critMultiplier, float moveBonus = 0f, float jumpBonus = 0f)
        {
            maxHealth = Mathf.Max(1, health);
            maxEnergy = Mathf.Max(1, energy);
            attackPower = Mathf.Max(0, attack);
            defense = Mathf.Max(0, armor);
            critChance = Mathf.Clamp01(crit);
            critResistance = Mathf.Clamp01(antiCrit);
            critDamageMultiplier = Mathf.Max(1f, critMultiplier);
            moveSpeedBonus = moveBonus;
            jumpForceBonus = jumpBonus;
            StatsChanged?.Invoke();
        }

        public void SetEquipmentBonus(StatModifier modifier)
        {
            equipmentBonus = modifier;
            StatsChanged?.Invoke();
        }

        public void SetCharmBonus(StatModifier modifier)
        {
            charmBonus = modifier;
            StatsChanged?.Invoke();
        }

        public int CalculateDamage(int baseDamage, CombatStats defender, out bool critical)
        {
            var attacker = Snapshot;
            var defenderStats = defender != null ? defender.Snapshot : default;
            float finalCritChance = Mathf.Clamp01(attacker.critChance - defenderStats.critResistance);
            critical = UnityEngine.Random.value < finalCritChance;
            float raw = Mathf.Max(0, baseDamage + attacker.attackPower);
            if (critical) raw *= attacker.critDamageMultiplier;
            int reduced = Mathf.RoundToInt(raw) - defenderStats.defense;
            return Mathf.Max(1, reduced);
        }
    }
}
