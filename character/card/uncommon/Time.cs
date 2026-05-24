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
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.uncommon
{
    [RegisterCard(typeof(YakumoAkaiCardPool

))]
    public sealed class Time : CardModel
    {
        public Time()
            : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust,CardKeyword.Retain];
        
        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<Timepower>(choiceContext, base.Owner.Creature, 1, base.Owner.Creature, this);
            if (IsUpgraded)
            {
                await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, base.Owner.Creature,1, base.Owner.Creature, this);
            } 
            NCreature? ownerNode = NCombatRoom.Instance?.GetCreatureNode(Owner.Creature);
            if (ownerNode != null)
            {
                Vector2 spawnPos = ownerNode.VfxSpawnPosition;
                Node2D? vfxNode = AkaiVfx.Playback("res://scenes/vfx/time/time.tscn", spawnPos);
        
                // 将特效节点保存到 Timepower 能力中
                var timepower = Owner.Creature.Powers.OfType<Timepower>().FirstOrDefault();
                if (timepower != null && vfxNode != null)
                {
                    timepower.SetVfxNode(vfxNode);
                }
            }
        }
        public override string PortraitPath => $"res://images/cards/skill/Time.png";

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
            // 升级后
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<Timepower>()];
        //关键词
    }
}

