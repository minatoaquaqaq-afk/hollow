using HollowStyleMVP.Combat;
using UnityEngine;

namespace HollowStyleMVP.Boss
{
    [RequireComponent(typeof(Health), typeof(Rigidbody2D))]
    public class BossController : MonoBehaviour
    {
        private enum BossState { Idle, Chase, Slash, Leap, Dead }
        [SerializeField] private BossConfig config;
        [SerializeField] private float detectRange = 12f;
        [SerializeField] private float moveSpeed = 3.4f;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float actionCooldown = 1.3f;
        [SerializeField] private float leapForce = 12f;
        [SerializeField] private DamageDealer slashHitbox;
        private Health health;
        private Rigidbody2D body;
        private Transform player;
        private BossState state;
        private float cooldown;

        private void Awake()
        {
            health = GetComponent<Health>();
            body = GetComponent<Rigidbody2D>();
            ApplyConfig(config);
            health.Died += () => state = BossState.Dead;
            if (slashHitbox != null) slashHitbox.gameObject.SetActive(false);
        }

        public void ApplyConfig(BossConfig newConfig)
        {
            if (newConfig == null) return;
            config = newConfig;
            detectRange = config.detectRange;
            moveSpeed = config.moveSpeed;
            attackRange = config.attackRange;
            actionCooldown = config.actionCooldown;
            leapForce = config.leapForce;
            if (health != null) health.Configure(config.maxHealth, 0.35f, true);
            if (TryGetComponent<ContactDamage>(out var contactDamage)) contactDamage.Configure(config.contactDamage, 8f);
        }

        private void Start()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        private void Update()
        {
            if (state == BossState.Dead || player == null) return;
            cooldown -= Time.deltaTime;
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance > detectRange) { state = BossState.Idle; return; }
            if (cooldown <= 0f)
            {
                state = distance <= attackRange ? BossState.Slash : BossState.Leap;
                cooldown = actionCooldown;
            }
            else if (state != BossState.Slash && state != BossState.Leap) state = BossState.Chase;
            TickState();
        }

        private void TickState()
        {
            float dir = Mathf.Sign(player.position.x - transform.position.x);
            transform.localScale = new Vector3(dir, 1f, 1f);
            switch (state)
            {
                case BossState.Chase:
                    body.velocity = new Vector2(dir * moveSpeed, body.velocity.y);
                    break;
                case BossState.Slash:
                    StartCoroutine(SlashRoutine());
                    state = BossState.Chase;
                    break;
                case BossState.Leap:
                    body.AddForce(new Vector2(dir * leapForce * 0.55f, leapForce), ForceMode2D.Impulse);
                    state = BossState.Chase;
                    break;
            }
        }

        private System.Collections.IEnumerator SlashRoutine()
        {
            if (slashHitbox == null) yield break;
            slashHitbox.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.18f);
            slashHitbox.gameObject.SetActive(false);
        }
    }
}

