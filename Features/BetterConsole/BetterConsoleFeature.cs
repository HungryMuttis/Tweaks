namespace Tweaks.Features.BetterConsole
{
    [Feature(true)]
    internal class BetterConsoleFeature : Feature<BetterConsoleFeature>
    {
        public override BepInEx.Logging.ManualLogSource LogSource => Tweaks.Logger;
        public override bool Required => true;
        public override string FeatureName => "BetterConsole";
        public override string FeatureDescription => "Enables the console and makes it better (for the devs mostly)";

        public override void Initialize()
        {
            ConsoleHandlerPatch.Init();
            CommandSuggestionPatch.Init();
            ConsolePagePatch.Init();
        }
    }
}