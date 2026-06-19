using System;
using System.Collections.Generic;
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
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Content;
using YakumoAkai.character.card.rare;
using YakumoAkai.character.card.special;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.uncommon
{
    [RegisterCard(typeof(YakumoAkaiCardPool

))]
    public sealed class LazyRock : ModCardTemplate
    {
        protected override List<DynamicVar> CanonicalVars => [
            new PowerVar<PlatingPower>(8),new BlockVar(4m,ValueProp.Move)
        ];
        // 动态变量
        public override IEnumerable<CardKeyword> CanonicalKeywords => [AkaiKeyword.Mpex];
        public LazyRock()
            : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<PlatingPower>(choiceContext,base.Owner.Creature, base.DynamicVars.Power<PlatingPower>().BaseValue, base.Owner.Creature, this);
            await PowerCmd.Apply<Lazy>(choiceContext,base.Owner.Creature,1, base.Owner.Creature, this);
            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 20)
            {
                await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);//防御
                await PowerCmd.Apply<mp>(choiceContext,base.Owner.Creature, -20m, base.Owner.Creature, this);
                Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 20;
                IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 4;
                Maidknifepower.maid[base.Owner] = Maidknifepower.GetValue(base.Owner) + 20;
                DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 20;
            }
            //mp效果
        }
        public override string PortraitPath => $"res://images/cards/skill/Lazyrock.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Power<PlatingPower>().UpgradeValueBy(2);//升级后
            base.DynamicVars.Block.UpgradeValueBy(3);
        }
        protected override IEnumerable<IHoverTip> AdditionalHoverTips  => [
            HoverTipFactory.FromPower<Lazy>(),
            HoverTipFactory.FromPower<PlatingPower>(),
            HoverTipFactory.FromPower<mp>()
            ];
        //关键词
    }
}
