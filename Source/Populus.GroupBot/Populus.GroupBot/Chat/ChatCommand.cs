using Populus.Core.World.Objects.Events;
using System;
using System.Collections.Generic;

namespace Populus.GroupBot.Chat
{
    public abstract class ChatCommand
    {
        #region Declarations

        private readonly Dictionary<string, Action<GroupBotHandler, ChatEventArgs>> mSubCommands = new Dictionary<string, Action<GroupBotHandler, ChatEventArgs>>();

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds an action handler to the command
        /// </summary>
        /// <param name="subCommand"></param>
        /// <param name="action"></param>
        protected void AddActionHandler(string subCommand, Action<GroupBotHandler, ChatEventArgs> action)
        {
            mSubCommands.Add(subCommand, action);
        }

        /// <summary>
        /// Handles chat commands by finding the action associated with the command and sub command
        /// </summary>
        /// <param name="botHandler"></param>
        /// <param name="chat"></param>
        protected void HandleCommands(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            // Process based on sub command. If action for sub command was not found, process the command without a sub command
            if (mSubCommands.ContainsKey(chat.GetSubCommand()))
            {
                mSubCommands[chat.GetSubCommand()].Invoke(botHandler, chat);
                return;
            }

            // No sub command action was found, use default command handler
            mSubCommands[string.Empty].Invoke(botHandler, chat);
        }

        #endregion
    }
}
