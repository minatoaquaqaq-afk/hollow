using System.Collections.Generic;
using UnityEngine;

namespace HollowStyleMVP.Roguelike
{
    public class RuntimeDungeonBinder : MonoBehaviour
    {
        [SerializeField] private RoomController roomPrefabRoot;
        [SerializeField] private RoomController[] existingRooms;

        private void Start()
        {
            if (RunManager.Instance == null) return;
            RunManager.Instance.RunStarted += BindExistingRooms;
            if (RunManager.Instance.CurrentLayout == null) RunManager.Instance.StartNewRun();
            BindExistingRooms(RunManager.Instance.CurrentLayout);
        }

        private void OnDestroy()
        {
            if (RunManager.Instance != null) RunManager.Instance.RunStarted -= BindExistingRooms;
        }

        private void BindExistingRooms(DungeonLayout layout)
        {
            if (layout == null) return;
            if (existingRooms == null || existingRooms.Length == 0)
                existingRooms = FindObjectsOfType<RoomController>(true);

            var rooms = new List<DungeonRoomNode>(layout.Rooms.Values);
            for (int i = 0; i < existingRooms.Length && i < rooms.Count; i++)
                existingRooms[i].Bind(rooms[i]);

            if (existingRooms.Length > 0) existingRooms[0].ActivateAsCurrentRoom();
        }
    }
}
