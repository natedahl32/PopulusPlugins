using Populus.Core.Shared;
using Populus.Core.World.Objects;
using Populus.Core.World.Objects.Events;
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
        private readonly GroupBotChatHandler mChatHandler;
        private Group mGroup;

        #endregion

        #region Constructors

        internal GroupBotHandler(Bot bot)
        {
            if (bot == null) throw new ArgumentNullException("bot");
            mBotOwner = bot;
            mChatHandler = new GroupBotChatHandler(this);

            // defaults
            IsOutOfRangeOfLeader = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether or not this bot is out of range of the leader of the group
        /// </summary>
        public bool IsOutOfRangeOfLeader { get; private set; }

        /// <summary>
        /// Gets the bot owner for this handler
        /// </summary>
        internal Bot BotOwner { get { return mBotOwner; } }

        /// <summary>
        /// Gets the group the bot is in
        /// </summary>
        internal Group Group
        {
            get
            {
                if (mGroup == null)
                    mGroup = GroupMgr.Groups.GetCharacterGroup(mBotOwner.Guid);
                return mGroup;
            }
        }

        /// <summary>
        /// Gets the chat handler
        /// </summary>
        internal GroupBotChatHandler ChatHandler { get { return mChatHandler; } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates during the bots OnTick event
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            // Follow the group leader if we aren't already
            FollowGroupLeader();
        }

        /// <summary>
        /// Follows the group leader
        /// </summary>
        public void FollowGroupLeader()
        {
            if (Group == null) return;
            FollowGroupMember(Group.Leader);
        }

        /// <summary>
        /// Called when the bot is no longer in a group
        /// </summary>
        public void GroupDisbanded()
        {
            mGroup = null;
            mBotOwner.RemoveFollow();
        }

        /// <summary>
        /// Handles following a member of the group
        /// </summary>
        public void FollowGroupMember(GroupMember member)
        {
            // No leader, no follow
            // If we ARE the leader, we follow no one!
            // Don't follow if we don't have the leaders position
            if (member == null || member.Guid == mBotOwner.Guid || member.Position == null)
            {
                mBotOwner.RemoveFollow();
                return;
            }

            // If the group leader is within X yards from us, set them as the follow target. Otherwise remove them as the follow target
            if (mBotOwner.DistanceFrom(member.Position) <= MAX_FOLLOW_DISTANCE)
            {
                mBotOwner.SetFollow(member.Guid);
                IsOutOfRangeOfLeader = false;
            }
            else
            {
                if (!IsOutOfRangeOfLeader)
                {
                    // Remove the follow target and tell the group leader they are too far away for you to follow
                    mBotOwner.RemoveFollow();
                    mBotOwner.ChatParty($"I can't follow {member.Name} you are too far away!");
                    IsOutOfRangeOfLeader = true;
                }
            }
        }

        #endregion

        #region Private Methods



        #endregion
    }
}
