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
using STS2RitsuLib.Interop.AutoRegistration;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.common
{
    [RegisterCard(typeof(YakumoAkaiCardPool))]
    public sealed class Lunar : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
            new PowerVar<Lunarpower>(30) //能力
        ];
        // 动态变量
        public Lunar()
            : base(1, CardType.Power, CardRarity.Common, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<Lunarpower>(choiceContext,base.Owner.Creature, 1, base.Owner.Creature, this);//mp
        }
        public override string PortraitPath => $"res://images/cards/power/Lunar.png";

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<Lunarpower>(),
            HoverTipFactory.FromPower<mp>()
            ];
    }
}
