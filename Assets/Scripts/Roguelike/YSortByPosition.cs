using UnityEngine;

namespace HollowStyleMVP.Roguelike
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class YSortByPosition : MonoBehaviour
    {
        [SerializeField] private int baseOrder = 100;
        [SerializeField] private int scale = 20;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void LateUpdate()
        {
            if (spriteRenderer == null) return;
            spriteRenderer.sortingOrder = baseOrder - Mathf.RoundToInt(transform.position.y * scale);
        }
    }
}
