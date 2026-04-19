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
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using YakumoAkai.character.card.ancient;
using YakumoAkai.character.card.rare;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.basic
{
    [Pool(typeof(YakumoAkaiCardPool))]
    public sealed class GodGungnir : CustomCardModel, ITranscendenceCard
    {
        protected override List<DynamicVar> CanonicalVars => [
            new DamageVar(10m, ValueProp.Move),new CardsVar(1) // 伤害值
        ];
        public override List<CardKeyword> CanonicalKeywords => [AkaiKeyword.Mpex];
        public GodGungnir()
            : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.LoseBlock(cardPlay.Target, cardPlay.Target.Block);
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
             .FromCard(this) // 攻击来源
             .Targeting(cardPlay.Target) // 攻击目标
             .Execute(choiceContext); // 执行攻击效果
            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 10)
            {
                await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);//抽卡
                await PowerCmd.Apply<mp>(base.Owner.Creature, -10m, base.Owner.Creature, this);
                Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 10;
                IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 1;
                DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 10;
            }//mp 效果
        }
        protected override void OnUpgrade()
        {
            base.DynamicVars.Damage.UpgradeValueBy(2m);
            base.DynamicVars.Cards.UpgradeValueBy(1);// 升级后
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<mp>()];
        //关键词
        public CardModel GetTranscendenceTransformedCard() => ModelDb.Card<Gungnir>(); // 实现方法。自己更改类型。
        public override string PortraitPath => $"res://images/cards/attack/Gungnir.png";

    }
}

