using System;
using Populus.Core.World.Objects;

namespace Populus.ActionManager.Actions
{
    public class BuySpellsFromTrainer : Action
    {
        #region Declarations

        private Unit mTrainer;
        private int mTotalSpellsPurchases = 0;
        private int mTotalSpellsConfirmationPurchase = -1;

        #endregion

        #region Constructors

        public BuySpellsFromTrainer(Bot bot, Unit trainer) : base(bot)
        {
            if (trainer == null) throw new ArgumentNullException("trainer");
            mTrainer = trainer;
        }

        #endregion

        #region Properties

        public override bool IsComplete => mTotalSpellsPurchases == mTotalSpellsConfirmationPurchase;

        public override int Timeout => 5000;

        #endregion

        #region Public Methods

        public override void Start()
        {
            base.Start();

            // Wire up events we need to process this action
            Bot.TrainerSpellListReceived += TrainerSpellListReceived;
            Bot.TrainerBuySpellSucceeded += TrainerBuySpellSucceeded;

            BotOwner.RequestSpellListFromTrainer(mTrainer.Guid);
        }

        public override void Completed()
        {
            base.Completed();

            // Remove events
            Bot.TrainerSpellListReceived -= TrainerSpellListReceived;
            Bot.TrainerBuySpellSucceeded -= TrainerBuySpellSucceeded;
        }

        #endregion

        #region Private Methods

        private void TrainerBuySpellSucceeded(Bot bot, uint eventArgs)
        {
            mTotalSpellsConfirmationPurchase++;
        }

        private void TrainerSpellListReceived(Bot bot, Core.World.Objects.Events.TrainerSpellListArgs eventArgs)
        {
            // Go through each spell received that we do not have yet and can learn and see if we can buy it
            var currentMoney = BotOwner.TotalCoin;
            foreach (var spell in eventArgs.CanLearnSpells)
            {
                if (currentMoney >= spell.Cost)
                {
                    BotOwner.BuySpellFromTrainer(mTrainer.Guid, spell.SpellId);
                    currentMoney -= spell.Cost;
                    mTotalSpellsPurchases++;
                }
            }

            // Initialzie our confirmation tally
            mTotalSpellsConfirmationPurchase = 0;
        }

        #endregion
    }
}
