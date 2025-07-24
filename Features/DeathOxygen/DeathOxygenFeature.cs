using BepInEx.Configuration;

namespace Tweaks.Features.DeathOxygen
{
    [Feature]
    internal class DeathOxygenFeature : Feature<DeathOxygenFeature>
    {
        public override BepInEx.Logging.ManualLogSource LogSource => Tweaks.Logger;
        public override string FeatureName => "DeathOxygen";
        public override string FeatureDescription => "Changes how much oxygen is consumed when the Player is dead.";

        public ConfigEntry<float> Consumption { get; private set; } = null!;

        public override void CreateConfig(ConfigSection section)
        {
            Consumption = section.Bind(
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
