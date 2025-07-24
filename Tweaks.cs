using System.Reflection;
using BepInEx;
using MonoMod.RuntimeDetour.HookGen;
using Tweaks.Features;

namespace Tweaks
{
    [ContentWarningPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION, false)]
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("RugbugRedfern.MyceliumNetworking")]
    [BepInDependency("Tipe.TipeMod", BepInDependency.DependencyFlags.SoftDependency)]
    public class Tweaks : BaseUnityPlugin
    {
        public const uint MOD_ID = 2389670781;

        public static Tweaks Instance { get; private set; } = default!;
        internal new static BepInEx.Logging.ManualLogSource Logger { get; private set; } = default!;
        internal static HarmonyPatcher Patcher { get; private set; } = default!;
        private FeatureManager Manager = default!;
        private const string PluginInfo = $"{MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION}";

        private void Awake()
        {
            Logger = base.Logger;
            Patcher = new(MyPluginInfo.PLUGIN_GUID, Logger);
            Instance = this;

            if (!Config.Bind("", "Enabled", true, "Enables this mod").Value)
            {
                Logger.LogInfo($"{PluginInfo} is disabled.");
                return;
            }

            Manager = new(Logger, Config);
            Logger.LogDebug("Hooking...");
            Manager.InitializeFeatures();
            Logger.LogDebug("Finished Hooking!");
            Logger.LogInfo($"{PluginInfo} has loaded!");
        }

        internal static void UnhookAll()
        {
            Logger.LogDebug("Unhooking...");

            HookEndpointManager.RemoveAllOwnedBy(Assembly.GetExecutingAssembly());
            Patcher.UnpatchAll();

            Logger.LogDebug("Finished Unhooking!");
        }
    }
}
