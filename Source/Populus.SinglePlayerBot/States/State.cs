using Populus.SinglePlayerBot.Goals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Populus.SinglePlayerBot.States
{
    /// <summary>
    /// Base state for Bot FSM
    /// </summary>
    public abstract class State
    {
        #region Declarations

        // Goals that belong to this state
        private readonly List<Goal> mGoals = new List<Goal>();

        #endregion

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

        /// <summary>
        /// Gets all goals for this state ordered by priority
        /// </summary>
        internal IEnumerable<Goal> Goals { get { return mGoals.OrderByDescending(g => g.Priority); } }

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
            // If we have actions to process, finish those first
            if (!handler.ActionQueue.IsEmpty) return;

            foreach (var goal in Goals)
                if (goal.ProcessGoal(handler))
                    break;
        }

        /// <summary>
        /// Add a goal to this state
        /// </summary>
        /// <param name="goal"></param>
        internal void AddGoal(Goal goal)
        {
            if (goal == null) throw new ArgumentNullException("goal");
            mGoals.Add(goal);
        }

        #endregion
    }
}
