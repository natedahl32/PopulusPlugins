using System;
using Populus.Core.World.Objects.Events;
using Populus.Core.World.Objects;

namespace Populus.GroupBot.Chat
{
    /// <summary>
    /// Chat command for interacting with vendors
    /// </summary>
    [ChatCommandKey("vendor")]
    public class VendorCommand : ChatCommand, IChatCommand
    {
        public VendorCommand()
        {
            AddActionHandler(string.Empty, Help);
            AddActionHandler("spells", VendorSpells);
        }

        public void ProcessCommand(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            if (botHandler == null) throw new ArgumentNullException("botHandler");
            if (chat == null) throw new ArgumentNullException("chat");

            // Any vendor command must come from the leader of the group
            if (chat.SenderGuid != botHandler.Group.Leader.Guid) return;

            // Handle chat commands
            HandleCommands(botHandler, chat);
        }

        /// <summary>
        /// Default action handler for vendor command
        /// </summary>
        /// <param name="botHandler"></param>
        /// <param name="chat"></param>
        private void Help(GroupBotHandler botHandler, ChatEventArgs chat)
        {

        }

        /// <summary>
        /// Purchases available spells from class trainer
        /// </summary>
        /// <param name="botHandler"></param>
        /// <param name="chat"></param>
        private void VendorSpells(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            // Get the leaders target
            var leaderObj = botHandler.BotOwner.GetPlayerByGuid(botHandler.Group.Leader.Guid);
            if (leaderObj == null) return;
            var target = botHandler.BotOwner.GetUnitByGuid(leaderObj.TargetGuid);
            if (target == null) return;

            HandleSpellTrainer(botHandler, target);
        }

        private void HandleSpellTrainer(GroupBotHandler botHandler, Unit unit)
        {
            //var actionQueue = ActionMgr.GetActionQueue(botHandler.BotOwner.Guid);
            //actionQueue.Add(new MoveTowardsObject(botHandler.BotOwner, unit, 1.0f));
            //actionQueue.Add(new BuySpellsFromTrainer(botHandler.BotOwner, unit));
        }
    }
}
