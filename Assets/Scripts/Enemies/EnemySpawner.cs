using System.Collections;
using UnityEngine;

namespace HollowStyleMVP.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private float respawnSeconds = 8f;
        [SerializeField] private bool respawn = true;
        private GameObject current;

        private void Start() => Spawn();

        private void Update()
        {
            if (respawn && current == null) StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            respawn = false;
            yield return new WaitForSeconds(respawnSeconds);
            Spawn();
            respawn = true;
        }

        public void Spawn()
        {
            if (enemyPrefab == null || current != null) return;
            current = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        }
    }
}
