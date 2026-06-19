using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.rare
{
    [RegisterCard(typeof(YakumoAkaiCardPool))]
    public sealed class DreamBirth : ModCardTemplate
    {
        public override bool GainsBlock => true;
        protected override List<DynamicVar> CanonicalVars => [
            new BlockVar(16m, ValueProp.Move)
        ];
        // 动态变量
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Ethereal];
        public DreamBirth()
            : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性
        public static int nowmp;
        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            NCreature? ownerNode = NCombatRoom.Instance?.GetCreatureNode(Owner.Creature);
                     if (ownerNode != null)
                     {
                         Vector2 spawnPos = ownerNode.VfxSpawnPosition;
                         Node2D? vfxNode = AkaiVfx.Play("res://scenes/vfx/dream/dream.tscn", spawnPos);
                 
                         // 将特效节点保存到 Timepower 能力中
                         var Dreambirth = Owner.Creature.Powers.OfType<Dreambirth>().FirstOrDefault();
                         if (Dreambirth != null && vfxNode != null)
                         {
                             Dreambirth.SetVfxNode(vfxNode);
                         }
                     }
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);//防御
            await PowerCmd.Apply<Dreambirth>(choiceContext,base.Owner.Creature, 1, base.Owner.Creature, this);
            nowmp = base.Owner.Creature.GetPowerAmount<mp>();
            await PowerCmd.Apply<mp>(choiceContext,base.Owner.Creature, 150, base.Owner.Creature, this);
            //mp 效果
            
        }
        public override string PortraitPath => $"res://images/cards/skill/Dream_birth.png";

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
        }
        protected override IEnumerable<IHoverTip> AdditionalHoverTips  => [
            HoverTipFactory.FromPower<Dreambirth>(),
            HoverTipFactory.FromPower<mp>()];
        //关键词
    }
}

