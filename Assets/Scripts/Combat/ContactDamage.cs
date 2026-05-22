using UnityEngine;

namespace HollowStyleMVP.Combat
{
    public class ContactDamage : MonoBehaviour
    {
        [SerializeField] private int damage = 1;
        [SerializeField] private float knockback = 7f;
        [SerializeField] private string targetTag = "Player";
        private CombatStats attackerStats;

        private void Awake()
        {
            attackerStats = GetComponent<CombatStats>();
        }

        public void Configure(int newDamage, float newKnockback)
        {
            damage = Mathf.Max(0, newDamage);
            knockback = Mathf.Max(0f, newKnockback);
        }

        private void OnCollisionEnter2D(Collision2D collision) => TryDamage(collision.collider);
        private void OnCollisionStay2D(Collision2D collision) => TryDamage(collision.collider);
        private void OnTriggerEnter2D(Collider2D other) => TryDamage(other);

        private void TryDamage(Collider2D other)
        {
            if (!string.IsNullOrWhiteSpace(targetTag) && !other.CompareTag(targetTag)) return;
            if (!other.TryGetComponent<Health>(out var health)) return;
            bool critical = false;
            int finalDamage = attackerStats != null ? attackerStats.CalculateDamage(damage, other.GetComponent<CombatStats>(), out critical) : damage;
            health.Damage(finalDamage, transform.position, knockback, critical);
        }
    }
}
