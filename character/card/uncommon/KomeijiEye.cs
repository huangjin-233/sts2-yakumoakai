using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
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
    public sealed class KomeijiEye : CardModel
    {
        public override bool GainsBlock => true;
        public override List<CardKeyword> CanonicalKeywords => [AkaiKeyword.Mpex];
        protected override List<DynamicVar> CanonicalVars => [
            new BlockVar(8m, ValueProp.Move),new DamageVar(9m, ValueProp.Move),new PowerVar<WeakPower>(1),new PowerVar<ArtifactPower>(1) // 伤害值
        ];
        // 动态变量

        public KomeijiEye()
            : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Target.Monster.IntendsToAttack)
            {
                await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);//防御
                if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 20)
                {
                    await PowerCmd.Apply<WeakPower>(cardPlay.Target, base.DynamicVars.Weak.BaseValue, base.Owner.Creature, this);//虚弱
                    await PowerCmd.Apply<mp>(base.Owner.Creature, -20m, base.Owner.Creature, this);
                    Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 20;
                    IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 4;
                    Maidknifepower.maid[base.Owner] = Maidknifepower.GetValue(base.Owner) + 20;
                    DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 20;
                }//mp效果
            }
            else
            {
                await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                    .FromCard(this) // 攻击来源
                    .Targeting(cardPlay.Target) // 攻击目标
                    .Execute(choiceContext); // 执行攻击效果
                if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 100)
                {
                    await PowerCmd.Apply<ArtifactPower>(base.Owner.Creature, base.DynamicVars.Power<ArtifactPower>().BaseValue, base.Owner.Creature, this);//人工制品
                    await PowerCmd.Apply<mp>(base.Owner.Creature, -100m, base.Owner.Creature, this);
                    Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 100;
                    IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 20;
                    Maidknifepower.maid[base.Owner] = Maidknifepower.GetValue(base.Owner) + 100;
                    DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 100;
                }//mp效果
            }
        }
        public override string PortraitPath => $"res://images/cards/attack/Komeiji_eye.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Block.UpgradeValueBy(3m);
            base.DynamicVars.Damage.UpgradeValueBy(3m);
            base.DynamicVars.Power<WeakPower>().UpgradeValueBy(1);// 升级后加 2 点伤害
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<WeakPower>(),
            HoverTipFactory.FromPower<mp>(),
            HoverTipFactory.FromPower<ArtifactPower>()
           ];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(KomeijiEye));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

