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
                prefix: nameof(Patrol_Prefix)
            );
            Tweaks.Patcher.Patch(
                "WalkAway", [typeof(Vector3)],
                prefix: nameof(Walk_Prefix)
            );
        }

        // PATCHES //
        public static bool Patrol_Prefix(Bot __instance, ref bool __result)
        {
            if (Level.currentLevel != null)
                return true;

            __result = false;
            __instance.patrolPoint = null;
            return false;
        }
        public static bool Walk_Prefix(Bot __instance)
        {
            if (Level.currentLevel != null) return true;

            __instance.patrolPoint = null;
            return false;
        }
    }
}
