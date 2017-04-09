using System;
using Populus.Core.World.Objects.Events;

namespace Populus.GroupBot.Chat
{
    /// <summary>
    /// Chat command for a group bot that handles any following orders
    /// </summary>
    [ChatCommandKey("follow")]
    public class FollowCommand : IChatCommand
    {
        public void ProcessCommand(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            if (botHandler == null) throw new ArgumentNullException("botHandler");
            if (chat == null) throw new ArgumentNullException("chat");

            // Any follow command must come from the leader of the group
            if (chat.SenderGuid != botHandler.Group.Leader.Guid) return;

            // TODO: Maybe add subcommand objects?

            // Turn follow off
            if (chat.MessageTokenized.Length > 1 && chat.MessageTokenized[1].ToLower() == "off")
            {
                botHandler.BotOwner.RemoveFollow();
                return;
            }
            
            // Handle another member of the group besides leader. This only works when the actual command comes from the leader
            if (chat.MessageTokenized.Length > 1)
            {
                var sub = chat.MessageTokenized[1];
                foreach (var member in botHandler.Group.Members)
                    if (member.Name != null && member.Name.ToLower() == sub.ToLower())
                    {
                        botHandler.FollowGroupMember(member);
                        return;
                    }
            }

            // Ignore any other sub commands and follow the group leader
            botHandler.FollowGroupLeader();
        }
    }
}
