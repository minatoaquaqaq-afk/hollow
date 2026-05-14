using HollowStyleMVP.Core;
using UnityEngine;
using UnityEngine.UI;

namespace HollowStyleMVP.UI
{
    public class DeathUIController : MonoBehaviour
    {
        public static DeathUIController Instance { get; private set; }
        [SerializeField] private GameObject panel;
        [SerializeField] private Text messageText;
        private bool modalRegistered;

        private void Awake()
        {
            Instance = this;
            if (panel != null) panel.SetActive(false);
        }

        public void Show(float respawnDelay)
        {
            if (panel == null) return;
            RegisterModal();
            panel.SetActive(true);
            if (messageText != null) messageText.text = $"死亡\n{respawnDelay:0.0}s 后复活";
            FeedbackManager.Instance?.Play(FeedbackSound.Death);
        }

        public void Hide()
        {
            if (panel != null) panel.SetActive(false);
            UnregisterModal();
        }

        private void RegisterModal()
        {
            if (modalRegistered) return;
            modalRegistered = true;
            UiModalState.Open();
        }

        private void UnregisterModal()
        {
            if (!modalRegistered) return;
            modalRegistered = false;
            UiModalState.Close();
        }
    }
}
