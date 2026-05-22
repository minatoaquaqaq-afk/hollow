using HollowStyleMVP.Interaction;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HollowStyleMVP.Level
{
    [RequireComponent(typeof(Collider2D))]
    public class SceneDoor : MonoBehaviour, IInteractable
    {
        private const float SpawnOffset = 1.4f;

        [SerializeField] private string sceneName;
        [SerializeField] private string spawnPointId;

        private static string pendingSpawnPointId;
        private bool loading;
        private bool locked;

        public string Prompt => locked ? "房间锁定：清理敌人后开启" : string.IsNullOrWhiteSpace(sceneName) ? string.Empty : "按 E 进入";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeSpawnHandler()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            pendingSpawnPointId = null;
        }

        private void Awake()
        {
            var doorCollider = GetComponent<Collider2D>();
            doorCollider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            LoadTargetScene();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            LoadTargetScene();
        }

        public void Interact()
        {
            LoadTargetScene();
        }

        private void LoadTargetScene()
        {
            if (locked || TestSceneRoomSetup.IsCurrentRoomLocked) return;
            if (loading || string.IsNullOrWhiteSpace(sceneName)) return;
            loading = true;
            pendingSpawnPointId = spawnPointId;
            SceneManager.LoadScene(sceneName);
        }

        public void SetLocked(bool value)
        {
            locked = value;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (string.IsNullOrWhiteSpace(pendingSpawnPointId)) return;

            var player = GameObject.FindGameObjectWithTag("Player");
            var spawnDoor = FindSpawnDoor(pendingSpawnPointId);
            pendingSpawnPointId = null;
            if (player == null || spawnDoor == null) return;

            var spawnPosition = spawnDoor.transform.position + GetInwardOffset(spawnDoor.transform.position);
            if (player.TryGetComponent<Rigidbody2D>(out var body))
            {
                body.position = spawnPosition;
                body.velocity = Vector2.zero;
                return;
            }

            player.transform.position = spawnPosition;
        }

        private static SceneDoor FindSpawnDoor(string spawnId)
        {
            var doors = FindObjectsByType<SceneDoor>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var door in doors)
                if (door != null && door.gameObject.name == spawnId)
                    return door;

            return null;
        }

        private static Vector3 GetInwardOffset(Vector3 doorPosition)
        {
            if (Mathf.Abs(doorPosition.x) >= Mathf.Abs(doorPosition.y))
                return new Vector3(doorPosition.x < 0f ? SpawnOffset : -SpawnOffset, 0f, 0f);

            return new Vector3(0f, doorPosition.y < 0f ? SpawnOffset : -SpawnOffset, 0f);
        }
    }
}
