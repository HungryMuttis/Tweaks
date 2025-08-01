using UnityEngine;

namespace Tweaks.Features.Commands.Patches
{
    public static class PlayerPatch
    {
        public static float ThrowStrengthMultiplier = 1f;

        internal static void Init()
        {
            Tweaks.Patcher.SaveInfo();
            Tweaks.Patcher.Patch(
                nameof(Player.RequestCreatePickup), [typeof(byte), typeof(ItemInstanceData), typeof(Vector3), typeof(Quaternion), typeof(Vector3), typeof(Vector3)],
                prefix: nameof(RequestCreatePickup_Prefix)
            );
            Tweaks.Patcher.Patch(
                "Start",
                postfix: nameof(Start_Postfix)
            );
        }

        // PATCHES //
        public static void RequestCreatePickup_Prefix(ref Vector3 vel)
        {
            vel *= ThrowStrengthMultiplier;
        }
        public static void Start_Postfix(Player __instance)
        {
            if (!__instance.ai && __instance.gameObject.GetComponent<PlayerNetworkHandler>() == null)
                __instance.gameObject.AddComponent<PlayerNetworkHandler>();
        }
    }
}
