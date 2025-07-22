using Tweaks.Features.BetterConsole;

namespace Tweaks.Features.Commands.Classes
{
    internal class Players : CommandsClass
    {
        public static new bool Enabled => CommandsFeature.Instance.Enabled;

        [ConsoleCommand("Sets the max health for all players to the specified value", "Max health")]
        public static void SetMaxHealth(float Value)
            => PlayerNetworkHandler.SendSetMaxHealthRequest(Value);
    }
}
