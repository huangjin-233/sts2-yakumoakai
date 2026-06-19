using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace YakumoAkai.character.card.special
{
    [RegisterCard(typeof(YakumoakaiTokenCardPool))]
    public sealed class YukaDress : CardModel
    {
        public override bool GainsBlock => true;
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust,CardKeyword.Retain];
        public YukaDress()
        : base(0, CardType.Skill, CardRarity.Token, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性
        protected override List<DynamicVar> CanonicalVars => [
            new BlockVar(10m, ValueProp.Move)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);//防御
        }
        public override string PortraitPath => $"res://images/cards/skill/Yuka_dress.png";

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
            // 升级后
        }
        public static async Task<CardModel> CreateInHand(Player owner, ICombatState combatState)
        {
            return (await CreateInHand(owner, 1, combatState)).FirstOrDefault();
        }
        public static async Task<IEnumerable<CardModel>> CreateInHand(Player owner, int count, ICombatState combatState)
        {
            if (count == 0)
            {
                return Array.Empty<CardModel>();
            }
            if (CombatManager.Instance.IsOverOrEnding)
            {
                return Array.Empty<CardModel>();
            }
            List<CardModel> YukaDress = new List<CardModel>();
            for (int i = 0; i < count; i++)
            {
                YukaDress.Add(combatState.CreateCard<YukaDress>(owner));
            }
            await CardPileCmd.AddGeneratedCardsToCombat(YukaDress, PileType.Hand, owner);
            return YukaDress;
        }
    }
}

