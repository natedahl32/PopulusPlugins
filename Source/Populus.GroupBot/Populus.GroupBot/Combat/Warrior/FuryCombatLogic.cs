using Populus.Core.World.Objects;

namespace Populus.GroupBot.Combat.Warrior
{
    public class FuryCombatLogic : WarriorCombatLogic
    {
        #region Declarations

        private bool mOverpowerProcced = false;

        #endregion

        #region Constructors

        public FuryCombatLogic(GroupBotHandler botHandler) : base(botHandler)
        {
            UseHeroicStrikeRageThreshold = 50;
        }

        #endregion

        #region Private Methods

        protected override CombatActionResult DoFirstCombatAction(Unit unit)
        {
            // Get in defensive stance if we are not already
            if (!BotHandler.BotOwner.HasAura(DEFENSIVE_STANCE) && HasSpellAndCanCast(DEFENSIVE_STANCE))
            {
                //Log.WriteLine(LogType.Debug, "Using DEFENSIVE_STANCE");
                BotHandler.CombatState.SpellCast(BotHandler.BotOwner, DEFENSIVE_STANCE);
                // Continue with first combat action if we hit this
                return CombatActionResult.ACTION_OK_CONTINUE_FIRST;
            }

            AttackMelee(unit);

            return base.DoFirstCombatAction(unit);
        }

        protected override CombatActionResult DoNextCombatAction(Unit unit)
        {
            // TODO: Build up fury warrior combat logic
            AttackMelee(unit);

            // Use execute if we can
            if (unit.HealthPercentage < 20f && HasSpellAndCanCast(EXECUTE))
            {
                BotHandler.CombatState.SpellCast(EXECUTE);
                return CombatActionResult.ACTION_OK;
            }

            // Use bloodthrist if off cooldown
            if (HasSpellAndCanCast(BLOODTHIRST))
            {
                BotHandler.CombatState.SpellCast(BLOODTHIRST);
                return CombatActionResult.ACTION_OK;
            }

            // Use whirlwind if off cooldown
            if (HasSpellAndCanCast(WHIRLWIND))
            {
                BotHandler.CombatState.SpellCast(WHIRLWIND);
                return CombatActionResult.ACTION_OK;
            }

            // Use overpower if it procced
            if (mOverpowerProcced && HasSpellAndCanCast(OVERPOWER) && (BotHandler.BotOwner.CurrentPower <= 45 || (BotHandler.BotOwner.SpellIsOnCooldown(BLOODTHIRST) && BotHandler.BotOwner.SpellIsOnCooldown(WHIRLWIND))))
            {
                BotHandler.CombatState.SpellCast(OVERPOWER);
                mOverpowerProcced = false;
                return CombatActionResult.ACTION_OK;
            }

            return base.DoNextCombatAction(unit);
        }

        protected override void CombatAttackUpdate(Bot bot, Core.World.Objects.Events.CombatAttackUpdateArgs eventArgs)
        {
            // check for revenge procs
            if (bot.Guid == BotHandler.BotOwner.Guid && eventArgs.AttackerGuid == BotHandler.BotOwner.Guid)
            {
                // Procs after block, dodge or parry
                if (BotHandler.BotOwner.HasSpell((ushort)REVENGE) && eventArgs.Dodged)
                    mOverpowerProcced = true;
            }

            // process base
            base.CombatAttackUpdate(bot, eventArgs);
        }

        #endregion
    }
}
