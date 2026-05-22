using System.Collections.Generic;
using HollowStyleMVP.Combat;
using HollowStyleMVP.Core;
using UnityEngine;

namespace HollowStyleMVP.Roguelike
{
    public class RoomController : MonoBehaviour
    {
        [SerializeField] private RoomType fallbackRoomType = RoomType.Combat;
        [SerializeField] private DoorController[] doors;
        [SerializeField] private Transform[] enemySpawnPoints;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private GameObject bossPrefab;
        [SerializeField] private RewardSpawner rewardSpawner;
        [SerializeField] private bool spawnOnEnter = true;

        private readonly List<Health> aliveEnemies = new List<Health>();
        private DungeonRoomNode roomNode;
        private bool activated;
        private bool cleared;

        private void Awake()
        {
            if (doors == null || doors.Length == 0) doors = GetComponentsInChildren<DoorController>(true);
            if (rewardSpawner == null) rewardSpawner = GetComponentInChildren<RewardSpawner>(true);
        }

        private void OnEnable()
        {
            if (RunManager.Instance != null) RunManager.Instance.RoomEntered += OnRoomEntered;
        }

        private void OnDisable()
        {
            if (RunManager.Instance != null) RunManager.Instance.RoomEntered -= OnRoomEntered;
        }

        public void Bind(DungeonRoomNode node)
        {
            roomNode = node;
            ConfigureDoors();
        }

        public void ActivateAsCurrentRoom()
        {
            if (activated) return;
            activated = true;
            if (roomNode == null && RunManager.Instance != null) roomNode = RunManager.Instance.CurrentRoom;
            ConfigureDoors();

            RoomType type = roomNode != null ? roomNode.Type : fallbackRoomType;
            if (type == RoomType.Start || type == RoomType.Shop || type == RoomType.Treasure)
            {
                ClearRoom();
                return;
            }

            LockDoors(true);
            if (spawnOnEnter) SpawnEncounter(type);
            if (aliveEnemies.Count == 0) ClearRoom();
            GameEvents.RaiseSceneMessage(type == RoomType.Boss ? "Boss房：清理敌人开启出口" : "战斗房：清理敌人开启出口");
        }

        private void OnRoomEntered(DungeonRoomNode node)
        {
            if (roomNode == null || node != roomNode) return;
            ActivateAsCurrentRoom();
        }

        private void SpawnEncounter(RoomType type)
        {
            aliveEnemies.Clear();
            var prefab = type == RoomType.Boss && bossPrefab != null ? bossPrefab : enemyPrefab;
            if (prefab == null) return;
            if (enemySpawnPoints == null || enemySpawnPoints.Length == 0)
                RegisterEnemy(Instantiate(prefab, transform.position, Quaternion.identity, transform));

            foreach (var spawnPoint in enemySpawnPoints)
            {
                if (spawnPoint == null) continue;
                if (type == RoomType.Boss && aliveEnemies.Count > 0) break;
                RegisterEnemy(Instantiate(prefab, spawnPoint.position, Quaternion.identity, transform));
            }
        }

        private void RegisterEnemy(GameObject enemy)
        {
            if (enemy == null || !enemy.TryGetComponent<Health>(out var health)) return;
            aliveEnemies.Add(health);
            health.Died += () => OnEnemyDied(health);
        }

        private void OnEnemyDied(Health health)
        {
            aliveEnemies.Remove(health);
            if (aliveEnemies.Count == 0) ClearRoom();
        }

        private void ClearRoom()
        {
            if (cleared) return;
            cleared = true;
            LockDoors(false);
            if (roomNode != null) RunManager.Instance?.MarkCurrentRoomCleared();
            rewardSpawner?.SpawnReward(roomNode != null ? roomNode.Type : fallbackRoomType);
        }

        private void ConfigureDoors()
        {
            if (doors == null) return;
            foreach (var door in doors)
            {
                if (door == null) continue;
                bool connected = roomNode == null || roomNode.Neighbors.ContainsKey(door.Direction);
                door.SetAvailable(connected);
            }
        }

        private void LockDoors(bool locked)
        {
            if (doors == null) return;
            foreach (var door in doors)
                if (door != null) door.SetLocked(locked);
        }
    }
}
