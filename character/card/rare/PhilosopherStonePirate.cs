using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibToRitsu.Generated;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.rare
{
    public sealed class PhilosopherStonePirate : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
           new CardsVar(2),new EnergyVar(3),new PowerVar<mp>(30),new PowerVar<Num>(0)
        ];
        // 动态变量
        public PhilosopherStonePirate()
            : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            base.DynamicVars.Power<Num>().UpgradeValueBy(1);
            CardModel cardModel = (await CardSelectCmd.FromHand(
           choiceContext,
           base.Owner,
           new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1),
           null,
           this)
           ).FirstOrDefault();
            //选择手卡
            if (cardModel != null)
            {
                await CardCmd.Exhaust(choiceContext, cardModel);//消耗
                await PowerCmd.Apply<mp>(base.Owner.Creature, base.DynamicVars.Power<mp>().BaseValue, base.Owner.Creature, this);
                await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue, base.Owner);//能量
            }
            if (base.DynamicVars.Power<Num>().BaseValue == base.DynamicVars.Cards.BaseValue)
            {
                await CardCmd.Exhaust(choiceContext, this);//消耗
                base.DynamicVars.Power<Num>().BaseValue = 0;
            }
        }
        public override string PortraitPath => $"res://images/cards/skill/Philosopher_stone_pirate.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Cards.UpgradeValueBy(1);
            base.DynamicVars.Power<mp>().UpgradeValueBy(15);
            // 升级后
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<mp>(),
            HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(PhilosopherStonePirate));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

