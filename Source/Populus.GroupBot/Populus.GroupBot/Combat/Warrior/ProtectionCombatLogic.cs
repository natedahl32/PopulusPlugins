using Populus.Core.Shared;
using Populus.Core.World.Objects;

namespace Populus.GroupBot.Combat.Warrior
{
    public class ProtectionCombatLogic : WarriorCombatLogic
    {
        #region Declarations

        private bool mRevengeProcced = false;

        #endregion

        #region Constructors

        public ProtectionCombatLogic(GroupBotHandler botHandler) : base(botHandler)
        {
        }

        #endregion

        #region Private Methods

        //protected override CombatActionResult DoFirstCombatAction(Unit unit)
        //{
        //    // Get in defensive stance if we are not already
        //    if (!BotHandler.BotOwner.HasAura(DEFENSIVE_STANCE) && HasSpellAndCanCast(DEFENSIVE_STANCE))
        //    {
        //        //Log.WriteLine(LogType.Debug, "Using DEFENSIVE_STANCE");
        //        BotHandler.CombatState.SpellCast(BotHandler.BotOwner, DEFENSIVE_STANCE);
        //        // Continue with first combat action if we hit this
        //        return CombatActionResult.ACTION_OK_CONTINUE_FIRST;
        //    }

        //    AttackMelee(unit);

        //    return base.DoFirstCombatAction(unit);
        //}

        //protected override CombatActionResult DoNextCombatAction(Unit unit)
        //{
        //    // TODO: Build up prot warrior combat logic
        //    AttackMelee(unit);

        //    // Use bloodrage if it's available and we are below a certain amount of rage
        //    if (!BotHandler.BotOwner.HasAura(BLOODRAGE) && BotHandler.BotOwner.CurrentPower < 20 && HasSpellAndCanCast(BLOODRAGE))
        //    {
        //        //Log.WriteLine(LogType.Debug, "Using BLOODRAGE");
        //        BotHandler.CombatState.SpellCast(BotHandler.BotOwner, BLOODRAGE);
        //        return CombatActionResult.ACTION_OK;
        //    }

        //    // If revenge procced, use that
        //    if (mRevengeProcced && HasSpellAndCanCast(REVENGE) && IsInMeleeRange(unit))
        //    {
        //        //Log.WriteLine(LogType.Debug, "Using REVENGE");
        //        BotHandler.CombatState.SpellCast(REVENGE);
        //        mRevengeProcced = false;
        //        return CombatActionResult.ACTION_OK;
        //    }

        //    // Get stacks of sunder armor up on current target
        //    if (unit.GetAuraForSpell(SUNDER_ARMOR) == null || 
        //        unit.GetAuraForSpell(SUNDER_ARMOR).Stacks < 3)
        //    {
        //        if (HasSpellAndCanCast(SUNDER_ARMOR) && IsInMeleeRange(unit))
        //        {
        //            //Log.WriteLine(LogType.Debug, "Using SUNDER_ARMOR");
        //            BotHandler.CombatState.SpellCast(SUNDER_ARMOR);
        //            return CombatActionResult.ACTION_OK;
        //        }
        //    }

        //    return base.DoNextCombatAction(unit);
        //}

        protected override void CombatAttackUpdate(Bot bot, Core.World.Objects.Events.CombatAttackUpdateArgs eventArgs)
        {
            // check for revenge procs
            if (bot.Guid == BotHandler.BotOwner.Guid && eventArgs.AttackerGuid == BotHandler.BotOwner.Guid)
            {
                // Procs after block, dodge or parry
                if (BotHandler.BotOwner.HasSpell((ushort)REVENGE) && 
                    (eventArgs.Blocked || eventArgs.Dodged || eventArgs.Parried))
                    mRevengeProcced = true;
            }

            // process base
            base.CombatAttackUpdate(bot, eventArgs);
        }

        #endregion
    }
}
