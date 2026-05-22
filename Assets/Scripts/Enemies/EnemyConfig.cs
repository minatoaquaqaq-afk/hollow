using HollowStyleMVP.Combat;
using HollowStyleMVP.Items;
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
        public int attackPower = 0;
        public int defense = 0;
        [Range(0f, 1f)] public float critChance = 0.03f;
        [Range(0f, 1f)] public float critResistance = 0f;
        public float critDamageMultiplier = 1.3f;
        public float patrolSpeed = 2f;
        public float chaseSpeed = 4f;
        public float detectRange = 7f;
        public float attackRange = 1.2f;
        public float knockback = 5f;
        public GameObject dropPrefab;
        public DropTable dropTable;
    }
}
