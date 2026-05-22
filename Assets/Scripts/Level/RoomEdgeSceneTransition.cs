using System.Text.RegularExpressions;
using HollowStyleMVP.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HollowStyleMVP.Level
{
    public class RoomEdgeSceneTransition : MonoBehaviour
    {
        private const float EdgeX = 6.05f;
        private const float SpawnOffset = 1.4f;
        private const int FirstRoom = 1;
        private const int LastRoom = 5;
        private const string RoomLabelName = "Runtime Testscene Label";

        private static string pendingSpawnPointId;

        private bool loading;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            EnsureExists();
        }

        public static void EnsureExists()
        {
            if (FindAnyObjectByType<RoomEdgeSceneTransition>() != null) return;

            var obj = new GameObject(nameof(RoomEdgeSceneTransition));
            DontDestroyOnLoad(obj);
            obj.AddComponent<RoomEdgeSceneTransition>();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Start()
        {
            var sceneName = SceneManager.GetActiveScene().name;
            if (TryGetRoomNumber(sceneName, out _)) UiModalState.Reset();
            RefreshRoomIdentity(sceneName);
            TestSceneRoomSetup.Apply(sceneName);
        }

        private void Update()
        {
            if (loading) return;

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;
            if (!TryGetRoomNumber(SceneManager.GetActiveScene().name, out int roomNumber)) return;
            if (TestSceneRoomSetup.IsCurrentRoomLocked) return;

            float x = player.transform.position.x;
            if (x >= EdgeX && roomNumber < LastRoom)
            {
                LoadRoom(roomNumber + 1, "西门");
            }
            else if (x <= -EdgeX && roomNumber > FirstRoom)
            {
                LoadRoom(roomNumber - 1, "东门");
            }
        }

        private void LoadRoom(int roomNumber, string spawnPointId)
        {
            loading = true;
            pendingSpawnPointId = spawnPointId;
            SceneManager.LoadScene(GetSceneName(roomNumber));
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            loading = false;
            if (TryGetRoomNumber(scene.name, out _)) UiModalState.Reset();
            RefreshRoomIdentity(scene.name);
            TestSceneRoomSetup.Apply(scene.name);

            if (string.IsNullOrWhiteSpace(pendingSpawnPointId)) return;

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                pendingSpawnPointId = null;
                return;
            }

            var spawnPoint = FindSceneObject(pendingSpawnPointId);
            Vector3 spawnPosition = spawnPoint != null
                ? spawnPoint.transform.position + GetInwardOffset(spawnPoint.transform.position)
                : GetFallbackSpawn(pendingSpawnPointId);

            pendingSpawnPointId = null;
            if (player.TryGetComponent<Rigidbody2D>(out var body))
            {
                body.position = spawnPosition;
                body.velocity = Vector2.zero;
                return;
            }

            player.transform.position = spawnPosition;
        }

        private static bool TryGetRoomNumber(string sceneName, out int roomNumber)
        {
            roomNumber = 0;
            if (sceneName == "TestRoom")
            {
                roomNumber = 1;
                return true;
            }

            var match = Regex.Match(sceneName, @"^TestRoom(\d+)$");
            return match.Success && int.TryParse(match.Groups[1].Value, out roomNumber);
        }

        private static string GetSceneName(int roomNumber)
        {
            return roomNumber <= 1 ? "TestRoom" : $"TestRoom{roomNumber}";
        }

        private static void RefreshRoomIdentity(string sceneName)
        {
            if (!TryGetRoomNumber(sceneName, out int roomNumber)) return;

            if (Camera.main != null)
                Camera.main.backgroundColor = GetRoomColor(roomNumber);

            var labelObject = GameObject.Find(RoomLabelName);
            if (labelObject == null)
            {
                labelObject = new GameObject(RoomLabelName);
                var text = labelObject.AddComponent<TextMesh>();
                text.anchor = TextAnchor.MiddleCenter;
                text.alignment = TextAlignment.Center;
                text.fontSize = 56;
                text.characterSize = 0.12f;
                text.color = Color.white;
                text.GetComponent<MeshRenderer>().sortingOrder = 1000;
            }

            labelObject.transform.position = new Vector3(0f, 3.25f, -4f);
            var label = labelObject.GetComponent<TextMesh>();
            if (label != null)
                label.text = $"TESTSCENE {roomNumber} / {LastRoom}\n{sceneName}";
        }

        private static Color GetRoomColor(int roomNumber)
        {
            switch (roomNumber)
            {
                case 1: return new Color(0.07f, 0.09f, 0.12f);
                case 2: return new Color(0.10f, 0.06f, 0.14f);
                case 3: return new Color(0.05f, 0.12f, 0.10f);
                case 4: return new Color(0.13f, 0.10f, 0.04f);
                case 5: return new Color(0.13f, 0.05f, 0.06f);
                default: return Color.black;
            }
        }

        private static GameObject FindSceneObject(string objectName)
        {
            foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                var found = FindChild(root.transform, objectName);
                if (found != null) return found.gameObject;
            }

            return null;
        }

        private static Transform FindChild(Transform parent, string objectName)
        {
            if (parent.name == objectName) return parent;

            foreach (Transform child in parent)
            {
                var found = FindChild(child, objectName);
                if (found != null) return found;
            }

            return null;
        }

        private static Vector3 GetInwardOffset(Vector3 doorPosition)
        {
            if (Mathf.Abs(doorPosition.x) >= Mathf.Abs(doorPosition.y))
                return new Vector3(doorPosition.x < 0f ? SpawnOffset : -SpawnOffset, 0f, 0f);

            return new Vector3(0f, doorPosition.y < 0f ? SpawnOffset : -SpawnOffset, 0f);
        }

        private static Vector3 GetFallbackSpawn(string spawnPointId)
        {
            return spawnPointId == "西门"
                ? new Vector3(-5.85f, 0f, 0f)
                : new Vector3(5.85f, 0f, 0f);
        }
    }
}
