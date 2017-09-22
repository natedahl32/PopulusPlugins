using FluentBehaviourTree;
using Populus.Core.World.Objects;

namespace Populus.GroupBot.Combat.Warrior
{
    public class FuryCombatLogic : WarriorCombatLogic
    {
        #region Declarations

        private bool mOverpowerProcced = false;

        #endregion

        #region Constructors

        public FuryCombatLogic(GroupBotHandler botHandler) : base(botHandler)
        {
            UseHeroicStrikeRageThreshold = 50;
        }

        #endregion

        #region Private Methods

        protected override IBehaviourTreeNode OutOfCombatBuffsTree()
        {
            var builder = new BehaviourTreeBuilder();
            builder.Selector("Fury Warrior Buffs")
                        .Do("Berserker Stance", t => SelfBuff(BERSERKER_STANCE))
                        .Do("Battle Shout", t => GroupBuff(BATTLE_SHOUT))
                   .End();
            return builder.Build();
        }

        protected override IBehaviourTreeNode CombatRotationTree()
        {
            var builder = new BehaviourTreeBuilder();
            builder.Selector("Fury Warrior Rotation")
                        .Do("Berserker Stance", t => SelfBuff(BERSERKER_STANCE))
                        .Do("Battle Shout", t => SelfBuff(BATTLE_SHOUT))        // We might not have the rage to do this until combat
                        .Do("Execute", t => Execute())
                        .Do("Bloodthirst", t => CastMeleeSpell(BLOODTHIRST))
                        .Do("Whirlwind", t => CastMeleeSpell(WHIRLWIND))
                        .Do("Overpower", t=> Overpower())
                   .End();
            return builder.Build();
        }

        protected override void CombatAttackUpdate(Bot bot, Core.World.Objects.Events.CombatAttackUpdateArgs eventArgs)
        {
            // check for revenge procs
            if (bot.Guid == BotHandler.BotOwner.Guid && eventArgs.AttackerGuid == BotHandler.BotOwner.Guid)
            {
                // Procs after block, dodge or parry
                if (BotHandler.BotOwner.HasSpell((ushort)REVENGE) && eventArgs.Dodged)
                    mOverpowerProcced = true;
            }

            // process base
            base.CombatAttackUpdate(bot, eventArgs);
        }

        #endregion

        #region Combat Behaviors

        /// <summary>
        /// Casts execute if we can use it
        /// </summary>
        /// <returns></returns>
        private BehaviourTreeStatus Execute()
        {
            // If target health is not below 20%, fail
            if (BotHandler.CombatState.CurrentTarget.HealthPercentage >= 20f)
                return BehaviourTreeStatus.Failure;
            return CastMeleeSpell(EXECUTE);
        }

        /// <summary>
        /// Casts overpower if we can use it
        /// </summary>
        /// <returns></returns>
        private BehaviourTreeStatus Overpower()
        {
            // If overpower has not procced, fail
            if (!mOverpowerProcced)
                return BehaviourTreeStatus.Failure;
            // If conditions are not right, fail
            if (!(BotHandler.BotOwner.CurrentPower <= 45 || (BotHandler.BotOwner.SpellIsOnCooldown(BLOODTHIRST) && BotHandler.BotOwner.SpellIsOnCooldown(WHIRLWIND))))
                return BehaviourTreeStatus.Failure;
            // If we are not within range
            if (!IsInMeleeRange(BotHandler.CombatState.CurrentTarget))
                return BehaviourTreeStatus.Failure;
            // If we can't cast, fail
            if (!HasSpellAndCanCast(OVERPOWER))
                return BehaviourTreeStatus.Failure;

            mOverpowerProcced = false;
            BotHandler.CombatState.SpellCast(OVERPOWER);
            return BehaviourTreeStatus.Success;
        }

        #endregion
    }
}
