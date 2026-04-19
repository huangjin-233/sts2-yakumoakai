using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using YakumoAkai.character;

namespace YakumoAkai
{
	internal class path
	{
		[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.AllRelicPools), MethodType.Getter)]
		public static class ModelDbAllRelicPoolsPatch
		{
			static void Postfix(ref IEnumerable<RelicPoolModel> __result)
			{
				__result = __result
					.Append(ModelDb.RelicPool<YakumoAkaiRelicPool>())
					.Distinct();
			}
		}
		public static class ModelDbAllPotionPoolsPatch
		{
			static void Postfix(ref IEnumerable<PotionPoolModel> __result)
			{
				__result = __result
					.Append(ModelDb.PotionPool<YakumoAkaiPotionPool>())
					.Distinct();
			}
		}
		public static class ModelDbAllCardPoolsPatch
		{
			static void Postfix(ref IEnumerable<CardPoolModel> __result)
			{
				__result = __result
					.Append(ModelDb.CardPool<YakumoAkaiCardPool>())
					.Distinct();
			}
		}
	}

}
