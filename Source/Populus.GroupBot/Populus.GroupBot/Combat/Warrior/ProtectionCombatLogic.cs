using FluentBehaviourTree;
using Populus.Core.Shared;
using Populus.Core.World.Objects;

namespace Populus.GroupBot.Combat.Warrior
{
    public class ProtectionCombatLogic : WarriorCombatLogic
    {
        #region Declarations

        private bool mRevengeProcced = false;
        private bool mHasHitTarget = false;

        #endregion

        #region Constructors

        public ProtectionCombatLogic(GroupBotHandler botHandler) : base(botHandler)
        {
        }

        #endregion

        #region Properties

        public override bool IsTank => true;

        #endregion

        #region Public Methods

        /// <summary>
        /// Resets any combat state flags
        /// </summary>
        public override void ResetCombatState()
        {
            mHasHitTarget = false;
            mHeroicStrikePrepared = false;
            mRevengeProcced = false;
        }

        #endregion

        #region Private Methods

        protected override IBehaviourTreeNode CombatRotationTree()
        {
            var builder = new BehaviourTreeBuilder();
            builder.Selector("Protection Warrior Rotation")
                        .Do("Defensive Stance", t => SelfBuff(DEFENSIVE_STANCE))
                        .Do("Mocking Blow", t => MockingBlow())
                        .Do("Taunt", t => Taunt())
                        .Do("Bloodrage", t => Bloodrage())
                        .Do("Revenge", t => Revenge())
                        .Do("Sunder Armors", t => SunderArmors(3))
                        .Do("Shield Block", t => SelfBuff(SHIELD_BLOCK))
                        .Do("Battle Shout", t => GroupBuff(BATTLE_SHOUT))   // Might not have the rage to do until combat
                   .End();
            return builder.Build();
        }

        protected override IBehaviourTreeNode OutOfCombatBuffsTree()
        {
            var builder = new BehaviourTreeBuilder();
            builder.Selector("Protection Warrior Buffs")
                        .Do("Defensive Stance", t => SelfBuff(DEFENSIVE_STANCE))
                        .Do("Battle Shout", t => GroupBuff(BATTLE_SHOUT))
                   .End();
            return builder.Build();
        }

        protected override void CombatAttackUpdate(Bot bot, Core.World.Objects.Events.CombatAttackUpdateArgs eventArgs)
        {
            // check if we are attacking the target
            if (bot.Guid == BotHandler.BotOwner.Guid && eventArgs.AttackerGuid == BotHandler.BotOwner.Guid && eventArgs.Hit)
                mHasHitTarget = true;

            // check for revenge procs
            if (bot.Guid == BotHandler.BotOwner.Guid && eventArgs.AttackerGuid == BotHandler.BotOwner.Guid)
            {
                // Procs after block, dodge or parry
                if (BotHandler.BotOwner.HasSpell((ushort)REVENGE) && 
                    (eventArgs.Blocked || eventArgs.Dodged || eventArgs.Parried))
                    mRevengeProcced = true;
            }

            // process base
            base.CombatAttackUpdate(bot, eventArgs);
        }

        #endregion

        #region Combat Behaviors

        /// <summary>
        /// Casts mocking blow if we don't have the attention of our target
        /// </summary>
        /// <returns></returns>
        private BehaviourTreeStatus MockingBlow()
        {
            // If our target is targeting us, we are good
            if (BotHandler.CombatState.CurrentTarget.TargetGuid == BotHandler.BotOwner.Guid)
                return BehaviourTreeStatus.Failure;

            return CastSpell(MOCKING_BLOW);
        }

        /// <summary>
        /// Casts taunt if we do not have the attention of our target
        /// </summary>
        /// <returns></returns>
        private BehaviourTreeStatus Taunt()
        {
            // TODO: Check if any target is hitting the healer. If they are, taunt off regardless of what else has happened.

            // If our target is targeting us, we are good
            if (BotHandler.CombatState.CurrentTarget.TargetGuid == BotHandler.BotOwner.Guid)
                return BehaviourTreeStatus.Failure;
            // If we haven't even hit the target yet, don't taunt
            if (!mHasHitTarget)
                return BehaviourTreeStatus.Failure;

            return CastSpell(TAUNT);
        }

        /// <summary>
        /// Casts bloodrage if we can use it
        /// </summary>
        /// <returns></returns>
        private BehaviourTreeStatus Bloodrage()
        {
            // Only use bloodrage if we are below 20 rage
            if (BotHandler.BotOwner.CurrentPower >= 20)
                return BehaviourTreeStatus.Failure;

            return SelfBuff(BLOODRAGE);
        }

        /// <summary>
        /// Casts revenge if we can use it
        /// </summary>
        /// <returns></returns>
        private BehaviourTreeStatus Revenge()
        {
            // Not available, fail
            if (!mRevengeProcced)
                return BehaviourTreeStatus.Failure;
            // If not in melee range, fail
            if (!IsInMeleeRange(BotHandler.CombatState.CurrentTarget))
                return BehaviourTreeStatus.Failure;
            // We cannot cast it, fail
            if (!HasSpellAndCanCast(REVENGE))
                return BehaviourTreeStatus.Failure;

            BotHandler.CombatState.SpellCast(REVENGE);
            mRevengeProcced = false;
            return BehaviourTreeStatus.Success;
        }

        /// <summary>
        /// Gets sunder armor stacks up on the current target
        /// </summary>
        /// <returns></returns>
        private BehaviourTreeStatus SunderArmors(int stacks)
        {
            // Check if the target already has 3 stacks
            var sunderAura = BotHandler.CombatState.CurrentTarget.GetAuraForSpell(SUNDER_ARMOR);
            if (sunderAura != null && sunderAura.Stacks >= stacks)
                return BehaviourTreeStatus.Failure;

            return CastMeleeSpell(SUNDER_ARMOR);
        }

        #endregion
    }
}
