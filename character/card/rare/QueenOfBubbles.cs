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
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.rare
{
    [RegisterCard(typeof(YakumoAkaiCardPool))]
    public sealed class QueenOfBubbles : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
           new PowerVar<Money>(2)
        ];
        // 动态变量
        public QueenOfBubbles()
            : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<Money>(choiceContext,base.Owner.Creature, base.DynamicVars.Power<Money>().BaseValue, base.Owner.Creature, this);
        }
        public override string PortraitPath => $"res://images/cards/power/Queen_of_bubbles.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Power<Money>().UpgradeValueBy(2);
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<Money>()];
        //关键词
    }
}

