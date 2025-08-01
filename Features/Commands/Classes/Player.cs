using Photon.Pun;
using Tweaks.Features.BetterConsole;
using UnityEngine;

namespace Tweaks.Features.Commands.Classes
{
    public class Player : ICommandsClass
    {
        public static bool Enabled => CommandsFeature.Instance.Enabled;

        [ConsoleCommand("Heals the specified player specified amount", "", "Amount to heal", "Is amount percent")]
        public static void Heal(global::Player Player, float Value, bool Percent = false)
        {
            if (Player.refs?.view == null) return;
            Player.refs.view.RPC("RPCA_Heal", RpcTarget.All, Percent ? global::Player.PlayerData.maxHealth * Value / 100 : Value);
        }

        [ConsoleCommand("Sets the gravity for the specified player", "", "Amount of gravity")]
        public static void SetGravity(global::Player Player, float Value) => ClientNetworkHandler.SendGravity(Player, Value);

        //[ConsoleCommand("Sets the gravity direction for the specified player", "", "Direction")]
        public static void SetGravityDirection(global::Player Player, Vector3 Direction) => PlayerNetworkHandler.SendGravityDirection(Player, Direction);

        [ConsoleCommand("Sets the health for the specified player", "", "Amount of health", "Is amount percent")]
        public static void SetHealth(global::Player Player, float Value, bool Percent = false) => PlayerNetworkHandler.SendHealth(Player, Percent ? global::Player.PlayerData.maxHealth * Value / 100 : Value);

        [ConsoleCommand("Sets the max oxygen for the specified player", "", "Amount of oxygen")]
        public static void SetMaxOxygen(global::Player Player, float Value) => PlayerNetworkHandler.SendMaxOxygen(Player, Value);

        [ConsoleCommand("Sets the remaining oxygen for the specified player", "", "Amount of oxygen", "Is amount percent")]
        public static void SetRemainingOxygen(global::Player Player, float Value, bool Percent = false) => PlayerNetworkHandler.SendRemainingOxygen(Player, Percent ? Player.data.maxOxygen * Value / 100 : Value);

        [ConsoleCommand("Sets the throw strength multiplier for the specified player", "", "Times to multiply the throw strength")]
        public static void SetThrowStrengthMultiplier(global::Player Player, float Multiplier) => ClientNetworkHandler.SendThrowStrengthMultiplier(Player, Multiplier);
    }
}