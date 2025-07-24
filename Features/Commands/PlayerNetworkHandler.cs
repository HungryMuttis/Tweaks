using MyceliumNetworking;
using Tweaks.Features.Commands.Patches;
using UnityEngine;

namespace Tweaks.Features.Commands
{
    internal class PlayerNetworkHandler : NetworkComponent<PlayerNetworkHandler, Player>
    {
        protected override uint MOD_ID => Tweaks.MOD_ID;
        protected override BepInEx.Logging.ManualLogSource LogSource => Tweaks.Logger;

        [CustomRPC] public void SetOxygen(float oxygen)
        {
            if (ParentComponent == null || ParentComponent.data == null) return;

            ParentComponent.data.remainingOxygen = Mathf.Clamp(oxygen, 0f, ParentComponent.data.maxOxygen);
        }
        public static void SendOxygen(Player targetPlayer, float oxygen, bool percent = false)
        {
            Send(targetPlayer, nameof(SetOxygen), ReliableType.Reliable,
                percent ? targetPlayer.data.maxOxygen * oxygen / 100f : oxygen
            );
        }

        [CustomRPC] public void SetThrowStrengthMultiplier(float multiplier)
        {
            if (ParentComponent == null) return;

            if (Player.localPlayer != ParentComponent) return;

            PlayerPatch.ThrowStrengthMultiplier = multiplier;
        }
        public static void SendThrowStrengthMultiplier(Player targetPlayer, float multiplier)
        {
            Send(targetPlayer, nameof(SetThrowStrengthMultiplier), ReliableType.Reliable,
                multiplier
            );
        }
    }
}
