using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace YakumoAkai.character.card.rare
{
    [RegisterCard(typeof(YakumoAkaiCardPool))]
    public sealed class GhostLiquor : ModCardTemplate
    {
        protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9m, ValueProp.Move)];
        // 动态变量
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
        public override CardPoolModel VisualCardPool => ModelDb.CardPool<YakumoAkaiCardPool>();

        public GhostLiquor()
            : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            AbstractRoom currentRoom = base.CombatState.RunState.CurrentRoom;
            if (currentRoom is CombatRoom combatRoom)
            {
                ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
                bool shouldTriggerFatal = cardPlay.Target.Powers.All((PowerModel p) => p.ShouldOwnerDeathTriggerFatal());
                AttackCommand attackCommand = await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
                    .WithHitFx("vfx/vfx_attack_slash")
                    .Execute(choiceContext);
                if (shouldTriggerFatal && attackCommand.Results.SelectMany((List<DamageResult> r) => r).Any((DamageResult r) => r.WasTargetKilled))
                {
                    combatRoom.AddExtraReward(base.Owner, new RelicReward(base.Owner));
                }
            }
        }
        public override string PortraitPath => $"res://images/cards/attack/GhostLiquor.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Damage.UpgradeValueBy(4);
        }
        protected override IEnumerable<IHoverTip> AdditionalHoverTips  => [
            HoverTipFactory.Static(StaticHoverTip.Fatal)];
        //关键词
    }
}
