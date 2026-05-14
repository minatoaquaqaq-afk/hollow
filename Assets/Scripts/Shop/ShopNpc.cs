using HollowStyleMVP.Interaction;
using UnityEngine;

namespace HollowStyleMVP.Shop
{
    public class ShopNpc : MonoBehaviour, IInteractable
    {
        [SerializeField] private string shopName = "商店";
        [SerializeField] private ShopItem[] items;
        public string Prompt => "按 E 打开商店";
        public void Interact() => ShopController.Instance?.Open(shopName, items);
    }
}
