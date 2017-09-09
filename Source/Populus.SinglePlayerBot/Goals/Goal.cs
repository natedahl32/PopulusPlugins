namespace Populus.SinglePlayerBot.Goals
{
    internal abstract class Goal
    {
        #region Properties

        /// <summary>
        /// Gets the priority of the goal. Goals with high priority are processed first
        /// </summary>
        internal abstract int Priority { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Processes the goal for the bot handler. Returns true if the goal was processed and goal execution should not continue. False if nothing was processed for this goal and execution should continue.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        internal virtual bool ProcessGoal(SpBotHandler handler)
        {
            // By default processing does stop execution
            return false;
        }

        #endregion
    }
}
