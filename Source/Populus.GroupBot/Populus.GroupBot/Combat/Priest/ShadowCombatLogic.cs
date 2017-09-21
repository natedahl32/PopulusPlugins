using Populus.Core.World.Objects;

namespace Populus.GroupBot.Combat.Priest
{
    public class ShadowCombatLogic : PriestCombatLogic
    {
        #region Constructors

        public ShadowCombatLogic(GroupBotHandler botHandler) : base(botHandler)
        {

        }

        #endregion

        #region Public Methods

        //public override CombatActionResult DoOutOfCombatAction()
        //{
        //    // Check for Shadowform
        //    if (HasSpellAndCanCast(SHADOWFORM) && !BotHandler.BotOwner.HasAura(SHADOW_WORD_PAIN))
        //    {
        //        BotHandler.CombatState.SpellCast(SHADOWFORM);
        //        return CombatActionResult.ACTION_OK;
        //    }

        //    return base.DoOutOfCombatAction();
        //}

        #endregion

        #region Private Methods

        //protected override CombatActionResult DoFirstCombatAction(Unit unit)
        //{
        //    // When not in a group, pull with Mind Blast
        //    if (BotHandler.Group == null)
        //    {
        //        if (HasSpellAndCanCast(MIND_BLAST))
        //        {
        //            BotHandler.CombatState.SpellCast(MIND_BLAST);
        //            return CombatActionResult.ACTION_OK;
        //        }
        //    }

        //    // SW:P to start when not in a group
        //    if (HasSpellAndCanCast(SHADOW_WORD_PAIN) && !unit.HasAura(SHADOW_WORD_PAIN))
        //    {
        //        BotHandler.CombatState.SpellCast(SHADOW_WORD_PAIN);
        //        return CombatActionResult.ACTION_OK;
        //    }

        //    return CombatActionResult.NO_ACTION_OK;
        //}

        //protected override CombatActionResult DoNextCombatAction(Unit unit)
        //{
        //    // If we don't have Shadow word pain up
        //    if (HasSpellAndCanCast(SHADOW_WORD_PAIN) && !unit.HasAura(SHADOW_WORD_PAIN))
        //    {
        //        BotHandler.CombatState.SpellCast(SHADOW_WORD_PAIN);
        //        return CombatActionResult.ACTION_OK;
        //    }

        //    // Mind blast if we can and we have the mana
        //    if (HasSpellAndCanCast(MIND_BLAST) && (BotHandler.BotOwner.PowerPercentage > 50.0f || unit.HealthPercentage < 20.0f))
        //    {
        //        BotHandler.CombatState.SpellCast(MIND_BLAST);
        //        return CombatActionResult.ACTION_OK;
        //    }

        //    // Mind flay if we have the mana
        //    if (HasSpellAndCanCast(MIND_FLAY) && BotHandler.BotOwner.PowerPercentage > 30.0f)
        //    {
        //        BotHandler.CombatState.SpellCast(MIND_FLAY);
        //        return CombatActionResult.ACTION_OK;
        //    }

        //    // Wand if we get here
        //    AttackWand(unit);
        //    return CombatActionResult.NO_ACTION_OK;
        //}

        #endregion
    }
}
