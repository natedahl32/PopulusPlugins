using Populus.Core.World.Objects;
using System;

namespace Populus.ActionManager.Actions
{
    public class CompleteQuests : Action
    {
        #region Declarations

        private WorldObject mQuestGiver;
        private bool mIsComplete = false;

        #endregion

        #region Constructors

        public CompleteQuests(Bot bot, WorldObject questGiver) : base(bot)
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
        }

        #endregion
    }
}
