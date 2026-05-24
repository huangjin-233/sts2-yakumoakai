using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;
namespace YakumoAkai.character
{
	public sealed class YakumoAkaiPotionPool : PotionPoolModel
	{
		public override string EnergyColorName => "yakumoakai";

		public override Color LabOutlineColor => new Color("ff0000ff");

		protected override List<PotionModel> GenerateAllPotions()
		{
			return [
				ModelDb.Potion<AttackPotion>()
			];
		}
	}
}

