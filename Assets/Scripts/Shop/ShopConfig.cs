using UnityEngine;

namespace HollowStyleMVP.Shop
{
    [CreateAssetMenu(menuName = "Hollow Style MVP/Config/Shop Config")]
    public class ShopConfig : ScriptableObject
    {
        public string shopName = "商店";
        public ShopItem[] items;
        public bool savePurchasedState = true;
    }
}
