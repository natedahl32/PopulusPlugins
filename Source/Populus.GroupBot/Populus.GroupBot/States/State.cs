namespace Populus.GroupBot.States
{
    /// <summary>
    /// Abstract state that all other states are derived from
    /// </summary>
    public abstract class State
    {
        /// <summary>
        /// Update method that is called each tick of the state machine
        /// </summary>
        /// <param name="handler">GroupBotHandler being updated</param>
        /// <param name="deltaTime">Delta time since last update</param>
        public virtual void Update(GroupBotHandler handler, float deltaTime) { }
    }
}
