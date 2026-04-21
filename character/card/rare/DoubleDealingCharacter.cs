using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

namespace YakumoAkai.character.card.rare
{
    public sealed class DoubleDealingCharacter : CardModel
    {
        public override bool GainsBlock => true;
        protected override List<DynamicVar> CanonicalVars => [
            new BlockVar(26m, ValueProp.Move)
        ];
        // 动态变量
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        public DoubleDealingCharacter()
            : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);//防御
        }
        public override string PortraitPath => $"res://images/cards/skill/DoubleDealingCharacter.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Block.UpgradeValueBy(5);
        }
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(DoubleDealingCharacter));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}
