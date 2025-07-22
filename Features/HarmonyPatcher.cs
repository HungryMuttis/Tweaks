using HarmonyLib;
using System;
using System.Reflection;

namespace Tweaks.Features
{
    internal static class HarmonyPatcher
    {
        private static readonly Harmony harmony = new(MyPluginInfo.PLUGIN_GUID);
        public static bool Patch(
            Type originalType,
            string originalMethodName,
            Type[]? originalMethodParameters = null,
            (Type type, string methodName)? prefix = null,
            (Type type, string methodName)? postfix = null,
            (Type type, string methodName)? transpiler = null)
        {
            MethodInfo? originalMethod = FindMethod(originalType, originalMethodName, originalMethodParameters);
            if (originalMethod == null) return false;

            HarmonyMethod? prefixMethod = null;
            if (prefix.HasValue)
            {
                prefixMethod = GetHarmonyMethod(prefix.Value.type, prefix.Value.methodName);
                if (prefixMethod == null) return false;
            }

            HarmonyMethod? postfixMethod = null;
            if (postfix.HasValue)
            {
                postfixMethod = GetHarmonyMethod(postfix.Value.type, postfix.Value.methodName);
                if (postfixMethod == null) return false;
            }

            HarmonyMethod? transpilerMethod = null;
            if (transpiler.HasValue)
            {
                transpilerMethod = GetHarmonyMethod(transpiler.Value.type, transpiler.Value.methodName);
                if (transpilerMethod == null) return false;
            }

            return Patch(originalMethod, prefixMethod, postfixMethod, transpilerMethod);
        }
        public static bool Patch(MethodInfo? original, HarmonyMethod? prefix = null, HarmonyMethod? postfix = null, HarmonyMethod? transpiler = null)
        {
            if (original == null)
            {
                Error("The original method to patch cannot be null.");
                return false;
            }

            try
            {
                harmony.Patch(original, prefix, postfix, transpiler);
                return true;
            }
            catch (Exception e)
            {
                Error($"Failed to patch {original.DeclaringType?.Name}.{original.Name}. Exception: {e}");
                return false;
            }
        }

        public static MethodInfo? FindMethod(Type type, string methodName, Type[]? parameters = null)
        {
            MethodInfo method = parameters == null
                ? AccessTools.Method(type, methodName)
                : AccessTools.Method(type, methodName, parameters);

            if (method == null)
                Error($"Failed to find {type.Name}.{methodName}");

            return method;
        }

        public static HarmonyMethod? GetHarmonyMethod(Type type, string methodName)
        {
            MethodInfo? method = FindMethod(type, methodName);
            return method != null ? new HarmonyMethod(method) : null;
        }

        public static void UnpatchAll() =>
            harmony.UnpatchSelf();

        private static void Error(string text)
            => Tweaks.Logger.LogError($"[Patcher] {text}");
    }
}