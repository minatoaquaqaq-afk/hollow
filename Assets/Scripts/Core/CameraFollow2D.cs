using UnityEngine;

namespace HollowStyleMVP.Core
{
    public class CameraFollow2D : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector2 roomMin = new Vector2(-8f, -4.4f);
        [SerializeField] private Vector2 roomMax = new Vector2(8f, 4.4f);
        [SerializeField] private Vector2 lookAhead = new Vector2(0f, 0.35f);
        [SerializeField] private float smoothTime = 0.16f;

        private Camera followCamera;
        private Vector3 velocity;

        private void Awake()
        {
            followCamera = GetComponentInChildren<Camera>();
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) target = player.transform;
            }
            if (target == null) return;

            var desired = target.position + new Vector3(lookAhead.x, lookAhead.y, 0f);
            desired.z = transform.position.z;
            desired = ClampToRoom(desired);
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
        }

        private Vector3 ClampToRoom(Vector3 desired)
        {
            if (followCamera == null) return desired;

            float halfHeight = followCamera.orthographicSize;
            float halfWidth = halfHeight * followCamera.aspect;
            float minX = roomMin.x + halfWidth;
            float maxX = roomMax.x - halfWidth;
            float minY = roomMin.y + halfHeight;
            float maxY = roomMax.y - halfHeight;

            desired.x = minX <= maxX ? Mathf.Clamp(desired.x, minX, maxX) : (roomMin.x + roomMax.x) * 0.5f;
            desired.y = minY <= maxY ? Mathf.Clamp(desired.y, minY, maxY) : (roomMin.y + roomMax.y) * 0.5f;
            return desired;
        }
    }
}
