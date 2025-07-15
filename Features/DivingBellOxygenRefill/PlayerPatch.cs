using UnityEngine;

namespace Tweaks.Features.DivingBellOxygenRefill
{
    internal class PlayerPatch
    {
        internal static void Init()
        {
            On.Player.PlayerData.UpdateValues += PlayerData_UpdateValues;
        }

        // HOOKS //
        private static void PlayerData_UpdateValues(On.Player.PlayerData.orig_UpdateValues orig, Player.PlayerData self)
        {
            orig(self);

            if (self.isInDiveBell)
            {
                Tweaks.Logger.LogWarning(DivingBellOxygenRefillFeature.Instance.RefillRate.Value);
                self.remainingOxygen = Mathf.Clamp(self.remainingOxygen + DivingBellOxygenRefillFeature.Instance.RefillRate.Value * Time.deltaTime, 0f, self.maxOxygen);
            }
        }
    }
}
