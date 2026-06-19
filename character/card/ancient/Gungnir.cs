 using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Content;
using YakumoAkai.character.card.rare;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.ancient
{
    [RegisterCard(typeof(YakumoAkaiCardPool))]
    public sealed class Gungnir : ModCardTemplate
    {
        protected override List<DynamicVar> CanonicalVars => [
            new DamageVar(16m, ValueProp.Move),new CardsVar(2) // 伤害值
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [AkaiKeyword.Mpex
                                                                ];
        public Gungnir()
        : base(1, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            foreach (Creature Enemy in base.CombatState.HittableEnemies)
            { 
                await CreatureCmd.LoseBlock(Enemy, Enemy.Block); 
            }
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
             .FromCard(this) // 攻击来源
             .TargetingAllOpponents(base.CombatState) // 攻击目标
             .Execute(choiceContext); // 执行攻击效果
            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 10)
            {
                await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);//抽卡
                await PowerCmd.Apply<mp>(choiceContext,base.Owner.Creature, -10m, base.Owner.Creature, this);
                Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 10;
                IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 2;
                DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 10;
                Maidknifepower.maid[base.Owner] = Maidknifepower.GetValue(base.Owner) + 10;
            }//mp 效果
        }
        public override string PortraitPath => $"res://images/cards/attack/Gungnir.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Damage.UpgradeValueBy(2m);
            base.DynamicVars.Cards.UpgradeValueBy(1);// 升级后
        }
        protected override IEnumerable<IHoverTip> AdditionalHoverTips  => [
            HoverTipFactory.FromPower<mp>()];
        //关键词
    }
}

