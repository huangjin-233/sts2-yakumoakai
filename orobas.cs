using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using YakumoAkai.character.relics;

namespace YakumoAkai
{
    [HarmonyPatch(typeof(TouchOfOrobas))]
    public static class TransBaseRelic
    {
        [HarmonyPostfix]
        [HarmonyPatch("get_RefinementUpgrades")]
        public static void Postfix(ref Dictionary<ModelId, RelicModel> __result)
        {
            var GensokyoOnlineBad = ModelDb.Relic<GensokyoOnlineBad>().Id;
            var GensokyoOnline = ModelDb.Relic<GensokyoOnline>();


            if (!__result.ContainsKey(GensokyoOnlineBad)) __result[GensokyoOnlineBad] = GensokyoOnline;
        }
    }
}

