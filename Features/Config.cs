using BepInEx.Configuration;

namespace Tweaks.Features
{
    public class ConfigSection(ConfigFile config, string name)
    {
        public ConfigFile Config { get; } = config;
        private string Name { get; } = name;

        public ConfigEntry<T> Bind<T>(string key, T defaultValue, ConfigDescription? configDescription = null) => Config.Bind(Name, key, defaultValue, configDescription);
        public ConfigEntry<T> Bind<T>(string key, T defaultValue, string? description = null) => Config.Bind(Name, key, defaultValue, description);
    }
}
