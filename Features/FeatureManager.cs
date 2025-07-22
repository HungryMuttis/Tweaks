using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Configuration;

namespace Tweaks.Features
{
    internal class FeatureManager
    {
        private readonly List<IFeature> Features = [];
        private readonly BepInEx.Logging.ManualLogSource Logger;
        private readonly ConfigFile Config;

        public FeatureManager(BepInEx.Logging.ManualLogSource logger, ConfigFile config)
        {
            Logger = logger;
            Config = config;
            RegisterFeaturesFromAssembly();
        }

        public void RegisterFeaturesFromAssembly()
        {
            Debug("Scanning for features...");
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
                        Debug($"Discovered and registered feature: {feature.FeatureName}");
                    }    
                });
        }

        public void InitializeFeatures()
        {
            Features.ForEach(f =>
            {
                f.CreateConfig(Config);
                if (f.Enabled)
                {
                    if (f.Required)
                        Info($"Feature '{f.FeatureName}' is required. Initializing...");
                    else
                        Info($"Feature '{f.FeatureName}' is enabled. Initializing...");
                    f.Initialize();
                }
                else
                    Info($"Feature '{f.FeatureName}' is disabled.");
            });
        }

        private void Debug(string text)
            => Logger.LogDebug($"[{nameof(FeatureManager)}] {text}");
        private void Info(string text)
            => Logger.LogInfo($"[{nameof(FeatureManager)}] {text}");
    }
}