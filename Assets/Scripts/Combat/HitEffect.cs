using UnityEngine;

namespace HollowStyleMVP.Combat
{
    public class HitEffect : MonoBehaviour
    {
        public static void Spawn(Vector3 position, Color color)
        {
            var obj = new GameObject("Hit Effect");
            obj.transform.position = position;
            var ps = obj.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startLifetime = 0.25f;
            main.startSpeed = 4f;
            main.startSize = 0.12f;
            main.startColor = color;
            main.maxParticles = 18;
            var emission = ps.emission;
            emission.rateOverTime = 0f;
            ps.Emit(16);
            Destroy(obj, 0.6f);
        }
    }
}
