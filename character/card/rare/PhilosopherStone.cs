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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.rare
{
    [RegisterCard(typeof(YakumoAkaiCardPool))]
    public sealed class PhilosopherStone : CardModel
    {
        // 动态变量
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust,CardKeyword.Ethereal];
        public PhilosopherStone()
            : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            NCreature? ownerNode = NCombatRoom.Instance?.GetCreatureNode(Owner.Creature);
            if (ownerNode != null)
            {
                Vector2 spawnPos = ownerNode.VfxSpawnPosition;
                Node2D? vfxNode = AkaiVfx.PlaySimple("res://scenes/vfx/PhilosopherStone/PhilosopherStone.tscn", spawnPos,1.7f);
            }

            foreach (CardModel card in PileType.Hand.GetPile(base.Owner).Cards)
            {
                if (!card.EnergyCost.CostsX)
                {
                    card.SetToFreeThisTurn();
                }
            }
            await PowerCmd.Apply<mp>(choiceContext,base.Owner.Creature, 200, base.Owner.Creature, this);
        }
        public override string PortraitPath => $"res://images/cards/skill/Philosopher_stone.png";

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Exhaust);
            // 升级后
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<mp>()];
        //关键词
    }
}

