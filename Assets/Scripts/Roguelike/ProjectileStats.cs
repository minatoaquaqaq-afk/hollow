using System;
using UnityEngine;

namespace HollowStyleMVP.Roguelike
{
    [Serializable]
    public struct ProjectileModifier
    {
        public int damageBonus;
        public float speedBonus;
        public float rangeBonus;
        public float fireDelayMultiplier;
        public float sizeBonus;

        public bool HasAnyValue =>
            damageBonus != 0 ||
            Mathf.Abs(speedBonus) > 0.001f ||
            Mathf.Abs(rangeBonus) > 0.001f ||
            Mathf.Abs(fireDelayMultiplier) > 0.001f ||
            Mathf.Abs(sizeBonus) > 0.001f;
    }

    public struct ProjectileStats
    {
        public int damage;
        public float speed;
        public float range;
        public float fireDelay;
        public float size;

        public void Apply(ProjectileModifier modifier)
        {
            damage = Mathf.Max(1, damage + modifier.damageBonus);
            speed = Mathf.Max(1f, speed + modifier.speedBonus);
            range = Mathf.Max(0.5f, range + modifier.rangeBonus);
            if (modifier.fireDelayMultiplier > 0.001f)
                fireDelay = Mathf.Max(0.05f, fireDelay * modifier.fireDelayMultiplier);
            size = Mathf.Max(0.1f, size + modifier.sizeBonus);
        }
    }
}
