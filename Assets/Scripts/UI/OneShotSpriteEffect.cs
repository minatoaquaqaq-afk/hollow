using UnityEngine;

namespace HollowStyleMVP.Visuals
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class OneShotSpriteEffect : MonoBehaviour
    {
        [SerializeField] private Sprite[] frames;
        [SerializeField] private float framesPerSecond = 18f;
        [SerializeField] private bool destroyWhenDone = true;
        private SpriteRenderer spriteRenderer;
        private float timer;
        private int frameIndex;

        public void Configure(Sprite[] newFrames, Color color, float scale)
        {
            frames = newFrames;
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = color;
            transform.localScale = Vector3.one * scale;
            SetFrame(0);
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            SetFrame(0);
        }

        private void Update()
        {
            if (frames == null || frames.Length == 0)
            {
                if (destroyWhenDone) Destroy(gameObject);
                return;
            }
            timer += Time.deltaTime;
            float frameTime = 1f / Mathf.Max(1f, framesPerSecond);
            if (timer < frameTime) return;
            timer -= frameTime;
            frameIndex++;
            if (frameIndex >= frames.Length)
            {
                if (destroyWhenDone) Destroy(gameObject);
                else frameIndex = 0;
                return;
            }
            SetFrame(frameIndex);
        }

        private void SetFrame(int index)
        {
            if (spriteRenderer == null || frames == null || frames.Length == 0) return;
            spriteRenderer.sprite = frames[Mathf.Clamp(index, 0, frames.Length - 1)];
        }
    }
}
