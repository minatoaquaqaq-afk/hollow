using UnityEngine;

namespace HollowStyleMVP.Inventory
{
    public enum ItemType { Currency, Consumable, Material, Quest, Ability }

    [CreateAssetMenu(menuName = "Hollow Style MVP/Inventory Item")]
    public class InventoryItem : ScriptableObject
    {
        public string id;
        public string displayName;
        public ItemType type;
        public Sprite icon;
        public int price;
        [TextArea] public string description;
    }
}
