using BepInEx.Configuration;
using CWAPI;

namespace Tweaks.Features.Defib
{
    [Feature]
    internal class DefibFeature : Feature<DefibFeature>
    {
        public override BepInEx.Logging.ManualLogSource LogSource => Tweaks.Logger;
        public override string FeatureName => "Defib";
        public override string FeatureDescription => "Restores a configurable amount of oxygen upon revival.";

        public ConfigEntry<Enums.Setting> Setting { get; private set; } = null!;
        public ConfigEntry<float> Amount { get; private set; } = null!;
        public ConfigEntry<Enums.Type> Type { get; private set; } = null!;

        public override void CreateConfig(ConfigSection section)
        {
            Setting = section.Bind(
                nameof(Setting),
                Enums.Setting.cset,
                """
                cset: After reviving, if oxygen is less than <Amount> set it to <Amount>
                set:  After reviving, set the oxygen to <Amount> (no matter if the remaining oxygen was more or less than <Amount>)
                add:  After reviving, add <Amount> oxygen (won't give more than 100% of oxygen)
                """
            );

            Amount = section.Bind(
                nameof(Amount),
                180f,
                "Amount of oxygen"
            );

            Type = section.Bind(
                nameof(Type),
                Enums.Type.seconds,
                """
                seconds: <Amount> of seconds
                percent: <Amount> of percent
                """
            );
        }

        public override void Initialize()
        {
            PlayerPatch.Init();
        }
    }
}