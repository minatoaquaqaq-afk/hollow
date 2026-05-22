using HollowStyleMVP.Combat;
using UnityEngine;

namespace HollowStyleMVP.Enemies
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Health), typeof(CombatStats))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] private EnemyConfig config;
        [SerializeField] private float patrolSpeed = 2f;
        [SerializeField] private float chaseSpeed = 4f;
        [SerializeField] private float detectRange = 7f;
        [SerializeField] private float attackRange = 1.2f;
        [SerializeField] private GameObject dropPrefab;
        [SerializeField] private HollowStyleMVP.Items.DropTable dropTable;
        [SerializeField] private Transform[] patrolPoints;
        private Rigidbody2D body;
        private Health health;
        private CombatStats stats;
        private Transform player;
        private int pointIndex;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            health = GetComponent<Health>();
            stats = GetComponent<CombatStats>();
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
            dropTable = config.dropTable;
            if (health != null) health.Configure(config.maxHealth, 0.35f, true);
            if (stats != null) stats.SetBase(config.maxHealth, 0, config.attackPower, config.defense, config.critChance, config.critResistance, config.critDamageMultiplier);
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
                body.velocity = Vector2.zero;
                return;
            }
            Vector2 target = patrolPoints[pointIndex].position;
            Vector2 direction = target - (Vector2)transform.position;
            if (direction.magnitude < 0.2f)
            {
                pointIndex = (pointIndex + 1) % patrolPoints.Length;
                return;
            }
            body.velocity = direction.normalized * patrolSpeed;
            Face(direction.x);
        }

        private void ChasePlayer()
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist <= attackRange)
            {
                body.velocity = Vector2.zero;
                return;
            }
            Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
            body.velocity = direction * chaseSpeed;
            Face(direction.x);
        }

        private void Face(float dir)
        {
            if (Mathf.Abs(dir) < 0.01f) return;
            transform.localScale = new Vector3(Mathf.Sign(dir), 1f, 1f);
        }

        private void Die()
        {
            if (dropTable != null) dropTable.Spawn(transform.position);
            else if (dropPrefab != null) Instantiate(dropPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject, 0.1f);
        }
    }
}
