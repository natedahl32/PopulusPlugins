using Populus.Core.Plugins;
using Populus.Core.Shared;
using Populus.Core.World.Objects;
using static Populus.Core.World.Objects.Bot;

namespace Populus.CombatManager
{
    public class CombatManager : PluginBase
    {
        #region Declarations

        // handlers
        EmptyEventDelegate loginHandler = null;

        // static instance of our bot handlers collection
        private static WoWGuidCollection<BotCombatState> mBotCombatCollection = new WoWGuidCollection<BotCombatState>();

        #endregion

        #region Constructors

        public CombatManager()
        {
            // Set priority to highest state because this plugin gathers information
            Priority = 0;
        }

        #endregion

        #region Properties

        public override string Name => "Combat Manager";

        public override string Author => "Kazadoom";

        public override string Website => "";

        #endregion

        #region Public Methods

        public override void Initialize()
        {
            // Add combat state for bot upon login
            loginHandler = bot =>
            {
                mBotCombatCollection.AddOrUpdate(bot.Guid, new BotCombatState(bot));
            };
            Bot.LoggedIn += loginHandler;
        }

        public override void Unload()
        {
            Bot.LoggedIn -= loginHandler;
        }

        public override void OnTick(Bot bot, float deltaTime)
        {
            // Update the combat state
            var state = mBotCombatCollection.Get(bot.Guid);
            if (state != null)
                state.UpdateState(deltaTime);

            base.OnTick(bot, deltaTime);
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Gets the combat state for a bot
        /// </summary>
        /// <param name="botGuid"></param>
        /// <returns></returns>
        public static BotCombatState GetCombatState(WoWGuid botGuid)
        {
            return mBotCombatCollection.Get(botGuid);
        }

        #endregion
    }
}
