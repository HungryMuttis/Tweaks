using HarmonyLib;
using Photon.Pun;
using System.Reflection;
using UnityEngine;

namespace Tweaks.Features.ReturnItems
{
    public static class ItemInstancePatch
    {
        private static readonly FieldInfo isHeldField = AccessTools.Field(typeof(ItemInstance), "isHeld") ?? throw new MemberNotFoundException($"{nameof(ItemInstance)} does not contain a field named 'isHeld'");
        private static readonly FieldInfo rigField = AccessTools.Field(typeof(ItemInstance), "rig") ?? throw new MemberNotFoundException($"{nameof(ItemInstance)} does not contain a field named 'rig'");
        private static readonly FieldInfo m_BoughtItemPositionField = AccessTools.Field(typeof(ShopHandler), "m_BoughtItemPosition") ?? throw new MemberNotFoundException($"{nameof(ShopHandler)} does not contain a field named 'm_BoughtItemPosition'");
        private static Transform? m_BoughtItemPosition;

        internal static void Init()
        {
            Tweaks.Patcher.SaveInfo();
            Tweaks.Patcher.Patch(
                nameof(ItemInstance.Update),
                postfix: nameof(Update_Postfix)
            );
        }

        // PATCHES //
        public static void Update_Postfix(ItemInstance __instance)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            Rigidbody rig = (Rigidbody)rigField.GetValue(__instance);
            if ((bool)isHeldField.GetValue(__instance) || rig == null || __instance.m_guid.IsNone) return;

            if (rig.position.y >= -150f && rig.position.y <= 100) return;

            if (!PhotonGameLobbyHandler.IsSurface || Hospital.instance == null) return;

            if (m_BoughtItemPosition == null)
                m_BoughtItemPosition = (Transform)m_BoughtItemPositionField.GetValue(ShopHandler.Instance);

            Vector3 pos = m_BoughtItemPosition.position;
            pos.y = 100;
            if (rig.position.y > 100f) rig.velocity *= -1;
            rig.position = pos;
        }
    }
}
