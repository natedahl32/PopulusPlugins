namespace Populus.GroupBot.Combat.Priest
{
    public class HolyCombatLogic : PriestCombatLogic
    {
        #region Constructors

        public HolyCombatLogic(GroupBotHandler botHandler) : base(botHandler)
        {

        }

        #endregion

        #region Properties

        public override bool IsHealer => true;

        #endregion
    }
}
