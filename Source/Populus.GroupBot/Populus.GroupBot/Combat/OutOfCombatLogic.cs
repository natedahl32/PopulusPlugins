using FluentBehaviourTree;

namespace Populus.GroupBot.Combat
{
    internal static class OutOfCombatLogic
    {
        #region Sub Trees

        /// <summary>
        /// This action handles out of combat health regen for the bot. If it succeeds, the bot is eating to regen health.
        /// If it fails, the bot either cannot or should not regenerate health.
        /// </summary>
        /// <returns></returns>
        internal static BehaviourTreeStatus OutOfCombatHealthRegen(GroupBotHandler handler)
        {
            // If we are already eating and not at full health yet, keep eating
            if (handler.BotOwner.IsEating && handler.BotOwner.HealthPercentage < 100f)
                return BehaviourTreeStatus.Running;

            // If the health level of the bot is not below the threshold, fail
            if (handler.BotOwner.HealthPercentage > handler.CombatHandler.OutOfCombatHealthRegenLevel)
                return BehaviourTreeStatus.Failure;

            // If the bot does not have food, create some food and fail
            if (!handler.CombatHandler.HasFood)
            {
                handler.CreateFood();
                return BehaviourTreeStatus.Failure;
            }

            // Time to eat! Make sure to remove follow so we don't get up prematurely
            handler.BotOwner.UseItem(handler.CombatHandler.GetFood());
            handler.BotOwner.RemoveFollow();
            return BehaviourTreeStatus.Success;
        }

        /// <summary>
        /// Creates a behavior tree node that regenerates mana out of combat. 
        /// Fails if mana cannot or should not be regenerated out of combat.
        /// Succeeds or Running if mana is currently be restored.
        /// </summary>
        /// <returns></returns>
        internal static BehaviourTreeStatus OutOfCombatManaRegen(GroupBotHandler handler)
        {
            // If we are already drinking and not at full mana yet, keep drinking
            if (handler.BotOwner.IsDrinking && handler.BotOwner.PowerPercentage < 100f)
                return BehaviourTreeStatus.Running;

            // If the mana level of the bot is not below the threshold, fail
            if (handler.BotOwner.PowerPercentage > handler.CombatHandler.OutOfCombatManaRegenLevel)
                return BehaviourTreeStatus.Failure;

            // If the bot does not have water, create some water and fail
            if (!handler.CombatHandler.HasWater)
            {
                handler.CreateWater();
                return BehaviourTreeStatus.Failure;
            }

            // Time to drink! Make sure to remove follow so we don't get up prematurely
            handler.BotOwner.UseItem(handler.CombatHandler.GetWater());
            handler.BotOwner.RemoveFollow();
            return BehaviourTreeStatus.Success;
        }

        #endregion

        #region Nodes

        internal static BehaviourTreeStatus FollowGroupLeader(GroupBotHandler handler)
        {
            if (handler.Group == null) return BehaviourTreeStatus.Failure;
            if (handler.BotOwner.FollowTarget != null && handler.Group.Leader.Guid == handler.BotOwner.FollowTarget.Guid) return BehaviourTreeStatus.Failure;

            handler.FollowGroupLeader();
            return BehaviourTreeStatus.Success;
        }
        
        #endregion
    }
}
