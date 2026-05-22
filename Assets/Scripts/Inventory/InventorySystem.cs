using System;
using System.Collections.Generic;
using HollowStyleMVP.Core;
using HollowStyleMVP.Player;
using HollowStyleMVP.Roguelike;
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
            if (amount > 0) FeedbackManager.Instance?.Play(FeedbackSound.Pickup);
        }

        public bool SpendCoins(int amount)
        {
            if (Coins < amount) return false;
            Coins -= amount;
            GameEvents.RaiseCoinsChanged(Coins);
            InventoryChanged?.Invoke();
            FeedbackManager.Instance?.Play(FeedbackSound.Buy);
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
            ApplyImmediateItemEffects(item);
            InventoryChanged?.Invoke();
            FeedbackManager.Instance?.Play(FeedbackSound.Pickup);
        }

        public IReadOnlyDictionary<string, int> GetSnapshot() => items;

        public bool TryGetItemDefinition(string id, out InventoryItem item) => itemLookup.TryGetValue(id, out item);

        public IEnumerable<(InventoryItem item, int amount)> GetItemsByType(ItemType type)
        {
            foreach (var pair in items)
            {
                if (itemLookup.TryGetValue(pair.Key, out var item) && item != null && item.type == type)
                    yield return (item, pair.Value);
            }
        }

        private void ApplyImmediateItemEffects(InventoryItem item)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            if (item.healAmount > 0 && player.TryGetComponent<HollowStyleMVP.Combat.Health>(out var health))
                health.Heal(item.healAmount);

            if (item.unlockAbilityOnPickup && player.TryGetComponent<PlayerAbilityController>(out var abilityController))
                abilityController.Unlock(item.abilityToUnlock);

            if (item.projectileModifier.HasAnyValue && player.TryGetComponent<PlayerProjectileModifiers>(out var projectileModifiers))
                projectileModifiers.AddModifier(item.projectileModifier);

            if (!item.autoEquipForTesting) return;
            var equipment = EquipmentSystem.Instance != null ? EquipmentSystem.Instance : player.GetComponent<EquipmentSystem>();
            if (equipment == null) return;
            if (item.type == ItemType.Equipment) equipment.TryEquip(item);
            if (item.type == ItemType.Charm) equipment.TryEquipCharm(item);
        }
    }
}
