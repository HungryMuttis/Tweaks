using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using Zorro.Core;
using Zorro.Core.CLI;

namespace Tweaks.Features.BetterConsole
{
    public class ConsolePagePatch
    {
        private static readonly MethodInfo FindSuggestions = AccessTools.Method(typeof(ConsolePage), "FindSuggestions");
        private static readonly FieldInfo ListViewField = AccessTools.Field(typeof(ConsolePage), "m_listView");
        private static readonly FieldInfo LogEntriesField = AccessTools.Field(typeof(ConsolePage), "m_logEntries");

        internal static void Init()
        {
            HarmonyPatcher.Patch(
                typeof(ConsolePage), nameof(ConsolePage.Update),
                postfix: (typeof(ConsolePagePatch), nameof(Update_Postfix))
            );
            HarmonyPatcher.Patch(
                typeof(ConsolePage), nameof(ConsolePage.LogRecieved),
                postfix: (typeof(ConsolePagePatch), nameof(Scrollfix_Postfix))
            );
            HarmonyPatcher.Patch(
                typeof(ConsolePage), "AttemptParseCommand",
                postfix: (typeof(ConsolePagePatch), nameof(Scrollfix_Postfix))
            );
        }

        // HOOKS //
        public static void Update_Postfix(ConsolePage __instance, ref Optionable<byte> ___m_selectedSuggestion, string ___m_currentInput)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ___m_selectedSuggestion = Optionable<byte>.None;
                FindSuggestions.Invoke(__instance, [___m_currentInput]);
            }
        }
        public static void Scrollfix_Postfix(object __instance)
        {
            ListView listView = (ListView)ListViewField.GetValue(__instance);
            if (listView == null) return;

            List<ConsoleLogEntry> logEntries = (List<ConsoleLogEntry>)LogEntriesField.GetValue(__instance);
            if (logEntries == null) return;

            listView.schedule.Execute(() => {
                listView.schedule.Execute(() =>
                {
                    if (logEntries.Count > 0)
                        listView.ScrollToItem(logEntries.Count - 1);
                });
            });
        }
    }
}
