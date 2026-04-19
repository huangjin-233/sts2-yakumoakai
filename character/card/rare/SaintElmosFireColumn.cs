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
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.rare
{
    public sealed class SaintElmosFireColumn : CardModel
    {
        public override bool GainsBlock => true;
        protected override List<DynamicVar> CanonicalVars => [
            new DamageVar(7m, ValueProp.Move),new BlockVar(5m, ValueProp.Move),new PowerVar<Fire>(3)
        ];
        protected override bool HasEnergyCostX => true;
        // 动态变量
        public override List<CardKeyword> CanonicalKeywords => [AkaiKeyword.Mpex];
        public SaintElmosFireColumn()
            : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
    
                int num = ResolveEnergyXValue(); 
            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 30)
            {
                num += 2;
                await PowerCmd.Apply<mp>(base.Owner.Creature, -30m, base.Owner.Creature, this);
                Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 30;
                IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 3;
                DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 30;
            }
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(num).FromCard(this)
               .Targeting(CurrentTarget)
               .WithHitFx("vfx/vfx_giant_horizontal_slash")
               .Execute(choiceContext);  //X攻击
            for (int i = 0; i < num; i++)
            {
                await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
                await PowerCmd.Apply<Fire>(cardPlay.Target, base.DynamicVars.Power<Fire>().BaseValue, base.Owner.Creature, this);
            }
        }
        public override string PortraitPath => $"res://images/cards/attack/Saint_elmos_fire_column.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Damage.UpgradeValueBy(2m);
            base.DynamicVars.Block.UpgradeValueBy(1m);
            base.DynamicVars.Power<Fire>().UpgradeValueBy(1);// 升级后
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
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(SaintElmosFireColumn));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

