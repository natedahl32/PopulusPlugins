namespace Populus.GroupBot.Combat.Paladin
{
    public class ProtectionCombatLogic : PaladinCombatLogic
    {
        #region Constructors

        public ProtectionCombatLogic(GroupBotHandler botHandler) : base(botHandler)
        {

        }

        #endregion

        #region Properties

        public override bool IsTank => true;

        #endregion
    }
}
