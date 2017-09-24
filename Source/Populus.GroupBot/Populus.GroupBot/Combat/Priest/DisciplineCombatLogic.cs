namespace Populus.GroupBot.Combat.Priest
{
    public class DisciplineCombatLogic : PriestCombatLogic
    {
        #region Constructors

        public DisciplineCombatLogic(GroupBotHandler botHandler) : base(botHandler)
        {

        }

        #endregion

        #region Properties

        public override bool IsHealer => true;

        #endregion
    }
}
