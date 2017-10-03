using Populus.Core.World.Objects.Events;
using Populus.GroupBot.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Populus.GroupBot
{
    /// <summary>
    /// Handles chat messages received
    /// </summary>
    public class GroupBotChatHandler
    {
        #region Declarations

        // reference to the group bot handler so we can pass to chat command
        private readonly GroupBotHandler mGroupBotHandler;

        // holds all chat commands we will process
        private static readonly Dictionary<string, IChatCommand> mChatCommand = new Dictionary<string, IChatCommand>();

        #endregion

        #region Constructors

        public GroupBotChatHandler(GroupBotHandler handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");
            mGroupBotHandler = handler;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles an incomming chat message
        /// </summary>
        /// <param name="chat">Chat message to handle</param>
        public void HandleChatMessage(ChatEventArgs chat)
        {
            var commandString = chat.MessageTokenized[0].ToLower();
            if (!mChatCommand.ContainsKey(commandString))
            {
                mGroupBotHandler.BotOwner.Logger.Log($"Chat Message to {mGroupBotHandler.BotOwner.Name}: {chat.MessageText}");
                return;
            }
                

            // Handle chat based on first token
            var command = mChatCommand[commandString];
            if (command != null)
                command.ProcessCommand(mGroupBotHandler, chat);
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Registers all chat commands in the plugin
        /// </summary>
        public static void RegisterChatCommands()
        {
            // Clear just in case this is called after load
            mChatCommand.Clear();

            var types = Assembly
                .GetExecutingAssembly()
                .GetTypes();

            foreach (var t in types)
            {
                var customAttributes = t.GetCustomAttributes<ChatCommandKeyAttribute>();
                if (customAttributes.Count() > 0)
                {
                    foreach (var attr in customAttributes)
                    {
                        var chatAttr = attr as ChatCommandKeyAttribute;
                        if (chatAttr != null)
                            mChatCommand.Add(chatAttr.Command.ToLower(), (IChatCommand)Activator.CreateInstance(t));
                    }
                }
            }
        }

        #endregion
    }
}
