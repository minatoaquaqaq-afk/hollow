using HollowStyleMVP.Combat;
using UnityEngine;

namespace HollowStyleMVP.Visuals
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class FrameAnimator2D : MonoBehaviour
    {
        [SerializeField] private Sprite[] idleFrames;
        [SerializeField] private Sprite[] runFrames;
        [SerializeField] private Sprite[] jumpFrames;
        [SerializeField] private Sprite[] attackFrames;
        [SerializeField] private Sprite[] upSlashFrames;
        [SerializeField] private Sprite[] downSlashFrames;
        [SerializeField] private Sprite[] hurtFrames;
        [SerializeField] private Sprite[] deathFrames;
        [SerializeField] private float framesPerSecond = 8f;
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private Health health;

        private SpriteRenderer spriteRenderer;
        private Sprite[] currentFrames;
        private float timer;
        private int frameIndex;
        private float attackTimer;
        private AttackProfile attackProfile = AttackProfile.Slash;
        private bool wasDead;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (body == null) body = GetComponentInParent<Rigidbody2D>();
            if (health == null) health = GetComponentInParent<Health>();
            currentFrames = idleFrames;
            SetFrame(0);
        }

        private void Update()
        {
            if (health != null && health.IsDead)
            {
                if (!wasDead)
                {
                    wasDead = true;
                    SwitchTo(HasFrames(deathFrames) ? deathFrames : idleFrames);
                }
                TickFrames(false);
                return;
            }

            wasDead = false;
            if (attackTimer > 0f) attackTimer -= Time.deltaTime;
            SwitchTo(PickFrames());
            TickFrames(true);
        }

        public void PlayAttack(float duration = 0.18f)
        {
            PlayAttack(AttackProfile.Slash, duration);
        }

        public void PlayAttack(AttackProfile profile, float duration = 0.18f)
        {
            attackProfile = profile;
            attackTimer = duration;
            SwitchTo(PickAttackFrames(profile), true);
        }

        private Sprite[] PickFrames()
        {
            if (attackTimer > 0f) return PickAttackFrames(attackProfile);
            if (body != null && body.velocity.sqrMagnitude > 0.04f && HasFrames(runFrames)) return runFrames;
            return HasFrames(idleFrames) ? idleFrames : currentFrames;
        }

        private Sprite[] PickAttackFrames(AttackProfile profile)
        {
            if (profile == AttackProfile.UpSlash && HasFrames(upSlashFrames)) return upSlashFrames;
            if (profile == AttackProfile.DownSlash && HasFrames(downSlashFrames)) return downSlashFrames;
            return HasFrames(attackFrames) ? attackFrames : idleFrames;
        }

        private void SwitchTo(Sprite[] target, bool force = false)
        {
            if (!force && target == currentFrames) return;
            currentFrames = target;
            frameIndex = 0;
            timer = 0f;
            SetFrame(frameIndex);
        }

        private void TickFrames(bool loop)
        {
            if (!HasFrames(currentFrames)) return;
            timer += Time.deltaTime;
            float frameTime = 1f / Mathf.Max(1f, framesPerSecond);
            if (timer < frameTime) return;
            timer -= frameTime;
            if (loop) frameIndex = (frameIndex + 1) % currentFrames.Length;
            else frameIndex = Mathf.Min(frameIndex + 1, currentFrames.Length - 1);
            SetFrame(frameIndex);
        }

        private void SetFrame(int index)
        {
            if (!HasFrames(currentFrames) || spriteRenderer == null) return;
            spriteRenderer.sprite = currentFrames[Mathf.Clamp(index, 0, currentFrames.Length - 1)];
            spriteRenderer.color = Color.white;
        }

        private static bool HasFrames(Sprite[] frames) => frames != null && frames.Length > 0;
    }
}
