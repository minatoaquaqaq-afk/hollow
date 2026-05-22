using UnityEngine;

namespace HollowStyleMVP.Player
{
    [CreateAssetMenu(menuName = "Hollow Style MVP/Config/Player Config")]
    public class PlayerConfig : ScriptableObject
    {
        [Header("Vitals")]
        public int maxHealth = 5;
        public int maxEnergy = 100;
        public int startCoins;

        [Header("Stats")]
        public int attackPower = 1;
        public int defense;
        [Range(0f, 1f)] public float critChance = 0.08f;
        [Range(0f, 1f)] public float critResistance;
        public float critDamageMultiplier = 1.5f;
        public float moveSpeed = 8f;
        public float jumpForce = 15f;
        public float variableJumpCutMultiplier = 0.5f;

        [Header("Abilities")]
        public bool startWithDoubleJump = true;
        public bool startWithDash = true;
        public bool startWithDownStrike = true;
        public bool startWithRangedAttack;
        public int maxAirJumps = 1;

        [Header("Dash")]
        public float dashSpeed = 18f;
        public float dashDuration = 0.16f;
        public float dashCooldown = 0.45f;
        public float dashInvulnerableSeconds = 0.08f;

        [Header("Combat")]
        public float attackTime = 0.14f;
        public float attackKnockback = 6f;
        public int comboCount = 1;

        [Header("Ranged Attack")]
        public int rangedAttackDamage = 2;
        public float rangedProjectileSpeed = 9f;
        public float rangedProjectileRange = 9f;
        public float rangedFireDelay = 0.32f;

        [Header("Respawn")]
        public float respawnDelay = 1.2f;
    }
}
