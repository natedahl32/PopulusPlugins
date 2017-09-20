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
    public class SpellsCommand : IChatCommand
    {
        public void ProcessCommand(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            if (botHandler == null) throw new ArgumentNullException("botHandler");
            if (chat == null) throw new ArgumentNullException("chat");

            // Turn follow off
            if (chat.MessageTokenized.Length > 1 && chat.MessageTokenized[1].ToLower() == "off")
            {
                botHandler.StopFollow();
                return;
            }

            // Default action is to list all spells the bot currently has
            ListAllSpells(botHandler);
        }

        /// <summary>
        /// List all spells the bot has
        /// </summary>
        private void ListAllSpells(GroupBotHandler botHandler)
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
