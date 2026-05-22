using HollowStyleMVP.Core;
using UnityEngine;

namespace HollowStyleMVP.Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 5;
        [SerializeField] private float invulnerableSeconds = 0.35f;
        [SerializeField] private SpriteRenderer flashRenderer;

        private float invulnerableTimer;
        private Color originalColor = Color.white;

        public int CurrentHealth { get; private set; }
        public int MaxHealth => maxHealth;
        public bool IsDead => CurrentHealth <= 0;

        public event System.Action<int, int> Changed;
        public event System.Action Died;
        public event System.Action Damaged;

        private void Awake()
        {
            CurrentHealth = maxHealth;
            if (flashRenderer == null) flashRenderer = GetComponentInChildren<SpriteRenderer>();
            if (flashRenderer != null) originalColor = flashRenderer.color;
        }

        private void Update()
        {
            if (invulnerableTimer <= 0f) return;
            invulnerableTimer -= Time.deltaTime;
            if (invulnerableTimer <= 0f && flashRenderer != null) flashRenderer.color = originalColor;
        }

        public void Configure(int newMaxHealth, float newInvulnerableSeconds, bool refill = true)
        {
            maxHealth = Mathf.Max(1, newMaxHealth);
            invulnerableSeconds = Mathf.Max(0f, newInvulnerableSeconds);
            if (refill) CurrentHealth = maxHealth;
            else CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);
            Changed?.Invoke(CurrentHealth, maxHealth);
        }

        public bool Damage(int amount, Vector2 sourcePosition, float knockback) => Damage(amount, sourcePosition, knockback, false);

        public bool Damage(int amount, Vector2 sourcePosition, float knockback, bool critical)
        {
            if (IsDead || invulnerableTimer > 0f || amount <= 0) return false;

            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            DamagePopup.Spawn(transform.position, amount, critical);
            HitEffect.Spawn(transform.position, critical ? Color.magenta : Color.red);
            FeedbackManager.Instance?.Play(critical ? FeedbackSound.Crit : FeedbackSound.Hit);
            FeedbackManager.Instance?.HitStop();
            CameraShake.Instance?.Shake(critical ? 0.18f : 0.1f, critical ? 0.22f : 0.14f);
            Changed?.Invoke(CurrentHealth, maxHealth);
            Damaged?.Invoke();
            invulnerableTimer = invulnerableSeconds;
            if (flashRenderer != null) flashRenderer.color = critical ? Color.magenta : Color.red;

            if (TryGetComponent<Rigidbody2D>(out var body))
            {
                Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;
                if (direction.sqrMagnitude <= 0.001f) direction = Vector2.right;
                body.velocity *= 0.25f;
                body.AddForce(direction * knockback, ForceMode2D.Impulse);
            }

            if (CurrentHealth <= 0)
            {
                FeedbackManager.Instance?.Play(FeedbackSound.Death);
                Died?.Invoke();
            }

            return true;
        }

        public void SetCurrent(int value)
        {
            CurrentHealth = Mathf.Clamp(value, 0, maxHealth);
            Changed?.Invoke(CurrentHealth, maxHealth);
            if (CurrentHealth <= 0)
            {
                FeedbackManager.Instance?.Play(FeedbackSound.Death);
                Died?.Invoke();
            }
        }

        public void Heal(int amount)
        {
            if (amount <= 0 || IsDead) return;
            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
            Changed?.Invoke(CurrentHealth, maxHealth);
            HitEffect.Spawn(transform.position, Color.green);
        }
    }
}
