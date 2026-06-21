using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Models;

namespace YakumoAkai.character;

[HarmonyPatch(typeof(AncientDialogueSet))]
public static class AncientDialogueSetPatch
{
    private const string AkaiId = "YAKUMO_AKAI_CHARACTER_AKAI";

    [HarmonyPostfix]
    [HarmonyPatch(nameof(AncientDialogueSet.GetValidDialogues))]
    public static void Postfix(
        AncientDialogueSet __instance,
        ModelId characterId,
        int charVisits,
        int totalVisits,
        bool allowAnyCharacterDialogues,
        ref IEnumerable<AncientDialogue> __result)
    {
        // 只处理八云红
        if (characterId.Entry != AkaiId) return;

        MyCustomModInitializer.Logger.Info($"[AncientDialogueSetPatch] 处理对话: charVisits={charVisits}, totalVisits={totalVisits}");

        // 尝试使用角色专属对话（匹配 charVisits）
        if (__instance.CharacterDialogues.TryGetValue(AkaiId, out var dialogues))
        {
            var matched = new List<AncientDialogue>();
            foreach (var dialogue in dialogues)
            {
                if (dialogue.VisitIndex == charVisits)
                    matched.Add(dialogue);
            }

            // 如果没有精确匹配，尝试使用重复标记的 (IsRepeating)
            if (matched.Count == 0)
            {
                foreach (var dialogue in dialogues)
                {
                    if (dialogue.IsRepeating && (!dialogue.VisitIndex.HasValue || charVisits >= dialogue.VisitIndex.Value))
                        matched.Add(dialogue);
                }
            }

            if (matched.Count > 0)
            {
                __result = matched;
                MyCustomModInitializer.Logger.Info($"[AncientDialogueSetPatch] 替换为角色专属对话（共 {matched.Count} 行）");
                return;
            }
        }

        // 如果没有找到专属对话，保留原逻辑（即 FirstVisitEver 或 ANY）
        MyCustomModInitializer.Logger.Warn("[AncientDialogueSetPatch] 未找到匹配的角色对话，保留原逻辑");
    }
}