using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace YakumoAkai.character.card.rare
{
    public sealed class GatesOfMordor : CardModel
    {
        public override bool GainsBlock => true;
        protected override List<DynamicVar> CanonicalVars => [
            new BlockVar(9m, ValueProp.Move)
        ];
        // 动态变量
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        public GatesOfMordor()
            : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Target.HasPower<ArtifactPower>())
            {
                await PowerCmd.Remove<ArtifactPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<CrabRagePower>())
            {
                await PowerCmd.Remove<CrabRagePower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<StrengthPower>() && cardPlay.Target.GetPowerAmount<StrengthPower>() > 0)
            {
                await PowerCmd.Remove<StrengthPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<CurlUpPower>())
            {
                await PowerCmd.Remove<CurlUpPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<EscapeArtistPower>())
            {
                await PowerCmd.Remove<EscapeArtistPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<FlutterPower>())
            {
                await PowerCmd.Remove<FlutterPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<GalvanicPower>())
            {
                await PowerCmd.Remove<GalvanicPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<HardToKillPower>())
            {
                await PowerCmd.Remove<HardToKillPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<HardenedShellPower>())
            {
                await PowerCmd.Remove<HardenedShellPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<HighVoltagePower>())
            {
                await PowerCmd.Remove<HighVoltagePower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<IllusionPower>())
            {
                await PowerCmd.Remove<IllusionPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<InfestedPower>())
            {
                await PowerCmd.Remove<InfestedPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<IntangiblePower>())
            {
                await PowerCmd.Remove<IntangiblePower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<NemesisPower>())
            {
                await PowerCmd.Remove<NemesisPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<PainfulStabsPower>())
            {
                await PowerCmd.Remove<PainfulStabsPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<PaperCutsPower>())
            {
                await PowerCmd.Remove<PaperCutsPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<PersonalHivePower>())
            {
                await PowerCmd.Remove<PersonalHivePower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<RampartPower>())
            {
                await PowerCmd.Remove<RampartPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<RampartPower>())
            {
                await PowerCmd.Remove<RampartPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<SkittishPower>())
            {
                await PowerCmd.Remove<SkittishPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<SlipperyPower>())
            {
                await PowerCmd.Remove<SlipperyPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<SoarPower>())
            {
                await PowerCmd.Remove<SoarPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<SteamEruptionPower>())
            {
                await PowerCmd.Remove<SteamEruptionPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<StockPower>())
            {
                await PowerCmd.Remove<StockPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<SuckPower>())
            {
                await PowerCmd.Remove<SuckPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<SwipePower>())
            {
                await PowerCmd.Remove<SwipePower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<VitalSparkPower>())
            {
                await PowerCmd.Remove<VitalSparkPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<ThornsPower>())
            {
                await PowerCmd.Remove<ThornsPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<HungerPower>())
            {
                await PowerCmd.Remove<HungerPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<ScrutinyPower>())
            {
                await PowerCmd.Remove<ScrutinyPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<GraspPower>())
            {
                await PowerCmd.Remove<GraspPower>(cardPlay.Target);
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            }
        }
        public override string PortraitPath => $"res://images/cards/skill/Gates_of_mordor.png";

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
        }
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(GatesOfMordor));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

