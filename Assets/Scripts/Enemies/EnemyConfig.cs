using UnityEngine;

namespace HollowStyleMVP.Enemies
{
    public enum EnemyArchetype { Walker, Flyer, Ranged, Jumper, Shield }

    [CreateAssetMenu(menuName = "Hollow Style MVP/Config/Enemy Config")]
    public class EnemyConfig : ScriptableObject
    {
        public EnemyArchetype archetype = EnemyArchetype.Walker;
        public int maxHealth = 3;
        public int contactDamage = 1;
        public float patrolSpeed = 2f;
        public float chaseSpeed = 4f;
        public float detectRange = 7f;
        public float attackRange = 1.2f;
        public float knockback = 5f;
        public GameObject dropPrefab;
    }
}
