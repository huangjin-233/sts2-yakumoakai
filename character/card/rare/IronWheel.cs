using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using YakumoAkai.character.card.special;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.rare
{
    public sealed class IronWheel : CardModel
    {
        public static Dictionary<Player, int> card = new Dictionary<Player, int>();
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];
        public static void SetValue(Player player, int value)
        {
            IronWheel.card[player] = value;
        }
        public static int GetValue(Player player)
        {
            return IronWheel.card.TryGetValue(player, out int value) ? value : 0;
        }
        public static void IncrementValue(Player player, int value)
        {
           IronWheel.card[player] = value;
        }
        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(1m, ValueProp.Move),
            new CalculationBaseVar(1m),
            new ExtraDamageVar(1m),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier(static(CardModel card,Creature _) => IronWheel.GetValue(card.Owner))];
        // 动态变量
        public override CardPoolModel VisualCardPool => ModelDb.CardPool<YakumoAkaiCardPool>();
        public IronWheel()
            : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性
        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(base.DynamicVars.CalculatedDamage)
         .FromCard(this) // 攻击来源
         .Targeting(cardPlay.Target) // 攻击目标
         .Execute(choiceContext); // 执行攻击效果
        }
        public override string PortraitPath => $"res://images/cards/attack/IronWheel.png";

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<mp>()
            ];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(IronWheel));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

