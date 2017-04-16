using Populus.Core.World.Objects;

namespace Populus.GroupBot.Combat.Warrior
{
    public class ProtectionCombatLogic : WarriorCombatLogic
    {
        #region Constructors

        public ProtectionCombatLogic(GroupBotHandler botHandler) : base(botHandler)
        {
        }

        #endregion

        #region Private Methods

        protected override CombatActionResult DoFirstCombatAction(Unit unit)
        {
            AttackMelee(unit);

            return base.DoFirstCombatAction(unit);
        }

        protected override CombatActionResult DoNextCombatAction(Unit unit)
        {
            // TODO: Build up prot warrior combat logic
            if (!mHeroicStrikePrepared && HasSpellAndCanCast(HEROIC_STRIKE))
            {
                mHeroicStrikePrepared = true;
                BotHandler.CombatState.SpellCast(HEROIC_STRIKE);
                return CombatActionResult.ACTION_OK;
            }

            return base.DoNextCombatAction(unit);
        }

        #endregion
    }
}
