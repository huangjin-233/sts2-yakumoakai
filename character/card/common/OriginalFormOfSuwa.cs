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
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.common
{
    [RegisterCard(typeof(YakumoAkaiCardPool))]
    public sealed class OriginalFormOfSuwa : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
            new DamageVar(2m, ValueProp.Move),new CardsVar(6)
        ];
        // 动态变量

        public OriginalFormOfSuwa()
            : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            NCreature? ownerNode = NCombatRoom.Instance?.GetCreatureNode(cardPlay.Target);
            if (ownerNode != null)
            {
                Vector2 spawnPos = ownerNode.VfxSpawnPosition;
                Node2D? vfxNode = AkaiVfx.PlaySimple("res://scenes/vfx/suwako/suwako.tscn", spawnPos, 2.37f);
            }
            for (int i = 0; i < base.DynamicVars.Cards.IntValue; i++)
            {
                await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
             .FromCard(this) // 攻击来源
             .Targeting(cardPlay.Target) // 攻击目标
             .Execute(choiceContext); // 执行攻击效果}
            } 
            //多段
                
           
        }
        public override string PortraitPath => $"res://images/cards/attack/Original_form_of_Suwa.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Cards.UpgradeValueBy(1m); // 升级后
        }
    }
}

