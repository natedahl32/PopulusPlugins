using FluentBehaviourTree;
using Populus.Core.Shared;
using Populus.Core.World.Objects;

namespace Populus.GroupBot.Combat.Warrior
{
    public class ProtectionCombatLogic : WarriorCombatLogic
    {
        #region Declarations

        private bool mRevengeProcced = false;

        #endregion

        #region Constructors

        public ProtectionCombatLogic(GroupBotHandler botHandler) : base(botHandler)
        {
        }

        #endregion

        #region Private Methods

        protected override IBehaviourTreeNode CombatRotationTree()
        {
            var builder = new BehaviourTreeBuilder();
            builder.Selector("Protection Warrior Rotation")
                        .Do("Defensive Stance", t => SelfBuff(DEFENSIVE_STANCE))
                        .Do("Bloodrage", t => Bloodrage())        // We might not have the rage to do this until combat
                        .Do("Revenge", t => Revenge())
                        .Do("Sunder Armors", t => SunderArmors(3))
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
