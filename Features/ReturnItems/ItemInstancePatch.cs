using HarmonyLib;
using Photon.Pun;
using System.Reflection;
using UnityEngine;

namespace Tweaks.Features.ReturnItems
{
    internal class ItemInstancePatch
    {
        private static readonly FieldInfo isHeldField = typeof(ItemInstance).GetField("isHeld", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new MemberNotFoundException($"{nameof(ItemInstance)} does not contain a field named 'isHeld'");
        private static readonly FieldInfo rigField = typeof(ItemInstance).GetField("rig", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new MemberNotFoundException($"{nameof(ItemInstance)} does not contain a field named 'rig'");
        private static readonly FieldInfo m_BoughtItemPositionField = typeof(ShopHandler).GetField("m_BoughtItemPosition", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new MemberNotFoundException($"{nameof(ShopHandler)} does not contain a field named 'm_BoughtItemPosition'");
        private static Transform? m_BoughtItemPosition;

        internal static void Init()
        {
            On.ItemInstance.Update += ItemInstance_Update;
        }

        // HOOKS //
        private static void ItemInstance_Update(On.ItemInstance.orig_Update orig, ItemInstance self)
        {
            orig(self);

            if (!PhotonNetwork.IsMasterClient) return;
            Rigidbody rig = (Rigidbody)rigField.GetValue(self);
            if ((bool)isHeldField.GetValue(self) || rig == null || self.m_guid.IsNone) return;

            bool revVel = false;
            if (rig.position.y >= -150f)
                if (rig.position.y <= 100) return;
                else revVel = true;

            if (!PhotonGameLobbyHandler.IsSurface || Hospital.instance == null) return;

            if (m_BoughtItemPosition == null)
                m_BoughtItemPosition = (Transform)m_BoughtItemPositionField.GetValue(ShopHandler.Instance);

            Vector3 pos = m_BoughtItemPosition.position;
            pos.y = 100;
            rig.position = pos;
            if (revVel) rig.velocity *= -1;
        }
    }
}
