namespace Populus.GroupBot.Combat.Warlock
{
    public class WarlockCombatLogic : CombatLogicHandler
    {
        #region Constructors

        public WarlockCombatLogic(GroupBotHandler botHandler) : base(botHandler)
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

        #endregion
    }
}
