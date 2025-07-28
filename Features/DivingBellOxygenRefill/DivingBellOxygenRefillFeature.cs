using BepInEx.Configuration;
using CWAPI;

namespace Tweaks.Features.DivingBellOxygenRefill
{
    [Feature]
    internal class DivingBellOxygenRefillFeature : Feature<DivingBellOxygenRefillFeature>
    {
        public override BepInEx.Logging.ManualLogSource LogSource => Tweaks.Logger;
        public override string FeatureName => "DivingBellOxygenRefilling";
        public override string FeatureDescription => "Refills oxygen when player is in diving bell";

        public ConfigEntry<float> RefillRate { get; private set; } = null!;

        public override void CreateConfig(ConfigSection section)
        {
            RefillRate = section.Bind(
                nameof(RefillRate),
                4f,
                """
                How much oxygen is refilled per second while in diving bell
                """
            );
        }

        public override void Initialize()
        {
            PlayerDataPatch.Init();
        }
    }
}
