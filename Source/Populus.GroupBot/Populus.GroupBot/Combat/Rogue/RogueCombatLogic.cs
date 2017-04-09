namespace Populus.GroupBot.Combat.Rogue
{
    public class RogueCombatLogic : CombatLogicHandler
    {
        #region Constructors

        public RogueCombatLogic(GroupBotHandler botHandler) : base(botHandler)
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether or not this class is primarily a melee class
        /// </summary>
        public override bool IsMelee
        {
            get { return true; }
        }

        #endregion
    }
}
