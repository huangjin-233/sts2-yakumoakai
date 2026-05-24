using System;
using HarmonyLib;

namespace BaseLibToRitsu.Generated;

internal static class LegacyHarmonyPatchBootstrap
{
    public static bool Apply(Harmony harmony)
    {
        bool success = true;
        return success;
    }

    private static void TryPatch(Harmony harmony, Type patchType, ref bool success)
    {
        try
        {
            harmony.CreateClassProcessor(patchType).Patch();
        }
        catch (Exception ex)
        {
            success = false;
            Console.Error.WriteLine($"[BaseLibToRitsu] Failed to patch {patchType.FullName}: {ex}");
        }
    }
}
