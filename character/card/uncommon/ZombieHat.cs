using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibToRitsu.Generated;
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
using YakumoAkai.character.card.rare;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.uncommon
{
    public sealed class ZombieHat : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [new PowerVar<StrengthPower>(5),new PowerVar<DexterityPower>(1)];// 动态变量
        public override List<CardKeyword> CanonicalKeywords => [AkaiKeyword.Mpex];
        public ZombieHat()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<Zombiedex>(base.Owner.Creature, -base.DynamicVars.Power<DexterityPower>().BaseValue, base.Owner.Creature, this);//临时敏捷
            await PowerCmd.Apply<Zombiestrength>(base.Owner.Creature, base.DynamicVars.Power<StrengthPower>().BaseValue, base.Owner.Creature, this);//临时力量
            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 15)
            {
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1, base.Owner.Creature, this);//力量
                await PowerCmd.Apply<mp>(base.Owner.Creature, -15, base.Owner.Creature, this);
                Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 15;
                IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 3;
                DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 15;
                Maidknifepower.maid[base.Owner] = Maidknifepower.GetValue(base.Owner) + 15;
            }
        }

        public override string PortraitPath => $"res://images/cards/skill/Zombie_hat.png";

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
            // 升级后
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
                HoverTipFactory.FromPower<DexterityPower>(),
                HoverTipFactory.FromPower<StrengthPower>()];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(ZombieHat));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

