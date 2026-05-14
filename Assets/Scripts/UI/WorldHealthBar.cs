using HollowStyleMVP.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace HollowStyleMVP.UI
{
    public class WorldHealthBar : MonoBehaviour
    {
        [SerializeField] private Health target;
        [SerializeField] private Slider slider;

        private void Awake()
        {
            if (target == null) target = GetComponentInParent<Health>();
            if (target != null) target.Changed += UpdateBar;
        }

        private void OnDestroy()
        {
            if (target != null) target.Changed -= UpdateBar;
        }

        private void UpdateBar(int current, int max)
        {
            if (slider == null) return;
            slider.maxValue = max;
            slider.value = current;
        }
    }
}
