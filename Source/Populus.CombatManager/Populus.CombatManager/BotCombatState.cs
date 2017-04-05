using Populus.Core.Shared;
using System;

namespace Populus.CombatManager
{
    public class BotCombatState
    {
        #region Declarations

        private readonly WoWGuid mBotGuid;

        #endregion

        #region Constructors

        protected BotCombatState(WoWGuid botGuid)
        {
            if (botGuid == null) throw new ArgumentNullException("botGuid");
            this.mBotGuid = botGuid;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Whether or not the bot is in combat
        /// </summary>
        public bool IsInCombat { get; private set; }

        #endregion
    }
}
