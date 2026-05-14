using UnityEngine;

namespace HollowStyleMVP.Level
{
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private Transform a;
        [SerializeField] private Transform b;
        [SerializeField] private float speed = 2f;
        private bool toB = true;
        private void Update()
        {
            if (a == null || b == null) return;
            Transform target = toB ? b : a;
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, target.position) < 0.03f) toB = !toB;
        }
    }
}
