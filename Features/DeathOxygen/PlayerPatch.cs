using UnityEngine;

namespace Tweaks.Features.DeathOxygen
{
    internal class PlayerPatch
    {
        internal static void Init()
        {
            On.Player.PlayerData.UpdateValues += PlayerData_UpdateValues;
        }

        private static void PlayerData_UpdateValues(On.Player.PlayerData.orig_UpdateValues orig, Player.PlayerData self)
        {
            if (!self.dead)
            {
                orig(self);
                return;
            }

            bool orig_usingOxygen = self.usingOxygen;
            self.usingOxygen = false;

            try
            {
                orig(self);
            }
            finally
            {
                self.remainingOxygen -= DeathOxygenFeature.Instance.Consumption.Value * Time.deltaTime;
                self.usingOxygen = orig_usingOxygen;
            }
        }
    }
}
