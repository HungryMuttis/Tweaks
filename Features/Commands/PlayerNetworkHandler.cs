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
                Tweaks.Logger.LogError($"[{nameof(PlayerNetworkHandler)}] Could not find Player component of PhotonView. Destroying self.");
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
                Tweaks.Logger.LogDebug($"Set player '{player.refs.view.Owner.NickName}' oxygen to {newOxygenValue}");
            }
        }

        public static void SendSetOxygenRequest(string playerName, float oxygen)
        {
            if (PlayerHandler.instance == null || PlayerHandler.instance.players == null)
            {
                Debug.LogError($"[{nameof(PlayerNetworkHandler)}] PlayerHandler not ready.");
                return;
            }

            Player? targetPlayer = null;
            foreach (Player p in PlayerHandler.instance.players)
            {
                if (string.Equals(p.refs.view?.Owner?.NickName, playerName, System.StringComparison.Ordinal))
                {
                    targetPlayer = p;
                    break;
                }
            }

            if (targetPlayer == null)
            {
                Debug.LogError($"Could not find player '{playerName}'");
                return;
            }

            int? viewId = targetPlayer.refs.view?.ViewID;
            if (viewId == null)
            {
                Debug.LogError($"Player '{playerName}' ViewID is null");
                return;
            }

            MyceliumNetwork.RPCMasked(Tweaks.MOD_ID, nameof(SetOxygenRPC), ReliableType.Reliable, (int)viewId, oxygen);
            Tweaks.Logger.LogDebug($"Sent RPC to '{playerName}' to set oxygen to {oxygen} using mask {viewId}");
            Debug.Log($"Player '{playerName}' oxygen set to {oxygen}");
        }
    }
}
