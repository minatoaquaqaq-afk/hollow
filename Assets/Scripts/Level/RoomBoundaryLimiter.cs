using UnityEngine;

namespace HollowStyleMVP.Level
{
    public class RoomBoundaryLimiter : MonoBehaviour
    {
        private const float MinX = -6.25f;
        private const float MaxX = 6.25f;
        private const float MinY = -3.25f;
        private const float MaxY = 3.25f;

        private Rigidbody2D body;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        private void LateUpdate()
        {
            ClampPosition();
        }

        private void FixedUpdate()
        {
            ClampPosition();
        }

        private void ClampPosition()
        {
            Vector2 position = body != null ? body.position : (Vector2)transform.position;
            Vector2 clamped = new Vector2(
                Mathf.Clamp(position.x, MinX, MaxX),
                Mathf.Clamp(position.y, MinY, MaxY));

            if ((clamped - position).sqrMagnitude <= 0.0001f) return;

            if (body != null)
            {
                Vector2 velocity = body.velocity;
                if ((position.x < MinX && velocity.x < 0f) || (position.x > MaxX && velocity.x > 0f))
                    velocity.x = 0f;
                if ((position.y < MinY && velocity.y < 0f) || (position.y > MaxY && velocity.y > 0f))
                    velocity.y = 0f;

                body.position = clamped;
                body.velocity = velocity;
                return;
            }

            transform.position = new Vector3(clamped.x, clamped.y, transform.position.z);
        }
    }
}
