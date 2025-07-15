using BepInEx.Configuration;

namespace Tweaks.Features.DeathOxygen
{
    [ModFeature]
    internal class DeathOxygenFeature : Feature
    {
        public static DeathOxygenFeature Instance { get; private set; } = null!;

        public ConfigEntry<float> Consumption { get; private set; } = null!;

        internal override string FeatureName => "DeathOxygen";
        protected override string FeatureDescription => "Changes how much oxygen is consumed when the Player is dead.";

        public DeathOxygenFeature()
        {
            Instance = this;
        }

        public override void CreateConfig(ConfigFile config)
        {
            base.CreateConfig(config);

            Consumption = config.Bind(
                FeatureName,
                nameof(Consumption),
                0f,
                """
                Changes how much oxygen is consumed when the player is dead
                Set this to 0 to turn off oxygen consumption when player is dead
                """
            );
        }

        public override void Initialize()
        {
            PlayerPatch.Init();
        }
    }
}
