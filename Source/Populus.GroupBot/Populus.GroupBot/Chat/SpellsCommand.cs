using Populus.Core.DBC;
using Populus.Core.World.Objects.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Populus.GroupBot.Chat
{
    /// <summary>
    /// Chat command for a group bot that handles spell related tasks
    /// </summary>
    [ChatCommandKey("spells")]
    public class SpellsCommand : ChatCommand, IChatCommand
    {
        public SpellsCommand()
        {
            AddActionHandler(string.Empty, ListAllSpells);
        }

        public void ProcessCommand(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            if (botHandler == null) throw new ArgumentNullException("botHandler");
            if (chat == null) throw new ArgumentNullException("chat");

            // Handle chat commands
            HandleCommands(botHandler, chat);
        }

        /// <summary>
        /// List all spells the bot has
        /// </summary>
        private void ListAllSpells(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            var spellList = new List<SpellEntry>();
            foreach (var s in botHandler.BotOwner.Spells)
            {
                var spell = SpellTable.Instance.getSpell(s);
                if (spell != null && !spell.IsPassive)
                    spellList.Add(spell);
            }
            spellList = spellList.OrderBy(s => s.SpellName).ThenBy(s => s.RankValue).ToList();

            foreach (var s in spellList)
                botHandler.BotOwner.ChatSay($"{s.SpellName} (Rank {s.RankValue})");
        }
    }
}
