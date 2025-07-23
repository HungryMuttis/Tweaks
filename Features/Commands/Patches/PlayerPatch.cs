using System.Collections;
using UnityEngine;

namespace Tweaks.Features.Commands.Patches
{
    internal class PlayerPatch
    {
        public static float ThrowStrengthMultiplier = 1f;

        internal static void Init()
        {
            On.Player.Start += Player_Start;
            On.Player.RequestCreatePickup_byte_ItemInstanceData_Vector3_Quaternion_Vector3_Vector3 += Player_RequestCreatePickup_byte_ItemInstanceData_Vector3_Quaternion_Vector3_Vector3;
        }

        // HOOKS //
        private static IEnumerator Player_Start(On.Player.orig_Start orig, Player self)
        {
            yield return orig(self);

            if (self.gameObject.GetComponent<PlayerNetworkHandler>() == null)
                self.gameObject.AddComponent<PlayerNetworkHandler>();
        }
        private static void Player_RequestCreatePickup_byte_ItemInstanceData_Vector3_Quaternion_Vector3_Vector3(On.Player.orig_RequestCreatePickup_byte_ItemInstanceData_Vector3_Quaternion_Vector3_Vector3 orig, Player self, byte itemID, ItemInstanceData data, Vector3 pos, Quaternion rot, Vector3 vel, Vector3 angVel)
            => orig(self, itemID, data, pos, rot, vel * ThrowStrengthMultiplier, angVel);
    }
}
