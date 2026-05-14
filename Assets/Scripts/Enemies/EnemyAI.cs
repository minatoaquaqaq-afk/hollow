using HollowStyleMVP.Combat;
using UnityEngine;

namespace HollowStyleMVP.Enemies
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Health))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] private EnemyConfig config;
        [SerializeField] private float patrolSpeed = 2f;
        [SerializeField] private float chaseSpeed = 4f;
        [SerializeField] private float detectRange = 7f;
        [SerializeField] private float attackRange = 1.2f;
        [SerializeField] private GameObject dropPrefab;
        [SerializeField] private Transform[] patrolPoints;
        private Rigidbody2D body;
        private Health health;
        private Transform player;
        private int pointIndex;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            health = GetComponent<Health>();
            ApplyConfig(config);
            health.Died += Die;
        }

        public void ApplyConfig(EnemyConfig newConfig)
        {
            if (newConfig == null) return;
            config = newConfig;
            patrolSpeed = config.patrolSpeed;
            chaseSpeed = config.chaseSpeed;
            detectRange = config.detectRange;
            attackRange = config.attackRange;
            dropPrefab = config.dropPrefab;
            if (health != null) health.Configure(config.maxHealth, 0.35f, true);
            if (TryGetComponent<ContactDamage>(out var contactDamage)) contactDamage.Configure(config.contactDamage, config.knockback);
        }

        private void Start()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        private void FixedUpdate()
        {
            if (health.IsDead) return;
            if (player != null && Vector2.Distance(transform.position, player.position) <= detectRange) ChasePlayer();
            else Patrol();
        }

        private void Patrol()
        {
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                body.velocity = new Vector2(patrolSpeed * Mathf.Sign(transform.localScale.x == 0 ? 1 : transform.localScale.x), body.velocity.y);
                return;
            }
            Vector2 target = patrolPoints[pointIndex].position;
            float dir = Mathf.Sign(target.x - transform.position.x);
            body.velocity = new Vector2(dir * patrolSpeed, body.velocity.y);
            Face(dir);
            if (Mathf.Abs(target.x - transform.position.x) < 0.2f) pointIndex = (pointIndex + 1) % patrolPoints.Length;
        }

        private void ChasePlayer()
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist <= attackRange)
            {
                body.velocity = new Vector2(0f, body.velocity.y);
                return;
            }
            float dir = Mathf.Sign(player.position.x - transform.position.x);
            body.velocity = new Vector2(dir * chaseSpeed, body.velocity.y);
            Face(dir);
        }

        private void Face(float dir)
        {
            if (Mathf.Abs(dir) < 0.01f) return;
            transform.localScale = new Vector3(Mathf.Sign(dir), 1f, 1f);
        }

        private void Die()
        {
            if (dropPrefab != null) Instantiate(dropPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject, 0.1f);
        }
    }
}

