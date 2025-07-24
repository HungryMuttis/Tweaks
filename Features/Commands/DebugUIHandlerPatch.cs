using Photon.Pun;
using UnityEngine;
using Zorro.Core.CLI;

namespace Tweaks.Features.Commands
{
    public static class DebugUIHandlerPatch
    {
        internal static bool AllowConsole = false;

        internal static void Init()
        {
            Tweaks.Patcher.SaveInfo(assembly: "Zorro.Core.Runtime", @namespace: "Zorro.Core.CLI");
            Tweaks.Patcher.Patch(
                "Update",
                postfix: nameof(Update_Postfix)
            );
        }

        // PATCHES //
        public static void Update_Postfix(DebugUIHandler __instance)
        {
            if (!Input.GetKeyDown(CommandsFeature.Instance.OpenConsoleKey.Value)) return;

            if (__instance.IsOpen)
            {
                __instance.Hide();
                return;
            }

            if (!PhotonNetwork.InRoom || (!PhotonNetwork.IsMasterClient && !AllowConsole)) return;

            __instance.Show();
        }
    }
}
