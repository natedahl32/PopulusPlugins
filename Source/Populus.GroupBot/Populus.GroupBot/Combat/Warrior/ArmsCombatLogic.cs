using FluentBehaviourTree;
using Populus.Core.World.Objects;

namespace Populus.GroupBot.Combat.Warrior
{
    public class ArmsCombatLogic : WarriorCombatLogic
    {
        #region Declarations

        private const uint IMP_OVERPOWER_1 = 12290;
        private const uint IMP_OVERPOWER_2 = 12963;

        private bool mOverpowerProcced = false;

        #endregion

        #region Constructors

        public ArmsCombatLogic(GroupBotHandler botHandler) : base(botHandler)
        {
        }

        #endregion

        #region Public Methods

        public override void CombatAttackUpdate(Bot bot, Core.World.Objects.Events.CombatAttackUpdateArgs eventArgs)
        {
            // check for overpower procs
            if (bot.Guid == BotHandler.BotOwner.Guid && eventArgs.AttackerGuid == BotHandler.BotOwner.Guid)
            {
                // Procs after dodge
                if (BotHandler.BotOwner.HasSpell((ushort)OVERPOWER) && eventArgs.Dodged)
                    mOverpowerProcced = true;
            }

            // process base
            base.CombatAttackUpdate(bot, eventArgs);
        }

        #endregion

        #region Private Methods

        protected override IBehaviourTreeNode OutOfCombatBuffsTree()
        {
            var builder = new BehaviourTreeBuilder();
            builder.Selector("Arms Warrior Buffs")
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
                        .Do("Improved Overpower", t => ImprovedOverpower())
                        .Do("Mortal Strike", t => CastMeleeSpell(MORTAL_STRIKE))
                        .Do("Whirlwind", t => CastMeleeSpell(WHIRLWIND))
                        .Do("Overpower", t => Overpower())
                        .Do("Slam", t => CastMeleeSpell(SLAM))
                        .Do("Heroic Strike", t => CastMeleeSpell(HEROIC_STRIKE))
                   .End();
            return builder.Build();
        }

        #endregion

        #region Combat Behaviors

        /// <summary>
        /// Casts overpower if it is talented
        /// </summary>
        /// <returns></returns>
        private BehaviourTreeStatus ImprovedOverpower()
        {
            // If we do not have at least one of the Imp OP talents, then fail
            if (!BotHandler.BotOwner.HasSpell((ushort)IMP_OVERPOWER_1) &&
                !BotHandler.BotOwner.HasSpell((ushort)IMP_OVERPOWER_2))
                return BehaviourTreeStatus.Failure;

            return Overpower();
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
