using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibToRitsu.Generated;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib;
using YakumoAkai.character.card.rare;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.uncommon
{
    public sealed class Mimi : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
            new DamageVar(12m, ValueProp.Move),new EnergyVar(2),new PowerVar<Fire>(3)
        ];
        // 动态变量
        private bool flag=false;
        public override List<CardKeyword> CanonicalKeywords => [AkaiKeyword.Mpex];
        public Mimi()
            : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性
        public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
        {
            if (this.EnergyCost.GetWithModifiers(CostModifiers.All) == 0)
            {
            }
            else
            {
                if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 15)
                {
                    this.EnergyCost.SetThisTurnOrUntilPlayed(1);
                    flag = true;

                }
                else
                {
                    flag = false;
                }
            }
        }//mp改变时
        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 20)
            {
                this.EnergyCost.SetThisTurn(1);
                flag = true;
            }
            else
            {
                this.EnergyCost.SetThisTurn(2);
                flag = false;
            }
        }
        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
             .FromCard(this) // 攻击来源
             .Targeting(cardPlay.Target) // 攻击目标
             .Execute(choiceContext); // 执行攻击效果
            await PowerCmd.Apply<Fire>(base.CombatState.HittableEnemies, base.DynamicVars.Power<Fire>().BaseValue, base.Owner.Creature, this);//燃烧
            if (flag == true)
            {
                await PowerCmd.Apply<mp>(base.Owner.Creature, -15m, base.Owner.Creature, this);
                Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 15;
                IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 3;
                Maidknifepower.maid[base.Owner] = Maidknifepower.GetValue(base.Owner) + 15;
                DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 15;
            }
            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 20)
            {
                this.EnergyCost.SetThisTurnOrUntilPlayed(1);
                flag = true;
            }
        }
        protected override PileType GetResultPileType()
        {
            PileType resultPileType = base.GetResultPileType();
            if (resultPileType != PileType.Discard)
            {
                return resultPileType;
            }
            return PileType.Hand;
        }
        public override string PortraitPath => $"res://images/cards/attack/Mimi.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Damage.UpgradeValueBy(5m); // 升级后
            base.DynamicVars.Power<Fire>().UpgradeValueBy(1);
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<mp>()];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(Mimi));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

