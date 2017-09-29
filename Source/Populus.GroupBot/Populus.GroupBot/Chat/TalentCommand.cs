using System;
using Populus.Core.World.Objects.Events;
using Populus.GroupBot.Talents;
using System.Linq;

namespace Populus.GroupBot.Chat
{
    [ChatCommandKey("talent")]
    public class TalentCommand : IChatCommand
    {
        public void ProcessCommand(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            if (botHandler == null) throw new ArgumentNullException("botHandler");
            if (chat == null) throw new ArgumentNullException("chat");

            // Any talent command must come from the leader of the group
            if (chat.SenderGuid != botHandler.Group.Leader.Guid) return;

            // TODO: Maybe add subcommand objects?

            // Assign talent spec
            if (chat.MessageTokenized.Length > 1 && chat.MessageTokenized[1].ToLower() == "assign")
            {
                AssignTalentSpec(botHandler, chat);
                return;
            }

            ListTalents(botHandler);
        }

        /// <summary>
        /// Assigns a talent spec to the bot
        /// </summary>
        /// <param name="botHandler"></param>
        /// <param name="chat"></param>
        private void AssignTalentSpec(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            // Get the talent spec number assigned
            int index = -1;
            if (int.TryParse(chat.MessageTokenized[2], out index))
            {
                if (index > 0)
                {
                    var specsAvailable = TalentManager.Instance.TalentSpecsByClass(botHandler.BotOwner.Class).ToList();
                    if (index <= specsAvailable.Count)
                    {
                        var specToAssign = specsAvailable[index - 1]; // Our index we get from chat is 1-based
                        botHandler.BotData.SpecName = specToAssign.Name;
                        return;
                    }
                }
            }

            botHandler.BotOwner.ChatParty("That isn't a valid talent spec.");
        }

        /// <summary>
        /// Lists all talent specs available for this bots class. Also identifies the talent spec the bot is currently using.
        /// </summary>
        /// <param name="botHandler"></param>
        private void ListTalents(GroupBotHandler botHandler)
        {
            var currentTalentSpec = botHandler.CurrentTalentSpec;
            botHandler.BotOwner.ChatParty($"I have {botHandler.BotOwner.FreeTalentPoints} unspent talent points and my current talent spec is set to: {(currentTalentSpec == null ? "NONE" : currentTalentSpec.Name)}");
            botHandler.BotOwner.ChatParty("The talent specs available for my class are:");
            var specsAvailable = TalentManager.Instance.TalentSpecsByClass(botHandler.BotOwner.Class).ToList();
            if (specsAvailable.Count == 0)
                botHandler.BotOwner.ChatParty("none");
            else
            {
                for (int i = 1; i <= specsAvailable.Count; i++)
                    botHandler.BotOwner.ChatParty($"{i}) {specsAvailable[i-1].Name}");
            }
        }
    }
}
