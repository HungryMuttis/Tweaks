using HarmonyLib;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Zorro.Core.CLI;

namespace Tweaks.Features.BetterConsole
{
    public class ConsoleHandlerPatch
    {
        private class AmbiguityContext(List<ConsoleCommand> overloads, List<string> arguments)
        {
            public readonly List<ConsoleCommand> Overloads = overloads;
            public readonly List<string> Arguments = arguments;
        }
        private static AmbiguityContext? _ambiguityContext;
        private static Dictionary<Type, CLITypeParser> Parsers = [];
        private static readonly MethodInfo ConvertMethod = AccessTools.Method(typeof(ConsoleHandler), "ConvertParameter");
        private static ConsoleCommand[] ConsoleCommands = null!;

        internal static void Init()
        {
            HarmonyPatcher.Patch(
                typeof(ConsoleHandler), nameof(ConsoleHandler.Initialize),
                postfix: (typeof(ConsoleHandlerPatch), nameof(Initialize_Postfix))
            );
            HarmonyPatcher.Patch(
                typeof(ConsoleHandler), nameof(ConsoleHandler.ProcessCommand),
                prefix: (typeof(ConsoleHandlerPatch), nameof(ProcessCommand_Prefix))
            );
            HarmonyPatcher.Patch(
                typeof(ConsoleHandler), nameof(ConsoleHandler.FindSuggestions),
                postfix: (typeof(ConsoleHandlerPatch), nameof(FindSuggestions_Postfix))
            );
            HarmonyPatcher.Patch(
                typeof(ConsoleHandler), "<FindSuggestions>g__FindCommandSuggestions|13_1", [typeof(string)],
                postfix: (typeof(ConsoleHandlerPatch), nameof(FilterCommandSuggestions_Postfix))
            );
        }

