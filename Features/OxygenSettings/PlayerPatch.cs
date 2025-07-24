namespace Tweaks.Features.OxygenSettings
{
    public static class PlayerPatch
    {
        internal static void Init()
        {
            Tweaks.Patcher.SaveInfo();
            Tweaks.Patcher.Patch(
                "Start",
                prefix: nameof(Start_Prefix)
            );
        }

        // PATCHES //
        public static void Start_Prefix(Player __instance)
        {
            __instance.data.remainingOxygen = OxygenSettingsFeature.Instance.MaxOxygen.Value;
        }
    }
}
