using System;
using System.Collections.Generic;
using HollowStyleMVP.Core;
using UnityEngine;

namespace HollowStyleMVP.Roguelike
{
    public class RunManager : MonoBehaviour
    {
        public static RunManager Instance { get; private set; }

        [SerializeField] private int seed;
        [SerializeField] private bool randomizeSeed = true;
        [SerializeField] private int targetRoomCount = 8;
        [SerializeField] private DungeonGenerator generator;

        public DungeonLayout CurrentLayout { get; private set; }
        public DungeonRoomNode CurrentRoom { get; private set; }
        public int Seed => seed;

        public event Action<DungeonLayout> RunStarted;
        public event Action<DungeonRoomNode> RoomEntered;
        public event Action<DungeonRoomNode> RoomCleared;

        private readonly HashSet<Vector2Int> clearedRooms = new HashSet<Vector2Int>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            if (generator == null) generator = GetComponent<DungeonGenerator>();
            if (generator == null) generator = gameObject.AddComponent<DungeonGenerator>();
        }

        private void Start()
        {
            if (CurrentLayout == null) StartNewRun();
        }

        public void StartNewRun()
        {
            if (randomizeSeed || seed == 0) seed = UnityEngine.Random.Range(1, int.MaxValue);
            CurrentLayout = generator.Generate(seed, targetRoomCount);
            clearedRooms.Clear();
            CurrentRoom = CurrentLayout.StartRoom;
            RunStarted?.Invoke(CurrentLayout);
            EnterRoom(CurrentRoom);
        }

        public void EnterRoom(DungeonRoomNode room)
        {
            if (room == null) return;
            CurrentRoom = room;
            room.Visited = true;
            RoomEntered?.Invoke(room);
        }

        public bool TryMove(Direction2D direction)
        {
            if (CurrentLayout == null || CurrentRoom == null) return false;
            if (!CurrentRoom.Neighbors.TryGetValue(direction, out var next)) return false;
            EnterRoom(next);
            return true;
        }

        public void MarkCurrentRoomCleared()
        {
            if (CurrentRoom == null || !clearedRooms.Add(CurrentRoom.GridPosition)) return;
            CurrentRoom.Cleared = true;
            RoomCleared?.Invoke(CurrentRoom);
            GameEvents.RaiseSceneMessage(CurrentRoom.Type == RoomType.Boss ? "Boss房清理完成" : "房间清理完成");
        }

        public bool IsCleared(DungeonRoomNode room) => room != null && clearedRooms.Contains(room.GridPosition);
    }
}
