using System;
using System.Collections.Generic;
using HollowStyleMVP.Core;
using UnityEngine;

namespace HollowStyleMVP.Player
{
    public class PlayerAbilityController : MonoBehaviour
    {
        private readonly HashSet<PlayerAbility> unlocked = new HashSet<PlayerAbility>();
        public event Action<PlayerAbility> AbilityUnlocked;

        public void ApplyStartingAbilities(PlayerConfig config)
        {
            unlocked.Clear();
            if (config == null) return;
            if (config.startWithDoubleJump) Unlock(PlayerAbility.DoubleJump, false);
            if (config.startWithDash) Unlock(PlayerAbility.Dash, false);
            if (config.startWithDownStrike) Unlock(PlayerAbility.DownStrike, false);
            if (config.startWithRangedAttack) Unlock(PlayerAbility.RangedAttack, false);
            BroadcastAll();
        }

        public bool Has(PlayerAbility ability) => unlocked.Contains(ability);

        public void Unlock(PlayerAbility ability, bool notify = true)
        {
            if (!unlocked.Add(ability)) return;
            GameEvents.RaiseAbilityChanged(ability, true);
            if (notify) AbilityUnlocked?.Invoke(ability);
        }

        public List<string> ToSaveList()
        {
            var list = new List<string>();
            foreach (var ability in unlocked) list.Add(ability.ToString());
            return list;
        }

        public void LoadFromSave(IEnumerable<string> abilityNames)
        {
            unlocked.Clear();
            if (abilityNames != null)
            {
                foreach (string abilityName in abilityNames)
                {
                    if (Enum.TryParse(abilityName, out PlayerAbility ability)) unlocked.Add(ability);
                }
            }
            BroadcastAll();
        }

        private void BroadcastAll()
        {
            foreach (PlayerAbility ability in Enum.GetValues(typeof(PlayerAbility)))
                GameEvents.RaiseAbilityChanged(ability, unlocked.Contains(ability));
        }
    }
}
