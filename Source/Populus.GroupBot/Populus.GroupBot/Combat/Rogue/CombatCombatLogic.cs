using Populus.Core.World.Objects;

namespace Populus.GroupBot.Combat.Rogue
{
    public class CombatCombatLogic : RogueCombatLogic
    {
        #region Constructors

        public CombatCombatLogic(GroupBotHandler botHandler) : base(botHandler)
        {

        }

        #endregion

        #region Private Methods

        //protected override CombatActionResult DoNextCombatAction(Unit unit)
        //{
        //    // If we have more than 50 energy and less than 4 CP's, use sinister strike
        //    if (HasSpellAndCanCast(SINISTER_STRIKE) && BotHandler.BotOwner.ComboPoints < 4 && BotHandler.BotOwner.CurrentPower >= 50)
        //    {
        //        BotHandler.CombatState.SpellCast(SINISTER_STRIKE);
        //        return CombatActionResult.ACTION_OK;
        //    }

        //    // TODO: If we do not have SnD up or SnD has less than 2 seconds and we have combo points get SnD up
        //    // TOOD: If we have 5 combo points use Eviscerate 
        //    // TODO: If we have less than 5 combo points, use Sinister Strike

        //    AttackMelee(unit);
        //    return base.DoNextCombatAction(unit);
        //}

        #endregion
    }
}
