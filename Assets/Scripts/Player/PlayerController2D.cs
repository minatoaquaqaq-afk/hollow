using HollowStyleMVP.Combat;
using HollowStyleMVP.Core;
using UnityEngine;

namespace HollowStyleMVP.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(PlayerAbilityController))]
    public class PlayerController2D : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private PlayerConfig config;

        [Header("Move")]
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float jumpForce = 15f;
        [SerializeField] private float variableJumpCutMultiplier = 0.5f;
        [SerializeField] private int maxAirJumps = 1;
        [SerializeField] private float dashSpeed = 18f;
        [SerializeField] private float dashDuration = 0.16f;
        [SerializeField] private float dashCooldown = 0.45f;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Vector2 groundCheckSize = new Vector2(0.75f, 0.12f);

        [Header("Combat")]
        [SerializeField] private DamageDealer attackHitbox;
        [SerializeField] private float attackTime = 0.14f;

        private Rigidbody2D body;
        private Animator animator;
        private PlayerAbilityController abilities;
        private PlayerStatsController stats;
        private int remainingAirJumps;
        private float dashTimer;
        private float dashCooldownTimer;
        private float attackTimer;
        private int facing = 1;
        private bool isDashing;

        public bool IsGrounded { get; private set; }
        public PlayerConfig Config => config;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            animator = GetComponentInChildren<Animator>();
            abilities = GetComponent<PlayerAbilityController>();
            stats = GetComponent<PlayerStatsController>();
            ApplyConfig(config);
            if (attackHitbox != null) attackHitbox.gameObject.SetActive(false);
        }

        public void ApplyConfig(PlayerConfig newConfig)
        {
            if (newConfig == null) return;
            config = newConfig;
            moveSpeed = config.moveSpeed;
            jumpForce = config.jumpForce;
            variableJumpCutMultiplier = config.variableJumpCutMultiplier;
            maxAirJumps = config.maxAirJumps;
            dashSpeed = config.dashSpeed;
            dashDuration = config.dashDuration;
            dashCooldown = config.dashCooldown;
            attackTime = config.attackTime;
            abilities ??= GetComponent<PlayerAbilityController>();
            abilities.ApplyStartingAbilities(config);
            if (TryGetComponent<Health>(out var health)) health.Configure(config.maxHealth, 0.35f, true);
            if (TryGetComponent<PlayerStatsController>(out var playerStats)) playerStats.ApplyConfig(config);
            if (attackHitbox != null) attackHitbox.Configure(config.attackPower, config.attackKnockback);
        }

        private void Update()
        {
            ReadCombatInput();
            ReadJumpInput();
            ReadDashInput();
            CutJumpIfReleased();
            UpdateTimers();
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            IsGrounded = groundCheck != null && Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundMask);
            if (IsGrounded) remainingAirJumps = abilities.Has(PlayerAbility.DoubleJump) ? maxAirJumps : 0;

            if (isDashing)
            {
                body.velocity = new Vector2(facing * dashSpeed, 0f);
                return;
            }

            float inputX = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(inputX) > 0.01f)
            {
                facing = inputX > 0f ? 1 : -1;
                transform.localScale = new Vector3(facing, 1f, 1f);
            }

            body.velocity = new Vector2(inputX * moveSpeed, body.velocity.y);
        }

        private void ReadJumpInput()
        {
            if (!Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyDown(KeyCode.W)) return;
            if (!IsGrounded && remainingAirJumps <= 0) return;

            if (!IsGrounded) remainingAirJumps--;
            body.velocity = new Vector2(body.velocity.x, jumpForce);
            FeedbackManager.Instance?.Play(FeedbackSound.Jump);
        }

        private void CutJumpIfReleased()
        {
            if ((Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.W)) && body.velocity.y > 0f)
            {
                body.velocity = new Vector2(body.velocity.x, body.velocity.y * variableJumpCutMultiplier);
            }
        }

        private void ReadDashInput()
        {
            if (!abilities.Has(PlayerAbility.Dash)) return;
            bool dashPressed = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftControl);
            if (!dashPressed || dashCooldownTimer > 0f) return;
            if (stats != null && !stats.TrySpendDashEnergy()) return;
            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;
            FeedbackManager.Instance?.Play(FeedbackSound.Dash);
            HitEffect.Spawn(transform.position, Color.cyan);
        }

        private void ReadCombatInput()
        {
            if (!Input.GetKeyDown(KeyCode.J) || attackHitbox == null || attackTimer > 0f) return;
            attackTimer = attackTime;
            attackHitbox.gameObject.SetActive(true);
            FeedbackManager.Instance?.Play(FeedbackSound.Attack);
            HitEffect.Spawn(transform.position + Vector3.right * facing * 0.7f, Color.yellow);
            animator?.SetTrigger("Attack");
        }

        private void UpdateTimers()
        {
            if (dashCooldownTimer > 0f) dashCooldownTimer -= Time.deltaTime;
            if (dashTimer > 0f)
            {
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0f) isDashing = false;
            }

            if (attackTimer > 0f)
            {
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0f && attackHitbox != null) attackHitbox.gameObject.SetActive(false);
            }
        }

        private void UpdateAnimator()
        {
            if (animator == null) return;
            animator.SetFloat("Speed", Mathf.Abs(body.velocity.x));
            animator.SetFloat("Vertical", body.velocity.y);
            animator.SetBool("Grounded", IsGrounded);
            animator.SetBool("Dashing", isDashing);
        }
    }
}

