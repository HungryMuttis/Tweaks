using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Tweaks.Features.Commands.Patches
{
    public static class UI_HealthPatch
    {
        internal static void Init()
        {
            Tweaks.Patcher.SaveInfo();
            Tweaks.Patcher.Patch(
                "Update",
                transpiler: nameof(Update_Transpiler)
            );
        }

        // PATCHES //
        public static IEnumerable<CodeInstruction> Update_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == 100f)
                    yield return new(OpCodes.Ldsfld, AccessTools.Field(typeof(Player.PlayerData), nameof(Player.PlayerData.maxHealth)));
                else
                    yield return instruction;
            }
        }
    }
}
