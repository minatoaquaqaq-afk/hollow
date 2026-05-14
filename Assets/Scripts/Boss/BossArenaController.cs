using UnityEngine;

namespace HollowStyleMVP.Boss
{
    public class BossArenaController : MonoBehaviour
    {
        [SerializeField] private GameObject[] arenaLocks;
        [SerializeField] private BossController boss;
        [SerializeField] private bool activeOnStart;

        private void Start()
        {
            SetLocked(activeOnStart);
        }

        public void BeginFight() => SetLocked(true);
        public void EndFight() => SetLocked(false);

        private void SetLocked(bool locked)
        {
            if (arenaLocks == null) return;
            foreach (var arenaLock in arenaLocks)
                if (arenaLock != null) arenaLock.SetActive(locked);
        }
    }
}
