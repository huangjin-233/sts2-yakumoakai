using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.rare
{
    [RegisterCard(typeof(YakumoAkaiCardPool))]
    public sealed class GatesOfMordor : ModCardTemplate
    {
        public override bool GainsBlock => true;
        protected override List<DynamicVar> CanonicalVars => [
            new BlockVar(9m, ValueProp.Move)
        ];
        // 动态变量
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        public GatesOfMordor()
            : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var powers = cardPlay.Target.Powers.Where(p => p.Type == PowerType.Buff && p.Amount > 0).ToList();
            foreach (var powered in powers)
            {
                if (!(powered is AdaptablePower
                    or BattlewornDummyTimeLimitPower
                    or BurrowedPower
                    or HeistPower
                    or MinionPower
                    or PersonalHivePower
                    or RavenousPower
                    or SteamEruptionPower
                    or SurprisePower
                    or SwipePower
                    or SandpitPower
                    or BackAttackLeftPower
                    or BackAttackRightPower))
                {
                    await PowerCmd.Remove(powered);
                    await PowerCmd.Apply<DexterityPower>(choiceContext, base.Owner.Creature, 1, base.Owner.Creature, this);
                    await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, 1, base.Owner.Creature, this);
                }
            }
        }
        public override string PortraitPath => $"res://images/cards/skill/Gates_of_mordor.png";

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
        }
        protected override IEnumerable<IHoverTip> AdditionalHoverTips  => [
            HoverTipFactory.FromPower<StrengthPower>(),
            HoverTipFactory.FromPower<DexterityPower>()];
        //关键词
    }
}

