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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using YakumoAkai.character.card.rare;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.uncommon
{
    public sealed class FoldingFanOfSaigyouji : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
            new PowerVar<StrengthPower>(2),
            new PowerVar<WeakPower>(3),
            new PowerVar<VulnerablePower>(2),
            new PowerVar<PoisonPower>(5),
            new PowerVar<DoomPower>(8),
            new PowerVar<Fire>(5)// 伤害值
        ];
        // 动态变量
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust,AkaiKeyword.Mpex];
        public FoldingFanOfSaigyouji()
            : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllAllies) { }
        // 卡牌的构造函数，指定卡牌的相关属性
        public override Task BeforeCardPlayed(CardPlay cardPlay)
        {
            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 15)
            {
                this.EnergyCost.SetThisTurnOrUntilPlayed(1);
            }
            else
            {
                this.EnergyCost.SetThisTurnOrUntilPlayed(2);
            }
            return Task.CompletedTask;
        }//打出其他牌时判定
        public override async Task AfterCardDrawnEarly(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
        {

            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 15)
            {
                this.EnergyCost.SetThisTurnOrUntilPlayed(1);
            }
            else
            {
                this.EnergyCost.SetThisTurnOrUntilPlayed(2);
            }
        }//抽卡时判定
        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<VulnerablePower>(base.CombatState.HittableEnemies, base.DynamicVars.Vulnerable.BaseValue, base.Owner.Creature, this);//易伤
            await PowerCmd.Apply<StrengthPower>(base.CombatState.HittableEnemies, -base.DynamicVars.Strength.BaseValue, base.Owner.Creature, this);//减力量
            await PowerCmd.Apply<WeakPower>(base.CombatState.HittableEnemies, base.DynamicVars.Weak.BaseValue, base.Owner.Creature, this);//虚弱
            await PowerCmd.Apply<PoisonPower>(base.CombatState.HittableEnemies, base.DynamicVars.Poison.BaseValue, base.Owner.Creature, this);//毒
            await PowerCmd.Apply<Fire>(base.CombatState.HittableEnemies,base.DynamicVars.Power<Fire>().BaseValue, base.Owner.Creature, this);//燃烧
            await PowerCmd.Apply<DoomPower>(base.CombatState.HittableEnemies, base.DynamicVars.Doom.BaseValue, base.Owner.Creature, this);//燃烧
            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 15)
            {
                await PowerCmd.Apply<mp>(base.Owner.Creature, -15m, base.Owner.Creature, this);
                Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 15;
                IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 1;
                DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 15;
            }
            //mp效果
        }
        public override string PortraitPath => $"res://images/cards/attack/Folding_fan_of_saigyouji.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Strength.UpgradeValueBy(1);
            base.DynamicVars.Vulnerable.UpgradeValueBy(1);
            base.DynamicVars.Weak.UpgradeValueBy(1);
            base.DynamicVars.Poison.UpgradeValueBy(3);
            base.DynamicVars.Doom.UpgradeValueBy(5);
            base.DynamicVars.Power<Fire>().UpgradeValueBy(3);/// 升级后
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<StrengthPower>(),
            HoverTipFactory.FromPower<WeakPower>(),
            HoverTipFactory.FromPower<VulnerablePower>(),
            HoverTipFactory.FromPower<PoisonPower>(),
            HoverTipFactory.FromPower<Fire>(),
            HoverTipFactory.FromPower<mp>()];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(FoldingFanOfSaigyouji));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

