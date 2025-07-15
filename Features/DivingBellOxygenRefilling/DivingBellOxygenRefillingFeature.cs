using BepInEx.Configuration;

namespace Tweaks.Features.DivingBellOxygenRefilling
{
    [ModFeature]
    internal class DivingBellOxygenRefillingFeature : Feature
    {
        public static DivingBellOxygenRefillingFeature Instance { get; private set; } = null!;

        internal override string FeatureName => "DivingBellOxygenRefilling";
        protected override string FeatureDescription => "Refills oxygen when player is in diving bell";

        public ConfigEntry<float> RefillRate { get; private set; } = null!;

        public DivingBellOxygenRefillingFeature()
        {
            Instance = this;
        }

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
