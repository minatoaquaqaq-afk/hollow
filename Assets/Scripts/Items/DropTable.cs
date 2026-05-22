using HollowStyleMVP.Inventory;
using UnityEngine;

namespace HollowStyleMVP.Items
{
    public enum DropKind { Coins, Heal, Item, Material }

    [System.Serializable]
    public class DropEntry
    {
        public DropKind kind = DropKind.Coins;
        public GameObject prefab;
        public InventoryItem item;
        [Range(0f, 1f)] public float chance = 1f;
        public int minAmount = 1;
        public int maxAmount = 1;
        public Color color = Color.yellow;
    }

    [CreateAssetMenu(menuName = "Hollow Style MVP/Config/Drop Table")]
    public class DropTable : ScriptableObject
    {
        public DropEntry[] drops;

        public void Spawn(Vector3 position)
        {
            if (drops == null) return;
            foreach (var drop in drops)
            {
                if (drop == null || Random.value > drop.chance) continue;
                int amount = Random.Range(Mathf.Min(drop.minAmount, drop.maxAmount), Mathf.Max(drop.minAmount, drop.maxAmount) + 1);
                Vector3 offset = new Vector3(Random.Range(-0.45f, 0.45f), Random.Range(0.15f, 0.55f), 0f);
                SpawnOne(drop, Mathf.Max(1, amount), position + offset);
            }
        }

        private static void SpawnOne(DropEntry drop, int amount, Vector3 position)
        {
            GameObject obj = drop.prefab != null ? Instantiate(drop.prefab, position, Quaternion.identity) : CreateRuntimePickup(drop, position);
            if (obj.TryGetComponent<PickupItem>(out var pickup))
            {
                int coins = drop.kind == DropKind.Coins ? amount : 0;
                int heal = drop.kind == DropKind.Heal ? amount : 0;
                InventoryItem item = drop.kind == DropKind.Item || drop.kind == DropKind.Material ? drop.item : null;
                pickup.Configure(coins, item, amount, heal);
            }
        }

        private static GameObject CreateRuntimePickup(DropEntry drop, Vector3 position)
        {
            var obj = new GameObject("Drop - " + drop.kind);
            obj.transform.position = position;
            var sprite = obj.AddComponent<SpriteRenderer>();
            sprite.sprite = MakeSprite();
            sprite.color = drop.kind == DropKind.Heal ? Color.green : drop.color;
            obj.transform.localScale = Vector3.one * 0.35f;
            var collider = obj.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            obj.AddComponent<PickupItem>();
            return obj;
        }

        private static Sprite MakeSprite()
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        }
    }
}
