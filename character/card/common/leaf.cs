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
    public sealed class Leaf : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
            new PowerVar<Fire>(3)
        ];
        // 动态变量
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, AkaiKeyword.Mpex];
        public Leaf()
            : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            
            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 10)
            {
                await PowerCmd.Apply<Fire>(base.CombatState.HittableEnemies, base.DynamicVars.Power<Fire>().BaseValue, base.Owner.Creature, this);//燃烧
                await PowerCmd.Apply<mp>(base.Owner.Creature, -10m, base.Owner.Creature, this);
                Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 10;
                IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 1;
                DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 10;
            }
            else
            {
                await PowerCmd.Apply<Fire>(cardPlay.Target, base.DynamicVars.Power<Fire>().BaseValue, base.Owner.Creature, null);//燃烧
            }
            //mp 效果
        }
        public override string PortraitPath => $"res://images/cards/attack/Leaf.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Power<Fire>().UpgradeValueBy(1);
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<Fire>()
            ];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(Leaf));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

