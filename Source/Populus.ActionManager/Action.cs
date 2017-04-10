using Populus.Core.World.Objects;
using System;

namespace Populus.ActionManager
{
    /// <summary>
    /// Base action for a bot. This base class provides some defaults and base funcationality.
    /// </summary>
    public abstract class Action : IAction
    {
        #region Declarations

        // Holds the current timeout value in delta time
        private int mCurrentDeltaTimeout = 0;
        private int mStartDeltaTimeout = 0;

        #endregion

        #region Constructors

        protected Action(Bot bot)
        {
            if (bot == null) throw new ArgumentNullException("bot");

            IsTimedOut = false;
            BotOwner = bot;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the bot that owns this action
        /// </summary>
        protected Bot BotOwner { get; }

        /// <summary>
        /// Default timeout is 5 seconds. Timeout of -1 means there is not timeout value
        /// </summary>
        public virtual int Timeout => 5000;

        public abstract bool IsComplete { get; }

        public bool IsTimedOut { get; protected set; }

        #endregion

        #region Public Methods

        public virtual void Completed() { }

        public virtual void Paused() { }

        public virtual void Removed() { }

        public virtual void Start() { }

        /// <summary>
        /// This method handles the timeout logic. If you intend to use the default timeout logic you must call this base method to update it.
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="deltaTime"></param>
        public virtual void Tick(float deltaTime)
        {
            if (Timeout > -1)
            {
                if (mStartDeltaTimeout == 0)
                {
                    mStartDeltaTimeout = (int)deltaTime;
                    return;
                }

                mCurrentDeltaTimeout = (int)deltaTime;
                if ((mCurrentDeltaTimeout - mStartDeltaTimeout) >= Timeout)
                    IsTimedOut = true;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method can be used to reset the timeout in the event an action is taking longer than expected and you don't want it to timeout.
        /// </summary>
        protected void ResetTimeout()
        {
            mCurrentDeltaTimeout = 0;
        }

        #endregion
    }
}
