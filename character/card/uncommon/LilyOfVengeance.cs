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
    public sealed class LilyOfVengeance : CardModel
    {
        protected override bool HasEnergyCostX => true;
        protected override List<DynamicVar> CanonicalVars => [
            new DamageVar(5m, ValueProp.Move) // 伤害值
        ];
        // 动态变量
        public override List<CardKeyword> CanonicalKeywords => [AkaiKeyword.Mpex];
        public LilyOfVengeance()
            : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            int num = ResolveEnergyXValue();
            if (base.IsUpgradable)
            {
                num++;
            }
            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 15)
            {
                base.DynamicVars.Damage.UpgradeValueBy(2m);
                await PowerCmd.Apply<mp>(base.Owner.Creature, -15m, base.Owner.Creature, this);
                Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 15;
                IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 3;
                Maidknifepower.maid[base.Owner] = Maidknifepower.GetValue(base.Owner) + 15;
                DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 15;
            }
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(num).FromCard(this)
                .TargetingAllOpponents(base.CombatState)
                .WithHitFx("vfx/vfx_giant_horizontal_slash")
                .Execute(choiceContext);  //X攻击
        }
        protected override void OnUpgrade()
        {
            base.DynamicVars.Damage.UpgradeValueBy(2m);
        }

        public override string PortraitPath => $"res://images/cards/attack/Lily_of_vengeance.png";
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<mp>()];
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(LilyOfVengeance));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

