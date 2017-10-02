using Populus.Core.World.Objects.Events;

namespace Populus.GroupBot.Chat
{
    public static class ChatEventArgExtensions
    {
        /// <summary>
        /// Gets a command from the chat event args
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GetCommand(this ChatEventArgs args)
        {
            if (args == null) return string.Empty;
            return args.MessageTokenized[0];
        }

        /// <summary>
        /// Gets a sub-command from the chat event args
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GetSubCommand(this ChatEventArgs args)
        {
            if (args == null || args.MessageTokenized.Length <= 1) return string.Empty;
            return args.MessageTokenized[1];
        }
    }
}
