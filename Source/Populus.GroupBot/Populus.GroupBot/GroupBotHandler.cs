using Populus.Core.Shared;
using Populus.Core.World.Objects;
using Populus.GroupManager;
using System;
using GroupMgr = Populus.GroupManager.GroupManager;

namespace Populus.GroupBot
{
    public class GroupBotHandler
    {
        #region Declarations

        private const float MAX_FOLLOW_DISTANCE = 40.0f;

        private readonly Bot mBotOwner;

        #endregion

        #region Constructors

        internal GroupBotHandler(Bot bot)
        {
            if (bot == null) throw new ArgumentNullException("bot");
            mBotOwner = bot;

            // defaults
            IsOutOfRangeOfLeader = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether or not this bot is out of range of the leader of the group
        /// </summary>
        public bool IsOutOfRangeOfLeader { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates during the bots OnTick event
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            // Get the group from the manager
            var group = GroupMgr.Groups.GetCharacterGroup(mBotOwner.Guid);
            if (group == null)
            {
                Log.WriteLine(LogType.Debug, $"Debug no group was found for {mBotOwner.Name}");
                return;
            }

            HandleFollowLeader(group.Leader);
        }

        /// <summary>
        /// Called when the bot is no longer in a group
        /// </summary>
        public void GroupDisbanded()
        {
            mBotOwner.RemoveFollow();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles following the leader of the group
        /// </summary>
        private void HandleFollowLeader(GroupMember leader)
        {
            // No leader, no follow
            // If we ARE the leader, we follow no one!
            if (leader == null || leader.Guid == mBotOwner.Guid)
            {
                mBotOwner.RemoveFollow();
                return;
            }

            // If the group leader is within X yards from us, set them as the follow target. Otherwise remove them as the follow target
            if (leader.Position != null && mBotOwner.DistanceFrom(leader.Position) <= MAX_FOLLOW_DISTANCE)
            {
                mBotOwner.SetFollow(leader.Guid);
                IsOutOfRangeOfLeader = false;
            }
            else
            {
                if (!IsOutOfRangeOfLeader)
                {
                    // Remove the follow target and tell the group leader they are too far away for you to follow
                    mBotOwner.RemoveFollow();
                    mBotOwner.ChatParty($"I can't follow you {leader.Name} you are too far away!");
                    IsOutOfRangeOfLeader = true;
                }
            }
        }

        #endregion
    }
}
