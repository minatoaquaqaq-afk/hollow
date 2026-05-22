using HollowStyleMVP.Combat;
using HollowStyleMVP.Player;
using HollowStyleMVP.Roguelike;
using UnityEngine;

namespace HollowStyleMVP.Inventory
{
    public enum ItemType { Currency, Consumable, Material, Quest, Equipment, Charm, Ability }
    public enum EquipmentSlot { None, Weapon, Armor, Accessory }

    [CreateAssetMenu(menuName = "Hollow Style MVP/Inventory Item")]
    public class InventoryItem : ScriptableObject
    {
        [Header("Basic")]
        public string id;
        public string displayName;
        public ItemType type;
        public Sprite icon;
        public int price;
        [TextArea] public string description;

        [Header("Use / Pickup")]
        public int healAmount;
        public PlayerAbility abilityToUnlock;
        public bool unlockAbilityOnPickup;
        public bool autoEquipForTesting;

        [Header("Equipment / Charm")]
        public EquipmentSlot equipmentSlot = EquipmentSlot.None;
        [Range(1, 5)] public int charmCost = 1;
        public StatModifier statModifier;

        [Header("Projectile Modifier")]
        public ProjectileModifier projectileModifier;
    }
}
