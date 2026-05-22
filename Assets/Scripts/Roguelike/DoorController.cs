using HollowStyleMVP.Core;
using UnityEngine;

namespace HollowStyleMVP.Roguelike
{
    [RequireComponent(typeof(Collider2D))]
    public class DoorController : MonoBehaviour
    {
        [SerializeField] private Direction2D direction = Direction2D.North;
        [SerializeField] private GameObject lockedVisual;
        [SerializeField] private GameObject availableVisual;
        [SerializeField] private Transform playerExitPoint;

        private Collider2D doorCollider;
        private bool locked;
        private bool available = true;

        public Direction2D Direction => direction;

        private void Awake()
        {
            doorCollider = GetComponent<Collider2D>();
            doorCollider.isTrigger = true;
            RefreshVisuals();
        }

        public void Configure(Direction2D newDirection, Transform exitPoint = null)
        {
            direction = newDirection;
            playerExitPoint = exitPoint;
            RefreshVisuals();
        }

        public void SetAvailable(bool value)
        {
            available = value;
            gameObject.SetActive(value);
            RefreshVisuals();
        }

        public void SetLocked(bool value)
        {
            locked = value;
            RefreshVisuals();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!available || locked || !other.CompareTag("Player")) return;
            if (RunManager.Instance == null || !RunManager.Instance.TryMove(direction)) return;
            if (playerExitPoint != null) other.transform.position = playerExitPoint.position;
            GameEvents.RaiseSceneMessage("进入新房间");
        }

        private void RefreshVisuals()
        {
            if (lockedVisual != null) lockedVisual.SetActive(locked);
            if (availableVisual != null) availableVisual.SetActive(available && !locked);
        }
    }
}
