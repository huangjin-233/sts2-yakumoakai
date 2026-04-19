using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace YakumoAkai.character.card.uncommon
{
    public sealed class Doulong : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
            new CardsVar(7) // 伤害值
        ];
        // 动态变量
        public Doulong()
            : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            decimal baseValue = base.DynamicVars.Cards.BaseValue;
            int count = base.Owner.PlayerCombatState.Hand.Cards.Count;
            decimal count2 = Math.Max(0m, baseValue - (decimal)count);
            await CardPileCmd.Draw(choiceContext, count2, base.Owner);
            //抽卡
        }
        public override string PortraitPath => $"res://images/cards/skill/Doulong.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Cards.UpgradeValueBy(1m); // 升级后
        }
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(Doulong));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

