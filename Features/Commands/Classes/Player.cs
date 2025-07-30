using Photon.Pun;
using Tweaks.Features.BetterConsole;

namespace Tweaks.Features.Commands.Classes
{
    public class Player : ICommandsClass
    {
        public static bool Enabled => CommandsFeature.Instance.Enabled;

        [ConsoleCommand("Heals the specified player specified amount", "", "Amount to heal", "Is amount percent")]
        public static void Heal(global::Player Player, float Value, bool Percent = false) => Player.refs.view.RPC("RPCA_Heal", RpcTarget.All, Percent ? global::Player.PlayerData.maxHealth * Value / 100 : Value);

        [ConsoleCommand("Sets the max oxygen for the specified player", "", "Amount of oxygen")]
        public static void SetMaxOxygen(global::Player Player, float Value) => PlayerNetworkHandler.SendMaxOxygen(Player, Value);

        [ConsoleCommand("Sets the remaining oxygen for the specified player", "", "Amount of oxygen", "Is amount percent")]
        public static void SetRemainingOxygen(global::Player Player, float Value, bool Percent = false) => PlayerNetworkHandler.SendRemainingOxygen(Player, Percent ? Player.data.maxOxygen * Value / 100 : Value);

        [ConsoleCommand("Sets the throw strength multiplier for the specified player", "", "Times to multiply the throw strength")]
        public static void SetThrowStrengthMultiplier(global::Player Player, float Multiplier) => PlayerNetworkHandler.SendThrowStrengthMultiplier(Player, Multiplier);
    }
}