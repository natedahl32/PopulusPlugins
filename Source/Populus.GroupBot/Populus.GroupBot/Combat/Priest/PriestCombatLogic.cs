namespace Populus.GroupBot.Combat.Priest
{
    public class PriestCombatLogic : CombatLogicHandler
    {
        #region Constructors

        public PriestCombatLogic(GroupBotHandler botHandler) : base(botHandler)
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
