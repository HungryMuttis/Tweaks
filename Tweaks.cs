using System.Reflection;
using BepInEx;
using BepInEx.Logging;
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

        public static Tweaks Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        private FeatureManager Manager = null!;

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

            /*
             *  HookEndpointManager is from MonoMod.RuntimeDetour.HookGen, and is used by the MMHOOK assemblies.
             *  We can unhook all methods hooked with HookGen using this.
             *  Or we can unsubscribe specific patch methods with 'On.Namespace.Type.Method -= CustomMethod;'
             */
            HookEndpointManager.RemoveAllOwnedBy(Assembly.GetExecutingAssembly());

            Logger.LogDebug("Finished Unhooking!");
        }
    }
}
