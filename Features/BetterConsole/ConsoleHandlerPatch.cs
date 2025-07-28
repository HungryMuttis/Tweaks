using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Zorro.Core;
using Zorro.Core.CLI;

namespace Tweaks.Features.BetterConsole
{
    public static class ConsoleHandlerPatch
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
        private static readonly Dictionary<MethodInfo, ConsoleCommandAttribute?> Attributes = [];

        private const string HelpCommand = "Help";

        internal static void Init()
        {
            Tweaks.Patcher.SaveInfo(assembly: "Zorro.Core.Runtime", @namespace: "Zorro.Core.CLI");
            Tweaks.Patcher.Patch(
                nameof(ConsoleHandler.Initialize),
                postfix: nameof(Initialize_Postfix)
            );
            Tweaks.Patcher.Patch(
                nameof(ConsoleHandler.ProcessCommand),
                prefix: nameof(ProcessCommand_Override)
            );
            Tweaks.Patcher.Patch(
                nameof(ConsoleHandler.FindSuggestions),
                prefix: nameof(FindSuggestions_Override)
            );
        }

        // PATCHES //
        public static void Initialize_Postfix()
        {
            FieldInfo m_consoleCommandsField = AccessTools.Field(typeof(ConsoleHandler), "m_consoleCommands");
            BetterConsoleFeature.Debug("Searching for custom commands...");

            List<MethodInfo> customCommands = FindCustomCommands();

            if (customCommands.Count == 0)
            {
                BetterConsoleFeature.Debug("No custom commands found.");
                return;
            }

            List<ConsoleCommand> consoleCommands = [.. (ConsoleCommand[])m_consoleCommandsField.GetValue(null)];

            Attributes.Clear();
            foreach (ConsoleCommand command in consoleCommands)
                Attributes[command.MethodInfo] = null;

            int commandsAdded = 0;
            foreach (MethodInfo method in customCommands)
            {
                Type declaringType = method.DeclaringType;
                if (declaringType == null) continue;

                bool add = false;
                if (!typeof(ICommandsClass).IsAssignableFrom(declaringType))
                    add = true;
                else
                {
                    PropertyInfo? enabled = declaringType.GetProperty("Enabled", BindingFlags.Public | BindingFlags.Static);
                    if (enabled != null && (bool)enabled.GetValue(null))
                        add = true;
                }

                if (add)
                {
                    consoleCommands.Add(new ConsoleCommand(method));
                    Attributes[method] = method.GetCustomAttribute<ConsoleCommandAttribute>();
                    commandsAdded++;
                }
            }

            m_consoleCommandsField.SetValue(null, consoleCommands.ToArray());
            ConsoleCommands = (ConsoleCommand[])m_consoleCommandsField.GetValue(null);

            BetterConsoleFeature.Debug($"Added {commandsAdded} new commands. Total commands: {consoleCommands.Count}");

            Parsers = (Dictionary<Type, CLITypeParser>)AccessTools.Field(typeof(ConsoleHandler), "m_typeParsers").GetValue(null);
        }
        public static bool ProcessCommand_Override(string command, ref bool __result)
        {
            if (HandleAmbiguity(command, ref __result)) return false;
            if (command.Equals(HelpCommand, StringComparison.OrdinalIgnoreCase) || command.TrimEnd().Equals($"{HelpCommand}.{HelpCommand}", StringComparison.OrdinalIgnoreCase)) return HandleHelpCommand(ref __result);

            _ambiguityContext = null;

            string[]? parts = StringUtility.SplitOnFirstOfChar(command, '.');
            if (parts == null)
            {
                __result = false;
                return false;
            }

            string domain = parts[0];
            string trimmed = parts[1].TrimEnd();
            string commandName = trimmed.Split(' ').First();
            string rawArgs = trimmed.Contains(' ') ? trimmed[commandName.Length..].Trim() : "";
            List<string> providedArgs = ParseArguments(rawArgs, out _, out _);

            if (commandName.Equals(HelpCommand, StringComparison.OrdinalIgnoreCase)) return HandleHelpCommand(ref __result, domain);

            List<ConsoleCommand> cmds = [.. ConsoleCommands.Where(c => c.DomainName == domain && c.Command == commandName)];
            if (cmds.Count == 0)
            {
                __result = false;
                return false;
            }

            return HandleCommandExecution(FindValidCmds(cmds, providedArgs), providedArgs, commandName, ref __result);
        }
        public static bool FindSuggestions_Override(string input, ref List<Suggestion> __result)
        {
            __result ??= [];
            if (string.IsNullOrEmpty(input)) return false;
            string[] split = StringUtility.SplitOnFirstOfChar(input, ' ');
            string domain;
            string? command;
            List<string> args;
            bool next = false;
            int level, lastStart = -1;

            if (split == null)
            {
                GetCommand(input, out domain, out command);
                if (command == null) level = 0;
                else level = 1;
                args = [];
            }
            else
            {
                GetCommand(split[0], out domain, out command);
                level = 2;
                args = ParseArguments(split[1], out next, out lastStart);
            }

            FindCommandSuggestions(domain, command, args, next, level, ref __result);
            AddParameterSuggestions(input, command, args, next, lastStart, level, ref __result);
            return false;
        }

