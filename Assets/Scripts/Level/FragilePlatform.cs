using System.Collections;
using UnityEngine;

namespace HollowStyleMVP.Level
{
    public class FragilePlatform : MonoBehaviour
    {
        [SerializeField] private float breakDelay = 0.5f;
        [SerializeField] private float restoreDelay = 2f;
        private Collider2D col;
        private SpriteRenderer sprite;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
            sprite = GetComponent<SpriteRenderer>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Player")) StartCoroutine(BreakRoutine());
        }

        private IEnumerator BreakRoutine()
        {
            yield return new WaitForSeconds(breakDelay);
            col.enabled = false;
            if (sprite != null) sprite.enabled = false;
            yield return new WaitForSeconds(restoreDelay);
            col.enabled = true;
            if (sprite != null) sprite.enabled = true;
        }
    }
}
