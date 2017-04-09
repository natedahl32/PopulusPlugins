namespace Populus.GroupBot.Combat.Druid
{
    public class DruidCombatLogic : CombatLogicHandler
    {
        #region Constructors

        public DruidCombatLogic(GroupBotHandler botHandler) : base(botHandler)
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
