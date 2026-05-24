using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibToRitsu.Generated;
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
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using YakumoAkai.character.card.special;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.rare
{
    [RegisterCard(typeof(YakumoAkaiCardPool))]
    public sealed class YukaSmile : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
            new DamageVar(18m, ValueProp.Move) // 伤害值
        ];
        // 动态变量
        public override List<CardKeyword> CanonicalKeywords => [AkaiKeyword.Mpex.GetModCardKeyword()
                                                                ];
        public YukaSmile()
            : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 45)
            {
                if (base.IsUpgraded)
                {
                    await DamageCmd.Attack(65).FromCard(this)
                    .TargetingRandomOpponents(base.CombatState)
                    .BeforeDamage(async delegate
                    {
                        List<Creature> enemies = base.CombatState.Enemies.Where((Creature e) => e.IsAlive).ToList();
                        NHyperbeamVfx nHyperbeamVfx = NHyperbeamVfx.Create(base.Owner.Creature, enemies.Last());
                        if (nHyperbeamVfx != null)
                        {
                            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nHyperbeamVfx);
                            await Cmd.Wait(0.5f);
                        }
                        foreach (Creature item in enemies)
                        {
                            NHyperbeamImpactVfx nHyperbeamImpactVfx = NHyperbeamImpactVfx.Create(base.Owner.Creature, item);
                            if (nHyperbeamImpactVfx != null)
                            {
                                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nHyperbeamImpactVfx);
                            }
                        }
                    })
                    .Execute(choiceContext);
                }
                else
                {
                    await DamageCmd.Attack(55).FromCard(this)
                  .TargetingRandomOpponents(base.CombatState)
                  .BeforeDamage(async delegate
                  {
                      List<Creature> enemies = base.CombatState.Enemies.Where((Creature e) => e.IsAlive).ToList();
                      NHyperbeamVfx nHyperbeamVfx = NHyperbeamVfx.Create(base.Owner.Creature, enemies.Last());
                      if (nHyperbeamVfx != null)
                      {
                          NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nHyperbeamVfx);
                          await Cmd.Wait(0.5f);
                      }
                      foreach (Creature item in enemies)
                      {
                          NHyperbeamImpactVfx nHyperbeamImpactVfx = NHyperbeamImpactVfx.Create(base.Owner.Creature, item);
                          if (nHyperbeamImpactVfx != null)
                          {
                              NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nHyperbeamImpactVfx);
                          }
                      }
                  })
                  .Execute(choiceContext);
                }
                await PowerCmd.Apply<mp>(choiceContext,base.Owner.Creature, -30m, base.Owner.Creature, this);
                Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 45;
                IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 9;
                Maidknifepower.maid[base.Owner] = Maidknifepower.GetValue(base.Owner) + 45;
                DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 45;
            }
            else
            {
                await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .TargetingAllOpponents(base.CombatState)
                    .BeforeDamage(async delegate
                    {
                        List<Creature> enemies = base.CombatState.Enemies.Where((Creature e) => e.IsAlive).ToList();
                        NHyperbeamVfx nHyperbeamVfx = NHyperbeamVfx.Create(base.Owner.Creature, enemies.Last());
                        if (nHyperbeamVfx != null)
                        {
                            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nHyperbeamVfx);
                            await Cmd.Wait(0.5f);
                        }
                        foreach (Creature item in enemies)
                        {
                            NHyperbeamImpactVfx nHyperbeamImpactVfx = NHyperbeamImpactVfx.Create(base.Owner.Creature, item);
                            if (nHyperbeamImpactVfx != null)
                            {
                                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nHyperbeamImpactVfx);
                            }
                        }
                    })
                    .Execute(choiceContext); // 执行攻击效果
                                             //群体攻击
            }
            await YukaDress.CreateInHand(base.Owner, 1, base.CombatState);//添加手牌
        }
        public override string PortraitPath => $"res://images/cards/attack/Yuka_smile.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Damage.UpgradeValueBy(6m);
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromCard<YukaDress>(),
            HoverTipFactory.FromPower<mp>()
           ];
        //关键词
    }
}

