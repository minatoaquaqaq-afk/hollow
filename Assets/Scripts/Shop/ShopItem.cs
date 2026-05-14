using HollowStyleMVP.Inventory;

namespace HollowStyleMVP.Shop
{
    [System.Serializable]
    public class ShopItem
    {
        public InventoryItem item;
        public int price = 5;
        public int stock = 1;
    }
}
