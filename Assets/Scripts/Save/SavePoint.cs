using HollowStyleMVP.Combat;
using HollowStyleMVP.Interaction;
using HollowStyleMVP.Inventory;
using HollowStyleMVP.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HollowStyleMVP.Save
{
    public class SavePoint : MonoBehaviour, IInteractable
    {
        public string Prompt => "按 E 存档";

        public void Interact()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            var data = new SaveData
            {
                sceneName = SceneManager.GetActiveScene().name,
                playerPosition = player != null ? player.transform.position : Vector3.zero,
                coins = InventorySystem.Instance != null ? InventorySystem.Instance.Coins : 0,
                health = player != null && player.TryGetComponent<Health>(out var h) ? h.CurrentHealth : 0
            };

            if (player != null && player.TryGetComponent<PlayerAbilityController>(out var abilities))
                data.unlockedAbilities = abilities.ToSaveList();

            if (InventorySystem.Instance != null)
            {
                foreach (var pair in InventorySystem.Instance.GetSnapshot())
                    data.inventory.Add(new InventorySaveEntry { itemId = pair.Key, amount = pair.Value });
            }

            SaveSystem.Save(data);
        }
    }
}
