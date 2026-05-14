using HollowStyleMVP.Player;
using UnityEngine;

namespace HollowStyleMVP.Level
{
    public class AbilityGate : MonoBehaviour
    {
        [SerializeField] private PlayerAbility requiredAbility = PlayerAbility.Dash;
        [SerializeField] private bool hideWhenUnlocked = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (!other.TryGetComponent<PlayerAbilityController>(out var abilities)) return;
            if (!abilities.Has(requiredAbility)) return;
            if (hideWhenUnlocked) gameObject.SetActive(false);
        }
    }
}
