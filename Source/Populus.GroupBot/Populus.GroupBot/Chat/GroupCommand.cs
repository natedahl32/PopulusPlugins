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

            // Group role commands
            if (chat.MessageTokenized.Length > 1 && chat.MessageTokenized[1].ToLower() == "roles")
            {
                if (chat.MessageTokenized.Length > 2 && chat.MessageTokenized[2].ToLower() == "set")
                {
                    if (chat.MessageTokenized.Length < 3)
                    {
                        botHandler.BotOwner.ChatSay("Provide a role name to set!");
                        return;
                    }

                    var role = chat.MessageTokenized[3];
                    if (botHandler.Group == null) return;
                    var member = botHandler.Group.GetMember(botHandler.BotOwner.Guid);
                    if (member == null)
                        return;

                    member.AssignRole(role);
                    return;
                }

                if (chat.MessageTokenized.Length > 2 && chat.MessageTokenized[2].ToLower() == "clear")
                {
                    if (botHandler.Group == null) return;
                    var member = botHandler.Group.GetMember(botHandler.BotOwner.Guid);
                    if (member == null)
                        return;

                    member.AssignRole(string.Empty);
                    return;
                }

                if (chat.MessageTokenized.Length > 2 && chat.MessageTokenized[2].ToLower() == "auto")
                {
                    botHandler.AssignGroupRole();
                    return;
                }

                ListAllRolesInGroup(botHandler);
                return;
            }
        }

        private void ListAllRolesInGroup(GroupBotHandler botHandler)
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
    }
}
