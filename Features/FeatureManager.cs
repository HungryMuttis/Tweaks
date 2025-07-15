using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace Tweaks.Features
{
    internal class FeatureManager
    {
        private readonly List<Feature> Features = [];
        private readonly ManualLogSource Logger;
        private readonly ConfigFile Config;

        public FeatureManager(ManualLogSource logger, ConfigFile config)
        {
            Logger = logger;
            Config = config;
            RegisterFeaturesFromAssembly();
        }

        public void RegisterFeaturesFromAssembly()
        {
            Logger.LogDebug("Scanning for features...");
            IEnumerable<Type> featureTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Feature)) && t.GetCustomAttribute<ModFeature>() != null);
            foreach (Type type in featureTypes)
                if (Activator.CreateInstance(type) is Feature feature)
                {
                    Features.Add(feature);
                    Logger.LogDebug($"Discovered and registered feature: {type.Name}");
                }
        }

        public void InitializeFeatures()
        {
            Features.ForEach(f =>
            {
                f.CreateConfig(Config);
                if (f.Enabled.Value)
                {
                    Logger.LogInfo($"Feature '{f.FeatureName}' is enabled. Initializing...");
                    f.Initialize();
                }
                else
                {
                    Logger.LogInfo($"Feature '{f.FeatureName}' is disabled.");
                }
            });
        }

        public bool IsAnyFeatureEnabled()
        {
            return Features.Any(f => f.Enabled.Value);
        }
    }
}