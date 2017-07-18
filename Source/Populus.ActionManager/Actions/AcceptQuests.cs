using Populus.Core.World.Objects;
using Populus.Core.World.Objects.Events;
using System;

namespace Populus.ActionManager.Actions
{
    public class AcceptQuests : Action
    {
        #region Declarations

        private WorldObject mQuestGiver;
        private bool mIsComplete = false;

        #endregion

        #region Constructors

        public AcceptQuests(Bot bot, WorldObject questGiver) : base(bot)
        {
            if (questGiver == null) throw new ArgumentNullException("questGiver");
            mQuestGiver = questGiver;
        }

        #endregion

        #region Properties

        public override bool IsComplete => mIsComplete;

        #endregion

        #region Public Methods

        public override void Start()
        {
            base.Start();

            // Wire up our events we need
            Bot.QuestListReceived += QuestListReceived;
            Bot.QuestOfferReceived += QuestOfferReceived;

            // Request quest list
            BotOwner.RequestQuestListFromQuestGiver(mQuestGiver.Guid);
        }

        public override void Removed()
        {
            base.Removed();

            Bot.QuestOfferReceived -= QuestOfferReceived;
            Bot.QuestListReceived -= QuestListReceived;
        }

        #endregion

        #region Private Methods

        private void QuestListReceived(Bot bot, QuestGiverListArgs args)
        {
            foreach (var q in args.QuestListItems)
                BotOwner.AcceptQuest(mQuestGiver.Guid, q.QuestId);
            mIsComplete = true;
        }

        private void QuestOfferReceived(Bot bot, uint questId)
        {
            BotOwner.AcceptQuest(mQuestGiver.Guid, questId);
            mIsComplete = true;
        }

        #endregion
    }
}
