using System;
using System.Collections.Generic;
using System.Linq;
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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.uncommon
{
    [RegisterCard(typeof(YakumoAkaiCardPool

))]
    public sealed class GodShose : ModCardTemplate
    {
        public override bool GainsBlock => true;
        protected override List<DynamicVar> CanonicalVars => [
            new BlockVar(3m, ValueProp.Move),new PowerVar<DexterityPower>(2)
        ];
        // 动态变量
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        public GodShose()
            : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);//防御
            await PowerCmd.Apply<DexterityPower>(choiceContext,base.Owner.Creature, base.DynamicVars.Dexterity.BaseValue, base.Owner.Creature, null);//敏捷
            //mp 效果
        }
        public override string PortraitPath => $"res://images/cards/skill/God_shose.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Block.UpgradeValueBy(2m);
            base.DynamicVars.Dexterity.UpgradeValueBy(1);// 升级后
        }
        protected override IEnumerable<IHoverTip> AdditionalHoverTips  => [
            HoverTipFactory.FromPower<DexterityPower>(),
            HoverTipFactory.FromPower<mp>()];
        //关键词
    }
}

