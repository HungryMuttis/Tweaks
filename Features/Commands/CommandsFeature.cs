using BepInEx.Bootstrap;
using BepInEx.Configuration;
using UnityEngine;

namespace Tweaks.Features.Commands
{
    [Feature]
    internal class CommandsFeature : Feature<CommandsFeature>
    {
        public override BepInEx.Logging.ManualLogSource LogSource => Tweaks.Logger;
        public override string FeatureName => "Commands";
        public override string FeatureDescription => "Adds various console commands.";

        public ConfigEntry<KeyCode> OpenConsoleKey { get; private set; } = null!;

        public override void CreateConfig(ConfigSection section)
        {
            OpenConsoleKey = section.Bind(
                nameof(OpenConsoleKey),
                KeyCode.BackQuote,
                """
                Sets the button to open the console
                ONLY WORKS WHEN TipeMod IS NOT INSTALLED
                """);
        }

        public override void Initialize()
        {
            new GameObject("PlayersNetworkHandler", typeof(PlayersNetworkHandler));
            Patches.PlayerPatch.Init();

            if (!Chainloader.PluginInfos.ContainsKey("Tipe.TipeMod")) DebugUIHandlerPatch.Init();
        }
    }
}
