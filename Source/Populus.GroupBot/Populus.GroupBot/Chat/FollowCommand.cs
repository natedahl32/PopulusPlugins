using System;
using Populus.Core.World.Objects.Events;
using System.Collections.Generic;

namespace Populus.GroupBot.Chat
{
    /// <summary>
    /// Chat command for a group bot that handles any following orders
    /// </summary>
    [ChatCommandKey("follow")]
    public class FollowCommand : ChatCommand, IChatCommand
    {
        public FollowCommand()
        {
            AddActionHandler(string.Empty, Follow);
            AddActionHandler("off", FollowOff);
        }

        public void ProcessCommand(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            if (botHandler == null) throw new ArgumentNullException("botHandler");
            if (chat == null) throw new ArgumentNullException("chat");

            // Any follow command must come from the leader of the group
            if (chat.SenderGuid != botHandler.Group.Leader.Guid) return;

            // Handle chat commands
            HandleCommands(botHandler, chat);
        }

        /// <summary>
        /// Follows the person commanded
        /// </summary>
        /// <param name="botHandler"></param>
        /// <param name="chat"></param>
        protected void Follow(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            // Handle another member of the group besides leader. This only works when the actual command comes from the leader
            if (chat.MessageTokenized.Length > 1)
            {
                var sub = chat.MessageTokenized[1];
                foreach (var member in botHandler.Group.Members)
                    if (member.Name != null && member.Name.ToLower() == sub.ToLower())
                    {
                        botHandler.TriggerState(Triggers.StateTriggers.Idle);
                        botHandler.FollowGroupMember(member);
                        return;
                    }
            }

            // Ignore any other sub commands and follow the group leader. Trigger idle state when we issue follow command in case we are stuck in another state
            botHandler.TriggerState(Triggers.StateTriggers.Idle);
            botHandler.FollowGroupLeader();
        }

        /// <summary>
        /// Stops following whatever we are currently following
        /// </summary>
        /// <param name="botHandler"></param>
        /// <param name="chat"></param>
        protected void FollowOff(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            botHandler.StopFollow();
        }
    }
}
