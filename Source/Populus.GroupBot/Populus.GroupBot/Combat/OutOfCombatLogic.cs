using FluentBehaviourTree;

namespace Populus.GroupBot.Combat
{
    internal static class OutOfCombatLogic
    {
        #region Sub Trees

        /// <summary>
        /// Creates a behavior tree node that regenerates health out of combat. Fails if health cannot or should not
        /// be regenerated out of combat.
        /// </summary>
        /// <returns></returns>
        internal static IBehaviourTreeNode OutOfCombatHealthRegen(GroupBotHandler handler)
        {
            // Should fail if we do not or can not regenreate health
            var builder = new BehaviourTreeBuilder();
            builder.Sequence("Out of Combat Health Regen")
                        .Do("Check Health Level", t => CheckHealthLevel(handler))
                        .Do("Does bot have food", t => CheckForFood(handler))
                        .Do("Eat food", t => EatFood(handler))
                   .End();
            return builder.Build();
        }

        /// <summary>
        /// Creates a behavior tree node that regenerates mana out of combat. Fails if mana cannot or should not
        /// be regenerated out of combat.
        /// </summary>
        /// <returns></returns>
        internal static IBehaviourTreeNode OutOfCombatManaRegen(GroupBotHandler handler)
        {
            // Should fail if we do not or can not regenreate mana
            var builder = new BehaviourTreeBuilder();
            builder.Sequence("Out of Combat Mana Regen")
                        .Do("Check Mana Level", t => CheckManaLevel(handler))
                        .Do("Does bot have water", t => CheckForWater(handler))
                        .Do("Drink water", t => DrinkWater(handler))
                   .End();
            return builder.Build();
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

        private static BehaviourTreeStatus CheckHealthLevel(GroupBotHandler handler)
        {
            if (handler.BotOwner.HealthPercentage <= handler.CombatHandler.OutOfCombatHealthRegenLevel)
                return BehaviourTreeStatus.Success;
            return BehaviourTreeStatus.Failure;
        }

        private static BehaviourTreeStatus CheckManaLevel(GroupBotHandler handler)
        {
            if (handler.BotOwner.PowerPercentage <= handler.CombatHandler.OutOfCombatManaRegenLevel)
                return BehaviourTreeStatus.Success;
            return BehaviourTreeStatus.Failure;
        }

        private static BehaviourTreeStatus CheckForFood(GroupBotHandler handler)
        {
            if (handler.CombatHandler.HasFood)
                return BehaviourTreeStatus.Success;

            // No food, so summon some but return failure
            handler.CreateFood();
            return BehaviourTreeStatus.Failure;
        }

        private static BehaviourTreeStatus CheckForWater(GroupBotHandler handler)
        {
            if (handler.CombatHandler.HasWater)
                return BehaviourTreeStatus.Success;

            // No food, so summon some but return failure
            handler.CreateWater();
            return BehaviourTreeStatus.Failure;
        }

        private static BehaviourTreeStatus EatFood(GroupBotHandler handler)
        {
            // If we are currently eating, we don't need to eat anymore
            if (handler.BotOwner.IsEating)
            {
                // If we don't have full health yet, keep eating
                if (handler.BotOwner.HealthPercentage < 100) return BehaviourTreeStatus.Running;

                // Fail if we are done eating
                return BehaviourTreeStatus.Failure;
            }

            // Use the food we have to eat
            handler.BotOwner.UseItem(handler.CombatHandler.GetFood());
            return BehaviourTreeStatus.Success;
        }

        private static BehaviourTreeStatus DrinkWater(GroupBotHandler handler)
        {
            // If we are currently drinking, we don't need to drink anymore
            if (handler.BotOwner.IsDrinking)
            {
                // If we don't have full mana yet, keep drinking
                if (handler.BotOwner.PowerPercentage < 100) return BehaviourTreeStatus.Running;

                // Fail if we are done drinking
                return BehaviourTreeStatus.Failure;
            }

            // Use the water we have to drink
            handler.BotOwner.UseItem(handler.CombatHandler.GetWater());
            return BehaviourTreeStatus.Success;
        }

        #endregion
    }
}
