using System.Collections;

namespace Tweaks.Features.Commands.Patches
{
    internal class PlayerPatch
    {
        internal static void Init()
        {
            On.Player.Start += Player_Start;
        }

        private static IEnumerator Player_Start(On.Player.orig_Start orig, Player self)
        {
            yield return orig(self);

            if (self.gameObject.GetComponent<PlayerNetworkHandler>() == null)
                self.gameObject.AddComponent<PlayerNetworkHandler>();
        }
    }
}
