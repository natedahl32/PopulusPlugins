using Populus.Core.World.Objects.Events;

namespace Populus.GroupBot.Chat
{
    /// <summary>
    /// Interface for processing chat commands
    /// </summary>
    public interface IChatCommand
    {
        /// <summary>
        /// Processes a specific chat command
        /// </summary>
        /// <param name="chat"></param>
        void ProcessCommand(GroupBotHandler botHandler, ChatEventArgs chat);
    }
}
