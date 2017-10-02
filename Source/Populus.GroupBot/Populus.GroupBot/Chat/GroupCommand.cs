using Populus.Core.World.Objects.Events;
using System;

namespace Populus.GroupBot.Chat
{
    [ChatCommandKey("group")]
    public class GroupCommand : ChatCommand, IChatCommand
    {
        public GroupCommand()
        {
            AddActionHandler(string.Empty, Help);
            AddActionHandler("drop", LeaveGroup);
            AddActionHandler("leave", LeaveGroup);
            AddActionHandler("disband", LeaveGroup);
            AddActionHandler("roles", Roles);
        }

        public void ProcessCommand(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            if (botHandler == null) throw new ArgumentNullException("botHandler");
            if (chat == null) throw new ArgumentNullException("chat");

            // Handle chat commands
            HandleCommands(botHandler, chat);
        }

        /// <summary>
        /// Default action for group command
        /// </summary>
        /// <param name="botHandler"></param>
        /// <param name="chat"></param>
        private void Help(GroupBotHandler botHandler, ChatEventArgs chat)
        {

        }

        /// <summary>
        /// Commands bot to leave their current group
        /// </summary>
        /// <param name="botHandler"></param>
        /// <param name="chat"></param>
        private void LeaveGroup(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            botHandler.BotOwner.LeaveGroup();
        }

        /// <summary>
        /// Sub commands for roles command
        /// </summary>
        /// <param name="botHandler"></param>
        /// <param name="chat"></param>
        private void Roles(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            var roleCommand = new GroupRoleCommand();
            chat.RemoveToken();
            roleCommand.ProcessCommand(botHandler, chat);
        }
    }
}
