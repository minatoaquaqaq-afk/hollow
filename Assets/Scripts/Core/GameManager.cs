using UnityEngine;
using UnityEngine.SceneManagement;
using HollowStyleMVP.Level;

namespace HollowStyleMVP.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public bool IsPaused { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Time.timeScale = 1f;
            UiModalState.Reset();
            RoomEdgeSceneTransition.EnsureExists();
        }

        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            if (UiModalState.HasOpenModal) return;
            SetPaused(!IsPaused);
        }

        public void NewGame()
        {
            SetPaused(false);
            UiModalState.Reset();
            TestSceneRoomSetup.ResetRunState();
            SceneManager.LoadScene("TestRoom");
        }

        public void ContinueGame()
        {
            NewGame();
        }

        public void SetPaused(bool paused)
        {
            IsPaused = paused;
            Time.timeScale = paused ? 0f : 1f;
            GameEvents.RaisePauseChanged(paused);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Time.timeScale = 1f;
            IsPaused = false;
            UiModalState.Reset();
            GameEvents.RaisePauseChanged(false);
        }
    }
}

