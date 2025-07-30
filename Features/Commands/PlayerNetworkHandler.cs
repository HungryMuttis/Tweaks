using MyceliumNetworking;
using Tweaks.Features.Commands.Patches;
using UnityEngine;
using CWAPI;

namespace Tweaks.Features.Commands
{
    internal class PlayerNetworkHandler : NetworkComponent<PlayerNetworkHandler, Player>
    {
        protected override uint MOD_ID => Tweaks.MOD_ID;
        protected override BepInEx.Logging.ManualLogSource LogSource => Tweaks.Logger;

        [CustomRPC] public void SetPlayerRemainingOxygen(float oxygen)
        {
            if (ParentComponent == null || ParentComponent.data == null) return;

            ParentComponent.data.remainingOxygen = Mathf.Clamp(oxygen, 0f, ParentComponent.data.maxOxygen);
        }
        public static void SendRemainingOxygen(Player targetPlayer, float oxygen)
        {
            Send(targetPlayer, nameof(SetPlayerRemainingOxygen), ReliableType.Reliable,
                oxygen
            );
        }

        [CustomRPC] public void SetPlayerMaxOxygen(float oxygen)
        {
            if (ParentComponent == null || ParentComponent.data == null) return;

            ParentComponent.data.maxOxygen = oxygen;
        }
        public static void SendMaxOxygen(Player targetPlayer, float oxygen)
        {
            Send(targetPlayer, nameof(SetPlayerMaxOxygen), ReliableType.Reliable,
                oxygen
            );
        }

        [CustomRPC] public void SetPlayerThrowStrengthMultiplier(float multiplier)
        {
            if (ParentComponent == null) return;

            if (Player.localPlayer != ParentComponent) return;

            PlayerPatch.ThrowStrengthMultiplier = multiplier;
        }
        public static void SendThrowStrengthMultiplier(Player targetPlayer, float multiplier)
        {
            Send(targetPlayer, nameof(SetPlayerThrowStrengthMultiplier), ReliableType.Reliable,
                multiplier
            );
        }
    }
}
