using Populus.Core.Plugins;
using Populus.Core.Shared;
using Populus.Core.World.Objects;
using Populus.Core.World.Objects.Events;
using static Populus.Core.World.Objects.Bot;

namespace Populus.CombatManager
{
    public class CombatManager : PluginBase
    {
        #region Declarations

        // handlers
        EmptyEventDelegate loginHandler = null;
        BotEventDelegate<RaidTargetUpdateArgs> raidTargetUpdateHandler = null;
        BotEventDelegate<SpellCastCompleteArgs> spellCastCompleteHandler = null;
        BotEventDelegate<SpellCastFailedArgs> spellCastFailedHandler = null;
        BotEventDelegate<SpellInterruptedArgs> spellInterruptedHandler = null;
        EmptyEventDelegate cancelCombatHandler = null;
        BotEventDelegate<CombatKillLogArgs> killLogHandler = null;

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

            // Handle raid target updates
            raidTargetUpdateHandler = (bot, args) =>
            {
                var state = mBotCombatCollection.Get(bot.Guid);
                if (state != null)
                    state.UpdateRaidTarget(args.Icon, args.ObjectGuid);
            };
            Bot.RaidTargetUpdate += raidTargetUpdateHandler;

            // Handle spell cast notifications
            spellCastCompleteHandler = (bot, args) =>
            {
                if (bot.Guid == args.CasterGuid)
                {
                    var state = mBotCombatCollection.Get(bot.Guid);
                    if (state != null)
                        state.SpellCastComplete(args.SpellId);
                }
            };
            Bot.SpellCastCompleted += spellCastCompleteHandler;

            spellCastFailedHandler = (bot, args) =>
            {
                var state = mBotCombatCollection.Get(bot.Guid);
                if (state != null)
                    state.SpellCastComplete(args.SpellId);
            };
            Bot.SpellCastFailed += spellCastFailedHandler;

            spellInterruptedHandler = (bot, args) =>
            {
                if (bot.Guid == args.CasterGuid)
                {
                    var state = mBotCombatCollection.Get(bot.Guid);
                    if (state != null)
                        state.SpellCastComplete(args.SpellId);
                }
            };
            Bot.SpellInterrupted += spellInterruptedHandler;

            // Cancel combat
            cancelCombatHandler = bot =>
            {
                var state = mBotCombatCollection.Get(bot.Guid);
                if (state != null)
                    state.CancelCombat();
            };
            Bot.CancelAttack += cancelCombatHandler;

            // Kill log
            killLogHandler = (bot, args) =>
            {
                var state = mBotCombatCollection.Get(bot.Guid);
                if (state != null)
                    state.UnitKilled(args.VictimGuid);
            };
            Bot.CombatKillLog += killLogHandler;
        }

        public override void Unload()
        {
            Bot.LoggedIn -= loginHandler;
            Bot.RaidTargetUpdate -= raidTargetUpdateHandler;
            Bot.SpellCastCompleted -= spellCastCompleteHandler;
            Bot.SpellCastFailed -= spellCastFailedHandler;
            Bot.SpellInterrupted -= spellInterruptedHandler;
            Bot.CancelAttack -= cancelCombatHandler;
            Bot.CombatKillLog -= killLogHandler;
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
