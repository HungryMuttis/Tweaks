using MyceliumNetworking;
using UnityEngine;

namespace Tweaks.Features.Commands
{
    internal class PlayerNetworkHandler : MonoBehaviour
    {
        private Player? player;
        private int viewId;

        void Awake()
        {
            player = GetComponent<Player>();
            if (player == null || player.refs.view == null)
            {
                CommandsFeature.Error($"Could not find Player component of PhotonView. Destroying self.");
                Destroy(this);
                return;
            }

            viewId = player.refs.view.ViewID;
            MyceliumNetwork.RegisterNetworkObject(this, Tweaks.MOD_ID, viewId);
        }

        void OnDestroy()
        {
            MyceliumNetwork.DeregisterNetworkObject(this, Tweaks.MOD_ID, viewId);
        }

        [CustomRPC]
        public void SetOxygenRPC(float newOxygenValue)
        {
            if (player != null && player.data != null)
            {
                player.data.remainingOxygen = Mathf.Clamp(newOxygenValue, 0f, player.data.maxOxygen);
                CommandsFeature.Debug($"Set player '{player.refs.view.Owner.NickName}' oxygen to {newOxygenValue}");
            }
        }

        public static void SendSetOxygenRequest(Player targetPlayer, float oxygen, bool percent = false)
        {
            if (targetPlayer == null)
            {
                Debug.LogError($"[{nameof(PlayerNetworkHandler)}] Target player cannot be null.");
                return;
            }

            int? viewId = targetPlayer.refs.view?.ViewID;
            string playerName = targetPlayer.refs.view?.Owner?.NickName ?? "Unknown Player";

            if (viewId == null)
            {
                Debug.LogError($"[{nameof(PlayerNetworkHandler)}] Player '{playerName}' has a null ViewID.");
                return;
            }

            float oxygenValue = percent ? targetPlayer.data.maxOxygen * oxygen / 100f : oxygen;

            MyceliumNetwork.RPCMasked(Tweaks.MOD_ID, nameof(SetOxygenRPC), ReliableType.Reliable, viewId.Value, oxygenValue);
            CommandsFeature.Debug($"Sent RPC to '{playerName}' to set oxygen to {oxygenValue} using mask {viewId.Value}");
            Debug.Log($"Player '{playerName}' oxygen set to {oxygenValue}");
        }
    }
}
