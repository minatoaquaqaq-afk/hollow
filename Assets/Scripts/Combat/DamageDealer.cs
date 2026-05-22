using System.Collections.Generic;
using HollowStyleMVP.Core;
using HollowStyleMVP.Player;
using UnityEngine;

namespace HollowStyleMVP.Combat
{
    public enum AttackProfile { Slash, UpSlash, DownSlash, Contact }

    public class DamageDealer : MonoBehaviour
    {
        [SerializeField] private int damage = 1;
        [SerializeField] private float knockback = 6f;
        [SerializeField] private string targetTag = "";
        [SerializeField] private AttackProfile attackProfile = AttackProfile.Slash;
        [SerializeField] private Color hitColor = Color.yellow;
        [SerializeField] private LayerMask hitMask = ~0;

        private CombatStats attackerStats;
        private PlayerController2D playerController;
        private Collider2D hitbox;
        private readonly HashSet<Health> hitThisSwing = new HashSet<Health>();
        private readonly Collider2D[] overlapResults = new Collider2D[16];

        public void Configure(int newDamage, float newKnockback)
        {
            damage = Mathf.Max(0, newDamage);
            knockback = Mathf.Max(0f, newKnockback);
        }

        public void SetProfile(AttackProfile profile, Color color)
        {
            attackProfile = profile;
            hitColor = color;
        }

        private void Awake()
        {
            attackerStats = GetComponentInParent<CombatStats>();
            playerController = GetComponentInParent<PlayerController2D>();
            hitbox = GetComponent<Collider2D>();
            if (hitbox != null) hitbox.isTrigger = true;
        }

        private void OnEnable()
        {
            hitThisSwing.Clear();
            ScanCurrentOverlaps();
        }

        private void FixedUpdate()
        {
            ScanCurrentOverlaps();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryHit(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TryHit(other);
        }

        private void ScanCurrentOverlaps()
        {
            if (hitbox == null || !hitbox.enabled) return;
            var filter = new ContactFilter2D { useTriggers = true };
            filter.SetLayerMask(hitMask);
            int count = hitbox.Overlap(filter, overlapResults);
            for (int i = 0; i < count; i++)
            {
                TryHit(overlapResults[i]);
                overlapResults[i] = null;
            }
        }

        private void TryHit(Collider2D other)
        {
            if (other == null) return;
            if (other.transform.root == transform.root) return;
            if (!string.IsNullOrWhiteSpace(targetTag) && !other.CompareTag(targetTag)) return;
            if (!other.TryGetComponent<Health>(out var health)) return;
            if (hitThisSwing.Contains(health)) return;

            var defenderStats = other.GetComponent<CombatStats>();
            bool critical = false;
            int finalDamage = attackerStats != null ? attackerStats.CalculateDamage(damage, defenderStats, out critical) : damage;
            Vector2 sourcePosition = playerController != null ? (Vector2)playerController.transform.position : (Vector2)transform.position;
            if (!health.Damage(finalDamage, sourcePosition, knockback, critical)) return;

            hitThisSwing.Add(health);
            HitEffect.Spawn(other.ClosestPoint(transform.position), critical ? Color.magenta : hitColor);
            FeedbackManager.Instance?.Play(critical ? FeedbackSound.Crit : FeedbackSound.Hit);
            GetComponentInParent<ComboCounter>()?.AddHit();
            if (attackProfile == AttackProfile.DownSlash) playerController?.BounceFromDownSlash();
        }
    }
}
