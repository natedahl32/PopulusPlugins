using System;
using Populus.Core.World.Objects.Events;

namespace Populus.GroupBot.Chat
{
    /// <summary>
    /// This is a sub command, action taken when the "group role" command is issued
    /// </summary>
    [ChatCommandKey("roles")]
    public class GroupRoleCommand : ChatCommand, IChatCommand
    {
        public GroupRoleCommand()
        {
            AddActionHandler(string.Empty, ListAllRolesInGroup);
            AddActionHandler("set", SetRole);
            AddActionHandler("clear", ClearRole);
            AddActionHandler("auto", AutoAssign);
        }

        public void ProcessCommand(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            if (botHandler == null) throw new ArgumentNullException("botHandler");
            if (chat == null) throw new ArgumentNullException("chat");

            // Handle chat commands
            HandleCommands(botHandler, chat);
        }

        /// <summary>
        /// List role the bot plays in the group (if any)
        /// </summary>
        /// <param name="botHandler"></param>
        /// <param name="chat"></param>
        private void ListAllRolesInGroup(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            if (botHandler.Group == null) return;
            var member = botHandler.Group.GetMember(botHandler.BotOwner.Guid);
            if (member == null)
                return;

            if (string.IsNullOrEmpty(member.Role) || member.Role.ToLower() == "none")
                botHandler.BotOwner.ChatSay("I am not in a role");
            else
                botHandler.BotOwner.ChatSay($"I am in role {member.Role}");
        }

        /// <summary>
        /// Sets role for the bot
        /// </summary>
        /// <param name="botHandler"></param>
        /// <param name="chat"></param>
        private void SetRole(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            if (chat.MessageTokenized.Length < 2)
            {
                botHandler.BotOwner.ChatSay("Provide a role name to set!");
                return;
            }

            // Get role to set
            var role = chat.MessageTokenized.Length == 3 ? 
                chat.MessageTokenized[2] :
                chat.MessageTokenized[3];
            // Get member to set for
            var memberName = chat.MessageTokenized.Length == 3 ?
                string.Empty :
                chat.MessageTokenized[2];

            if (botHandler.Group == null) return;
            // If name was supplied, set that members role
            var member = string.IsNullOrEmpty(memberName) ? 
                botHandler.Group.GetMember(botHandler.BotOwner.Guid) :
                botHandler.Group.GetMember(memberName);
            if (member == null)
                return;

            member.AssignRole(role);
            return;
        }

        /// <summary>
        /// Clears role for the bot
        /// </summary>
        /// <param name="botHandler"></param>
        /// <param name="chat"></param>
        private void ClearRole(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            if (botHandler.Group == null) return;
            var member = botHandler.Group.GetMember(botHandler.BotOwner.Guid);
            if (member == null)
                return;

            member.AssignRole(string.Empty);
        }

        /// <summary>
        /// Auto assigns role for the bot
        /// </summary>
        /// <param name="botHandler"></param>
        /// <param name="chat"></param>
        private void AutoAssign(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            botHandler.AssignGroupRole();
        }
    }
}
