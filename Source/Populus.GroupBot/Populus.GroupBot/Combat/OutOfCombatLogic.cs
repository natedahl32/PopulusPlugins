using FluentBehaviourTree;
using Populus.Core.DBC;

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

        /// <summary>
        /// Creates a behavior tree node that uses free/unspent talent points to fill out the assigned talent spec
        /// for the bot.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        internal static BehaviourTreeStatus UseFreeTalentPoints(GroupBotHandler handler)
        {
            // If we do not have any unspent talent points, fail
            if (handler.BotOwner.FreeTalentPoints <= 0 || handler.CurrentTalentSpec == null)
                return BehaviourTreeStatus.Failure;

            // Find the next talent to learn. If there were not talents found, exit out
            var nextTalent = FindNextTalent(handler);
            if (nextTalent == 0)
            {
                handler.BotOwner.Logger.Log($"I have unspent talent points, but I could not find the next talent to use for my spec! My spec is currently set to {handler.CurrentTalentSpec.Name}");
                return BehaviourTreeStatus.Failure;
            }

            // If we are currently learning this talent, fail
            if (nextTalent == handler.BotOwner.LearningTalent)
                return BehaviourTreeStatus.Failure;

            // Learn the talent
            var spell = SpellTable.Instance.getSpell(nextTalent);
            if (spell != null)
                handler.BotOwner.ChatParty($"I am learning the talent {spell.SpellName}");
            handler.BotOwner.LearnTalent(nextTalent);
            return BehaviourTreeStatus.Success;
        }

        #endregion

        #region Nodes

        internal static BehaviourTreeStatus FollowGroupLeader(GroupBotHandler handler)
        {
            if (handler.Group == null) return BehaviourTreeStatus.Failure;
            if (handler.BotOwner.FollowTarget != null && handler.Group.Leader.Guid == handler.BotOwner.FollowTarget.Guid)
            {
                // If we are not on the same map, teleport to them
                if (handler.BotOwner.MapId != handler.Group.Leader.MapId)
                {
                    handler.TeleportToGroupMember(handler.Group.Leader);
                    return BehaviourTreeStatus.Success;
                }

                // If we are really far away from the leader, teleport to them
                if (handler.BotOwner.DistanceFrom(handler.Group.Leader.Position) > GroupBotHandler.MAX_FOLLOW_DISTANCE)
                {
                    handler.TeleportToGroupMember(handler.Group.Leader);
                    return BehaviourTreeStatus.Success;
                }

                return BehaviourTreeStatus.Failure;
            }


            handler.BotOwner.SetFollow(handler.Group.Leader.Guid);
            handler.FollowGroupLeader();
            return BehaviourTreeStatus.Success;
        }

        /// <summary>
        /// Finds the next talent to learn
        /// </summary>
        /// <returns></returns>
        private static uint FindNextTalent(GroupBotHandler handler)
        {
            // Find the next talent to purchase
            uint nextTalent = 0;
            foreach (var talent in handler.CurrentTalentSpec.Talents)
                if (!handler.BotOwner.HasTalentOrBetter((ushort)talent))
                {
                    nextTalent = talent;
                    break;
                }

            return nextTalent;
        }

        #endregion
    }
}
