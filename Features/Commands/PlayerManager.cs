using System;
using UnityEngine;

namespace Tweaks.Features.Commands
{
    public static class PlayerManager
    {
        public static void SetGravity(float gravity)
        {
            if (Player.localPlayer?.refs?.controller == null) return;
            Player.localPlayer.refs.controller.gravity = gravity;
        }

        public static void SetHealth(Player? player, float health)
        {
            if (player?.data == null) return;
            player.data.health = health;
        }
        public static void SetGravityDirection(Player? player, Vector3 direction)
        {
            if (player?.data == null) return;
            player.data.gravityDirection = direction;
        }
        public static void SetMaxOxygen(Player? player, float maxOxygen)
        {
            if (player?.data == null) return;
            player.data.maxOxygen = maxOxygen;
        }
        public static void SetRemainingOxygen(Player? player, float remainingOxygen, bool percent)
        {
            if (player?.data == null) return;
            player.data.remainingOxygen = percent ? player.data.maxOxygen * remainingOxygen / 100 : remainingOxygen;
        }

        public static void SetMaxHealth(float maxHealth, bool addHealth)
        {
            float healthAdded = maxHealth - Player.PlayerData.maxHealth;
            healthAdded = healthAdded < 0 ? 0 : healthAdded;
            Player.PlayerData.maxHealth = maxHealth;
            All(p =>
            {
                if (p?.data == null) return;
                p.data.health = Mathf.Clamp(addHealth ? healthAdded + p.data.health : p.data.health, 0f, maxHealth);
            });
        }

        // HELPER METHODS //
        private static void All(Action<Player?> action)
        {
            if (PlayerHandler.instance?.players == null) return;
            foreach (Player? player in PlayerHandler.instance.players)
                action(player);
        }
    }
}
