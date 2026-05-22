using HollowStyleMVP.Inventory;
using HollowStyleMVP.Items;
using UnityEngine;

namespace HollowStyleMVP.Roguelike
{
    public class RewardSpawner : MonoBehaviour
    {
        [SerializeField] private InventoryItem treasureItem;
        [SerializeField] private InventoryItem bossItem;
        [SerializeField] private int combatRoomCoins = 3;
        [SerializeField] private int bossCoins = 12;
        [SerializeField] private Color coinColor = Color.yellow;
        [SerializeField] private Color treasureColor = Color.cyan;

        public void SpawnReward(RoomType roomType)
        {
            switch (roomType)
            {
                case RoomType.Combat:
                    SpawnPickup(combatRoomCoins, null, coinColor);
                    break;
                case RoomType.Treasure:
                    SpawnPickup(0, treasureItem, treasureColor);
                    break;
                case RoomType.Boss:
                    SpawnPickup(bossCoins, bossItem, Color.magenta);
                    break;
            }
        }

        private void SpawnPickup(int coins, InventoryItem item, Color color)
        {
            if (coins <= 0 && item == null) return;
            var obj = new GameObject("Room Reward");
            obj.transform.position = transform.position;
            var sprite = obj.AddComponent<SpriteRenderer>();
            sprite.sprite = MakeSprite();
            sprite.color = color;
            obj.transform.localScale = Vector3.one * 0.45f;
            var collider = obj.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            obj.AddComponent<PickupItem>().Configure(coins, item, 1);
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
