using System.Collections.Generic;
using UnityEngine;

namespace HollowStyleMVP.Roguelike
{
    public class PlayerProjectileModifiers : MonoBehaviour
    {
        private readonly List<ProjectileModifier> modifiers = new List<ProjectileModifier>();

        public void AddModifier(ProjectileModifier modifier)
        {
            if (!modifier.HasAnyValue) return;
            modifiers.Add(modifier);
        }

        public ProjectileStats ApplyTo(ProjectileStats stats)
        {
            foreach (var modifier in modifiers)
                stats.Apply(modifier);
            return stats;
        }
    }
}
