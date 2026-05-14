using UnityEngine;

namespace HollowStyleMVP.UI
{
    public class UiWindow : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private KeyCode closeKey = KeyCode.Escape;
        public bool IsOpen => panel != null && panel.activeSelf;

        private void Awake()
        {
            if (panel != null) panel.SetActive(false);
        }

        private void Update()
        {
            if (IsOpen && Input.GetKeyDown(closeKey)) Close();
        }

        public void Open()
        {
            if (panel != null) panel.SetActive(true);
        }

        public void Close()
        {
            if (panel != null) panel.SetActive(false);
        }

        public void Toggle()
        {
            if (IsOpen) Close(); else Open();
        }
    }
}
