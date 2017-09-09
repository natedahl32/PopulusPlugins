using ActionMgr = Populus.ActionManager.ActionManager;
using CombatMgr = Populus.CombatManager.CombatManager;
using Populus.Core.World.Objects;
using System;
using Populus.ActionManager;
using Populus.CombatManager;
using Stateless;
using Populus.SinglePlayerBot.States;

namespace Populus.SinglePlayerBot
{
    /// <summary>
    /// Handler class for Single Player Bots. This class is responsible for overall control of the bot while the Single Player plugin is running. At a high level
    /// this handler contains the State of the bot. State determines what the bots high level goals are currently. For example, all bots start out with a goal of leveling
    /// in order to open up a wider range of possibilities. A bot may choose to focus on tradeskills, so the goal (state) of the bot may then change to gathering or tradeskilling.
    /// </summary>
    internal class SpBotHandler
    {
        #region Declarations

        internal enum StateTriggers { Combat, LevelUp }

        private readonly Bot mBotOwner;
        private readonly ActionQueue mActionQueue;
        private readonly BotCombatState mCombatState;
        private readonly StateMachine<State, StateTriggers> mStateMachine;

        #endregion

        #region Constructors

        internal SpBotHandler(Bot bot)
        {
            if (bot == null) throw new ArgumentNullException("bot");
            mBotOwner = bot;
            mActionQueue = ActionMgr.GetActionQueue(mBotOwner.Guid);
            mCombatState = CombatMgr.GetCombatState(mBotOwner.Guid);
            mStateMachine = new StateMachine<State, StateTriggers>(new LevelingState());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the bot owner for this handler
        /// </summary>
        internal Bot BotOwner { get { return mBotOwner; } }

        /// <summary>
        /// Gets the action queue for this bot
        /// </summary>
        internal ActionQueue ActionQueue { get { return mActionQueue; } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates during the bots OnTick event
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            // Let current state handle the actions
            mStateMachine.State.OnTick(this);
        }

        /// <summary>
        /// Saves bot data to a file
        /// </summary>
        internal void SaveBotData()
        {
            //mGroupBotData.Serialize(mBotOwner.Guid.GetOldGuid());
        }

        #endregion
    }
}
