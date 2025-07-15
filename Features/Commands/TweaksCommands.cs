using UnityEngine;
using Zorro.Core.CLI;

namespace Tweaks.Features.Commands
{
    public class TweaksCommands
    {
        private static bool CheckEnabled()
        {
            if (!CommandsFeature.Instance.Enabled.Value)
            {
                Tweaks.Logger.LogWarning($"Feature {CommandsFeature.Instance.FeatureName} is not enabled");
                return false;
            }
            return true;
        }

        [ConsoleCommand]
        public static void SetPlayerOxygen(string username, float oxygen)
        {
            if (!CheckEnabled()) return;

            if (string.IsNullOrEmpty(username))
            {
                Debug.LogError($"Usage: {nameof(TweaksCommands)}.{nameof(SetPlayerOxygen)} <Username> <Value>");
                return;
            }

            PlayerNetworkHandler.SendSetOxygenRequest(username, oxygen);
        }
    }
}
