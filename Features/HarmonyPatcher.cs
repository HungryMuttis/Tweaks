using HarmonyLib;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Tweaks.Features
{
    internal class HarmonyPatcher(string id, BepInEx.Logging.ManualLogSource logger)
    {
        private readonly Harmony harmony = new(id);
        private readonly ManualLogSource Logger = new(logger, nameof(HarmonyPatcher));

        public Type? OriginalType { get; set; }
        public Type? PatchType { get; set; }

        /// <summary>
        /// **IMPORTANT** SaveInfo must be used to save the PatchType and OriginalType
        /// </summary>
        /// <returns></returns>
        public bool Patch(string originalMethodName, Type[]? originalMethodParameters = null, string? prefix = null, string? postfix = null, string? transpiler = null)
        {
            if (OriginalType == null || PatchType == null)
            {
                Logger.LogError($"{nameof(OriginalType)} or {nameof(PatchType)} is null");
                return false;
            }

            return Patch(OriginalType, originalMethodName, originalMethodParameters, prefix == null ? null : (PatchType, prefix), postfix == null ? null : (PatchType, postfix), transpiler == null ? null : (PatchType, transpiler));
        }
        /// <summary>
        /// **IMPORTANT** SaveInfo must be used to save the PatchType
        /// </summary>
        public bool Patch(Type originalType, string originalMethodName, Type[]? originalMethodParameters = null, string? prefix = null, string? postfix = null, string? transpiler = null)
        {
            if (PatchType == null)
            {
                Logger.LogError($"{nameof(PatchType)} is null");
                return false;
            }

            return Patch(originalType, originalMethodName, originalMethodParameters, prefix == null ? null : (PatchType, prefix), postfix == null ? null : (PatchType, postfix), transpiler == null ? null : (PatchType, transpiler));
        }
        /// <summary>
        /// **IMPORTANT** SaveInfo must be used to save the OriginalType
        /// </summary>
        public bool Patch(
            string originalMethodName, Type[]? originalMethodParameters = null,
            (Type type, string methodName)? prefix = null,
            (Type type, string methodName)? postfix = null,
            (Type type, string methodName)? transpiler = null)
        {
            if (OriginalType == null)
            {
                Logger.LogError($"{nameof(OriginalType)} is null");
                return false;
            }

            return Patch(OriginalType, originalMethodName, originalMethodParameters, prefix, postfix, transpiler);
        }
        public bool Patch(
            Type originalType, string originalMethodName, Type[]? originalMethodParameters = null,
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
        public bool Patch(MethodInfo? original, HarmonyMethod? prefix = null, HarmonyMethod? postfix = null, HarmonyMethod? transpiler = null)
        {
            if (original == null)
            {
                Logger.LogError("The original method to patch cannot be null.");
                return false;
            }

            try
            {
                harmony.Patch(original, prefix, postfix, transpiler);
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError($"Failed to patch {original.DeclaringType?.Name}.{original.Name}. Exception: {e}");
                return false;
            }
        }

        /// <summary>
        /// **IMPORTANT** If youre experiencing weird issues with the release build, this methiod might be causing them. Use SaveInfo(Type originalType, Type patchType) instead <br/>
        /// Saves the class of the caller method as PatcherType
        /// </summary>
        public void ChangeInfo(int depth = 1) => PatchType = new StackTrace().GetFrame(depth).GetMethod().DeclaringType;
        /// <summary>
        /// Changes the original type to the specified one
        /// </summary>
        public void ChangeInfo(Type originalType) => OriginalType = originalType;
        /// <summary>
        /// **IMPORTANT** If youre experiencing weird issues with the release build, this methiod might be causing them. Use SaveInfo(Type originalType, Type patchType) instead <br/>
        /// **WARNING** Your patcher class must be named exactly like the class you are patching + optional 'Patch' suffix. Note: if you are patching a class that name ends in 'Patch' in the name, you must add a 'Patch' suffix to your class or set fixName = false. <br/>
        /// Examples: ClassYouArePatching (result: ClassYouArePatching), ClassYouArePatchingPatch (result: ClassYouArePatching), ClassYouArePatchingPatchPatch (result: ClassYouArePatchingPatch), ClassYouArePatchingPatch fixName = false (result: ClassYouArePatchingPatch) <br/>
        /// Saves the class of the caller method as PatcherType and tries to save OriginalType by searching for it inside Assembly-CSharp.dll <br/>
        /// Saved info is used when calling some of the Patch overloads
        /// </summary>
        /// <returns>true if the OriginalType was successfully saved, false otherwise</returns>
        public bool SaveInfo(string assembly = "Assembly-CSharp", string @namespace = "", bool fixName = true, int depth = 1)
        {
            PatchType = new StackTrace().GetFrame(depth).GetMethod().DeclaringType;

            string toSearch = PatchType.Name;
            if (fixName)
            {
                int index = toSearch.LastIndexOf("Patch");
                if (index != -1)
                    toSearch = toSearch.Remove(index, 5);
            }
            if (!string.IsNullOrEmpty(@namespace))
                toSearch = (@namespace.EndsWith('.') ? @namespace : @namespace + '.') + toSearch;
            OriginalType = Type.GetType($"{toSearch}, {assembly}");
            return OriginalType != null;
        }
        /// <summary>
        /// **IMPORTANT** If youre experiencing weird issues with the release build, this methiod might be causing them. Use SaveInfo(Type originalType, Type patchType) instead <br/>
        /// Saves the class of the caller method as PatcherType and the given original type <br/>
        /// Saved info is used when calling some of the Patch overloads
        /// </summary>
        public void SaveInfo(Type originalType, int depth = 1) => SaveInfo(originalType, new StackTrace().GetFrame(depth).GetMethod().DeclaringType);
        /// <summary>
        /// Saves the given info
        /// Saved info is used when calling some of the Patch overloads
        /// </summary>
        public void SaveInfo(Type originalType, Type patchType)
        {
            OriginalType = originalType;
            PatchType = patchType;
        }

        public MethodInfo? FindMethod(Type type, string methodName, Type[]? parameters = null)
        {
            MethodInfo method = parameters == null
                ? AccessTools.Method(type, methodName)
                : AccessTools.Method(type, methodName, parameters);

            if (method == null)
                Logger.LogError($"Failed to find {type.Name}.{methodName}");

            return method;
        }
        public HarmonyMethod? GetHarmonyMethod(Type type, string methodName)
        {
            MethodInfo? method = FindMethod(type, methodName);
            return method == null ? null : new(method);
        }
        public void UnpatchAll() => harmony.UnpatchSelf();
    }
}