using MyceliumNetworking;
using UnityEngine;
using CWAPI;
using Tweaks.Features.Commands.Patches;

namespace Tweaks.Features.Commands
{
    internal class PlayersNetworkHandler : SingletonNetworkComponent<PlayerNetworkHandler>
    {
        protected override BepInEx.Logging.ManualLogSource LogSource => Tweaks.Logger;
        protected override uint MOD_ID => Tweaks.MOD_ID;

        // PLAYER //
        [CustomRPC] public void SetRemainingOxygen(float oxygen, bool percent)
        {
            foreach (Player player in PlayerHandler.instance.players)
                player.data.remainingOxygen = Mathf.Clamp(percent ? player.data.maxOxygen * oxygen / 100 : oxygen, 0f, player.data.maxOxygen);
        }
        public static void SendRemainingOxygen(float oxygen, bool percent)
        {
            Send(nameof(SetRemainingOxygen), ReliableType.Reliable,
                oxygen, percent
            );
        }

        [CustomRPC] public void SetMaxOxygen(float oxygen)
        {
            foreach (Player player in PlayerHandler.instance.players)
                player.data.maxOxygen = oxygen;
        }
        public static void SendMaxOxygen(float oxygen)
        {
            Send(nameof(SetMaxOxygen), ReliableType.Reliable,
                oxygen
            );
        }

        [CustomRPC] public void SetThrowStrengthMultiplier(float multiplier)
        {
            PlayerPatch.ThrowStrengthMultiplier = multiplier;
        }
        public static void SendThrowStrengthMultiplier(float multiplier)
        {
            Send(nameof(SetThrowStrengthMultiplier), ReliableType.Reliable,
                multiplier
            );
        }

        // PLAYERS //
        [CustomRPC] void SetMaxHealth(float maxHealth, bool addHealth)
        {
            Player.PlayerData.maxHealth = maxHealth;
            PlayerHandler.instance.playersAlive.ForEach(p => p.data.health = addHealth ? maxHealth : Mathf.Clamp(p.data.health, 0f, maxHealth));
        }
        public static void SendMaxHealth(float maxHealth, bool addhealth)
        {
            Send(nameof(SetMaxHealth), ReliableType.Reliable,
                maxHealth, addhealth
            );
        }
    }
}