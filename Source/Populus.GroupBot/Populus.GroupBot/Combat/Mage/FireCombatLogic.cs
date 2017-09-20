using Populus.Core.World.Objects;

namespace Populus.GroupBot.Combat.Mage
{
    public class FireCombatLogic : MageCombatLogic
    {
        #region Constructors

        public FireCombatLogic(GroupBotHandler botHandler) : base(botHandler)
        {

        }

        #endregion

        #region Private Methods

        protected override CombatActionResult DoNextCombatAction(Unit unit)
        {
            if (HasSpellAndCanCast(FIREBALL))
            {
                BotHandler.CombatState.SpellCast(FIREBALL);
                return CombatActionResult.ACTION_OK;
            }

            AttackWand(unit);
            return base.DoNextCombatAction(unit);
        }

        #endregion
    }
}
