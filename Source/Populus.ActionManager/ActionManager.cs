using Populus.Core.Plugins;
using static Populus.Core.World.Objects.Bot;
using Populus.Core.World.Objects;
using Populus.Core.Shared;

namespace Populus.ActionManager
{
    /// <summary>
    /// This class manages complex actions for bots using an action queue. Queued ations run one at a time and the next action
    /// in the queue is started when the previous action completes. 
    /// </summary>
    public class ActionManager : PluginBase
    {
        #region Declarations

        // handlers
        EmptyEventDelegate loginHandler = null;

        // static instance of our bot handlers collection
        private static WoWGuidCollection<ActionQueue> mBotActionQueueCollection = new WoWGuidCollection<ActionQueue>();

        #endregion

        #region Constructors

        public ActionManager()
        {
            // State/information plugins should use priority of 0, we want this plugin to run after state and information is updated
            // Use 1 for priority in this plugin
            Priority = 1;
        }

        #endregion

        #region Properties

        public override string Name => "Action Manager";

        public override string Author => "Kazadoom";

        public override string Website => "";

        #endregion

        #region Public Methods

        public override void Initialize()
        {
            // Add combat state for bot upon login
            loginHandler = bot =>
            {
                mBotActionQueueCollection.AddOrUpdate(bot.Guid, new ActionQueue(bot));
            };
            Bot.LoggedIn += loginHandler;
        }

        public override void Unload()
        {
            Bot.LoggedIn -= loginHandler;
        }

        public override void OnTick(Bot bot, float deltaTime)
        {
            // Update the action queue
            var queue = mBotActionQueueCollection.Get(bot.Guid);
            if (queue != null)
                queue.UpdateActions(deltaTime);

            base.OnTick(bot, deltaTime);
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Gets the combat state for a bot
        /// </summary>
        /// <param name="botGuid"></param>
        /// <returns></returns>
        public static ActionQueue GetActionQueue(WoWGuid botGuid)
        {
            return mBotActionQueueCollection.Get(botGuid);
        }

        #endregion
    }
}
