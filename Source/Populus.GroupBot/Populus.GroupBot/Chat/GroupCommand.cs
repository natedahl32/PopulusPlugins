using Populus.Core.World.Objects.Events;
using System;

namespace Populus.GroupBot.Chat
{
    [ChatCommandKey("group")]
    public class GroupCommand : IChatCommand
    {
        public void ProcessCommand(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            if (botHandler == null) throw new ArgumentNullException("botHandler");
            if (chat == null) throw new ArgumentNullException("chat");

            // TODO: Maybe add subcommand objects?

            // Turn follow off
            if (chat.MessageTokenized.Length > 1 && chat.MessageTokenized[1].ToLower() == "drop")
            {
                botHandler.BotOwner.LeaveGroup();
                return;
            }
        }
    }
}
