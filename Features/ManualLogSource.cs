namespace Tweaks.Features
{
    public class ManualLogSource
    {
        private BepInEx.Logging.ManualLogSource Logger { get; }
        public string Prefix { get; }

        internal ManualLogSource(BepInEx.Logging.ManualLogSource logger, string prefix)
        {
            Logger = logger;
            Prefix = prefix + ' ';
        }

        public void LogDebug(object data) => Logger.LogDebug(Prefix + data);
        public void LogInfo(object data) => Logger.LogInfo(Prefix + data);
        public void LogMessage(object data) => Logger.LogMessage(Prefix + data);
        public void LogWarning(object data) => Logger.LogWarning(Prefix + data);
        public void LogError(object data) => Logger.LogError(Prefix + data);
        public void LogFatal(object data) => Logger.LogFatal(Prefix + data);
    }
}
