using Populus.Core.World.Objects;
using Populus.GroupBot.Combat;
using Populus.GroupManager;
using System;
using GroupMgr = Populus.GroupManager.GroupManager;
using CombatMgr = Populus.CombatManager.CombatManager;
using Populus.CombatManager;
using Populus.Core.Shared;
using System.Linq;
using Populus.GroupBot.Talents;
using Populus.Core.World.Objects.Events;
using Populus.Core.Constants;
using Populus.GroupBot.States;
using Stateless;
using Populus.GroupBot.Triggers;

namespace Populus.GroupBot
{
    public class GroupBotHandler
    {
        #region Declarations

        internal const float MAX_FOLLOW_DISTANCE = 40.0f;

        private readonly Bot mBotOwner;
        private readonly GroupBotData mGroupBotData;
        private readonly GroupBotChatHandler mChatHandler;
        private readonly BotCombatState mCombatState;
        private CombatLogicHandler mCombatLogic;
        private Group mGroup;

        private Coordinate mLastKnownFollowPosition;
        private bool mCanFollow = true;

        private State mCurrentState = Idle.Instance;
        private readonly StateMachine<State, StateTriggers> mStateMachine;

        #endregion

        #region Constructors

        internal GroupBotHandler(Bot bot)
        {
            if (bot == null) throw new ArgumentNullException("bot");
            mBotOwner = bot;
            mChatHandler = new GroupBotChatHandler(this);
            mCombatState = CombatMgr.GetCombatState(mBotOwner.Guid);
            mGroupBotData = GroupBotData.LoadData(bot.Guid.GetOldGuid());
            // Occurs after data so we can get spec if one is specified
            mCombatLogic = CombatLogicHandler.Create(this, mBotOwner.Class, CurrentTalentSpec);
            mCombatLogic.InitializeSpells();

            // defaults
            IsOutOfRangeOfLeader = false;

            mStateMachine = new StateMachine<States.State, StateTriggers>(() => mCurrentState, s => mCurrentState = s);
            BuildStateMachine();
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

        /// <summary>
        /// Coordinates the bot is teleporting to
        /// </summary>
        public Coordinate TeleportingTo { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates during the bots OnTick event
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            // Update the current state
            mCurrentState.Update(this, deltaTime);
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
        }

        /// <summary>
        /// Teleports to a group members position
        /// </summary>
        /// <param name="member"></param>
        internal void TeleportToGroupMember(GroupMember member)
        {
            mBotOwner.ChatSay($".go {member.Name}");
            mBotOwner.Logger.Log($"Teleporting to group member {member.Name}");
            IsOutOfRangeOfLeader = true;
            TeleportingTo = member.Position;
            TriggerState(StateTriggers.Teleporting);
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
            mGroupBotData.Serialize();
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

        /// <summary>
        /// Triggers a state
        /// </summary>
        /// <param name="trigger"></param>
        internal void TriggerState(StateTriggers trigger)
        {
            BotOwner.Logger.Log($"Firing state trigger {trigger}");
            mStateMachine.Fire(trigger);
        }

        /// <summary>
        /// Learn all spells up to the bots level (does not learn talents)
        /// </summary>
        internal void LearnLevelSpells(uint newLevel)
        {
            // Make sure all bots have certain skills
            mCombatLogic.CheckForSkills();

            // Level 1 spells are already learned
            if (newLevel <= 1) return;

            // Get all spells we should know at this level
            var spells = mCombatLogic.GetSpellsUpToLevel(newLevel);
            foreach (var spell in spells)
                if (!BotOwner.HasSpell((ushort)spell))
                    LearnSpell(spell);
        }

        /// <summary>
        /// Creates some food for the bot to regenerate health
        /// </summary>
        internal void CreateFood()
        {
            // Clear the target so we get the food
            BotOwner.ClearTarget();
            var food = CombatHandler.GetFood();
            if (food > 0)
                BotOwner.ChatSay($".additem {food} 10");
        }

        /// <summary>
        /// Creates some water for the bot to regenerate mana
        /// </summary
        internal void CreateWater()
        {
            // Clear the target so we get the water
            BotOwner.ClearTarget();
            var water = CombatHandler.GetWater();
            if (water > 0)
                BotOwner.ChatSay($".additem {water} 10");
        }

        /// <summary>
        /// Tries to automatically assign a role to the group member based on talent spec
        /// </summary>
        internal void AssignGroupRole()
        {
            if (Group == null)
                return;

            var member = Group.GetMember(BotOwner.Guid);
            if (member == null)
                return;

            if (CombatHandler.IsTank)
                member.AssignRole(GroupMember.ROLE_TANK);
            else if (CombatHandler.IsHealer)
                member.AssignRole(GroupMember.ROLE_HEALER);
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

        /// <summary>
        /// Build the state machine for the bot
        /// </summary>
        private void BuildStateMachine()
        {
            mStateMachine.Configure(Idle.Instance)
                .Permit(StateTriggers.Teleporting, Teleport.Instance)
                .Permit(StateTriggers.Combat, States.Combat.Instance)
                .Permit(StateTriggers.Died, Dead.Instance);

            mStateMachine.Configure(Teleport.Instance)
                .Permit(StateTriggers.Idle, Idle.Instance);

            mStateMachine.Configure(States.Combat.Instance)
                .OnEntry(() =>
                {
                    // Remove follow target for range when combat starts
                    if (!CombatHandler.IsMelee)
                        BotOwner.RemoveFollow();
                })
                .Permit(StateTriggers.Idle, Idle.Instance)
                .Permit(StateTriggers.Died, Dead.Instance);
                

            mStateMachine.Configure(Dead.Instance)
                .OnEntry(() =>
                {
                    // Remove follow target when dead
                    BotOwner.RemoveFollow();
                })
                .Permit(StateTriggers.Resurrected, Idle.Instance);
        }

        /// <summary>
        /// Issues command to learn a spell with the id
        /// </summary>
        /// <param name="spellId"></param>
        private void LearnSpell(uint spellId)
        {
            mBotOwner.ChatSay($".learn {spellId}");
        }

        #endregion
    }
}
