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
using Populus.GroupBot.Talents;
using Populus.GroupBot.Actions;
using Populus.Core.World.Objects.Events;
using Populus.Core.Constants;

namespace Populus.GroupBot
{
    public class GroupBotHandler
    {
        #region Declarations

        private const float MAX_FOLLOW_DISTANCE = 40.0f;

        private readonly Bot mBotOwner;
        private readonly GroupBotData mGroupBotData;
        private readonly GroupBotChatHandler mChatHandler;
        private readonly BotCombatState mCombatState;
        private CombatLogicHandler mCombatLogic;
        private readonly ActionQueue mActionQueue;
        private Group mGroup;

        private Coordinate mLastKnownFollowPosition;
        private bool mCanFollow = true;

        #endregion

        #region Constructors

        internal GroupBotHandler(Bot bot)
        {
            if (bot == null) throw new ArgumentNullException("bot");
            mBotOwner = bot;
            mChatHandler = new GroupBotChatHandler(this);
            mActionQueue = ActionMgr.GetActionQueue(mBotOwner.Guid);
            mCombatState = CombatMgr.GetCombatState(mBotOwner.Guid);
            mGroupBotData = GroupBotData.LoadData(bot.Guid.GetOldGuid());
            // Occurs after data so we can get spec if one is specified
            mCombatLogic = CombatLogicHandler.Create(this, mBotOwner.Class, CurrentTalentSpec);
            mCombatLogic.InitializeSpells();

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

        /// <summary>
        /// Gets the combat state for this bot
        /// </summary>
        internal BotCombatState CombatState { get { return mCombatState; } }

        /// <summary>
        /// Gets the group bot data for this bot
        /// </summary>
        internal GroupBotData BotData { get { return mGroupBotData; } }

        /// <summary>
        /// Gets the bots currently assigned talent spec
        /// </summary>
        public TalentSpec CurrentTalentSpec
        {
            get { return TalentManager.Instance.TalentSpecsByClass(mBotOwner.Class).Where(s => s.Name == BotData.SpecName).SingleOrDefault(); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates during the bots OnTick event
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            // If we are in combat, do combat logic
            if (mCombatState.IsInCombat)
            {
                mCombatLogic.Update(deltaTime);
                return;
            }   
            
            // Do not update if we already have actions we need to process
            if (!mActionQueue.IsEmpty) return;

            // Follow the group leader if we aren't already and we can
            if (mCanFollow)
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
                // Set our flag to can follow
                mCanFollow = true;

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

        internal void StopFollow()
        {
            mCanFollow = false;
            mBotOwner.RemoveFollow();
        }

        /// <summary>
        /// Saves bot data to a file
        /// </summary>
        internal void SaveBotData()
        {
            mGroupBotData.Serialize(mBotOwner.Guid.GetOldGuid());
        }

        /// <summary>
        /// Handles any free talent points the bot has saved up
        /// </summary>
        internal void HandleFreeTalentPoints()
        {
            if (mBotOwner.FreeTalentPoints > 0 && CurrentTalentSpec != null)
                mActionQueue.Add(
                    new SpendFreeTalentPoints(BotOwner, 
                                              CurrentTalentSpec, 
                                              () => 
                                              {
                                                  mCombatLogic = CombatLogicHandler.Create(this, mBotOwner.Class, CurrentTalentSpec);
                                                  mCombatLogic.InitializeSpells();
                                              }));
        }

        /// <summary>
        /// Handles a loot roll start event
        /// </summary>
        /// <param name="update"></param>
        internal void HandleLootRoll(LootRollStartArgs update)
        {
            // If we can roll need
            if (update.RollOptions.HasFlag(RollVoteMask.ROLL_VOTE_MASK_NEED))
                mBotOwner.LootRoll(update.LootSourceGuid, update.ItemSlot, RollVote.ROLL_NEED);
            else
                mBotOwner.LootRoll(update.LootSourceGuid, update.ItemSlot, RollVote.ROLL_GREED);
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
