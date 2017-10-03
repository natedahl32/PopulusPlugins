using System;
using Populus.Core.World.Objects.Events;
using Populus.Core.DBC;
using Populus.Core.Log;

namespace Populus.GroupBot.Chat
{
    [ChatCommandKey("debug")]
    public class DebugCommand : ChatCommand, IChatCommand
    {
        public DebugCommand()
        {
            AddActionHandler(string.Empty, ToggleDebug);
            AddActionHandler("on", ToggleOnDebug);
            AddActionHandler("off", ToggleOffDebug);
            AddActionHandler("spell", DebugSpell);
        }

        public void ProcessCommand(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            if (botHandler == null) throw new ArgumentNullException("botHandler");
            if (chat == null) throw new ArgumentNullException("chat");

            // Any debug command must come from the leader of the group
            if (chat.SenderGuid != botHandler.Group.Leader.Guid) return;

            // Any debug command must come in the form of a whisper
            if (chat.MessageType != Core.Constants.ChatMsg.Whisper) return;

            // Handle chat commands
            HandleCommands(botHandler, chat);
        }

        public void ToggleDebug(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            botHandler.Debug = !botHandler.Debug;
        }

        public void ToggleOnDebug(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            botHandler.Debug = true;
        }

        public void ToggleOffDebug(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            botHandler.Debug = false;
        }

        public void DebugSpell(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            if (chat.MessageTokenized.Length < 3)
            {
                botHandler.BotOwner.ChatWhisper("You must supply a spellid to debug", chat.SenderName);
                return;
            }

            var spellId = Convert.ToUInt32(chat.MessageTokenized[2]);
            var spell = SpellTable.Instance.getSpell(spellId);
            if (spell != null)
                Logger.Debug.Log(spell.DumpInfo());
        }
    }
}
