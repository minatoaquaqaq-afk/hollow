using System;
using System.Collections.Generic;
using HollowStyleMVP.Core;
using UnityEngine;

namespace HollowStyleMVP.Inventory
{
    public class InventorySystem : MonoBehaviour
    {
        public static InventorySystem Instance { get; private set; }

        private readonly Dictionary<string, int> items = new Dictionary<string, int>();
        private readonly Dictionary<string, InventoryItem> itemLookup = new Dictionary<string, InventoryItem>();

        public int Coins { get; private set; }
        public event Action InventoryChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void AddCoins(int amount)
        {
            Coins = Mathf.Max(0, Coins + amount);
            GameEvents.RaiseCoinsChanged(Coins);
            InventoryChanged?.Invoke();
        }

        public bool SpendCoins(int amount)
        {
            if (Coins < amount) return false;
            Coins -= amount;
            GameEvents.RaiseCoinsChanged(Coins);
            InventoryChanged?.Invoke();
            return true;
        }

        public void SetCoins(int coins)
        {
            Coins = Mathf.Max(0, coins);
            GameEvents.RaiseCoinsChanged(Coins);
            InventoryChanged?.Invoke();
        }

        public void AddItem(InventoryItem item, int amount)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.id) || amount <= 0) return;
            itemLookup[item.id] = item;
            items.TryGetValue(item.id, out int current);
            items[item.id] = current + amount;
            InventoryChanged?.Invoke();
        }

        public IReadOnlyDictionary<string, int> GetSnapshot() => items;

        public bool TryGetItemDefinition(string id, out InventoryItem item) => itemLookup.TryGetValue(id, out item);
    }
}
