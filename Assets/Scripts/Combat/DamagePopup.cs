using UnityEngine;

namespace HollowStyleMVP.Combat
{
    public class DamagePopup : MonoBehaviour
    {
        [SerializeField] private float lifetime = 0.75f;
        [SerializeField] private float riseSpeed = 1.5f;
        private TextMesh textMesh;
        private float timer;

        public static void Spawn(Vector3 position, int amount)
        {
            var obj = new GameObject("Damage Popup");
            obj.transform.position = position + Vector3.up * 0.7f;
            var text = obj.AddComponent<TextMesh>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 48;
            text.characterSize = 0.12f;
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.color = Color.red;
            text.text = "-" + amount;
            var renderer = obj.GetComponent<MeshRenderer>();
            renderer.sortingOrder = 100;
            renderer.sharedMaterial = text.font.material;
            obj.AddComponent<DamagePopup>().textMesh = text;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            transform.position += Vector3.up * (riseSpeed * Time.deltaTime);
            if (textMesh != null)
            {
                Color c = textMesh.color;
                c.a = Mathf.Clamp01(1f - timer / lifetime);
                textMesh.color = c;
            }
            if (timer >= lifetime) Destroy(gameObject);
        }
    }
}
