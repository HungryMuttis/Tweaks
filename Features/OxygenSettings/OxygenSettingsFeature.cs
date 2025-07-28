using BepInEx.Configuration;
using CWAPI;

namespace Tweaks.Features.OxygenSettings
{
    [Feature]
    internal class OxygenSettingsFeature : Feature<OxygenSettingsFeature>
    {
        public override BepInEx.Logging.ManualLogSource LogSource => Tweaks.Logger;
        public override string FeatureName => "OxygenSettings";
        public override string FeatureDescription => "Variuos settings related to oxygen";

        public ConfigEntry<float> MaxOxygen { get; private set; } = null!;

        public override void CreateConfig(ConfigSection section)
        {
            MaxOxygen = section.Bind(
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
