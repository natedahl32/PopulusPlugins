﻿namespace Populus.GroupBot.Combat.Druid
{
    public class RestorationCombatLogic : DruidCombatLogic
    {
        #region Constructors

        public RestorationCombatLogic(GroupBotHandler botHandler) : base(botHandler)
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether or not this class is primarily a melee class
        /// </summary>
        public override bool IsMelee
        {
            get { return false; }
        }

        /// <summary>
        /// Gets whether or not this class is a caster
        /// </summary>
        public override bool IsCaster
        {
            get { return true; }
        }

        public override bool IsHealer => true;

        #endregion
    }
}
