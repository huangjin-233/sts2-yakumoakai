using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibToRitsu.Generated;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.special
{
    public sealed class Ghost : CardModel
    {
        public Ghost()
        : base(1, CardType.Power, CardRarity.Token, TargetType.Self) { }
        protected override List<DynamicVar> CanonicalVars => [new EnergyVar(3),new CardsVar(2)];// 动态变量
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue, base.Owner);//能量
            await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);//抽卡
            await PowerCmd.Apply<Ghostpower>(base.Owner.Creature, 3, base.Owner.Creature, this);
        }
        public override string PortraitPath => $"res://images/cards/power/Ghost.png";

        protected override void OnUpgrade()
        {
            base.AddKeyword(CardKeyword.Retain);
            // 升级后
        }
        public static async Task<CardModel> CreateInHand(Player owner, CombatState combatState)
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
            List<CardModel> Ghost = new List<CardModel>();
            for (int i = 0; i < count; i++)
            {
                Ghost.Add(combatState.CreateCard<Ghost>(owner));
            }
            await CardPileCmd.AddGeneratedCardsToCombat(Ghost, PileType.Hand, addedByPlayer: true);
            return Ghost;
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
                HoverTipFactory.FromPower<Ghostpower>()];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoakaiTokenCardPool), typeof(Ghost));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

