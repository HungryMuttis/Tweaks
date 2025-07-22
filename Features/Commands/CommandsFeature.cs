namespace Tweaks.Features.Commands
{
    [ModFeature]
    internal class CommandsFeature : Feature<CommandsFeature>
    {
        public override string FeatureName => "Commands";
        protected override string FeatureDescription => "Adds various console commands.";

        public override void Initialize()
        {
            Patches.PlayerPatch.Init();
        }
    }
}
