using BepInEx.Configuration;

namespace Tweaks.Features
{
    internal abstract class Feature
    {
        public ConfigEntry<bool> Enabled { get; protected set; } = null!;

        internal abstract string FeatureName { get; }
        protected abstract string FeatureDescription { get; }

        public abstract void Initialize();

        public virtual void CreateConfig(ConfigFile config)
        {
            Enabled = config.Bind(
                FeatureName,
                nameof(Enabled),
                true,
                $"Enables feature: {FeatureDescription}"
            );
        }
    }
}