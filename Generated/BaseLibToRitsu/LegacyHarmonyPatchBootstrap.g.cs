using System;
using HarmonyLib;

namespace BaseLibToRitsu.Generated;

internal static class LegacyHarmonyPatchBootstrap
{
    public static bool Apply(Harmony harmony)
    {
        bool success = true;
        TryPatch(harmony, typeof(global::YakumoAkai.character.relics.TransBaseRelic), ref success);
        TryPatch(harmony, typeof(global::YakumoAkai.path.ModelDbAllRelicPoolsPatch), ref success);
        TryPatch(harmony, typeof(global::YakumoAkai.TransBaseRelic), ref success);
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
