using UnityEngine;

namespace Tweaks.Features.DeathOxygen
{
    public static class PlayerDataPatch
    {
        internal static void Init()
        {
            Tweaks.Patcher.SaveInfo(typeof(Player.PlayerData));
            Tweaks.Patcher.Patch(
                "UpdateValues",
                prefix: nameof(UpdateValues_Prefix),
                postfix: nameof(UpdateValues_Postfix)
            );
        }

        // PATCHES //
        public static bool UpdateValues_Prefix(Player.PlayerData __instance, out bool __state)
        {
            __state = __instance.usingOxygen;
            if (!__instance.dead) return true;

            __instance.usingOxygen = false;
            return true;
        }
        public static void UpdateValues_Postfix(Player.PlayerData __instance, bool __state)
        {
            if (!__instance.dead) return;

            __instance.usingOxygen = __state;
            __instance.remainingOxygen -= DeathOxygenFeature.Instance.Consumption.Value * Time.deltaTime;
        }
    }
}
