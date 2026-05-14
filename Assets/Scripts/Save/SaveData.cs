using System.Collections.Generic;
using UnityEngine;

namespace HollowStyleMVP.Save
{
    [System.Serializable]
    public class InventorySaveEntry
    {
        public string itemId;
        public int amount;
    }

    [System.Serializable]
    public class SaveData
    {
        public string sceneName = "TestRoom";
        public Vector3 playerPosition;
        public int coins;
        public int health;
        public List<string> unlockedAbilities = new List<string>();
        public List<InventorySaveEntry> inventory = new List<InventorySaveEntry>();
        public List<string> defeatedBossIds = new List<string>();
        public List<string> openedChestIds = new List<string>();
        public List<string> purchasedShopItemIds = new List<string>();
        public List<string> storyFlags = new List<string>();
        public List<string> exploredRooms = new List<string>();
        public List<string> flags = new List<string>();
    }
}
