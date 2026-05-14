using HollowStyleMVP.Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HollowStyleMVP.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public bool IsPaused { get; private set; }
        public bool ShouldApplySaveOnNextScene { get; private set; }

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
            ShouldApplySaveOnNextScene = false;
            SaveSystem.DeleteSave();
            SceneManager.LoadScene("TestRoom");
        }

        public void ContinueGame()
        {
            SetPaused(false);
            UiModalState.Reset();
            ShouldApplySaveOnNextScene = true;
            var data = SaveSystem.Load();
            SceneManager.LoadScene(string.IsNullOrWhiteSpace(data.sceneName) ? "TestRoom" : data.sceneName);
        }

        public bool ConsumeSaveApplyRequest()
        {
            bool value = ShouldApplySaveOnNextScene;
            ShouldApplySaveOnNextScene = false;
            return value;
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
