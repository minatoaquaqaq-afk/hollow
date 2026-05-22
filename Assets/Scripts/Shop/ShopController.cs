using HollowStyleMVP.Core;
using HollowStyleMVP.Inventory;
using HollowStyleMVP.UI;
using UnityEngine;
using UnityEngine.UI;

namespace HollowStyleMVP.Shop
{
    public class ShopController : MonoBehaviour
    {
        public static ShopController Instance { get; private set; }
        [SerializeField] private GameObject panel;
        [SerializeField] private Text titleText;
        [SerializeField] private Transform listRoot;
        [SerializeField] private Button rowPrefab;
        private ShopItem[] currentItems;
        private bool modalRegistered;

        private void Awake()
        {
            Instance = this;
            RoguelikeRoomHud.StyleShopPanel(panel);
            if (panel != null) panel.SetActive(false);
        }

        private void OnDisable() => UnregisterModal();

        private void Update()
        {
            if (panel != null && panel.activeSelf && Input.GetKeyDown(KeyCode.Escape)) Close();
        }

        public void Open(string title, ShopItem[] items)
        {
            currentItems = items;
            if (panel == null) return;
            RegisterModal();
            panel.SetActive(true);
            Time.timeScale = 1f;
            if (titleText != null) titleText.text = title;
            RoguelikeRoomHud.StyleShopPanel(panel);
            FeedbackManager.Instance?.Play(FeedbackSound.Open);
            RebuildRows();
        }

        private void RebuildRows()
        {
            if (listRoot == null || rowPrefab == null || currentItems == null) return;
            foreach (Transform child in listRoot)
            {
                if (child == rowPrefab.transform) continue;
                Destroy(child.gameObject);
            }

            for (int i = 0; i < currentItems.Length; i++)
            {
                int index = i;
                var row = Instantiate(rowPrefab, listRoot);
                row.gameObject.SetActive(true);
                row.interactable = currentItems[i].stock != 0;
                var label = row.GetComponentInChildren<Text>();
                string name = currentItems[i].item != null ? currentItems[i].item.displayName : "商品";
                string type = currentItems[i].item != null ? TypeName(currentItems[i].item.type) : "";
                string stock = currentItems[i].stock < 0 ? "∞" : currentItems[i].stock.ToString();
                if (label != null) label.text = $"[{type}] {name} - {currentItems[i].price}G x{stock}";
                RoguelikeRoomHud.StyleShopRow(row);
                row.onClick.AddListener(() => Buy(index));
            }
        }

        private void Buy(int index)
        {
            if (InventorySystem.Instance == null || currentItems[index].stock == 0) return;
            if (!InventorySystem.Instance.SpendCoins(currentItems[index].price)) return;
            if (currentItems[index].item != null) InventorySystem.Instance.AddItem(currentItems[index].item, 1);
            if (currentItems[index].stock > 0) currentItems[index].stock--;
            RebuildRows();
        }

        private void Close()
        {
            if (panel != null) panel.SetActive(false);
            UnregisterModal();
            Time.timeScale = 1f;
            FeedbackManager.Instance?.Play(FeedbackSound.UiCancel);
        }

        private static string TypeName(ItemType type)
        {
            return type switch
            {
                ItemType.Consumable => "消耗品",
                ItemType.Material => "材料",
                ItemType.Quest => "任务",
                ItemType.Equipment => "装备",
                ItemType.Charm => "护符",
                ItemType.Ability => "技能",
                _ => "物品"
            };
        }

        private void RegisterModal()
        {
            if (modalRegistered) return;
            modalRegistered = true;
            UiModalState.Open();
        }

        private void UnregisterModal()
        {
            if (!modalRegistered) return;
            modalRegistered = false;
            UiModalState.Close();
        }
    }
}
