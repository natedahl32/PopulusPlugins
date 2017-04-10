using System;
using Populus.Core.World.Objects;
using Populus.Core.World.Objects.Events;

namespace Populus.ActionManager.Actions
{
    public class LootObject : Action
    {
        #region Declarations

        private WorldObject mLootObject;

        // Completed when we receive the number of items we loot
        private int mNumberOfItemsLooted = -1;
        private int mNumberOfItemsReceived = 0;

        #endregion

        #region Constructors

        public LootObject(Bot bot, WorldObject wo) : base(bot)
        {
            if (wo == null) throw new ArgumentNullException("wo");
            mLootObject = wo;
        }

        #endregion

        #region Properties

        public override bool IsComplete => mNumberOfItemsLooted == mNumberOfItemsReceived;

        #endregion

        #region Public Methods

        public override void Start()
        {
            base.Start();

            // Wire up our loot response handler so we can get the response from our loot request
            Bot.LootResponse += LootResponseHandler;
            Bot.ItemReceived += BotReceivedItem;
            Bot.InventoryChangeFailure += BotInventoryChangeFailure;

            // TODO: We don't know this is a corpse/unit. What about game objects?
            BotOwner.LootCorpse(mLootObject.Guid);
        }

        public override void Completed()
        {
            base.Completed();

            BotOwner.DoneLooting(mLootObject.Guid);
        }

        public override void Removed()
        {
            base.Removed();
            Bot.LootResponse -= LootResponseHandler;
            Bot.ItemReceived -= BotReceivedItem;
            Bot.InventoryChangeFailure -= BotInventoryChangeFailure;
        }

        #endregion

        #region Private Methods

        private void LootResponseHandler(Bot bot, LootResponseArgs args)
        {
            // TODO: This is completely broken as it is. This will respond to the loot response any bot that loots. This is NOT correct. We only want the event from OUR bot.

            // If there is money, request that the money be looted
            if (args.GoldAmount > 0) bot.LootCoin();

            // If there are no items to loot, set the number of looted items so we hit our completed condition
            if (args.ItemCount == 0) mNumberOfItemsReceived = -1;

            // Go through each item and loot it if we can
            foreach (var item in args.Items)
                if (item.LootSlotType == Core.Constants.LootSlotType.LOOT_SLOT_NORMAL)
                {
                    bot.LootItem(item.LootSlot);
                    if (mNumberOfItemsLooted == -1)
                        mNumberOfItemsLooted = 1;
                    else
                        mNumberOfItemsLooted++;
                }
        }

        private void BotReceivedItem(Bot bot, ItemPushResultArgs args)
        {
            // TODO: This is completely broken as it is. This will respond to the loot response any bot that loots. This is NOT correct. We only want the event from OUR bot.

            mNumberOfItemsReceived++;
        }

        private void BotInventoryChangeFailure(Bot bot, InventoryChangeFailureArgs args)
        {
            // TODO: This is completely broken as it is. This will respond to the loot response any bot that loots. This is NOT correct. We only want the event from OUR bot.

            mNumberOfItemsReceived++;
        }

        #endregion
    }
}
