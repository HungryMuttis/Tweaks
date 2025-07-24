using BepInEx.Configuration;

namespace Tweaks.Features.DivingBellOxygenRefill
{
    [ModFeature]
    internal class DivingBellOxygenRefillFeature : Feature<DivingBellOxygenRefillFeature>
    {
        public override string FeatureName => "DivingBellOxygenRefilling";
        public override string FeatureDescription => "Refills oxygen when player is in diving bell";

        public ConfigEntry<float> RefillRate { get; private set; } = null!;

        public override void CreateConfig(ConfigFile config)
        {
            base.CreateConfig(config);

            RefillRate = config.Bind(
                FeatureName,
                nameof(RefillRate),
                4f,
                """
                How much oxygen is refilled per second while in diving bell
                """
            );
        }

        public override void Initialize()
        {
            PlayerPatch.Init();
        }
    }
}
