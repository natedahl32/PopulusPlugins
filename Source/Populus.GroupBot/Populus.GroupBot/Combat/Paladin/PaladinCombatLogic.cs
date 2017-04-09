namespace Populus.GroupBot.Combat.Paladin
{
    public class PaladinCombatLogic : CombatLogicHandler
    {
        #region Constructors

        public PaladinCombatLogic(GroupBotHandler botHandler) : base(botHandler)
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether or not this class is primarily a melee class
        /// </summary>
        public override bool IsMelee
        {
            // TODO: Depending on spec, for now return true
            get { return true; }
        }

        #endregion
    }
}
