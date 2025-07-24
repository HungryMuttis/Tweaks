using BepInEx.Bootstrap;
using UnityEngine;

namespace Tweaks.Features.Commands
{
    [ModFeature]
    internal class CommandsFeature : Feature<CommandsFeature>
    {
        public override BepInEx.Logging.ManualLogSource LogSource => Tweaks.Logger;
        public override string FeatureName => "Commands";
        public override string FeatureDescription => "Adds various console commands.";

        public override void Initialize()
        {
            new GameObject("PlayersNetworkHandler", typeof(PlayersNetworkHandler));
            Patches.PlayerPatch.Init();

            if (!Chainloader.PluginInfos.ContainsKey("Tipe.TipeMod")) DebugUIHandlerPatch.Init();
        }
    }
}