        // HELPER METHODS //
        private static void FindCommandSuggestions(string domain, string? command, List<string>? args, bool next, int level, ref List<Suggestion> __result)
        {
            if (level == 0)
            {
                __result = [.. ConsoleCommands.Select(c => c.DomainName).Distinct().Where(d => d.Contains(domain, StringComparison.OrdinalIgnoreCase)).Select(d => new DomainSuggestion(d))];
                if (HelpCommand.Contains(domain, StringComparison.OrdinalIgnoreCase) && !__result.Any(s => s is DomainSuggestion d && d.Domain.Equals(HelpCommand, StringComparison.OrdinalIgnoreCase)))
                    __result.Add(new DomainSuggestion(HelpCommand));
                return;
            }

            HashSet<ConsoleCommand> possibleCommands = [.. ConsoleCommands.Where(c => c.DomainName == domain)];
            if (possibleCommands.Count == 0) return;

            if (level == 1)
            {
                __result = [.. possibleCommands.Where(c => c.Command.StartsWith(command)).Select(c => new CommandSuggestion(c.DomainName, c.Command, c.ParameterInfo))];
                if (HelpCommand.Contains(command, StringComparison.OrdinalIgnoreCase) && !__result.Any(s => s is CommandSuggestion c && c.Command.Equals(HelpCommand, StringComparison.OrdinalIgnoreCase)))
                    __result.Add(new CommandSuggestion(domain, HelpCommand, []));
                return;
            }
            possibleCommands = [.. possibleCommands.Where(c => c.MethodInfo.Name == command && c.ParameterInfo.Length != 0)];

            foreach (ConsoleCommand cmd in possibleCommands)
            {
                if (next && cmd.ParameterInfo.Length <= args!.Count) continue;
                if (cmd.ParameterInfo.Length < args!.Count) continue;

                bool possible = true;
                for (int i = 0; i < args.Count; i++)
                {
                    if (i == args.Count - 1 && !next)
                    {
                        if (!PossibleArgument(args[i], cmd.ParameterInfo[i].ParameterType))
                        {
                            possible = false;
                            break;
                        }
                    }
                    else if (!TryConvertParameter(args[i], cmd.ParameterInfo[i].ParameterType, out object? _))
                    {
                        possible = false;
                        break;
                    }
                }

                if (possible)
                    __result.Add(new CommandSuggestion(cmd.DomainName, cmd.Command, cmd.ParameterInfo));
            }
        }
        private static void AddParameterSuggestions(string input, string? command, List<string>? args, bool next, int lastStart, int level, ref List<Suggestion> __result)
        {

            if (level == 0) return;
            if (level == 1 && !next) return;
            args ??= [];

            string carg = "";
            int index = 0;

            if (next)
                index = args.Count;
            else
            {
                if (args.Count > 0)
                {
                    index = args.Count - 1;
                    carg = args[index];
                }
                else
                    index = 0;
            }

            string typeLine = next ? input : input[..(StringUtility.SplitOnFirstOfChar(input, ' ')[0].Length + 1 + lastStart)];

            HashSet<string> addedAutocompletes = [];

            for (int i = __result.Count - 1; i >= 0; i--)
            {
                if (__result[i] is not CommandSuggestion cmdSuggestion || !cmdSuggestion.Command.Equals(command, StringComparison.OrdinalIgnoreCase)) continue;
                if (cmdSuggestion.ParameterInfos.Length <= index) continue;

                cmdSuggestion.HighlightParameter(index);
                ParameterInfo info = cmdSuggestion.ParameterInfos[index];

                if (!Parsers.TryGetValue(info.ParameterType, out CLITypeParser parser)) continue;

                List<ParameterAutocomplete> autocompletes = parser.FindAutocomplete(carg);
                string displayLine = cmdSuggestion.GetDisplayTextWithMaxParameter(index, false);
                foreach (ParameterAutocomplete autocomplete in autocompletes)
                    if (addedAutocompletes.Add(autocomplete.Value))
                        __result.Add(new ParameterSuggestion(displayLine, typeLine, autocomplete.Value));
            }
        }
        private static bool HandleAmbiguity(string command, ref bool result)
        {
            if (!int.TryParse(command, out int selection) || selection <= 0) return false;
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
        private static List<string> ParseArguments(string argumentString, out bool next, out int lastStart)
        {
            next = false;
            lastStart = 0;
            List<string> args = [];
            if (string.IsNullOrWhiteSpace(argumentString)) return args;

            StringBuilder carg = new();
            bool inQuotes = false;
            int currStart = 0;

            for (int i = 0; i < argumentString.Length; i++)
            {
                char c = argumentString[i];

                if (c == '\\')
                {
                    next = false;
                    if (i + 1 < argumentString.Length)
                    {
                        carg.Append(argumentString[i + 1]);
                        i++;
                    }
                    else
                        carg.Append(c);
                    continue;
                }

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    if (!inQuotes && carg.Length == 0 && i > currStart)
                    {
                        args.Add("");
                        currStart = i + 1;
                    }
                    next = false;
                    continue;
                }

                if (c == ' ' && !inQuotes)
                {
                    lastStart = currStart;
                    currStart = i + 1;
                    args.Add(carg.ToString());
                    carg.Clear();
                    next = true;
                }
                else
                {
                    if (next)
                        lastStart = currStart;
                    next = false;
                    carg.Append(c);
                }
            }

            if (carg.Length > 0)
                args.Add(carg.ToString());

            return args;
        }
        private static void GetCommand(string info, out string domain, out string? command)
        {
            string[] split = StringUtility.SplitOnFirstOfChar(info, '.');
            if (split == null)
            {
                domain = info;
                command = null;
            }
            else
            {
                domain = split[0];
                command = split[1];
            }
        }
        private static List<MethodInfo> FindCustomCommands()
        {
            List<MethodInfo> methods = [];
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    methods.AddRange(assembly.GetTypes()
                        .SelectMany(AccessTools.GetDeclaredMethods)
                        .Where(m => m.GetCustomAttribute<ConsoleCommandAttribute>() != null)
                    );
                }
                catch (ReflectionTypeLoadException ex)
                {
                    StringBuilder sb = new();
                    sb.AppendLine($"Could not load types from assembly {assembly.FullName}: {ex.Message}");
                    if (ex.LoaderExceptions != null)
                        foreach (Exception loaderEx in ex.LoaderExceptions)
                            sb.AppendLine($"  - LoaderException: {loaderEx.Message}");
                    BetterConsoleFeature.Error(sb);
                }
                catch (Exception ex)
                {
                    BetterConsoleFeature.Error($"An unexpected error occurred while scanning assembly {assembly.FullName}: {ex.Message}");
                }
            }
            return methods;
        }
        private static bool HandleHelpCommand(ref bool __result, string? domain = null)
        {
            StringBuilder sb = new();
            if (domain == null)
            {
                IEnumerable<string> domains = ConsoleCommands.Select(c => c.DomainName).Distinct().OrderBy(d => d);
                foreach (string cdomain in domains)
                    sb.AppendLine(cdomain);
                sb.Append($"Type '<Domain>.{HelpCommand}' for help with a specific domain");
            }
            else
            {
                List<ConsoleCommand> commands = [.. ConsoleCommands.Where(c => c.DomainName.Equals(domain, StringComparison.OrdinalIgnoreCase))];
                if (!commands.Any())
                {
                    ConsoleError($"No domain named '{domain}' found");
                    __result = true;
                    return false;
                }

                bool first = true;
                foreach (ConsoleCommand cmd in commands)
                {
                    Attributes.TryGetValue(cmd.MethodInfo, out ConsoleCommandAttribute? attr);
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
            }
            ConsoleLog(sb.ToString());
            __result = true;
            return false;
        }
        private static void ConsoleError(string text) => UnityEngine.Debug.LogError(text);
        private static void ConsoleLog(string text) => UnityEngine.Debug.Log(text);
    }
}