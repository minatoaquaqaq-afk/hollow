using UnityEngine;

namespace HollowStyleMVP.Visuals
{
    public class SlashEffectSpawner : MonoBehaviour
    {
        [SerializeField] private Sprite[] slashFrames;
        [SerializeField] private float scale = 1.35f;

        public void Spawn(Vector3 position, int facing, bool vertical, Color color)
        {
            if (slashFrames == null || slashFrames.Length == 0) return;
            var obj = new GameObject("Slash Sprite Effect");
            obj.transform.position = position;
            obj.transform.localScale = new Vector3(Mathf.Sign(facing) * scale, scale, 1f);
            if (vertical) obj.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = 80;
            var effect = obj.AddComponent<OneShotSpriteEffect>();
            effect.Configure(slashFrames, color, scale);
        }
    }
}

