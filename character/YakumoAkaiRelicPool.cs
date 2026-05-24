using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Models;
using YakumoAkai.character.relics;
namespace YakumoAkai.character
{
	public sealed class YakumoAkaiRelicPool : RelicPoolModel
	{
		public override string EnergyColorName => "yakumoakai";

		public override Color LabOutlineColor => new Color("ff0000ff");

		protected override List<RelicModel> GenerateAllRelics()
		{
			return new List<RelicModel>([
				ModelDb.Relic<GensokyoOnlineBad>(),
                ModelDb.Relic<GensokyoOnline>()
            ]);
		}
	}
}

