using HollowStyleMVP.Combat;
using UnityEngine;

namespace HollowStyleMVP.Level
{
    public class HazardDamage : MonoBehaviour
    {
        [SerializeField] private int damage = 1;
        [SerializeField] private float knockback = 9f;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<Health>(out var health)) health.Damage(damage, transform.position, knockback);
        }
    }
}
