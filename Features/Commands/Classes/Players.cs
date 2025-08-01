using Photon.Pun;
using Tweaks.Features.BetterConsole;
using UnityEngine;

namespace Tweaks.Features.Commands.Classes
{
    public class Players : ICommandsClass
    {
        public static bool Enabled => CommandsFeature.Instance.Enabled;

        // PLAYER //
        //[ConsoleCommand("Changes if all the players can ragdoll after falling", "Can ragdoll")]
        public static void CanRagdoll(bool Ragdoll) => ClientNetworkHandler.SendCanRagdoll(Ragdoll);

        [ConsoleCommand("Heals all of the players the specified amount", "Amount to heal", "Is amount percent")]
        public static void Heal(float Value, bool Percent = false)
        {
            if (PlayerHandler.instance?.players == null) return;
            foreach (global::Player? player in PlayerHandler.instance.players)
            {
                if (player?.refs?.view == null) continue;
                player.refs.view.RPC("RPCA_Heal", RpcTarget.All, Percent ? global::Player.PlayerData.maxHealth * Value / 100 : Value);
            }
        }

        [ConsoleCommand("Sets the gravity for all players", "Amount of gravity")]
        public static void SetGravity(float Value) => ClientNetworkHandler.SendGravity(Value);

        //[ConsoleCommand("Sets the gravity direction for all players", "Direction")]
        public static void SetGravityDirection(Vector3 Direction) => ClientNetworkHandler.SendGravityDirection(Direction);

        [ConsoleCommand("Sets the health for all players", "Amount of health", "Is amount percent")]
        public static void SetHealth(float Value, bool Percent = false) => ClientNetworkHandler.SendHealth(Percent ? global::Player.PlayerData.maxHealth * Value / 100 : Value);

        [ConsoleCommand("Sets the max oxygen for all of the players", "Amount of oxygen")]
        public static void SetMaxOxygen(float Value) => ClientNetworkHandler.SendMaxOxygen(Value);

        [ConsoleCommand("Sets the remaining oxygen for all of the players", "Amount of oxygen", "Is amount percent (calculated individually)")]
        public static void SetRemainingOxygen(float Value, bool Percent = false) => ClientNetworkHandler.SendRemainingOxygen(Value, Percent);

        [ConsoleCommand("Sets the throw strength multiplier for the specified player", "Times to multiply the throw strength")]
        public static void SetThrowStrengthMultiplier(float Multiplier) => ClientNetworkHandler.SendThrowStrengthMultiplier(Multiplier);

        // PLAYERS //
        [ConsoleCommand("Sets max health for all players to the specified value", "Max health", "Heal the player")]
        public static void SetMaxHealth(float Value, bool AddHealth = false) => ClientNetworkHandler.SendMaxHealth(Value, AddHealth);
    }
}
