using UnityEngine;
using UnityEngine.Events;

namespace HollowStyleMVP.Interaction
{
    public class Lever : MonoBehaviour, IInteractable
    {
        [SerializeField] private string prompt = "按 E 拉动机关";
        [SerializeField] private bool oneShot = true;
        [SerializeField] private UnityEvent onActivated;
        private bool activated;

        public string Prompt => activated && oneShot ? "机关已启动" : prompt;

        public void Interact()
        {
            if (activated && oneShot) return;
            activated = true;
            onActivated?.Invoke();
        }
    }
}
