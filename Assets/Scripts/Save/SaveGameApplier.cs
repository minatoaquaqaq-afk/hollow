using HollowStyleMVP.Combat;
using HollowStyleMVP.Core;
using HollowStyleMVP.Inventory;
using HollowStyleMVP.Player;
using UnityEngine;

namespace HollowStyleMVP.Save
{
    public class SaveGameApplier : MonoBehaviour
    {
        [SerializeField] private bool loadOnStart = true;

        private void Start()
        {
            if (loadOnStart) ApplySaveIfPresent();
        }

        public void ApplySaveIfPresent()
        {
            if (!SaveSystem.HasSave()) return;
            if (GameManager.Instance != null && !GameManager.Instance.ConsumeSaveApplyRequest()) return;

            SaveData data = SaveSystem.Load();
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = data.playerPosition;
                if (player.TryGetComponent<Health>(out var health) && data.health > 0) health.SetCurrent(data.health);
                if (data.unlockedAbilities != null && data.unlockedAbilities.Count > 0 && player.TryGetComponent<PlayerAbilityController>(out var abilities)) abilities.LoadFromSave(data.unlockedAbilities);
            }

            if (InventorySystem.Instance != null) InventorySystem.Instance.SetCoins(data.coins);
        }
    }
}


