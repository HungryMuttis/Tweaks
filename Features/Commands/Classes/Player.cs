using Tweaks.Features.BetterConsole;

namespace Tweaks.Features.Commands.Classes
{
    public class Player : CommandsClass
    {
        public static new bool Enabled => CommandsFeature.Instance.Enabled;

        [ConsoleCommand("Sets the remaining oxygen for the specified player", "", "Amount of oxygen", "Is amount percent")]
        public static void SetRemainingOxygen(global::Player Player, float Value, bool Percent = false)
            => PlayerNetworkHandler.SendSetOxygenRequest(Player, Value, Percent);

        [ConsoleCommand("Heals the specified player specified value", "", "Amount to heal", "Is amount percent")]
        public static void Heal(global::Player Player, float Value, bool Percent = false)
            => Player.CallHeal(Percent ? global::Player.PlayerData.maxHealth * Value / 100 : Value);
    }
}