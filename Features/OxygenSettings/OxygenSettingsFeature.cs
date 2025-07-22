using BepInEx.Configuration;

namespace Tweaks.Features.OxygenSettings
{
    [ModFeature]
    internal class OxygenSettingsFeature : Feature<OxygenSettingsFeature>
    {
        public override string FeatureName => "OxygenSettings";
        protected override string FeatureDescription => "Variuos settings related to oxygen";

        public ConfigEntry<float> MaxOxygen { get; private set; } = null!;

        public override void CreateConfig(ConfigFile config)
        {
            base.CreateConfig(config);

            MaxOxygen = config.Bind(
                FeatureName,
                nameof(MaxOxygen),
                500f,
                """
                Maximum seconds of oxygen player can have
                """
            );
        }

        public override void Initialize()
        {
            PlayerPatch.Init();
        }
    }
}
