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
using YakumoAkai.character.card.special;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.rare
{
    public sealed class YukaSmile : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
            new DamageVar(18m, ValueProp.Move) // 伤害值
        ];
        // 动态变量
        public override List<CardKeyword> CanonicalKeywords => [AkaiKeyword.Mpex];
        public YukaSmile()
            : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 45)
            {
                if (base.IsUpgraded)
                {
                    await DamageCmd.Attack(65).FromCard(this)
                    .TargetingRandomOpponents(base.CombatState)
                    .WithHitFx("vfx/vfx_attack_slash")
                    .Execute(choiceContext);
                }
                else
                {
                    await DamageCmd.Attack(55).FromCard(this)
                  .TargetingRandomOpponents(base.CombatState)
                  .WithHitFx("vfx/vfx_attack_slash")
                  .Execute(choiceContext);
                }
                await PowerCmd.Apply<mp>(base.Owner.Creature, -30m, base.Owner.Creature, this);
                Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 45;
                IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 4;
                DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 45;
            }
            else
            {
                await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .TargetingAllOpponents(base.CombatState)
                    .WithHitFx("vfx/vfx_attack_slash", null, "blunt_attack.mp3")
                    .Execute(choiceContext); // 执行攻击效果
                                             //群体攻击
            }
            await YukaDress.CreateInHand(base.Owner, 1, base.CombatState);//添加手牌
        }
        public override string PortraitPath => $"res://images/cards/attack/Yuka_smile.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Damage.UpgradeValueBy(6m);
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromCard<YukaDress>(),
            HoverTipFactory.FromPower<mp>()
           ];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(YukaSmile));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

