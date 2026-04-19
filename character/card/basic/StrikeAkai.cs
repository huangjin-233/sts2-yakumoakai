using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using YakumoAkai.character;

public sealed class StrikeAkai : CardModel
{
	protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag>
{
	CardTag.Strike // 将这张卡牌指定为“打击”类卡牌
};
	protected override List<DynamicVar> CanonicalVars => [
		new DamageVar(6m, ValueProp.Move) // 伤害值
	];
	// 动态变量

	public StrikeAkai()
		: base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) { }
	// 卡牌的构造函数，指定卡牌的相关属性

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
		 .FromCard(this) // 攻击来源
		 .Targeting(cardPlay.Target) // 攻击目标
		 .Execute(choiceContext); // 执行攻击效果
	}
	public override string PortraitPath => $"res://images/cards/attack/Strike_akai.png";

	protected override void OnUpgrade()
	{
		base.DynamicVars.Damage.UpgradeValueBy(3m); // 升级后加 2 点伤害
	}
	[ModInitializer(nameof(Initialize))]
	public static class YakumoakaiInitializer
	{
		public static void Initialize()
		{
			{
				ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(StrikeAkai));

				var harmony = new Harmony("huangjin.yakumoakai");
				harmony.PatchAll();
				// 初始化 harmony 库
			}
		}
	}
}

