using HollowStyleMVP.Combat;
using HollowStyleMVP.Core;
using UnityEngine;

namespace HollowStyleMVP.Roguelike
{
    [RequireComponent(typeof(CombatStats))]
    public class PlayerShooter : MonoBehaviour
    {
        [SerializeField] private int baseDamage = 1;
        [SerializeField] private float projectileSpeed = 9f;
        [SerializeField] private float projectileRange = 7f;
        [SerializeField] private float fireDelay = 0.28f;
        [SerializeField] private float projectileSize = 0.32f;
        [SerializeField] private Color projectileColor = Color.cyan;
        [SerializeField] private KeyCode fireKey = KeyCode.K;
        [SerializeField] private float targetAcquireRange = 12f;
        [SerializeField] private float homingTurnSpeed = 14f;

        private CombatStats stats;
        private PlayerProjectileModifiers modifiers;
        private Vector2 aim = Vector2.right;
        private float cooldown;

        private void Awake()
        {
            stats = GetComponent<CombatStats>();
            modifiers = GetComponent<PlayerProjectileModifiers>();
        }

        private void Update()
        {
            if (UiModalState.HasOpenModal) return;
            if (cooldown > 0f) cooldown -= Time.deltaTime;

            if (Mathf.Abs(transform.localScale.x) > 0.01f)
                aim = transform.localScale.x >= 0f ? Vector2.right : Vector2.left;

            if (!Input.GetKeyDown(fireKey)) return;
            TryShoot();
        }

        public void Configure(int damage, float speed, float range, float delay)
        {
            baseDamage = Mathf.Max(1, damage);
            projectileSpeed = Mathf.Max(1f, speed);
            projectileRange = Mathf.Max(0.5f, range);
            fireDelay = Mathf.Max(0.05f, delay);
        }

        private void TryShoot()
        {
            var projectileStats = BuildStats();
            if (cooldown > 0f) return;
            cooldown = projectileStats.fireDelay;
            SpawnProjectile(projectileStats, FindNearestTarget());
        }

        private ProjectileStats BuildStats()
        {
            var projectileStats = new ProjectileStats
            {
                damage = baseDamage,
                speed = projectileSpeed,
                range = projectileRange,
                fireDelay = fireDelay,
                size = projectileSize
            };
            return modifiers != null ? modifiers.ApplyTo(projectileStats) : projectileStats;
        }

        private void SpawnProjectile(ProjectileStats projectileStats, Transform target)
        {
            var obj = new GameObject("Player Projectile");
            Vector2 direction = target != null ? ((Vector2)target.position - (Vector2)transform.position).normalized : aim;
            obj.transform.position = transform.position + (Vector3)(direction * 0.75f);
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = MakeSprite();
            renderer.color = projectileColor;
            renderer.sortingOrder = 30;
            var collider = obj.AddComponent<CircleCollider2D>();
            collider.radius = 0.5f;
            var projectile = obj.AddComponent<Projectile>();
            projectile.Configure(direction, projectileStats, stats, "");
            projectile.ConfigureHoming(target, homingTurnSpeed);
            FeedbackManager.Instance?.Play(FeedbackSound.Attack);
        }

        private Transform FindNearestTarget()
        {
            Transform best = null;
            float bestSqrDistance = targetAcquireRange * targetAcquireRange;
            var candidates = FindObjectsByType<Health>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var health in candidates)
            {
                if (health == null || health.IsDead) continue;
                if (health.transform.root == transform.root) continue;
                if (health.CompareTag("Player")) continue;

                float sqrDistance = ((Vector2)health.transform.position - (Vector2)transform.position).sqrMagnitude;
                if (sqrDistance > bestSqrDistance) continue;

                bestSqrDistance = sqrDistance;
                best = health.transform;
            }

            return best;
        }

        private static Sprite MakeSprite()
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        }
    }
}
