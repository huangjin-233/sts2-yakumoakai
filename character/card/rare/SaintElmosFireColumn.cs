using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibToRitsu.Generated;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Content;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.rare
{
    [RegisterCard(typeof(YakumoAkaiCardPool))]
    public sealed class SaintElmosFireColumn : ModCardTemplate
    {
        public override bool GainsBlock => true;
        protected override List<DynamicVar> CanonicalVars => [
            new DamageVar(7m, ValueProp.Move),new BlockVar(5m, ValueProp.Move),new PowerVar<Fire>(3)
        ];
        protected override bool HasEnergyCostX => true;
        // 动态变量
        public override IEnumerable<CardKeyword> CanonicalKeywords => [AkaiKeyword.Mpex
                                                                ];
        public SaintElmosFireColumn()
            : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            NCreature? ownerNode = NCombatRoom.Instance?.GetCreatureNode(cardPlay.Target);
            if (ownerNode != null)
            {
                Vector2 spawnPos = ownerNode.VfxSpawnPosition;
                Node2D? vfxNode = AkaiVfx.PlaySimple("res://scenes/vfx/saint/saint.tscn", spawnPos, 1f);
            }
            int num = ResolveEnergyXValue(); 
            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 30)
            {
                num += 2;
                await PowerCmd.Apply<mp>(choiceContext,base.Owner.Creature, -30m, base.Owner.Creature, this);
                Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 30;
                IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 6;
                Maidknifepower.maid[base.Owner] = Maidknifepower.GetValue(base.Owner) + 30;
                DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 30;
            }
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(num).FromCard(this)
               .Targeting(CurrentTarget)
               .WithHitFx("vfx/vfx_giant_horizontal_slash")
               .Execute(choiceContext);  //X攻击
            for (int i = 0; i < num; i++)
            {
                await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
                await PowerCmd.Apply<Fire>(choiceContext,cardPlay.Target, base.DynamicVars.Power<Fire>().BaseValue, base.Owner.Creature, this);
            }
        }
        public override string PortraitPath => $"res://images/cards/attack/Saint_elmos_fire_column.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Damage.UpgradeValueBy(2m);
            base.DynamicVars.Block.UpgradeValueBy(1m);
            base.DynamicVars.Power<Fire>().UpgradeValueBy(1);// 升级后
        }
        protected override IEnumerable<IHoverTip> AdditionalHoverTips  => [
            HoverTipFactory.FromPower<Fire>(),
            HoverTipFactory.FromPower<mp>()
        ];
        //关键词
    }
}

