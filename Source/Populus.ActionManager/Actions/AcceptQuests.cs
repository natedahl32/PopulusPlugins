using Populus.Core.World.Objects;
using Populus.Core.World.Objects.Events;
using System;

namespace Populus.ActionManager.Actions
{
    public class AcceptQuests : Action
    {
        #region Declarations

        private WorldObject mQuestGiver;

        #endregion

        #region Constructors

        public AcceptQuests(Bot bot, WorldObject questGiver) : base(bot)
        {
            if (questGiver == null) throw new ArgumentNullException("questGiver");
            mQuestGiver = questGiver;
        }

        #endregion

        #region Properties

        public override bool IsComplete => throw new NotImplementedException();

        #endregion

        #region Public Methods

        public override void Start()
        {
            base.Start();

            // Wire up our events we need
            Bot.QuestListReceived += QuestListReceived;
        }

        #endregion

        #region Private Methods

        private void QuestListReceived(Bot bot, QuestGiverListArgs args)
        {
            // TODO: Accept all quests returned to us
        }

        #endregion
    }
}
