using MyceliumNetworking;
using Photon.Pun;
using UnityEngine;

namespace Tweaks.Features
{
    public abstract class NetworkComponent<THandler, TParent> : MonoBehaviour where THandler : NetworkComponent<THandler, TParent> where TParent : class
    {
        private ManualLogSource? _logger;
        protected abstract BepInEx.Logging.ManualLogSource LogSource { get; }
        protected ManualLogSource Logger => _logger ??= new(LogSource, GetType().Name);
        protected abstract uint MOD_ID { get; }
        protected TParent? ParentComponent { get; private set; }
        protected int ViewId { get; private set; }

        protected virtual void Awake()
        {
            ParentComponent = GetComponent<TParent>();
            if (ParentComponent == null)
            {
                Logger.LogError($"Could not find the required component of type '{typeof(TParent).Name}'. Destroying self");
                Destroy(this);
                return;
            }

            PhotonView view = GetComponentInParent<PhotonView>();
            if (view == null)
            {
                Logger.LogError($"Could not find the required component '{nameof(PhotonView)}'. Destroying self");
                Destroy(this);
                return;
            }

            ViewId = view.ViewID;
            MyceliumNetwork.RegisterNetworkObject(this, MOD_ID, ViewId);
            SuccessfulAwake();
        }
        protected virtual void SuccessfulAwake() { }
        protected virtual void OnDestroy()
        {
            if (ViewId != 0)
                MyceliumNetwork.DeregisterNetworkObject(this, MOD_ID, ViewId);
        }

        protected static bool Send(Player targetPlayer, string methodName, ReliableType reliable, params object[] parameters)
        {
            THandler handler = targetPlayer.GetComponent<THandler>();
            if (handler == null)
            {
                Debug.LogError($"[{typeof(THandler).Name}] Target player does not have a handler");
                return false;
            }

            MyceliumNetwork.RPCMasked(handler.MOD_ID, methodName, reliable, handler.ViewId, parameters);
            return true;
        }
    }
}
