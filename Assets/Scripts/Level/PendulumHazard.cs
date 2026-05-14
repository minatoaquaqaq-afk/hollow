using UnityEngine;

namespace HollowStyleMVP.Level
{
    public class PendulumHazard : MonoBehaviour
    {
        [SerializeField] private float angle = 55f;
        [SerializeField] private float speed = 2f;
        private void Update() => transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(Time.time * speed) * angle);
    }
}
