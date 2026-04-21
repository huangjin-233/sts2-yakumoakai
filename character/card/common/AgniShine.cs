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

namespace YakumoAkai.character.card.common
{
    public sealed class AgniShine : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
            new DamageVar(4m, ValueProp.Move),new PowerVar<Fire>(3)
        ];
        // 动态变量
        public override List<CardKeyword> CanonicalKeywords => [AkaiKeyword.Mpex];
        public AgniShine()
            : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                     .FromCard(this) // 攻击来源
                     .Targeting(cardPlay.Target) // 攻击目标
                     .Execute(choiceContext); // 执行攻击效果
            await PowerCmd.Apply<Fire>(cardPlay.Target, base.DynamicVars.Power<Fire>().BaseValue, base.Owner.Creature, null);//燃烧
            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 10)
            {
                await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                      .FromCard(this) // 攻击来源
                      .Targeting(cardPlay.Target) // 攻击目标
                      .Execute(choiceContext); // 执行攻击效果
                await PowerCmd.Apply<mp>(base.Owner.Creature, -10m, base.Owner.Creature, this);
                Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 10;
                IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 2;
                Maidknifepower.maid[base.Owner] = Maidknifepower.GetValue(base.Owner) + 10;
                DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 10;
            }
        }
        public override string PortraitPath => $"res://images/cards/attack/Agni_Shine.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Power<Fire>().UpgradeValueBy(1);
            base.DynamicVars.Damage.UpgradeValueBy(3);
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<Fire>(),
            HoverTipFactory.FromPower<mp>()
            ];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(AgniShine));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