        // HOOKS //
        public static void Initialize_Postfix()
        {
            FieldInfo commands = AccessTools.Field(typeof(ConsoleHandler), "m_consoleCommands");

            BetterConsoleFeature.Debug("Searching for custom commands...");

            List<MethodInfo> customCommands = FindCustomCommands();

            if (customCommands.Count == 0)
            {
                BetterConsoleFeature.Debug("No custom commands found.");
                return;
            }

            List<ConsoleCommand> cmds = [.. (ConsoleCommand[])commands.GetValue(null)];

            int commandsAdded = 0;
            foreach (MethodInfo method in customCommands)
            {
                Type declaringType = method.DeclaringType;
                if (declaringType == null) continue;

                bool add = false;
                if (!typeof(CommandsClass).IsAssignableFrom(declaringType))
                    add = true;
                else
                {
                    PropertyInfo? enabled = declaringType.GetProperty("Enabled", BindingFlags.Public | BindingFlags.Static);
                    if (enabled != null && (bool)enabled.GetValue(null))
                        add = true;
                }

                if (add)
                {
                    cmds.Add(new ConsoleCommand(method));
                    commandsAdded++;
                }
            }

            commands.SetValue(null, cmds.ToArray());
            ConsoleCommands = (ConsoleCommand[])commands.GetValue(null);

            BetterConsoleFeature.Debug($"Added {commandsAdded} new commands. Total commands: {cmds.Count}");

            Parsers = (Dictionary<Type, CLITypeParser>)AccessTools.Field(typeof(ConsoleHandler), "m_typeParsers").GetValue(null);
        }
        public static bool ProcessCommand_Prefix(string command, ref bool __result)
        {
            if (HandleAmbiguity(command, ref __result)) return false;
            _ambiguityContext = null;

            if (command.Equals("Help", StringComparison.OrdinalIgnoreCase) || command.TrimEnd().Equals("Help.Help", StringComparison.OrdinalIgnoreCase))
            {
                IEnumerable<string> domains = ConsoleCommands.Select(c => c.DomainName).Distinct().OrderBy(d => d);
                StringBuilder sb = new();
                foreach (string cdomain in domains)
                    sb.AppendLine($"  - {cdomain}");
                sb.Append($"Type '<Domain>.Help' for help with a specific domain");
                ConsoleLog(sb.ToString());
                __result = true;
                return false;
            }

            string[]? parts = Zorro.Core.StringUtility.SplitOnFirstOfChar(command, '.');
            if (parts == null)
            {
                __result = false;
                return false;
            }

            string domain = parts[0];
            string trimmed = parts[1].TrimEnd();
            string commandName = trimmed.Split(' ').First();
            string rawArgs = trimmed.Contains(' ') ? trimmed[commandName.Length..].Trim() : "";
            List<string> providedArgs = ParseArguments(rawArgs);

            if (commandName.Equals("Help", StringComparison.OrdinalIgnoreCase))
            {
                List<ConsoleCommand> commands = [.. ConsoleCommands.Where(c => c.DomainName.Equals(domain, StringComparison.OrdinalIgnoreCase))];
                if (!commands.Any())
                {
                    ConsoleError($"No domain named '{domain}' found");
                    __result = true;
                    return false;
                }

                StringBuilder sb = new();
                bool first = true;
                foreach (ConsoleCommand cmd in commands)
                {
                    ConsoleCommandAttribute? attr = cmd.MethodInfo.GetCustomAttribute<ConsoleCommandAttribute>();
                    if (!first) sb.AppendLine();
                    else first = false;
                    sb.Append(cmd.Command);
                    sb.Append(' ');
                    sb.Append(string.Join(" ", Enumerable.Range(0, cmd.ParameterInfo.Length).Select(i =>
                    {
                        string param = $"{(attr?.Arguments != null && i < attr.Arguments.Length && !string.IsNullOrEmpty(attr.Arguments[i]) ? attr.Arguments[i] : cmd.ParameterInfo[i].Name)} ({cmd.ParameterInfo[i].ParameterType.Name})";
                        return cmd.ParameterInfo[i].IsOptional ? $"[{param}]" : $"<{param}>";
                    })));
                    if (attr?.Description != null)
                    {
                        sb.AppendLine();
                        sb.Append("  - ");
                        sb.Append(attr?.Description);
                    }
                }
                ConsoleLog(sb.ToString());
                __result = true;
                return false;
            }

            List<ConsoleCommand> cmds = [.. ConsoleCommands.Where(c => c.DomainName == domain && c.Command == commandName)];
            if (cmds.Count == 0)
            {
                __result = false;
                return false;
            }

            List<ConsoleCommand> validCmds = FindValidCmds(cmds, providedArgs);

            return HandleCommandExecution(validCmds, providedArgs, commandName, ref __result);
        }
        public static void FindSuggestions_Postfix(string input, ref List<Suggestion> __result)
        {
            if (input == null) return;

            if (!input.Contains('.') && "Help".StartsWith(input, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(input))
            {
                if (!__result.Any(s => s is DomainSuggestion ds && ds.Domain == "Help"))
                    __result.Insert(0, new DomainSuggestion("Help"));
            }

            if (input.Contains(".") && !input.Contains(' '))
            {
                string[] cparts = input.Split('.');
                if (!ConsoleCommands.Any(c => c.DomainName.Equals(cparts[0], StringComparison.OrdinalIgnoreCase)))
                {
                    __result.Clear();
                    return;
                }
                if ("Help".StartsWith(cparts[1], StringComparison.OrdinalIgnoreCase))
                    if (!__result.Any(s => s is CommandSuggestion cs && cs.Command == "Help"))
                        __result.Add(new CommandSuggestion(cparts[0], "Help", []));
            }

            if (__result.Any(s => s is ParameterSuggestion) || !__result.Any(s => s is CommandSuggestion)) return;

            string[] parts = input.Split(' ');
            if (parts.Length < 2 || !input.Contains('.'))
                return;

            int index = parts.Length - 2;
            string text = input.EndsWith(" ") ? "" : parts.Last();
            string line = "";
            for (int i = 0; i < parts.Length - 1; i++)
                line += parts[i] + " ";

            List<Suggestion> suggestions = [];
            foreach (Suggestion suggestion in __result)
            {
                suggestions.Add(suggestion);

                if (suggestion is not CommandSuggestion cmdSuggestion || cmdSuggestion.ParameterInfos.Length <= index)
                    continue;

                cmdSuggestion.HighlightParameter(index);
                ParameterInfo info = cmdSuggestion.ParameterInfos[index];
                if (!Parsers.TryGetValue(info.ParameterType, out CLITypeParser parser))
                    continue;

                List<ParameterAutocomplete> autocompletes = parser.FindAutocomplete(text);
                string display = cmdSuggestion.GetDisplayTextWithMaxParameter(index, false);
                foreach (ParameterAutocomplete autocomplete in autocompletes)
                    suggestions.Add(new ParameterSuggestion(display, line, autocomplete.Value));
            }

            __result = suggestions;
        }
        public static void FilterCommandSuggestions_Postfix(string input, ref List<Suggestion> __result)
        {
            if (__result == null || __result.Count == 0) return;

            string[]? parts = Zorro.Core.StringUtility.SplitOnFirstOfChar(input, '.');
            if (parts == null) return;

            string name = parts[1].Split(' ').First();
            string rawArgs = "";
            if (parts[1].Length > name.Length)
                rawArgs = parts[1][name.Length..];

            List<string> provided = ParseArguments(rawArgs.TrimStart(' '));
            bool trailingSpace = rawArgs.EndsWith(" ");

            List<Suggestion> suggestions = [];
            foreach (Suggestion suggestion in __result)
            {
                if (suggestion is not CommandSuggestion cmdSuggestion)
                {
                    suggestions.Add(suggestion);
                    continue;
                }

                if (trailingSpace && cmdSuggestion.ParameterInfos.Length <= provided.Count) continue;
                if (cmdSuggestion.ParameterInfos.Length < provided.Count) continue;

                bool possible = true;
                for (int i = 0; i < provided.Count; i++)
                {
                    string arg = provided[i];
                    ParameterInfo param = cmdSuggestion.ParameterInfos[i];

                    if ((i == provided.Count - 1) && !trailingSpace)
                    {
                        if (!PossibleArgument(arg, param.ParameterType))
                        {
                            possible = false;
                            break;
                        }
                    }
                    else if (!TryConvertParameter(arg, param.ParameterType, out object? _))
                    {
                        possible = false;
                        break;
                    }
                }

                if (possible)
                    suggestions.Add(suggestion);
            }

            __result = suggestions;
        }

        // HELPER METHODS //
        private static bool HandleAmbiguity(string command, ref bool result)
        {
            if (!int.TryParse(command, out int selection) || selection <= 0)
                return false;
            result = true;

            if (_ambiguityContext == null)
            {
                ConsoleError("No ambiguous command to resolve. Enter a full command.");
                return true;
            }

            int selected = selection - 1;
            if (selected >= _ambiguityContext.Overloads.Count)
            {
                ConsoleError($"Invalid selection. Enter a number between 1 and {_ambiguityContext.Overloads.Count}");
                return true;
            }

            ExecuteCommand(_ambiguityContext.Overloads[selected], _ambiguityContext.Arguments);
            _ambiguityContext = null;
            return true;
        }
        private static bool HandleCommandExecution(List<ConsoleCommand> validCmds, List<string> providedArgs, string commandName, ref bool result)
        {
            if (validCmds.Count == 0)
            {
                ConsoleError($"Error: No overload for '{commandName}' matches the provided argument types.");
                result = true;
                return false;
            }

            if (validCmds.Count > 1)
            {
                _ambiguityContext = new(validCmds, providedArgs);
                StringBuilder sb = new();
                sb.AppendLine($"Ambiguous command. {validCmds.Count} overloads match the arguments.");
                sb.Append("Enter the number of the command you want to execute:");
                for (int i = 0; i < validCmds.Count; i++)
                {
                    ConsoleCommand cmd = validCmds[i];
                    sb.AppendLine();
                    sb.Append($"  {i + 1}: {cmd.Command}(");
                    sb.Append(string.Join(", ", cmd.ParameterInfo.Select(p => $"{p.ParameterType.Name} {p.Name}")));
                    sb.Append(")");
                }
                ConsoleError(sb.ToString());
                result = true;
                return false;
            }

            ExecuteCommand(validCmds[0], providedArgs);
            result = true;
            return false;
        }
        private static bool PossibleArgument(string arg, Type paramType)
        {
            if (string.IsNullOrEmpty(arg)) return true;
            if (paramType == typeof(string)) return true;
            if (paramType == typeof(float)) return float.TryParse(arg, NumberStyles.Any, CultureInfo.InvariantCulture, out float _);
            if (paramType == typeof(int)) return int.TryParse(arg, out int _);
            if (Parsers.TryGetValue(paramType, out CLITypeParser parser))
                return parser.FindAutocomplete(arg).Any();
            return true;
        }
        private static List<ConsoleCommand> FindValidCmds(List<ConsoleCommand> candidates, List<string> providedArgs)
        {
            List<ConsoleCommand> validCmds = [];
            foreach (ConsoleCommand candidate in candidates)
            {
                if (providedArgs.Count < candidate.ParameterInfo.Count(p => !p.IsOptional) || providedArgs.Count > candidate.ParameterInfo.Length)
                    continue;

                if (Enumerable.Range(0, providedArgs.Count).All(i => TryConvertParameter(providedArgs[i], candidate.ParameterInfo[i].ParameterType, out object? _)))
                    validCmds.Add(candidate);
            }
            return validCmds;
        }
        private static void ExecuteCommand(ConsoleCommand command, List<string> providedArgs)
        {
            ParameterInfo[] parameters = command.ParameterInfo;
            object[] finalArgs = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                if (i < providedArgs.Count)
                {
                    if (TryConvertParameter(providedArgs[i], parameters[i].ParameterType, out object? convertedValue))
                        finalArgs[i] = convertedValue;
                    else
                    {
                        ConsoleError($"Failed to convert argument '{providedArgs[i]}' to type {parameters[i].ParameterType.Name}.");
                        return;
                    }
                }
                else
                    finalArgs[i] = parameters[i].DefaultValue;
            }

            try
            {
                command.MethodInfo.Invoke(null, finalArgs);
            }
            catch (Exception ex)
            {
                ConsoleError("Failed to execute command: " + (ex.InnerException ?? ex).ToString());
            }
        }
        private static bool TryConvertParameter(string arg, Type type, [NotNullWhen(true)] out object? value)
        {
            try
            {
                value = ConvertMethod.Invoke(null, [arg, type]);
                if (value is Exception)
                {
                    value = null;
                    return false;
                }
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }
        private static List<string> ParseArguments(string argumentString)
        {
            List<string> args = [];
            if (string.IsNullOrWhiteSpace(argumentString)) return args;

            StringBuilder carg = new();
            bool inQuotes = false;

            for (int i = 0; i < argumentString.Length; i++)
            {
                char c = argumentString[i];

                if (c == '\\' && i + 1 < argumentString.Length)
                {
                    carg.Append(argumentString[i + 1]);
                    i++;
                    continue;
                }

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (c == ' ' && !inQuotes)
                {
                    if (carg.Length > 0)
                    {
                        args.Add(carg.ToString());
                        carg.Clear();
                    }
                }
                else
                    carg.Append(c);
            }

            if (carg.Length > 0)
                args.Add(carg.ToString());

            return args;
        }
        private static List<MethodInfo> FindCustomCommands()
        {
            List<MethodInfo> methods = [];
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    methods.AddRange(assembly.GetTypes()
                        .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                        .Where(method => method.GetCustomAttribute<ConsoleCommandAttribute>() != null)
                    );
                }
                catch (ReflectionTypeLoadException ex)
                {
                    BetterConsoleFeature.Error($"Could not load types from assembly {assembly.FullName}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    BetterConsoleFeature.Error($"An unexpected error occurred while scanning assembly {assembly.FullName}: {ex.Message}");
                }
            }
            return methods;
        }
        private static void ConsoleError(string text)
            => UnityEngine.Debug.LogError(text);
        private static void ConsoleLog(string text)
            => UnityEngine.Debug.Log(text);
    }
}