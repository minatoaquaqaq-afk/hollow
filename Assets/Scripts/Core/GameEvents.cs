using System;
using HollowStyleMVP.Player;

namespace HollowStyleMVP.Core
{
    public static class GameEvents
    {
        public static event Action<int> CoinsChanged;
        public static event Action<string> SceneMessage;
        public static event Action<bool> PauseChanged;
        public static event Action<int, int> PlayerEnergyChanged;
        public static event Action<int> ComboChanged;
        public static event Action<PlayerAbility, bool> AbilityChanged;

        public static void RaiseCoinsChanged(int coins) => CoinsChanged?.Invoke(coins);
        public static void RaiseSceneMessage(string message) => SceneMessage?.Invoke(message);
        public static void RaisePauseChanged(bool paused) => PauseChanged?.Invoke(paused);
        public static void RaisePlayerEnergyChanged(int current, int max) => PlayerEnergyChanged?.Invoke(current, max);
        public static void RaiseComboChanged(int combo) => ComboChanged?.Invoke(combo);
        public static void RaiseAbilityChanged(PlayerAbility ability, bool unlocked) => AbilityChanged?.Invoke(ability, unlocked);
    }
}
