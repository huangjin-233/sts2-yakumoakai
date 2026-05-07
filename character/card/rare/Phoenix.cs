using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibToRitsu.Generated;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.rare
{
    public sealed class Phoenix : CardModel
    {
        protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move), new PowerVar<Fire>(5)];
        // 动态变量
        public override CardPoolModel VisualCardPool => ModelDb.CardPool<YakumoAkaiCardPool>();
        public Phoenix()
            : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
         .FromCard(this) // 攻击来源
         .Targeting(cardPlay.Target) // 攻击目标
         .Execute(choiceContext); // 执行攻击效果
            await PowerCmd.Apply<Fire>(cardPlay.Target, base.DynamicVars.Power<Fire>().BaseValue, base.Owner.Creature, this);//燃烧
            if (cardPlay.Target.HasPower<Fire>() && cardPlay.Target.GetPowerAmount<Fire>() >= 30)
            {
                await PowerCmd.Apply<Rebirth>(base.Owner.Creature, 1, base.Owner.Creature, this);//复活
                await CardCmd.Exhaust(choiceContext, this);//消耗
            }
        }
        public override string PortraitPath => $"res://images/cards/attack/Phoenix.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Damage.UpgradeValueBy(3);
            base.DynamicVars.Power<Fire>().UpgradeValueBy(1);
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<Rebirth>(),
            HoverTipFactory.FromPower<Fire>(),
            HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
            ];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(Phoenix));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

