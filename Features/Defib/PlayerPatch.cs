using Tweaks.Enums;
using UnityEngine;

namespace Tweaks.Features.Defib
{
    internal class PlayerPatch
    {
        internal static void Init()
        {
            On.Player.RPCA_PlayerRevive += Player_RPCA_PlayerRevive;
        }

        // HOOKS //
        private static void Player_RPCA_PlayerRevive(On.Player.orig_RPCA_PlayerRevive orig, Player self)
        {
            self.data.remainingOxygen = CalculateOxygen(self.data.remainingOxygen, self.data.maxOxygen);
            orig(self);
        }

        // HELPER METHODS //
        private static float CalculateOxygen(float remainingOxygen, float maxOxygen)
        {
            DefibFeature feature = DefibFeature.Instance;

            remainingOxygen = remainingOxygen < 0 ? 0 : remainingOxygen;
            float amount = feature.Type.Value == Type.seconds ? feature.Amount.Value : maxOxygen * feature.Amount.Value / 100;
            float calculatedOxygen;

            switch (feature.Setting.Value)
            {
                case Setting.add:
                    calculatedOxygen = remainingOxygen + amount;
                    break;
                case Setting.cset:
                    calculatedOxygen = Mathf.Max(remainingOxygen, amount);
                    break;
                case Setting.set:
                    calculatedOxygen = amount;
                    break;
                default:
                    BetterDefib.Logger.LogWarning("Invalid setting: 'Oxygen.Setting'. Review config file.");
                    calculatedOxygen = remainingOxygen;
                    break;
            }

            return Mathf.Clamp(calculatedOxygen, 0f, maxOxygen);
        }
    }
}
