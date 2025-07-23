using System.Reflection;
using BepInEx;
using MonoMod.RuntimeDetour.HookGen;
using Tweaks.Features;

namespace Tweaks
{
    [ContentWarningPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION, false)]
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("RugbugRedfern.MyceliumNetworking")]
    public class Tweaks : BaseUnityPlugin
    {
        public const uint MOD_ID = 2389670781;

        public static Tweaks Instance { get; private set; } = default!;
        internal new static BepInEx.Logging.ManualLogSource Logger { get; private set; } = default!;
        private FeatureManager Manager = default!;

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            if (!Config.Bind("", "Enabled", true, "Enables this mod").Value)
            {
                Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} is disabled.");
                return;
            }

            Manager = new(Logger, Config);
            Logger.LogDebug("Hooking...");
            Manager.InitializeFeatures();
            Logger.LogDebug("Finished Hooking!");

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        }

        internal static void UnhookAll()
        {
            Logger.LogDebug("Unhooking...");

            HookEndpointManager.RemoveAllOwnedBy(Assembly.GetExecutingAssembly());
            HarmonyPatcher.UnpatchAll();

            Logger.LogDebug("Finished Unhooking!");
        }
    }
}
