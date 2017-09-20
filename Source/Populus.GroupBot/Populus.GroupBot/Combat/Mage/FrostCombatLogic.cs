using Populus.Core.World.Objects;

namespace Populus.GroupBot.Combat.Mage
{
    public class FrostCombatLogic : MageCombatLogic
    {
        #region Constructors

        public FrostCombatLogic(GroupBotHandler botHandler) : base(botHandler)
        {

        }

        #endregion

        #region Private Methods

        protected override CombatActionResult DoNextCombatAction(Unit unit)
        {
            if (HasSpellAndCanCast(FROSTBOLT))
            {
                BotHandler.CombatState.SpellCast(FROSTBOLT);
                return CombatActionResult.ACTION_OK;
            }

            AttackWand(unit);
            return base.DoNextCombatAction(unit);
        }

        #endregion
    }
}
