using UnityEngine;

namespace HollowStyleMVP.Boss
{
    [CreateAssetMenu(menuName = "Hollow Style MVP/Config/Boss Config")]
    public class BossConfig : ScriptableObject
    {
        public int maxHealth = 20;
        public int contactDamage = 2;
        public float detectRange = 12f;
        public float moveSpeed = 3.4f;
        public float attackRange = 2f;
        public float actionCooldown = 1.3f;
        public float leapForce = 12f;
        public int phaseTwoHealthPercent = 50;
        public GameObject rewardPrefab;
    }
}
