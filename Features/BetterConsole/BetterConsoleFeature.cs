namespace Tweaks.Features.BetterConsole
{
    [ModFeature(true)]
    internal class BetterConsoleFeature : Feature<BetterConsoleFeature>
    {
        public override bool Required => true;
        public override string FeatureName => "BetterConsole";
        protected override string FeatureDescription => "Enables the console and makes it better (for the devs mostly)";

        public override void Initialize()
        {
            ConsoleHandlerPatch.Init();
            CommandSuggestionPatch.Init();
            ConsolePagePatch.Init();
        }
    }
}