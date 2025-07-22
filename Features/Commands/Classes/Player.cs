using Tweaks.Features.BetterConsole;
using UnityEngine;

namespace Tweaks.Features.Commands.Classes
{
    public class Player : CommandsClass
    {
        public static new bool Enabled => CommandsFeature.Instance.Enabled;

        [ConsoleCommand("Sets the remaining oxygen for the specified player", "", "Amount of oxygen", "Is amount percent")]
        public static void SetRemainingOxygen(global::Player Player, float Value, bool Percent = false)
        {
            if (Player is null || Player == null)
            {
                Debug.LogError("Enter a player");
                //Help(nameof(SetRemainingOxygen)); // TODO: fix help
                return;
            }

            PlayerNetworkHandler.SendSetOxygenRequest(Player, Value, Percent);
        }
    }
}