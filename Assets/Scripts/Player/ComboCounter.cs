using HollowStyleMVP.Core;
using UnityEngine;

namespace HollowStyleMVP.Player
{
    public class ComboCounter : MonoBehaviour
    {
        [SerializeField] private float resetSeconds = 1.2f;
        private float timer;
        public int CurrentCombo { get; private set; }

        private void Update()
        {
            if (CurrentCombo <= 0) return;
            timer -= Time.deltaTime;
            if (timer <= 0f) ResetCombo();
        }

        public void AddHit()
        {
            CurrentCombo++;
            timer = resetSeconds;
            GameEvents.RaiseComboChanged(CurrentCombo);
        }

        public void ResetCombo()
        {
            CurrentCombo = 0;
            GameEvents.RaiseComboChanged(0);
        }
    }
}
