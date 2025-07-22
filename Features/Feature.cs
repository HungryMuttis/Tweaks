using BepInEx.Configuration;

namespace Tweaks.Features
{
    internal interface IFeature
    {
        string FeatureName { get; }
        bool Enabled { get; }
        bool Required { get; }

        void CreateConfig(ConfigFile config);
        void Initialize();
    }
    internal abstract class Feature<T> : IFeature where T : Feature<T>
    {
        public static T Instance { get; private set; } = default!;

        private ManualLogSource? _logger;
        private ConfigEntry<bool>? _enabled;

        public bool Enabled => _enabled?.Value ?? Required;

        public virtual bool Required { get; }
        public abstract string FeatureName { get; }


        protected abstract string FeatureDescription { get; }
        public ManualLogSource Logger => _logger ??= new ManualLogSource(Tweaks.Logger, $"[{FeatureName}]");

        public Feature()
        {
            Instance = (T)this;
        }


        public abstract void Initialize();
        public virtual void CreateConfig(ConfigFile config)
        {
            if (!Required)
                _enabled = config.Bind(
                    FeatureName,
                    nameof(Enabled),
                    true,
                    $"Enables feature: {FeatureDescription}"
                );
        }

        public static void Debug(object data)
            => Instance.Logger.LogDebug(data);
        public static void Message(object data)
            => Instance.Logger.LogMessage(data);
        public static void Info(object data)
            => Instance.Logger.LogInfo(data);
        public static void Warning(object data)
            => Instance.Logger.LogWarning(data);
        public static void Error(object data)
            => Instance.Logger.LogError(data);
        public static void Fatal(object data)
            => Instance.Logger.LogFatal(data);
    }
}