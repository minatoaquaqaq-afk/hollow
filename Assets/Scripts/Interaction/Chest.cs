using HollowStyleMVP.Core;
using HollowStyleMVP.Inventory;
using UnityEngine;

namespace HollowStyleMVP.Interaction
{
    public class Chest : MonoBehaviour, IInteractable
    {
        [SerializeField] private int coins = 5;
        [SerializeField] private InventoryItem item;
        [SerializeField] private int amount = 1;
        [SerializeField] private bool opened;

        public string Prompt => opened ? "宝箱已打开" : "按 E 打开宝箱";

        public void Interact()
        {
            if (opened || InventorySystem.Instance == null) return;
            opened = true;
            if (coins > 0) InventorySystem.Instance.AddCoins(coins);
            if (item != null) InventorySystem.Instance.AddItem(item, amount);
            FeedbackManager.Instance?.Play(FeedbackSound.Open);
        }
    }
}
