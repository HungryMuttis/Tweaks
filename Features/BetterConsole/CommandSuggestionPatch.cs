using HarmonyLib;
using System.Linq;
using System.Reflection;
using System.Text;
using Zorro.Core.CLI;

namespace Tweaks.Features.BetterConsole
{
    public class CommandSuggestionPatch
    {
        private static ConsoleCommand[]? ConsoleCommands;
        private static readonly FieldInfo SelectedParameterIndexField = AccessTools.Field(typeof(CommandSuggestion), "selectedParameterIndex");

        internal static void Init()
        {
            HarmonyPatcher.Patch(
                typeof(CommandSuggestion), nameof(CommandSuggestion.GetDisplayTextWithMaxParameter),
                prefix: (typeof(CommandSuggestionPatch), nameof(GetDisplayTextWithMaxParameter_Prefix))
            );
        }

        public static bool GetDisplayTextWithMaxParameter_Prefix(CommandSuggestion __instance, int maxParameterIndex, bool color, ref string __result)
        {
            ConsoleCommands ??= (ConsoleCommand[])AccessTools.Field(typeof(ConsoleHandler), "m_consoleCommands").GetValue(null);
            if (ConsoleCommands == null)
                return true;

            ConsoleCommand cmd = ConsoleCommands.FirstOrDefault(c => c.DomainName == __instance.Domain && c.Command == __instance.Command);
            if (cmd.MethodInfo == null)
                return true;

            ConsoleCommandAttribute? customAttr = cmd.MethodInfo?.GetCustomAttribute<ConsoleCommandAttribute>();

            StringBuilder paramsText = new();
            for (int i = 0; i < __instance.ParameterInfos.Length && i < maxParameterIndex; i++)
            {
                ParameterInfo paramInfo = __instance.ParameterInfos[i];
                bool selected = i == (int)SelectedParameterIndexField.GetValue(__instance);

                string paramDisplay = selected && customAttr?.Arguments != null && i < customAttr.Arguments.Length && !string.IsNullOrEmpty(customAttr.Arguments[i]) ? customAttr.Arguments[i] : paramInfo.Name;

                if (color)
                    paramsText.Append(selected ? "<color=#ffffff>" : "<color=#cccaca>");

                paramsText.Append($" {(paramInfo.IsOptional ? '[' : '<')}{paramDisplay} ({paramInfo.ParameterType.Name}){(paramInfo.IsOptional ? ']' : '>')}");
            }

            string fullCommand = __instance.FullCommand;
            if (color)
                fullCommand = "<color=#cccaca>" + fullCommand;

            __result = fullCommand + paramsText.ToString();

            return false;
        }
    }
}
