using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.special
{
    internal class KindSoul : CardModel
    {
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

        // 动态变量

        public KindSoul()
            : base(10, CardType.Power, CardRarity.Token, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            Kind.mp[base.Owner] =0;
            await PowerCmd.Apply<Kind>(base.Owner.Creature, 1m, base.Owner.Creature, this);//给予能力
        }
        public static async Task<CardModel?> CreateInHand(Player owner, CombatState combatState)
        {
            return (await CreateInHand(owner, 1, combatState)).FirstOrDefault();
        }
        public static async Task<IEnumerable<CardModel>> CreateInHand(Player owner, int count, CombatState combatState)
        {
            if (count == 0)
            {
                return Array.Empty<CardModel>();
            }
            if (CombatManager.Instance.IsOverOrEnding)
            {
                return Array.Empty<CardModel>();
            }
            List<CardModel> kindsoul = new List<CardModel>();
            for (int i = 0; i < count; i++)
            {
                kindsoul.Add(combatState.CreateCard<KindSoul>(owner));
            }
            await CardPileCmd.AddGeneratedCardsToCombat(kindsoul, PileType.Hand, addedByPlayer: true);
            return kindsoul;
        }
        //计数
        public override string PortraitPath => $"res://images/cards/power/Kind_soul.png";
        public override Task AfterCardEnteredCombat(CardModel card)
        {
            if (card != this)
            {
                return Task.CompletedTask;
            }
            if (base.IsClone)
            {
                return Task.CompletedTask;
            }
            int amount = CombatManager.Instance.History.CardPlaysFinished.Count((CardPlayFinishedEntry e) =>e.CardPlay.Card.Owner == base.Owner);
            ReduceCostBy(amount);
            return Task.CompletedTask;
        }
        public override Task BeforeCardPlayed(CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != base.Owner)
            {
                return Task.CompletedTask;
            }
            ReduceCostBy(1);//减费
            return Task.CompletedTask;
        }

        private void ReduceCostBy(int amount)
        {
            base.EnergyCost.AddThisCombat(-amount);
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<mp>()];
        //关键词
    }
    [ModInitializer(nameof(Initialize))]
    public static class YakumoakaiInitializer
    {
        public static void Initialize()
        {
            {
                ModHelper.AddModelToPool<ColorlessCardPool, KindSoul>();

                var harmony = new Harmony("huangjin.yakumoakai");
                harmony.PatchAll();
                // 初始化 harmony 库
            }
        }
    }
}

