using UnityEngine;

namespace Tweaks.Features.Commands.Patches
{
    public static class BotPatch
    {
        internal static void Init()
        {
            Tweaks.Patcher.SaveInfo();
            Tweaks.Patcher.Patch(
                nameof(Bot.Patrol), [typeof(bool), typeof(bool), typeof(float), typeof(bool), typeof(Vector3), typeof(bool)],
                prefix: nameof(NullReferenceFix_Prefix)
            );
            Tweaks.Patcher.Patch(
                nameof(Bot.Walk), [typeof(Vector3)], // void return type
                prefix: nameof(NullReferenceFix_Prefix)
            );
        }

        // PATCHES //
        public static bool NullReferenceFix_Prefix(Bot __instance, ref bool __result)
        {
            if (Level.currentLevel != null)
                return true;

            __result = false;
            __instance.patrolPoint = null;
            return false;
        }
    }
}
