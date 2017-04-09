namespace Populus.GroupBot.Combat.Mage
{
    public class MageCombatLogic : CombatLogicHandler
    {
        #region Constructors

        public MageCombatLogic(GroupBotHandler botHandler) : base(botHandler)
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
