using HollowStyleMVP.Combat;
using HollowStyleMVP.Core;
using HollowStyleMVP.Inventory;
using HollowStyleMVP.Roguelike;
using HollowStyleMVP.Visuals;
using UnityEngine;

namespace HollowStyleMVP.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(PlayerAbilityController))]
    public class PlayerController2D : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private PlayerConfig config;

        [Header("Top Down Move")]
        [SerializeField] private float moveSpeed = 5.8f;
        [SerializeField] private float acceleration = 58f;
        [SerializeField] private float deceleration = 72f;
        [SerializeField] private float dashSpeed = 12.5f;
        [SerializeField] private float dashDuration = 0.12f;
        [SerializeField] private float dashCooldown = 0.34f;

        [Header("Legacy Config Fields")]
        [SerializeField] private float jumpForce = 1f;
        [SerializeField] private float variableJumpCutMultiplier = 0.5f;
        [SerializeField] private int maxAirJumps = 1;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Vector2 groundCheckSize = new Vector2(0.75f, 0.16f);

        [Header("Combat")]
        [SerializeField] private DamageDealer attackHitbox;
        [SerializeField] private float attackTime = 0.12f;
        [SerializeField] private Vector2 slashSize = new Vector2(1.15f, 0.78f);
        [SerializeField] private Vector2 verticalSlashSize = new Vector2(0.78f, 1.15f);
        [SerializeField] private float recoilForce = 3.5f;

        private Rigidbody2D body;
        private Animator animator;
        private PlayerAbilityController abilities;
        private PlayerStatsController stats;
        private CombatStats combatStats;
        private FrameAnimator2D frameAnimator;
        private SlashEffectSpawner slashEffectSpawner;
        private Vector2 moveInput;
        private Vector2 facing = Vector2.right;
        private float dashTimer;
        private float dashCooldownTimer;
        private float attackTimer;
        private bool isDashing;

        public bool IsGrounded { get; private set; } = true;
        public PlayerConfig Config => config;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            animator = GetComponentInChildren<Animator>();
            abilities = GetComponent<PlayerAbilityController>();
            stats = GetComponent<PlayerStatsController>();
            combatStats = GetComponent<CombatStats>();
            frameAnimator = GetComponentInChildren<FrameAnimator2D>();
            slashEffectSpawner = GetComponentInChildren<SlashEffectSpawner>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            ApplyConfig(config);
            if (attackHitbox != null) attackHitbox.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (combatStats != null) combatStats.StatsChanged += ApplyStatMovementBonuses;
        }

        private void OnDisable()
        {
            if (combatStats != null) combatStats.StatsChanged -= ApplyStatMovementBonuses;
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
            combatStats ??= GetComponent<CombatStats>();
            if (combatStats != null)
                combatStats.SetBase(config.maxHealth, config.maxEnergy, config.attackPower, config.defense, config.critChance, config.critResistance, config.critDamageMultiplier);
            if (attackHitbox != null) attackHitbox.Configure(config.attackPower, config.attackKnockback);
            if (TryGetComponent<PlayerShooter>(out var shooter))
                shooter.Configure(config.rangedAttackDamage, config.rangedProjectileSpeed, config.rangedProjectileRange, config.rangedFireDelay);
            ApplyStatMovementBonuses();
        }

        private void Start()
        {
            if (config != null && config.startCoins > 0 && InventorySystem.Instance != null && InventorySystem.Instance.Coins == 0)
                InventorySystem.Instance.AddCoins(config.startCoins);
        }

        private void Update()
        {
            if (UiModalState.HasOpenModal)
            {
                moveInput = Vector2.zero;
                UpdateTimers();
                UpdateAnimator();
                return;
            }

            ReadMoveInput();
            ReadCombatInput();
            ReadDashInput();
            UpdateTimers();
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            IsGrounded = true;
            if (isDashing)
            {
                body.velocity = facing * dashSpeed;
                return;
            }

            Vector2 targetVelocity = moveInput * moveSpeed;
            float rate = moveInput.sqrMagnitude > 0.001f ? acceleration : deceleration;
            body.velocity = Vector2.MoveTowards(body.velocity, targetVelocity, rate * Time.fixedDeltaTime);
        }

        private void ReadMoveInput()
        {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (moveInput.sqrMagnitude > 1f) moveInput.Normalize();
            if (moveInput.sqrMagnitude <= 0.01f) return;

            facing = moveInput.normalized;
            if (Mathf.Abs(facing.x) > 0.05f)
                transform.localScale = new Vector3(facing.x >= 0f ? 1f : -1f, 1f, 1f);
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
            ConfigureAttackHitbox();
            attackTimer = attackTime;
            attackHitbox.gameObject.SetActive(true);
            FeedbackManager.Instance?.Play(FeedbackSound.Attack);
            Vector3 effectPosition = transform.position + (Vector3)(facing * 0.75f);
            HitEffect.Spawn(effectPosition, Color.yellow);
            slashEffectSpawner?.Spawn(effectPosition, facing.x >= 0f ? 1 : -1, Mathf.Abs(facing.y) > Mathf.Abs(facing.x), Color.white);
            CameraShake.Instance?.Shake(0.06f, 0.06f);
            animator?.SetTrigger("Attack");
            frameAnimator?.PlayAttack(ResolveAttackProfile(), attackTime);
        }

        private AttackProfile ResolveAttackProfile()
        {
            if (Mathf.Abs(facing.y) > Mathf.Abs(facing.x))
                return facing.y < 0f && abilities.Has(PlayerAbility.DownStrike) ? AttackProfile.DownSlash : AttackProfile.UpSlash;
            return AttackProfile.Slash;
        }

        private void ConfigureAttackHitbox()
        {
            var hitboxTransform = attackHitbox.transform;
            hitboxTransform.localRotation = Quaternion.identity;
            bool vertical = Mathf.Abs(facing.y) > Mathf.Abs(facing.x);
            hitboxTransform.localPosition = vertical ? new Vector3(0f, Mathf.Sign(facing.y) * 0.78f, 0f) : new Vector3(0.78f, 0f, 0f);
            var box = attackHitbox.GetComponent<BoxCollider2D>();
            if (box != null)
                box.size = vertical ? verticalSlashSize : slashSize;
            attackHitbox.SetProfile(ResolveAttackProfile(), Color.yellow);
        }

        public void BounceFromDownSlash()
        {
            if (body == null) return;
            body.velocity = -facing * recoilForce;
            FeedbackManager.Instance?.Play(FeedbackSound.Hit);
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
            animator.SetFloat("Speed", body.velocity.magnitude);
            animator.SetFloat("Vertical", body.velocity.y);
            animator.SetBool("Grounded", true);
            animator.SetBool("Dashing", isDashing);
        }

        private void ApplyStatMovementBonuses()
        {
            if (combatStats == null || config == null) return;
            var snapshot = combatStats.Snapshot;
            moveSpeed = Mathf.Max(1f, config.moveSpeed + snapshot.moveSpeedBonus);
            jumpForce = Mathf.Max(1f, config.jumpForce + snapshot.jumpForceBonus);
        }
    }
}
