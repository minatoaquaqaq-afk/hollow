using HollowStyleMVP.Core;
using UnityEngine;

namespace HollowStyleMVP.Interaction
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private float interactCooldown = 0.2f;
        private IInteractable current;
        private float cooldownTimer;

        private void Update()
        {
            if (cooldownTimer > 0f) cooldownTimer -= Time.unscaledDeltaTime;
            if (!Input.GetKeyDown(KeyCode.E) || cooldownTimer > 0f) return;
            if (UiModalState.HasOpenModal) return;
            current?.Interact();
            cooldownTimer = interactCooldown;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var interactable = other.GetComponent<IInteractable>();
            if (interactable == null) return;
            current = interactable;
            GameEvents.RaiseSceneMessage(current.Prompt);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (current != null && other.GetComponent<IInteractable>() == current)
            {
                current = null;
                GameEvents.RaiseSceneMessage(string.Empty);
            }
        }
    }
}
