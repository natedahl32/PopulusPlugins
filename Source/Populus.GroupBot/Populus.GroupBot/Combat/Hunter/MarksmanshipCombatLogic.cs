using Populus.Core.World.Objects;

namespace Populus.GroupBot.Combat.Hunter
{
    public class MarksmanshipCombatLogic : HunterCombatLogic
    {
        #region Constructors

        public MarksmanshipCombatLogic(GroupBotHandler botHandler) : base(botHandler)
        {

        }

        #endregion

        #region Private Methods

        protected override CombatActionResult DoNextCombatAction(Unit unit)
        {
            // TODO: Attack range, do hunters shoot?
            AttackMelee(unit);

            if (HasSpellAndCanCast(AIMED_SHOT))
            {
                BotHandler.CombatState.SpellCast(AIMED_SHOT);
                return CombatActionResult.ACTION_OK;
            }

            if (HasSpellAndCanCast(MULTI_SHOT))
            {
                BotHandler.CombatState.SpellCast(MULTI_SHOT);
                return CombatActionResult.ACTION_OK;
            }

            return base.DoNextCombatAction(unit);
        }

        #endregion
    }
}
