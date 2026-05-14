using HollowStyleMVP.Inventory;
using UnityEngine;

namespace HollowStyleMVP.Items
{
    public class PickupItem : MonoBehaviour
    {
        [SerializeField] private int coins = 1;
        [SerializeField] private InventoryItem item;
        [SerializeField] private int amount = 1;
        [SerializeField] private float magnetRange = 2.8f;
        [SerializeField] private float magnetSpeed = 7f;
        private Transform player;

        private void Start()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        private void Update()
        {
            if (player == null) return;
            if (Vector2.Distance(transform.position, player.position) <= magnetRange)
                transform.position = Vector2.MoveTowards(transform.position, player.position, magnetSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player") || InventorySystem.Instance == null) return;
            if (coins > 0) InventorySystem.Instance.AddCoins(coins);
            if (item != null) InventorySystem.Instance.AddItem(item, amount);
            Destroy(gameObject);
        }
    }
}
