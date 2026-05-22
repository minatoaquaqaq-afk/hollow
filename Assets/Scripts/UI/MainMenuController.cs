using HollowStyleMVP.Core;
using UnityEngine;
using UnityEngine.UI;

namespace HollowStyleMVP.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button continueButton;
        private void Start()
        {
            if (continueButton != null) continueButton.interactable = true;
        }
        public void NewGame() => GameManager.Instance.NewGame();
        public void ContinueGame() => GameManager.Instance.ContinueGame();
        public void Quit() => Application.Quit();
    }
}

