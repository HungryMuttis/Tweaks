namespace Tweaks.Features.Commands
{
    [ModFeature]
    internal class CommandsFeature : Feature
    {
        public static CommandsFeature Instance { get; set; } = null!;

        internal override string FeatureName => "Commands";
        protected override string FeatureDescription => "Adds various console commands.";

        public CommandsFeature()
        {
            Instance = this;
        }

        public override void Initialize()
        {
            PlayerPatch.Init();
        }
    }
}
