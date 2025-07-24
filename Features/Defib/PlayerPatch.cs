using Tweaks.Enums;
using UnityEngine;

namespace Tweaks.Features.Defib
{
    public static class PlayerPatch
    {
        internal static void Init()
        {
            Tweaks.Patcher.SaveInfo();
            Tweaks.Patcher.Patch(
                "RPCA_PlayerRevive",
                prefix: nameof(RPCA_PlayerRevive_Prefix)
            );
        }

        // PATCHES //
        public static void RPCA_PlayerRevive_Prefix(Player __instance)
        {
            __instance.data.remainingOxygen = CalculateOxygen(__instance.data.remainingOxygen, __instance.data.maxOxygen);
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
                    DefibFeature.Error("Invalid setting: 'Oxygen.Setting'. Review config file.");
                    calculatedOxygen = remainingOxygen;
                    break;
            }

            return Mathf.Clamp(calculatedOxygen, 0f, maxOxygen);
        }
    }
}
