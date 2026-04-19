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
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.special
{
    public sealed class White : CardModel
    {
        public White()
        : base(1, CardType.Skill, CardRarity.Token, TargetType.Self) { }
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (base.Owner.Creature.HasPower<AccelerantPower>())
            {
                await PowerCmd.Apply<AccelerantPower>(base.Owner.Creature, base.Owner.Creature.GetPowerAmount<AccelerantPower>(), base.Owner.Creature, this);
            }
            if (base.Owner.Creature.HasPower<AccuracyPower>())
            {
                await PowerCmd.Apply<AccuracyPower>(base.Owner.Creature, base.Owner.Creature.GetPowerAmount<AccuracyPower>(), base.Owner.Creature, this);
            }
            if (base.Owner.Creature.HasPower<AfterimagePower>())
            {
                await PowerCmd.Apply<AfterimagePower>(base.Owner.Creature, base.Owner.Creature.GetPowerAmount<AfterimagePower>(), base.Owner.Creature, this);
            }
            if (base.Owner.Creature.HasPower<ArtifactPower>())
            {
                await PowerCmd.Apply<ArtifactPower>(base.Owner.Creature, base.Owner.Creature.GetPowerAmount<ArtifactPower>(), base.Owner.Creature, this);
            }
            if (base.Owner.Creature.HasPower<ArsenalPower>())
            {
                await PowerCmd.Apply<ArsenalPower>(base.Owner.Creature, base.Owner.Creature.GetPowerAmount<ArsenalPower>(), base.Owner.Creature, this);
            }
            if (base.Owner.Creature.HasPower<StrengthPower>() && base.Owner.Creature.GetPowerAmount<StrengthPower>() > 0)
            {
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, base.Owner.Creature.GetPowerAmount<StrengthPower>(), base.Owner.Creature, this);
            }
            if (base.Owner.Creature.HasPower<DexterityPower>() && base.Owner.Creature.GetPowerAmount<DexterityPower>() > 0)
            {
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, base.Owner.Creature.GetPowerAmount<DexterityPower>(), base.Owner.Creature, this);
            }
            if (base.Owner.Creature.HasPower<AutomationPower>())
            {
                await PowerCmd.Apply<AutomationPower>(base.Owner.Creature, base.Owner.Creature.GetPowerAmount<AutomationPower>(), base.Owner.Creature, this);
            }
            if (base.Owner.Creature.HasPower<BeaconOfHopePower>())
            {
                await PowerCmd.Apply<BeaconOfHopePower>(base.Owner.Creature, base.Owner.Creature.GetPowerAmount<BeaconOfHopePower>(), base.Owner.Creature, this);
            }
            if (base.Owner.Creature.HasPower<BufferPower>())
            {
                await PowerCmd.Apply<BufferPower>(base.Owner.Creature, base.Owner.Creature.GetPowerAmount<BufferPower>(), base.Owner.Creature, this);
            }
            if (base.Owner.Creature.HasPower<BurstPower>())
            {
                await PowerCmd.Apply<BurstPower>(base.Owner.Creature, base.Owner.Creature.GetPowerAmount<BurstPower>(), base.Owner.Creature, this);
            }
            if (base.Owner.Creature.HasPower<CuriousPower>())
            {
                await PowerCmd.Apply<CuriousPower>(base.Owner.Creature, base.Owner.Creature.GetPowerAmount<CuriousPower>(), base.Owner.Creature, this);
            }
            if (base.Owner.Creature.HasPower<MayhemPower>())
            {
                await PowerCmd.Apply<MayhemPower>(base.Owner.Creature, base.Owner.Creature.GetPowerAmount<MayhemPower>(), base.Owner.Creature, this);
            }
            if (base.Owner.Creature.HasPower<PlatingPower>())
            {
                await PowerCmd.Apply<PlatingPower>(base.Owner.Creature, base.Owner.Creature.GetPowerAmount<PlatingPower>(), base.Owner.Creature, this);
            }
            if (base.Owner.Creature.HasPower<RollingBoulderPower>())
            {
                await PowerCmd.Apply<RollingBoulderPower>(base.Owner.Creature, base.Owner.Creature.GetPowerAmount<RollingBoulderPower>(), base.Owner.Creature, this);
            }
            if (base.Owner.Creature.HasPower<ThornsPower>())
            {
                await PowerCmd.Apply<ThornsPower>(base.Owner.Creature, base.Owner.Creature.GetPowerAmount<ThornsPower>(), base.Owner.Creature, this);
            }
            if (base.Owner.Creature.HasPower<VigorPower>())
            {
                await PowerCmd.Apply<VigorPower>(base.Owner.Creature, base.Owner.Creature.GetPowerAmount<VigorPower>(), base.Owner.Creature, this);
            }
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
            List<CardModel> White = new List<CardModel>();
            for (int i = 0; i < count; i++)
            {
                White.Add(combatState.CreateCard<White>(owner));
            }
            await CardPileCmd.AddGeneratedCardsToCombat(White, PileType.Hand, addedByPlayer: true);
            return White;
        }
        public override string PortraitPath => $"res://images/cards/skill/White.png";

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Exhaust);
            // 升级后
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
                HoverTipFactory.FromPower<WeakPower>()];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(ColorlessCardPool), typeof(White));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

