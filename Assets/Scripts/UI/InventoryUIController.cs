using HollowStyleMVP.Core;
using HollowStyleMVP.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace HollowStyleMVP.UI
{
    public class InventoryUIController : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Transform contentRoot;
        [SerializeField] private Text rowPrefab;
        [SerializeField] private Text coinsText;
        private bool modalRegistered;

        private void Awake()
        {
            if (panel != null) panel.SetActive(false);
        }

        private void OnEnable()
        {
            if (InventorySystem.Instance != null) InventorySystem.Instance.InventoryChanged += Rebuild;
        }

        private void OnDisable()
        {
            if (InventorySystem.Instance != null) InventorySystem.Instance.InventoryChanged -= Rebuild;
            UnregisterModal();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I)) Toggle();
            if (panel != null && panel.activeSelf && Input.GetKeyDown(KeyCode.Escape)) Close();
        }

        public void Toggle()
        {
            if (panel == null) return;
            if (panel.activeSelf) Close();
            else Open();
        }

        private void Open()
        {
            RegisterModal();
            panel.SetActive(true);
            Rebuild();
            Time.timeScale = 1f;
        }

        public void Close()
        {
            if (panel != null) panel.SetActive(false);
            UnregisterModal();
            Time.timeScale = 1f;
        }

        private void Rebuild()
        {
            if (InventorySystem.Instance == null) return;
            if (coinsText != null) coinsText.text = $"Geo {InventorySystem.Instance.Coins}";
            if (contentRoot == null || rowPrefab == null) return;

            foreach (Transform child in contentRoot)
            {
                if (child == rowPrefab.transform) continue;
                Destroy(child.gameObject);
            }

            bool hasItem = false;
            foreach (var pair in InventorySystem.Instance.GetSnapshot())
            {
                hasItem = true;
                var row = Instantiate(rowPrefab, contentRoot);
                row.gameObject.SetActive(true);
                string displayName = pair.Key;
                if (InventorySystem.Instance.TryGetItemDefinition(pair.Key, out var item) && item != null)
                    displayName = item.displayName;
                row.text = $"{displayName} x{pair.Value}";
            }

            if (!hasItem)
            {
                var row = Instantiate(rowPrefab, contentRoot);
                row.gameObject.SetActive(true);
                row.text = "暂无物品，去商店购买测试药水";
            }
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
