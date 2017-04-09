using System;

namespace Populus.GroupBot.Chat
{
    /// <summary>
    /// Attribute that can be applied to chat commands which will be processed when one or more chat commands are sent
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ChatCommandKeyAttribute : Attribute
    {
        public ChatCommandKeyAttribute(string command)
        {
            if (string.IsNullOrEmpty(command)) throw new ArgumentNullException("command");
            Command = command;
        }

        public string Command { get; set; }
    }
}
