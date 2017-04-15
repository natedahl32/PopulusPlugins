using System;
using Populus.Core.World.Objects;

namespace Populus.CombatManager.Actions
{
    public class CastSpellAbility : ActionManager.Action
    {
        #region Declarations

        private readonly Unit mTarget;
        private readonly uint mSpellId;
        private bool mIsComplete = false;

        #endregion

        #region Constructors

        internal CastSpellAbility(Bot bot, Unit target, uint spellId) : base(bot)
        {
            if (target == null) throw new ArgumentNullException("target");
            mTarget = target;
            mSpellId = spellId;
        }

        #endregion

        #region Properties

        public override bool IsComplete => mIsComplete;

        public override int Timeout => 11000;   // 11 seconds

        #endregion

        #region Public Methods

        public override void Start()
        {
            base.Start();

            // Handle spell cast events
            Bot.SpellCastCompleted += Bot_SpellCastCompleted;
            Bot.SpellCastFailed += Bot_SpellCastFailed;
            Bot.SpellInterrupted += Bot_SpellInterrupted;

            // Cast the spell
            BotOwner.CastSpellAbility(mTarget.Guid, mSpellId);
        }

        public override void Completed()
        {
            base.Completed();

            // Unregister events
            Bot.SpellCastCompleted -= Bot_SpellCastCompleted;
            Bot.SpellCastFailed -= Bot_SpellCastFailed;
            Bot.SpellInterrupted -= Bot_SpellInterrupted;
        }

        #endregion

        #region Private Methods

        private void Bot_SpellInterrupted(Bot bot, Core.World.Objects.Events.SpellInterruptedArgs eventArgs)
        {
            if (bot.Guid == BotOwner.Guid && eventArgs.SpellId == mSpellId)
                mIsComplete = true;
        }

        private void Bot_SpellCastFailed(Bot bot, Core.World.Objects.Events.SpellCastFailedArgs eventArgs)
        {
            if (bot.Guid == BotOwner.Guid && eventArgs.SpellId == mSpellId)
                mIsComplete = true;
        }

        private void Bot_SpellCastCompleted(Bot bot, Core.World.Objects.Events.SpellCastCompleteArgs eventArgs)
        {
            if (bot.Guid == BotOwner.Guid && eventArgs.SpellId == mSpellId)
                mIsComplete = true;
        }

        #endregion
    }
}
