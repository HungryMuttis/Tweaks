namespace Tweaks.Features.OxygenSettings
{
    internal class PlayerPatch
    {
        public static void Init()
        {
            On.Player.Start += Player_Start;
        }

        private static System.Collections.IEnumerator Player_Start(On.Player.orig_Start orig, Player self)
        {
            self.data.remainingOxygen = OxygenSettingsFeature.Instance.MaxOxygen.Value;
            return orig(self);
        }
    }
}
