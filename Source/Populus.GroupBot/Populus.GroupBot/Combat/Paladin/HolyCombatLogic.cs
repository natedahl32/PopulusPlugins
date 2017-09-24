﻿namespace Populus.GroupBot.Combat.Paladin
{
    public class HolyCombatLogic : PaladinCombatLogic
    {
        #region Constructors

        public HolyCombatLogic(GroupBotHandler botHandler) : base(botHandler)
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

        public override bool IsHealer => true;

        #endregion
    }
}
