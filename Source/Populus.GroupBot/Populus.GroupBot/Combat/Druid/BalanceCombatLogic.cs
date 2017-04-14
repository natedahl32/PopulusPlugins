namespace Populus.GroupBot.Combat.Druid
{
    public class BalanceCombatLogic : DruidCombatLogic
    {
        #region Constructors

        public BalanceCombatLogic(GroupBotHandler botHandler) : base(botHandler)
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
