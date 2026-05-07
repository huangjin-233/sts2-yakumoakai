using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibToRitsu.Generated;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace YakumoAkai.character.card.rare
{
    [RegisterCard(typeof(YakumoAkaiCardPool))]
    public sealed class ImmortalWieldsEnergy : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
           new PowerVar<IntangiblePower>(1)
        ];
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        // 动态变量
        public ImmortalWieldsEnergy()
            : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            IEnumerable<CardModel> distinctForCombat= CardFactory.GetDistinctForCombat(
                base.Owner, 
                from c in base.Owner.Creature.Player.Character.CardPool.GetUnlockedCards(
                    base.Owner.Creature.Player.UnlockState, 
                    base.Owner.Creature.CombatState.RunState.CardMultiplayerConstraint)
                where c.Rarity == CardRarity.Rare
                select c,
                1,
                base.Owner.Creature.CombatState.RunState.Rng.CombatCardGeneration).ToList();
            foreach(CardModel card in distinctForCombat)
            {
                card.SetToFreeThisCombat();
                card.AddKeyword(CardKeyword.Exhaust);
                if (base.IsUpgraded)
                {
                    CardCmd.Upgrade(card);
                }
            }
            await CardPileCmd.AddGeneratedCardsToCombat(distinctForCombat, PileType.Hand, addedByPlayer: true);
        }
        public override string PortraitPath => $"res://images/cards/skill/Immortal_wields_energy.png";

        protected override void OnUpgrade()
        {
        }
    }
}

