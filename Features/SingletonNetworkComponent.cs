using MyceliumNetworking;
using System;
using UnityEngine;

namespace Tweaks.Features
{
    public abstract class SingletonNetworkComponent<THandler> : MonoBehaviour where THandler : MonoBehaviour
    {
        private ManualLogSource? _logger;
        protected abstract BepInEx.Logging.ManualLogSource LogSource { get; }
        protected ManualLogSource Logger => _logger ??= new(LogSource, GetType().Name);
        protected abstract uint MOD_ID { get; }
        public static SingletonNetworkComponent<THandler>? Instance { get; private set; }
        public static THandler? TypedInstance => Instance as THandler;

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Logger.LogWarning($"This singleton already exists. Destroying self");
                Destroy(gameObject);
                return;
            }

            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags |= HideFlags.DontSave;
            Instance = this;
            MyceliumNetwork.RegisterNetworkObject(this, MOD_ID);
            SuccessfulAwake();
        }
        protected virtual void SuccessfulAwake() { }
        protected virtual void OnDestroy()
        {
            if (Instance == this)
            {
                Debug.LogError("Goodbye world");
                Instance = null;
                MyceliumNetwork.DeregisterNetworkObject(this, MOD_ID);
            }
        }

        protected static void Send(string methodName, ReliableType reliable, params object[] parameters) => MyceliumNetwork.RPC(Instance?.MOD_ID ?? throw new InvalidOperationException($"Cannot send RPC. '{typeof(THandler).Name}' is not initialized"), methodName, reliable, parameters);
    }
}
