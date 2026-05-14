using HollowStyleMVP.Player;
using UnityEngine;

namespace HollowStyleMVP.Combat
{
    public class DamageDealer : MonoBehaviour
    {
        [SerializeField] private int damage = 1;
        [SerializeField] private float knockback = 6f;
        [SerializeField] private string targetTag = "";

        public void Configure(int newDamage, float newKnockback)
        {
            damage = Mathf.Max(0, newDamage);
            knockback = Mathf.Max(0f, newKnockback);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform.root == transform.root) return;
            if (!string.IsNullOrWhiteSpace(targetTag) && !other.CompareTag(targetTag)) return;
            if (other.TryGetComponent<Health>(out var health))
            {
                health.Damage(damage, transform.position, knockback);
                GetComponentInParent<ComboCounter>()?.AddHit();
            }
        }
    }
}

