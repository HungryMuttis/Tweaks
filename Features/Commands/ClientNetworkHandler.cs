using CWAPI;
using CWAPI.Extensions;
using MyceliumNetworking;
using Steamworks;
using System;
using Tweaks.Features.Commands.Patches;
using UnityEngine;

namespace Tweaks.Features.Commands
{
    internal class ClientNetworkHandler : SingletonNetworkComponent<ClientNetworkHandler>
    {
        protected override BepInEx.Logging.ManualLogSource LogSource => Tweaks.Logger;
        protected override uint MOD_ID => Tweaks.MOD_ID;

        #region PLAYER
        [CustomRPC] private void RPC_SetHealth(float health) => All(p => PlayerManager.SetHealth(p, health));
        public static void SendHealth(float health) => Send(nameof(RPC_SetHealth), ReliableType.Reliable,
            health);

        [CustomRPC] private void RPC_SetGravityDirection(Vector3 direction) => All(p => PlayerManager.SetGravityDirection(p, direction));
        public static void SendGravityDirection(Vector3 direction) => Send(nameof(RPC_SetGravityDirection), ReliableType.Reliable,
            direction);

        [CustomRPC] private void RPC_SetMaxOxygen(float maxOxygen) => All(p => PlayerManager.SetMaxOxygen(p, maxOxygen));
        public static void SendMaxOxygen(float maxOxygen) => Send(nameof(RPC_SetMaxOxygen), ReliableType.Reliable,
            maxOxygen);

        [CustomRPC] private void RPC_SetRemainingOxygen(float remainingOxygen, bool percent) => All(p => PlayerManager.SetRemainingOxygen(p, remainingOxygen, percent));
        public static void SendRemainingOxygen(float remainingOxygen, bool percent) => Send(nameof(RPC_SetRemainingOxygen), ReliableType.Reliable,
            remainingOxygen, percent);
        #endregion PLAYER

        [CustomRPC] private void RPC_PlayerConsole(bool enabled) => DebugUIHandlerPatch.AllowConsole = enabled;
        public static void SendConsoleEnabled(Player player, bool enabled) => Send(nameof(RPC_PlayerConsole), player, ReliableType.Reliable,
            enabled);

        [CustomRPC] private void RPC_SetGravity(float gravity) => PlayerManager.SetGravity(gravity);
        public static void SendGravity(Player player, float gravity) => Send(nameof(RPC_SetGravity), player, ReliableType.Reliable,
            gravity);
        public static void SendGravity(float gravity) => Send(nameof(RPC_SetGravity), ReliableType.Reliable,
            gravity);

        [CustomRPC] private void RPC_SetPlayerThrowStrengthMultiplier(float multiplier) => PlayerPatch.ThrowStrengthMultiplier = multiplier;
        public static void SendThrowStrengthMultiplier(Player player, float multiplier) => Send(nameof(RPC_SetPlayerThrowStrengthMultiplier), player, ReliableType.Reliable,
            multiplier);
        public static void SendThrowStrengthMultiplier(float multiplier) => Send(nameof(RPC_SetPlayerThrowStrengthMultiplier), ReliableType.Reliable,
            multiplier);


        [CustomRPC] private void RPC_CanRagdoll(bool ragdoll) => PlayerRagdoll.RagdollIfFellForLongerThan = ragdoll ? 1.5f : float.MaxValue;
        public static void SendCanRagdoll(bool ragdoll) => Send(nameof(RPC_CanRagdoll), ReliableType.Reliable,
            ragdoll);

        [CustomRPC] private void RPC_SetMaxHealth(float maxHealth, bool addHealth) => PlayerManager.SetMaxHealth(maxHealth, addHealth);
        public static void SendMaxHealth(float maxHealth, bool addHealth) => Send(nameof(RPC_SetMaxHealth), ReliableType.Reliable,
            maxHealth, addHealth);


        // HELPER METHODS //
        private static void All(Action<Player?> action)
        {
            if (PlayerHandler.instance?.players == null) return;
            foreach (Player? player in PlayerHandler.instance.players)
                action(player);
        }
        private static void Send(string methodName, Player? plr, ReliableType reliable, params object[] parameters)
        {
            if (plr?.TryGetSteamID(out CSteamID target) != true)
            {
                Debug.LogError("Failed to get CSteamID from Player");
                return;
            }

            Send(methodName, target, reliable, parameters);
        }
    }
}