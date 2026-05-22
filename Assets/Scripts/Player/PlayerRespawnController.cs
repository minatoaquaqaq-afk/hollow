using HollowStyleMVP.Combat;
using HollowStyleMVP.Core;
using HollowStyleMVP.Level;
using HollowStyleMVP.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HollowStyleMVP.Player
{
    [RequireComponent(typeof(Health))]
    public class PlayerRespawnController : MonoBehaviour
    {
        [SerializeField] private Transform respawnPoint;
        private Health health;
        private PlayerController2D controller;
        private Rigidbody2D body;
        public bool IsDead { get; private set; }

        private void Awake()
        {
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

            DeathUIController.Instance?.Hide();
            UiModalState.Reset();
            TestSceneRoomSetup.ResetRunState();
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }
}
