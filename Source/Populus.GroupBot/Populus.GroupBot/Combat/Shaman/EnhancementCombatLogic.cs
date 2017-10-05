namespace Populus.GroupBot.Combat.Shaman
{
    public class EnhancementCombatLogic : ShamanCombatLogic
    {
        #region Constructors

        public EnhancementCombatLogic(GroupBotHandler botHandler) : base(botHandler)
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether or not this class is a caster
        /// </summary>
        public override bool IsCaster
        {
            get { return false; }
        }

        #endregion
    }
}
