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
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using YakumoAkai.character.card.rare;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.common
{
    [RegisterCard(typeof(YakumoAkaiCardPool))]
    public sealed class FlanrdeWing : ModCardTemplate
    {
        protected override List<DynamicVar> CanonicalVars => [
            new PowerVar<mp>(50)
        ];
        // 动态变量
        public FlanrdeWing()
            : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.Damage(choiceContext, base.Owner.Creature, 3, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
            await PowerCmd.Apply<mp>(base.Owner.Creature, base.DynamicVars.Power<mp>().BaseValue, base.Owner.Creature, null);//燃烧
        }
        public override string PortraitPath => $"res://images/cards/skill/FlanrdeWing.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Power<mp>().UpgradeValueBy(10);
        }
        protected override IEnumerable<IHoverTip> AdditionalHoverTips  => [
            HoverTipFactory.FromPower<mp>()];
        //关键词
    }
}

