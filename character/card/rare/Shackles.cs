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
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Content;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.rare
{
    [RegisterCard(typeof(YakumoAkaiCardPool))]
    public sealed class Shackles : ModCardTemplate
    {
        protected override List<DynamicVar> CanonicalVars => [
            new PowerVar<Guai>(3),new PowerVar<StrengthPower>(2),new HpLossVar(3)
        ];
        public override IEnumerable<CardKeyword> CanonicalKeywords => [AkaiKeyword.Mpex
                                                                ];
        public Shackles()
            : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.Damage(choiceContext, base.Owner.Creature, base.DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
            await PowerCmd.Apply<Guai>(choiceContext,base.Owner.Creature, base.DynamicVars.Power<Guai>().BaseValue, base.Owner.Creature, this);
            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 30)
            {
                await PowerCmd.Apply<StrengthPower>(choiceContext,base.Owner.Creature, base.DynamicVars.Power<StrengthPower>().BaseValue, base.Owner.Creature, this);
                await PowerCmd.Apply<mp>(choiceContext,base.Owner.Creature, -30m, base.Owner.Creature, this);
                Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 30;
                IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 6;
                Maidknifepower.maid[base.Owner] = Maidknifepower.GetValue(base.Owner) + 30;
                DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 30;
            }
        }
        public override string PortraitPath => $"res://images/cards/power/Shackles.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.HpLoss.UpgradeValueBy(-1);
            base.DynamicVars.Power<Guai>().UpgradeValueBy(1);// 升级后
        }
        protected override IEnumerable<IHoverTip> AdditionalHoverTips  => [
            HoverTipFactory.FromPower<Guai>(),
            HoverTipFactory.FromPower<StrengthPower>(),
            HoverTipFactory.FromPower<mp>()
        ];
        //关键词
    }
}

