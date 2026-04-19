using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.ValueProps;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.rare
{
    public sealed class FriendOfDevil : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
            new HpLossVar(3) // 伤害值
        ];
        // 动态变量

        public FriendOfDevil()
            : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            int h = (int)((int)Owner.Creature.CurrentHp * 30 / 100m);
            await CreatureCmd.Damage(choiceContext, base.Owner.Creature, h, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
            if (base.Owner.Creature.MaxHp <= 0)
            {
                await CreatureCmd.Heal(base.Owner.Creature, 1);
            }
            AttackCommand attackCommand=await DamageCmd.Attack(h*1.5m).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext); // 执行攻击效果
            await CreatureCmd.GainBlock(base.Owner.Creature, attackCommand.Results.Sum((DamageResult r) => r.TotalDamage + r.OverkillDamage), ValueProp.Move, cardPlay);
        }
        public override string PortraitPath => $"res://images/cards/attack/Friend_of_devil.png";

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
        }
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(FriendOfDevil));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

