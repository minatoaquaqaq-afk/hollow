using HollowStyleMVP.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HollowStyleMVP.UI
{
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject panel;

        private void Awake()
        {
            if (panel != null) panel.SetActive(false);
        }

        private void OnEnable() => GameEvents.PauseChanged += OnPauseChanged;
        private void OnDisable() => GameEvents.PauseChanged -= OnPauseChanged;

        public void Resume() => GameManager.Instance?.SetPaused(false);
        public void BackToTitle() => SceneManager.LoadScene("MainMenu");

        private void OnPauseChanged(bool paused)
        {
            if (panel != null) panel.SetActive(paused);
        }
    }
}
