using UnityEngine;

namespace Tweaks.Features.DivingBellOxygenRefill
{
    public static class PlayerDataPatch
    {
        internal static void Init()
        {
            Tweaks.Patcher.SaveInfo(typeof(Player.PlayerData));
            Tweaks.Patcher.Patch(
                "UpdateValues",
                postfix: nameof(UpdateValues_Postfix)
            );
        }

        // PATCHES //
        public static void UpdateValues_Postfix(Player.PlayerData __instance)
        {
            if (__instance.isInDiveBell)
                __instance.remainingOxygen = Mathf.Clamp(__instance.remainingOxygen + DivingBellOxygenRefillFeature.Instance.RefillRate.Value * Time.deltaTime, 0f, __instance.maxOxygen);
        }
    }
}
