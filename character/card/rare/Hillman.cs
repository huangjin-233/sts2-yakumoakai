using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.rare
{
    public sealed class Hillman : CardModel
    {
        protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move), new CardsVar(3)];
        // 动态变量
        public override CardPoolModel VisualCardPool => ModelDb.CardPool<YakumoAkaiCardPool>();
        public Hillman()
            : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性
        public override List<CardKeyword> CanonicalKeywords => [AkaiKeyword.Mpex];
        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
                await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
             .FromCard(this) // 攻击来源
             .Targeting(cardPlay.Target) // 攻击目标
             .Execute(choiceContext); // 执行攻击效果
            if (cardPlay.Target.Powers.Any(p=>p.Type == PowerType.Debuff && p.Amount > 0)){ //debuff判定
                for (int i = 0 ;i < cardPlay.Target.Powers.Where(p => p.Type == PowerType.Debuff).Distinct().Count(); i++)
                {                 
                    await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                        .FromCard(this) // 攻击来源
                        .Targeting(cardPlay.Target) // 攻击目标
                        .Execute(choiceContext); // 执行攻击效果
                        }
            
                }    
            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 25)
            {
                await PowerCmd.Apply<VulnerablePower>(cardPlay.Target,2, base.Owner.Creature, this);//易伤
                await PowerCmd.Apply<mp>(base.Owner.Creature, -25, base.Owner.Creature, this);
                Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 25;
                IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 2;
                DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 25;
            }
        }
        public override string PortraitPath => $"res://images/cards/attack/Hillman.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Damage.UpgradeValueBy(3);
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<mp>(),
            HoverTipFactory.FromPower<VulnerablePower>()
            ];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(Hillman));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

