namespace Populus.GroupBot.Combat.Warrior
{
    public class WarriorCombatLogic : CombatLogicHandler
    {
        #region Constructors

        public WarriorCombatLogic(GroupBotHandler botHandler) : base(botHandler)
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
