using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace YakumoAkai.character.card.basic
{
    public sealed class DefendAkai : CardModel
    {
        public override bool GainsBlock => true;
        protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag>
        {
            CardTag.Defend
        };
        protected override List<DynamicVar> CanonicalVars => [
            new BlockVar(5m, ValueProp.Move)
        ];
        // 动态变量
        public DefendAkai()
            : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);//防御
        }
        public override string PortraitPath => $"res://images/cards/skill/Defend_akai.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Block.UpgradeValueBy(3m); // 升级后
        }
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(DefendAkai));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}


        