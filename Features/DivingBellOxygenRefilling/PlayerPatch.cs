using UnityEngine;

namespace Tweaks.Features.DivingBellOxygenRefilling
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
                Tweaks.Logger.LogWarning(DivingBellOxygenRefillingFeature.Instance.RefillRate.Value);
                self.remainingOxygen = Mathf.Clamp(self.remainingOxygen + DivingBellOxygenRefillingFeature.Instance.RefillRate.Value * Time.deltaTime, 0f, self.maxOxygen);
            }
        }
    }
}
