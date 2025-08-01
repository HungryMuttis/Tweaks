using Tweaks.Features.BetterConsole;

namespace Tweaks.Features.Commands.Classes
{
    internal class Console : ICommandsClass
    {
        public static bool Enabled => CommandsFeature.Instance.Enabled;

        [ConsoleCommand("Disables console for the specified player")]
        public static void Disable(global::Player Player) => ClientNetworkHandler.SendConsoleEnabled(Player, false);
        [ConsoleCommand("Enables console for the specified player")]
        public static void Enable(global::Player Player) => ClientNetworkHandler.SendConsoleEnabled(Player, true);
    }
}
