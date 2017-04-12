using Populus.Core.World.Objects;
using Populus.GroupBot.Combat;
using Populus.GroupManager;
using System;
using GroupMgr = Populus.GroupManager.GroupManager;
using CombatMgr = Populus.CombatManager.CombatManager;
using ActionMgr = Populus.ActionManager.ActionManager;
using Populus.ActionManager;
using Populus.CombatManager;
using Populus.Core.Shared;
using System.Linq;

namespace Populus.GroupBot
{
    public class GroupBotHandler
    {
        #region Declarations

        private const float MAX_FOLLOW_DISTANCE = 40.0f;

        private readonly Bot mBotOwner;
        private readonly GroupBotChatHandler mChatHandler;
        private readonly BotCombatState mCombatState;
        private readonly CombatLogicHandler mCombatLogic;
        private readonly ActionQueue mActionQueue;
        private Group mGroup;

        private Coordinate mLastKnownFollowPosition;

        #endregion

        #region Constructors

        internal GroupBotHandler(Bot bot)
        {
            if (bot == null) throw new ArgumentNullException("bot");
            mBotOwner = bot;
            mChatHandler = new GroupBotChatHandler(this);
            mCombatLogic = CombatLogicHandler.Create(this, mBotOwner.Class);
            mActionQueue = ActionMgr.GetActionQueue(mBotOwner.Guid);
            mCombatState = CombatMgr.GetCombatState(mBotOwner.Guid);

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

        /// <summary>
        /// Gets the combat handler
        /// </summary>
        internal CombatLogicHandler CombatHandler { get { return mCombatLogic; } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates during the bots OnTick event
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            // TODO: If we are in combat, do combat logic
            if (mCombatState.IsInCombat) return;
            // Do not update if we already have actions we need to process
            if (!mActionQueue.IsEmpty) return;

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
            if (mBotOwner.DistanceFrom(member.Position) <= MAX_FOLLOW_DISTANCE && mBotOwner.MapId == member.MapId)
            {
                mBotOwner.SetFollow(member.Guid);
                IsOutOfRangeOfLeader = false;
                mLastKnownFollowPosition = mBotOwner.Position;
            }
            else
            {
                if (!IsOutOfRangeOfLeader)
                {
                    // If we have a last known position, move to that, it might take us through a portal
                    if (mLastKnownFollowPosition != null)
                    {
                        mBotOwner.MoveToPosition(mLastKnownFollowPosition);
                        IsOutOfRangeOfLeader = true;
                    }
                    else
                    {
                        // Remove the follow target and tell the group leader they are too far away for you to follow
                        mBotOwner.RemoveFollow();
                        mBotOwner.ChatParty($"I can't follow {member.Name} you are too far away!");
                        IsOutOfRangeOfLeader = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gives orders to attack a specified unit
        /// </summary>
        /// <param name="target">Unit to attack</param>
        public void Attack(Unit target)
        {
            mCombatLogic.StartAttack(target);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Drops the current group if the bot is the only person currently online
        /// </summary>
        private void DropGroupIfOnlyPersonOnline()
        {
            if (Group == null) return;
            if (!Group.Members.Any(m => m.IsOnline))
                mBotOwner.LeaveGroup();
        }

        #endregion
    }
}
