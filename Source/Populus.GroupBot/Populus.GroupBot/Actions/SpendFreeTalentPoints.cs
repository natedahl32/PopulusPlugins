using Populus.Core.World.Objects;
using Populus.ActionManager;
using System;
using Populus.GroupBot.Talents;
using Populus.Core.DBC;

namespace Populus.GroupBot.Actions
{
    public class SpendFreeTalentPoints : ActionManager.Action
    {
        #region Declarations

        private bool mCantLearnAnyTalents = false;
        private TalentSpec mCurrentTalentSpec;
        private System.Action mCompletedCallback;

        #endregion

        #region Constructors

        public SpendFreeTalentPoints(Bot bot, TalentSpec currentTalentSpec) : base(bot)
        {
            if (currentTalentSpec == null) throw new ArgumentNullException("currentTalentSpec");
            mCurrentTalentSpec = currentTalentSpec;
        }

        public SpendFreeTalentPoints(Bot bot, TalentSpec currentTalentSpec, System.Action completedCallback) : this(bot, currentTalentSpec)
        {
            if (completedCallback == null) throw new ArgumentNullException("completedCallback");
            mCompletedCallback = completedCallback;
        }

        #endregion

        #region Properties

        public override bool IsComplete => BotOwner.FreeTalentPoints == 0 || mCantLearnAnyTalents;

        #endregion

        #region Public Methods

        public override void Start()
        {
            base.Start();

            // Find the next talent to learn
            var nextTalent = FindNextTalent();

            // If there were no talents found, exit out
            if (nextTalent == 0)
            {
                mCantLearnAnyTalents = true;
                return;
            }

            // Wire up events we need and try to learn the talent
            Bot.LearnedSpell += LearnedSpell;

            // Learn the talent
            BotOwner.LearnTalent(nextTalent);
        }

        public override void Completed()
        {
            base.Completed();

            // Remove events
            Bot.LearnedSpell -= LearnedSpell;

            // If we have a completed callback, invoke it
            mCompletedCallback?.Invoke();
        }

        #endregion

        #region Private Methods

        private void LearnedSpell(Bot bot, uint eventArgs)
        {
            var spell = SpellTable.Instance.getSpell(eventArgs);
            if (spell != null)
                BotOwner.ChatParty($"I learned the talent {spell.SpellName}");
            
            if (BotOwner.FreeTalentPoints > 0)
            {
                // Find the next talent to learn
                var nextTalent = FindNextTalent();

                // If there were no talents found, exit out
                if (nextTalent == 0)
                {
                    mCantLearnAnyTalents = true;
                    return;
                }

                // Learn the next talent
                BotOwner.LearnTalent(nextTalent);
            }
        }

        /// <summary>
        /// Finds the next talent to learn
        /// </summary>
        /// <returns></returns>
        private uint FindNextTalent()
        {
            // Find the next talent to purchase
            uint nextTalent = 0;
            foreach (var talent in mCurrentTalentSpec.Talents)
                if (!BotOwner.HasTalentOrBetter((ushort)talent))
                {
                    nextTalent = talent;
                    break;
                }

            return nextTalent;
        }

        #endregion
    }
}
