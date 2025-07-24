using Tweaks.Features.BetterConsole;

namespace Tweaks.Features.Commands.Classes
{
    public class Players : ICommandsClass
    {
        public static bool Enabled => CommandsFeature.Instance.Enabled;

        [ConsoleCommand("Sets the max health for all players to the specified value", "Max health", "Heal the player to max health")]
        public static void SetMaxHealth(float Value, bool AddHealth = false) => PlayersNetworkHandler.SendMaxHealth(Value, AddHealth);
    }
}
