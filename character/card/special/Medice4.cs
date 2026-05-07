using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibToRitsu.Generated;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace YakumoAkai.character.card.special
{
    public sealed class Medice4 : CardModel
    {
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, AkaiKeyword.Medice];
        protected override List<DynamicVar> CanonicalVars => [new CardsVar(2),new PowerVar<PoisonPower>(9)];// 动态变量
        public Medice4()
        : base(0, CardType.Attack, CardRarity.Token, TargetType.AllEnemies) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<PoisonPower>(base.CombatState.HittableEnemies,base.DynamicVars.Power<PoisonPower>().BaseValue,base.Owner.Creature, this);//中毒
        }
        public override string PortraitPath => $"res://images/cards/attack/Medice4.png";

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
            // 升级后
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
                HoverTipFactory.FromPower<PoisonPower>()];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoakaiTokenCardPool), typeof(Medice4));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

