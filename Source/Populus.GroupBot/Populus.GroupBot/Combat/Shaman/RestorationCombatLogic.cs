namespace Populus.GroupBot.Combat.Shaman
{
    public class RestorationCombatLogic : ShamanCombatLogic
    {
        #region Constructors

        public RestorationCombatLogic(GroupBotHandler botHandler) : base(botHandler)
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
