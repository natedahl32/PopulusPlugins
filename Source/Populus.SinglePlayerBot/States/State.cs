using Populus.Core.World.Objects;
using System;

namespace Populus.SinglePlayerBot.States
{
    /// <summary>
    /// Base state for Bot FSM
    /// </summary>
    public abstract class State
    {
        #region Constructors

        protected State(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            
            Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the state
        /// </summary>
        public string Name { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// On entry action for state
        /// </summary>
        /// <param name="bot"></param>
        internal virtual void OnEntry(SpBotHandler handler)
        {
            handler.BotOwner.Logger.Log($"Entering state {Name}");
        }

        /// <summary>
        /// On exit action for state
        /// </summary>
        /// <param name="bot"></param>
        internal virtual void OnExit(SpBotHandler handler)
        {
            handler.BotOwner.Logger.Log($"Exiting state {Name}");
        }

        /// <summary>
        /// On tick action for state
        /// </summary>
        /// <param name="bot"></param>
        internal virtual void OnTick(SpBotHandler handler)
        {
        }

        #endregion
    }
}
