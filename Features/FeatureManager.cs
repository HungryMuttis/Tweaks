using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Configuration;

namespace Tweaks.Features
{
    public class FeatureManager
    {
        private readonly List<IFeature> Features = [];
        private readonly ManualLogSource Logger;
        private readonly ConfigFile Config;

        public FeatureManager(BepInEx.Logging.ManualLogSource logger, ConfigFile config)
        {
            Logger = new(logger, nameof(FeatureManager));
            Config = config;
            RegisterFeaturesFromAssembly();
        }

        public void RegisterFeaturesFromAssembly()
        {
            Logger.LogDebug("Scanning for features...");
            Type baseType = typeof(Feature<>);
            Assembly.GetExecutingAssembly().GetTypes()
                .Where(t =>
                {
                    if (!t.IsClass || t.IsAbstract || t.GetCustomAttribute<ModFeatureAttribute>() == null) return false;
                    Type? current = t.BaseType;
                    while (current != null)
                    {
                        if (current.IsGenericType && current.GetGenericTypeDefinition() == baseType)
                            return true;
                        current = current.BaseType;
                    }
                    return false;
                }).ToList()
                .ForEach(t =>
                {
                    if (Activator.CreateInstance(t) is IFeature feature)
                    {
                        Features.Add(feature);
                        Logger.LogDebug($"Discovered and registered feature: {feature.FeatureName}");
                    }    
                });
        }

        public void InitializeFeatures() => Features.ForEach(f =>
        {
            f.CreateRequiredConfig(Config);
            f.CreateConfig(Config);
            if (f.Enabled)
            {
                if (f.Required)
                    Logger.LogInfo($"Feature '{f.FeatureName}' is required. Initializing...");
                else
                    Logger.LogInfo($"Feature '{f.FeatureName}' is enabled. Initializing...");
                f.Initialize();
            }
            else
                Logger.LogInfo($"Feature '{f.FeatureName}' is disabled.");
        });
    }
}