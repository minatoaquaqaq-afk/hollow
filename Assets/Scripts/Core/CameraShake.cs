using System.Collections;
using UnityEngine;

namespace HollowStyleMVP.Core
{
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake Instance { get; private set; }
        private Vector3 baseLocalPosition;
        private Coroutine routine;

        private void Awake()
        {
            Instance = this;
            baseLocalPosition = transform.localPosition;
        }

        public void Shake(float duration = 0.12f, float strength = 0.16f)
        {
            if (routine != null) StopCoroutine(routine);
            routine = StartCoroutine(ShakeRoutine(duration, strength));
        }

        private IEnumerator ShakeRoutine(float duration, float strength)
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                Vector2 offset = Random.insideUnitCircle * strength;
                transform.localPosition = baseLocalPosition + new Vector3(offset.x, offset.y, 0f);
                yield return null;
            }
            transform.localPosition = baseLocalPosition;
            routine = null;
        }
    }
}
