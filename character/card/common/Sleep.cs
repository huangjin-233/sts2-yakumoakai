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
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.common
{
    [RegisterCard(typeof(YakumoAkaiCardPool))]
    public sealed class Sleep : ModCardTemplate
    {
        protected override List<DynamicVar> CanonicalVars => [
            new PowerVar<IntangiblePower>(2) //能力
        ];
        // 动态变量
        public Sleep()
            : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<IntangiblePower>(choiceContext,cardPlay.Target, base.DynamicVars.Power<IntangiblePower>().BaseValue, base.Owner.Creature, this);//mp
            await CreatureCmd.Stun(cardPlay.Target);
        }
        public override string PortraitPath => $"res://images/cards/attack/Sleep.png";

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
        }
        protected override IEnumerable<IHoverTip> AdditionalHoverTips  => [
            HoverTipFactory.Static(StaticHoverTip.Stun),
            HoverTipFactory.FromPower<IntangiblePower>()];
    }
}

