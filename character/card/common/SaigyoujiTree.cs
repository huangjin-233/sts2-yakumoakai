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
using MegaCrit.Sts2.Core.ValueProps;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.common
{
    public sealed class SaigyoujiTree : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [new EnergyVar(1),new PowerVar<mp>(10)];// 动态变量
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];


        public SaigyoujiTree()
        : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {

            await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue, base.Owner);//能量
            await PowerCmd.Apply<mp>(base.Owner.Creature, base.DynamicVars.Power<mp>().BaseValue, base.Owner.Creature, this);
        }
        public override string PortraitPath => $"res://images/cards/skill/Saigyouji_tree.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Energy.UpgradeValueBy(1m);
            base.DynamicVars.Power<mp>().UpgradeValueBy(5);
            // 升级后
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
                HoverTipFactory.FromPower<mp>()];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(SaigyoujiTree));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}