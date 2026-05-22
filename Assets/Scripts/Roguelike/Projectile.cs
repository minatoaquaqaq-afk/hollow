using HollowStyleMVP.Combat;
using HollowStyleMVP.Core;
using UnityEngine;

namespace HollowStyleMVP.Roguelike
{
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private int damage = 1;
        [SerializeField] private float speed = 9f;
        [SerializeField] private float range = 7f;
        [SerializeField] private float knockback = 4f;
        [SerializeField] private string targetTag = "";

        private Vector2 direction = Vector2.right;
        private Vector2 startPosition;
        private CombatStats ownerStats;
        private Transform ownerRoot;
        private Transform homingTarget;
        private float homingTurnSpeed;
        private bool useOwnerDamageBonus = true;

        public void Configure(Vector2 newDirection, ProjectileStats stats, CombatStats attackerStats, string newTargetTag, bool newUseOwnerDamageBonus = true)
        {
            direction = newDirection.sqrMagnitude > 0.001f ? newDirection.normalized : Vector2.right;
            damage = stats.damage;
            speed = stats.speed;
            range = stats.range;
            targetTag = newTargetTag;
            ownerStats = attackerStats;
            ownerRoot = attackerStats != null ? attackerStats.transform.root : null;
            useOwnerDamageBonus = newUseOwnerDamageBonus;
            transform.localScale = Vector3.one * stats.size;
            startPosition = transform.position;
        }

        public void ConfigureHoming(Transform target, float turnSpeed)
        {
            homingTarget = target;
            homingTurnSpeed = Mathf.Max(0f, turnSpeed);
        }

        private void Awake()
        {
            startPosition = transform.position;
            var collider = GetComponent<Collider2D>();
            collider.isTrigger = true;
        }

        private void Update()
        {
            UpdateHomingDirection();
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
            if (Vector2.Distance(startPosition, transform.position) >= range) Destroy(gameObject);
        }

        private void UpdateHomingDirection()
        {
            if (homingTarget == null || homingTurnSpeed <= 0f) return;
            if (!homingTarget.TryGetComponent<Health>(out var targetHealth) || targetHealth.IsDead)
            {
                homingTarget = null;
                return;
            }

            Vector2 desired = ((Vector2)homingTarget.position - (Vector2)transform.position);
            if (desired.sqrMagnitude <= 0.001f) return;
            direction = Vector2.Lerp(direction, desired.normalized, homingTurnSpeed * Time.deltaTime).normalized;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (ownerRoot != null && other.transform.root == ownerRoot) return;
            if (!string.IsNullOrWhiteSpace(targetTag) && !other.CompareTag(targetTag)) return;
            if (!other.TryGetComponent<Health>(out var health)) return;

            var defenderStats = other.GetComponent<CombatStats>();
            bool critical = false;
            int finalDamage = ownerStats != null && useOwnerDamageBonus ? ownerStats.CalculateDamage(damage, defenderStats, out critical) : damage;
            health.Damage(finalDamage, transform.position, knockback, critical);
            HitEffect.Spawn(other.ClosestPoint(transform.position), critical ? Color.magenta : Color.cyan);
            FeedbackManager.Instance?.Play(critical ? FeedbackSound.Crit : FeedbackSound.Hit);
            Destroy(gameObject);
        }
    }
}
