using Populus.Core.World.Objects;
using System;
using System.Collections.Generic;

namespace Populus.ActionManager
{
    /// <summary>
    /// Contains a queue of actions the bot will perform, one at a time
    /// </summary>
    public class ActionQueue
    {
        #region Declarations

        // Owner of the action queue
        private readonly Bot mBotOwner;

        // Contains our actions that will be performed
        private readonly LinkedList<IAction> mActions = new LinkedList<IAction>();
        // Lock for our actions to make it thread-safe
        private readonly object mActionLock = new object();

        #endregion

        #region Constructors

        public ActionQueue(Bot owner)
        {
            if (owner == null) throw new ArgumentNullException("owner");
            mBotOwner = owner;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether or not the action queue is empty
        /// </summary>
        public bool IsEmpty
        {
            get { return mActions.Count == 0; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds an action to the front of the queue that will be performed immediately. 
        /// The current action will be pushed back in the queue and performed after the action being added has completed.
        /// </summary>
        /// <param name="action">Action to use immediately</param>
        public void AddFirst(IAction action)
        {
            lock(mActionLock)
            {
                if (mActions.Count > 0)
                {
                    // Pause the first action
                    mActions.First.Value.Paused();
                }

                // Insert the action and start it
                mActions.AddFirst(action);
                action.Start();
            }
        }

        /// <summary>
        /// Adds an action to the back of the queue. The action will be performed after all actions
        /// in front of it in the queue.
        /// </summary>
        /// <param name="action">Action to add</param>
        public void Add(IAction action)
        {
            lock(mActionLock)
            {
                // If there are no actions in the queue, this is the first so we need to start it
                if (mActions.Count == 0)
                    action.Start();
                mActions.AddLast(action);
            }
        }

        /// <summary>
        /// Updates actions by processing the current action and moving actions in the queue if they are completed
        /// </summary>
        /// <param name="deltaTime"></param>
        internal void UpdateActions(float deltaTime)
        {
            lock (mActionLock)
            {
                // if not actions, do nothing
                if (mActions.Count == 0) return;

                var currentAction = mActions.First.Value;

                // Process the current action in the queue
                currentAction.Tick(deltaTime);

                // If completed, remove the current action and start the next action
                if (currentAction.IsComplete)
                {
                    currentAction.Completed();
                    mActions.RemoveFirst();
                    currentAction.Removed();

                    StartCurrentAction();
                    return;
                }

                // If timed out, remove it but don't complete it and start the next action
                if (currentAction.IsTimedOut)
                {
                    mActions.RemoveFirst();
                    currentAction.Removed();

                    StartCurrentAction();
                    return;
                }
            }
            
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Starts the current action
        /// </summary>
        private void StartCurrentAction()
        {
            if (mActions.Count > 0)
                mActions.First.Value.Start();
        }

        #endregion
    }
}
