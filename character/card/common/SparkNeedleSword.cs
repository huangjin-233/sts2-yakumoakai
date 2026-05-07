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
using MegaCrit.Sts2.Core.Models.Powers;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.common
{
        public sealed class SparkNeedleSword : CardModel
        {
            protected override List<DynamicVar> CanonicalVars => [
                new PowerVar<DexterityPower>(3),new PowerVar<ShrinkPower>(2),new PowerVar<WeakPower>(2) //能力
            ];
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        // 动态变量
        public SparkNeedleSword()
                : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }
            // 卡牌的构造函数，指定卡牌的相关属性

            protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
            {
                await PowerCmd.Apply<ShrinkPower>(base.Owner.Creature,base.DynamicVars.Power<ShrinkPower>().BaseValue, base.Owner.Creature, null);//缩小
                await PowerCmd.Apply<WeakPower>(base.Owner.Creature, base.DynamicVars.Power<WeakPower>().BaseValue, base.Owner.Creature, null);//缩小
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature,base.DynamicVars.Power<DexterityPower>().BaseValue, base.Owner.Creature, null);//敏捷
             }
            public override string PortraitPath => $"res://images/cards/skill/SparkNeedleSword.png";

            protected override void OnUpgrade()
            {
            base.DynamicVars.Power<ShrinkPower>().UpgradeValueBy(-1);
            base.DynamicVars.Power<WeakPower>().UpgradeValueBy(-1);
        }
            protected override IEnumerable<IHoverTip> ExtraHoverTips => [
                HoverTipFactory.FromPower<DexterityPower>(),
                HoverTipFactory.FromPower<ShrinkPower>(),
                HoverTipFactory.FromPower<WeakPower>()];
            [ModInitializer(nameof(Initialize))]
            public static class YakumoakaiInitializer
            {
                public static void Initialize()
                {
                    {
                        ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(SparkNeedleSword));

                        var harmony = new Harmony("huangjin.yakumoakai");
                        harmony.PatchAll();
                        // 初始化 harmony 库
                    }
                }
            }
        }
    }
