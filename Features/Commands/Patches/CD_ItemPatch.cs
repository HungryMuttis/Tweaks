using HarmonyLib;
using System.Reflection;

namespace Tweaks.Features.Commands.Patches
{
    public static class CD_ItemPatch
    {
        private static readonly FieldInfo machineField = AccessTools.Field(typeof(CD_Item), "machine");

        internal static void Init()
        {
            Tweaks.Patcher.SaveInfo();
            Tweaks.Patcher.Patch(
                "Update",
                prefix: nameof(Update_Prefix)
            );
        }

        // PATCHES //
        public static bool Update_Prefix(CD_Item __instance)
        {
            if (machineField.GetValue(__instance) != null) return true;
            return false;
        }
    }
}
