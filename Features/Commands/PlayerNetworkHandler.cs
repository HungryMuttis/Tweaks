using MyceliumNetworking;
using CWAPI;
using UnityEngine;

namespace Tweaks.Features.Commands
{
    internal class PlayerNetworkHandler : NetworkComponent<PlayerNetworkHandler, Player>
    {
        protected override BepInEx.Logging.ManualLogSource LogSource => Tweaks.Logger;
        protected override uint MOD_ID => Tweaks.MOD_ID;

        [CustomRPC] private void RPC_SetPlayerHealth(float health) => PlayerManager.SetHealth(ParentComponent, health);
        public static void SendHealth(Player targetPlayer, float health) => Send(targetPlayer, nameof(RPC_SetPlayerHealth), ReliableType.Reliable,
            health);

        [CustomRPC] private void RPC_SetPlayerGravityDirection(Vector3 direction) => PlayerManager.SetGravityDirection(ParentComponent, direction);
        public static void SendGravityDirection(Player targetPlayer, Vector3 direction) => Send(targetPlayer, nameof(RPC_SetPlayerGravityDirection), ReliableType.Reliable,
            direction);

        [CustomRPC] private void RPC_SetPlayerMaxOxygen(float maxOxygen) => PlayerManager.SetMaxOxygen(ParentComponent, maxOxygen);
        public static void SendMaxOxygen(Player targetPlayer, float maxOxygen) => Send(targetPlayer, nameof(RPC_SetPlayerMaxOxygen), ReliableType.Reliable,
            maxOxygen);

        [CustomRPC] private void RPC_SetPlayerRemainingOxygen(float remainingOxygen) => PlayerManager.SetRemainingOxygen(ParentComponent, remainingOxygen, false);
        public static void SendRemainingOxygen(Player targetPlayer, float remainingOxygen) => Send(targetPlayer, nameof(RPC_SetPlayerRemainingOxygen), ReliableType.Reliable,
            remainingOxygen);
    }
}
