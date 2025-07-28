using MyceliumNetworking;
using UnityEngine;
using CWAPI;

namespace Tweaks.Features.Commands
{
    internal class PlayersNetworkHandler : SingletonNetworkComponent<PlayerNetworkHandler>
    {
        protected override BepInEx.Logging.ManualLogSource LogSource => Tweaks.Logger;
        protected override uint MOD_ID => Tweaks.MOD_ID;

        [CustomRPC] void SetMaxHealth(float maxHealth, bool addHealth)
        {
            Player.PlayerData.maxHealth = maxHealth;
            PlayerHandler.instance.playersAlive.ForEach(p => p.data.health = addHealth ? maxHealth : Mathf.Clamp(p.data.health, 0f, maxHealth));
        }
        public static void SendMaxHealth(float maxHealth, bool addhealth)
        {
            Send(nameof(SetMaxHealth), ReliableType.Reliable,
                maxHealth, addhealth
            );
        }
    }
}