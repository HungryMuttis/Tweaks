using Photon.Pun;
using Tweaks.Features.BetterConsole;

namespace Tweaks.Features.Commands.Classes
{
    public class Players : ICommandsClass
    {
        public static bool Enabled => CommandsFeature.Instance.Enabled;

        // PLAYER //
        [ConsoleCommand("Sets the remaining oxygen for all of the players", "Amount of oxygen", "Is amount percent (calculated individually)")]
        public static void SetRemainingOxygen(float Value, bool Percent = false) => PlayersNetworkHandler.SendRemainingOxygen(Value, Percent);
        [ConsoleCommand("Sets the max oxygen for all of the players", "Amount of oxygen")]
        public static void SetMaxOxygen(float Value) => PlayersNetworkHandler.SendMaxOxygen(Value);

        [ConsoleCommand("Heals all of the players the specified amount", "Amount to heal", "Is amount percent")]
        public static void Heal(float Value, bool Percent = false)
        {
            foreach (global::Player player in PlayerHandler.instance.players)
                player.refs.view.RPC("RPCA_Heal", RpcTarget.All, Percent ? global::Player.PlayerData.maxHealth * Value / 100 : Value);
        }

        [ConsoleCommand("Sets the throw strength multiplier for the specified player", "Times to multiply the throw strength")]
        public static void SetThrowStrengthMultiplier(float Multiplier) => PlayersNetworkHandler.SendThrowStrengthMultiplier(Multiplier);

        // PLAYERS //
        [ConsoleCommand("Sets the max health for all players to the specified value", "Max health", "Heal the player to max health")]
        public static void SetMaxHealth(float Value, bool AddHealth = false) => PlayersNetworkHandler.SendMaxHealth(Value, AddHealth);
    }
}
