namespace Populus.GroupBot.Combat.Shaman
{
    public class ElementalCombatLogic : ShamanCombatLogic
    {
        #region Constructors

        public ElementalCombatLogic(GroupBotHandler botHandler) : base(botHandler)
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
