using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.rare
{
    public sealed class GoldenDragonClaw : CardModel
    {
        protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move),new CardsVar(3)];
        // 动态变量
        public override CardPoolModel VisualCardPool => ModelDb.CardPool<YakumoAkaiCardPool>();
        public override List<CardKeyword> CanonicalKeywords => [AkaiKeyword.Mpex];
        public GoldenDragonClaw()
            : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            for (int i=0;i < base.DynamicVars.Cards.BaseValue; i++)
            {
                await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
             .FromCard(this) // 攻击来源
             .Targeting(cardPlay.Target) // 攻击目标
             .Execute(choiceContext); // 执行攻击效果
            }
            if (base.Owner.Creature.HasPower<mp>()   && base.Owner.Creature.GetPowerAmount<mp>()>= 30)
            {
                var debuff = base.Owner.Creature.Powers.Where(p => p.Type == PowerType.Debuff && p.Amount > 0).ToList();
                if (debuff.Any())
                {
                    var selected = debuff[new Random().Next(debuff.Count)];
                    await PowerCmd.Remove(selected);
                    await PowerCmd.Apply<mp>(base.Owner.Creature, -30, base.Owner.Creature, this);
                    Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 30;
                    IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 3;
                    DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 30;
                }
            }
        }
        public override string PortraitPath => $"res://images/cards/attack/Golden_dragon_claw.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Cards.UpgradeValueBy(1);
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
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(GoldenDragonClaw));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

