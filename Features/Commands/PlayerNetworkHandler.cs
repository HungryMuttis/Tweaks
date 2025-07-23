using MyceliumNetworking;
using Tweaks.Features.Commands.Patches;
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

        [CustomRPC] public void SetOxygenRPC(float oxygen)
        {
            if (player == null || player.data == null) return;

            player.data.remainingOxygen = Mathf.Clamp(oxygen, 0f, player.data.maxOxygen);
        }
        public static void SendSetOxygen(Player targetPlayer, float oxygen, bool percent = false)
        {
            Send(nameof(SetOxygenRPC), ReliableType.Reliable, targetPlayer,
                percent ? targetPlayer.data.maxOxygen * oxygen / 100f : oxygen
            );
        }

        [CustomRPC] public void SetMaxHealthRPC(float maxHealth)
        {
            Player.PlayerData.maxHealth = maxHealth;
            CommandsFeature.Debug($"Set max health to '{maxHealth}' for everyone");
        }
        public static void SendSetMaxHealth(float maxHealth)
        {
            Send(nameof(SetMaxHealthRPC), ReliableType.Reliable, null,
                maxHealth
            );
        }

        [CustomRPC] public void SetThrowStrengthMultiplier(float multiplier)
        {
            if (player == null) return;

            if (Player.localPlayer != player) return;

            PlayerPatch.ThrowStrengthMultiplier = multiplier;
        }
        public static void SendThrowStrengthMultiplier(Player targetPlayer, float multiplier)
        {
            Send(nameof(SetThrowStrengthMultiplier), ReliableType.Reliable, targetPlayer,
                multiplier
            );
        }

        // HELPER METHODS //
        private static bool Send(string methodName, ReliableType reliable, Player? targetPlayer = null, params object[] parameters)
        {
            if (targetPlayer == null)
            {
                MyceliumNetwork.RPC(Tweaks.MOD_ID, methodName, reliable, parameters);
                return true;
            }

            int? viewId = targetPlayer.refs.view?.ViewID;
            if (viewId == null)
            {
                Debug.LogError($"[{nameof(PlayerNetworkHandler)}] Player '{targetPlayer.refs.view?.Owner?.NickName ?? "Unknown Player"}' has a null ViewID.");
                return false;
            }

            MyceliumNetwork.RPCMasked(Tweaks.MOD_ID, methodName, reliable, viewId.Value, parameters);
            return true;
        }
    }
}
