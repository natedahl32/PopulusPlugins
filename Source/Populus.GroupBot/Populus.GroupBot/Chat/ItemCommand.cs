using System;
using Populus.Core.World.Objects.Events;
using System.Linq;

namespace Populus.GroupBot.Chat
{
    /// <summary>
    /// Chat commands for dealing with items
    /// </summary>
    [ChatCommandKey("item")]
    public class ItemCommand : ChatCommand, IChatCommand
    {
        public ItemCommand()
        {
            AddActionHandler(string.Empty, InventoryItems);
            AddActionHandler("equip", EquipItem);
        }

        public void ProcessCommand(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            if (botHandler == null) throw new ArgumentNullException("botHandler");
            if (chat == null) throw new ArgumentNullException("chat");

            // Any item command must come from the leader of the group
            if (chat.SenderGuid != botHandler.Group.Leader.Guid) return;

            // Handle chat commands
            HandleCommands(botHandler, chat);
        }

        /// <summary>
        /// Displays the bots items in inventory
        /// </summary>
        /// <param name="botHandler"></param>
        /// <param name="chat"></param>
        private void InventoryItems(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            botHandler.BotOwner.ChatParty("My items in inventory are:");
            foreach (var invItem in botHandler.BotOwner.InventoryItems)
            {
                if (invItem.Item.StackCount > 1)
                    botHandler.BotOwner.ChatParty($"{invItem.Item.ItemGameLink} x{invItem.Item.StackCount}");
                else
                    botHandler.BotOwner.ChatParty($"{invItem.Item.ItemGameLink}");
            }
        }

        /// <summary>
        /// Commands the bot to equip and item from inventory
        /// </summary>
        /// <param name="botHandler"></param>
        /// <param name="chat"></param>
        private void EquipItem(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            // Get the linked item id from chat
            var linkedItems = chat.LinkedItems.ToList();
            if (linkedItems.Count <= 0)
            {
                botHandler.BotOwner.ChatParty("Link the item you would like me to equip.");
                return;
            }

            var linkedItemId = linkedItems[0].ItemId;

            // If we were sent a specific slot to equip to
            if (chat.MessageTokenized.Length > 2 && chat.MessageTokenized[2].ToLower() == "finger1")
                botHandler.BotOwner.AutoEquipItem(linkedItemId, Core.Constants.EquipmentSlots.EQUIPMENT_SLOT_FINGER1);
            else if (chat.MessageTokenized.Length > 2 && chat.MessageTokenized[2].ToLower() == "finger2")
                botHandler.BotOwner.AutoEquipItem(linkedItemId, Core.Constants.EquipmentSlots.EQUIPMENT_SLOT_FINGER2);
            else if (chat.MessageTokenized.Length > 2 && chat.MessageTokenized[2].ToLower() == "trinket1")
                botHandler.BotOwner.AutoEquipItem(linkedItemId, Core.Constants.EquipmentSlots.EQUIPMENT_SLOT_TRINKET1);
            else if (chat.MessageTokenized.Length > 2 && chat.MessageTokenized[2].ToLower() == "trinket2")
                botHandler.BotOwner.AutoEquipItem(linkedItemId, Core.Constants.EquipmentSlots.EQUIPMENT_SLOT_TRINKET2);
            else if (chat.MessageTokenized.Length > 2 && chat.MessageTokenized[2].ToLower() == "mainhand")
                botHandler.BotOwner.AutoEquipItem(linkedItemId, Core.Constants.EquipmentSlots.EQUIPMENT_SLOT_MAINHAND);
            else if (chat.MessageTokenized.Length > 2 && chat.MessageTokenized[2].ToLower() == "offhand")
                botHandler.BotOwner.AutoEquipItem(linkedItemId, Core.Constants.EquipmentSlots.EQUIPMENT_SLOT_OFFHAND);
            else
            {
                // Try and get the item from inventory
                var itm = botHandler.BotOwner.GetInventoryItem(linkedItemId);
                // Handle trying to equip ammo, this is done differently
                if (itm != null && itm.Item.BaseInfo.InventoryType == Core.Constants.InventoryType.INVTYPE_AMMO)
                {
                    botHandler.BotOwner.SetAmmo(linkedItemId);
                    return;
                }

                // By default, autoequip the item to any slot
                botHandler.BotOwner.AutoEquipItem(linkedItemId);
            }
        }
    }
}
