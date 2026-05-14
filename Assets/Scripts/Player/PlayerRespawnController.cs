using System.Collections;
using HollowStyleMVP.Combat;
using HollowStyleMVP.UI;
using UnityEngine;

namespace HollowStyleMVP.Player
{
    [RequireComponent(typeof(Health))]
    public class PlayerRespawnController : MonoBehaviour
    {
        [SerializeField] private Transform respawnPoint;
        [SerializeField] private float respawnDelay = 1.2f;
        private Health health;
        private Vector3 fallbackSpawn;
        private PlayerController2D controller;
        private Rigidbody2D body;
        public bool IsDead { get; private set; }

        private void Awake()
        {
            fallbackSpawn = transform.position;
            health = GetComponent<Health>();
            controller = GetComponent<PlayerController2D>();
            body = GetComponent<Rigidbody2D>();
            health.Died += OnDied;
        }

        public void SetRespawnPoint(Transform point) => respawnPoint = point;

        private void OnDied()
        {
            if (IsDead) return;
            IsDead = true;
            if (controller != null) controller.enabled = false;
            if (body != null) body.velocity = Vector2.zero;
            DeathUIController.Instance?.Show(respawnDelay);
            StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(respawnDelay);
            transform.position = respawnPoint != null ? respawnPoint.position : fallbackSpawn;
            health.SetCurrent(health.MaxHealth);
            IsDead = false;
            if (controller != null) controller.enabled = true;
            DeathUIController.Instance?.Hide();
        }
    }
}


