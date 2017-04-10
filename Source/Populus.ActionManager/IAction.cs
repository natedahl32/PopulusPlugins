using Populus.Core.World.Objects;

namespace Populus.ActionManager
{
    public interface IAction
    {
        #region Properties

        /// <summary>
        /// Gets the timeout for this action in milliseconds
        /// </summary>
        int Timeout { get; }

        /// <summary>
        /// Gets whether or not the action is complete
        /// </summary>
        bool IsComplete { get; }

        /// <summary>
        /// Gets whether or not the action is timed out
        /// </summary>
        bool IsTimedOut { get; }

        #endregion

        #region Action Method

        /// <summary>
        /// This is called on the action when the action becomes the current action in the queue
        /// </summary>
        void Start();

        /// <summary>
        /// This is called on the action before it is removed from the action queue as the current action. This is only
        /// called when the action completes as the CURRENT action.
        /// </summary>
        void Completed();

        /// <summary>
        /// This is called when the action is the current action in the queue, but another action gets inserted into
        /// the queue before this action has completed. When the action becomes current again, Start will be called.
        /// </summary>
        void Paused();

        /// <summary>
        /// This is called when the action is removed from the action queue, regardless of whether it was the current action.
        /// This is called AFTER the action has completed for the current action. This should be used for action cleanup. When this
        /// is called the action is no longer a part of the queue and as such, is no longer the current action.
        /// </summary>
        void Removed();

        /// <summary>
        /// This is called on each update tick. This can be used to check state to update completed and/or timeout conditions
        /// </summary>
        /// <param name="deltaTime"></param>
        void Tick(float deltaTime);

        #endregion
    }
}
