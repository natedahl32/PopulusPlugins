namespace Populus.GroupBot.Combat.Hunter
{
    public class HunterCombatLogic : CombatLogicHandler
    {
        #region Constructors

        public HunterCombatLogic(GroupBotHandler botHandler) : base(botHandler)
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
