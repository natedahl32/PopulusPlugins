using System;
using Populus.Core.World.Objects.Events;
using ActionMgr = Populus.ActionManager.ActionManager;
using Populus.ActionManager.Actions;
using Populus.Core.World.Objects;

namespace Populus.GroupBot.Chat
{
    /// <summary>
    /// Chat command for interacting with vendors
    /// </summary>
    [ChatCommandKey("vendor")]
    public class VendorCommand : IChatCommand
    {
        public void ProcessCommand(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            if (botHandler == null) throw new ArgumentNullException("botHandler");
            if (chat == null) throw new ArgumentNullException("chat");

            // Any vendor command must come from the leader of the group
            if (chat.SenderGuid != botHandler.Group.Leader.Guid) return;

            // Get the leaders target
            var leaderObj = botHandler.BotOwner.GetPlayerByGuid(botHandler.Group.Leader.Guid);
            if (leaderObj == null) return;
            var target = botHandler.BotOwner.GetUnitByGuid(leaderObj.TargetGuid);
            if (target == null) return;

            // TODO: Maybe add subcommand objects?

            // Loot all corpses that we can when the sub command is "all"
            if (chat.MessageTokenized.Length > 1 && chat.MessageTokenized[1].ToLower() == "spells")
            {
                HandleSpellTrainer(botHandler, target);
                return;
            }
        }

        private void HandleSpellTrainer(GroupBotHandler botHandler, Unit unit)
        {
            var actionQueue = ActionMgr.GetActionQueue(botHandler.BotOwner.Guid);
            actionQueue.Add(new MoveTowardsObject(botHandler.BotOwner, unit, 1.0f));
            actionQueue.Add(new BuySpellsFromTrainer(botHandler.BotOwner, unit));
        }
    }
}
