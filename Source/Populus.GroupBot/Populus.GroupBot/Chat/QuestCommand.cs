using System;
using Populus.Core.Utils;
using Populus.Core.World.Objects.Events;
using Populus.Core.World.Quest;
using System.Linq;
using Populus.Core.World.Objects;

namespace Populus.GroupBot.Chat
{
    /// <summary>
    /// Chat command for listing, accepting, dropping and turning in quests.
    /// </summary>
    [ChatCommandKey("quest")]
    public class QuestCommand : ChatCommand, IChatCommand
    {
        private const float MAX_QUESTGIVER_DISTANCE = 40.0f;

        public QuestCommand()
        {
            AddActionHandler(string.Empty, ListQuests);
            AddActionHandler("accept", AcceptQuests);
            AddActionHandler("complete", CompleteQuests);
        }

        public void ProcessCommand(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            if (botHandler == null) throw new ArgumentNullException("botHandler");
            if (chat == null) throw new ArgumentNullException("chat");

            // Any quest command must come from the leader of the group
            if (chat.SenderGuid != botHandler.Group.Leader.Guid) return;

            // Handle chat commands
            HandleCommands(botHandler, chat);
        }

        /// <summary>
        /// Accept quests
        /// </summary>
        /// <param name="botHandler"></param>
        /// <param name="args"></param>
        private void AcceptQuests(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            if (chat.MessageTokenized.Length > 2 && chat.MessageTokenized[2].ToLower() == "all")
            {
                // TODO: Find all quest givers in range and get any quests we can from them
                return;
            }

            // TODO: Handle an item that was linked in chat that we should accept a quest from

            // Get the leaders target
            var leaderObj = botHandler.BotOwner.GetPlayerByGuid(botHandler.Group.Leader.Guid);
            if (leaderObj == null) return;
            var target = botHandler.BotOwner.GetWorldObjectByGuid(leaderObj.TargetGuid);
            if (target == null)
            {
                botHandler.BotOwner.ChatParty("Target what you would like me to accept a quest from.");
                return;
            }

            // Get the object that we were told to loot.
            var questGiver = botHandler.BotOwner.GetWorldObjectByGuid(target.Guid);
            if (questGiver == null)
            {
                botHandler.BotOwner.ChatParty($"That target does not exist, I can't accept quests from that that.");
                return;
            }

            // If the target is not a questgiver, we can't accept quests from them
            if ((target is Unit && !((target as Unit).IsQuestGiver)) ||
                (target is GameObject && !((target as GameObject).IsQuestGiver)))
            {
                botHandler.BotOwner.ChatParty($"That target is not a quest giver.");
                return;
            }

            // Make sure we are close enough to the object to accept quests from them
            float dist = botHandler.BotOwner.DistanceFrom(questGiver.Position);
            if (dist > MAX_QUESTGIVER_DISTANCE)
            {
                botHandler.BotOwner.ChatParty($"The questgiver is too far away. I can only accept quests within {MAX_QUESTGIVER_DISTANCE} yards. The questgiver is {dist.ToNearestInt()} yards away.");
                return;
            }

            // If the questgiver is dead, we can't interact withit
            if (target is Unit && (target as Unit).IsDead)
            {
                botHandler.BotOwner.ChatParty($"The questgiver is dead, I can't accept quests from them.");
                return;
            }

            // Accept quests from the quest giver
            AcceptQuestFromObject(botHandler, target);
        }

        /// <summary>
        /// Complete quests
        /// </summary>
        /// <param name="botHandler"></param>
        /// <param name="args"></param>
        private void CompleteQuests(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            if (chat.MessageTokenized.Length > 2 && chat.MessageTokenized[2].ToLower() == "all")
            {
                // TODO: Find all quest givers in range and complete any quests we can from them
                return;
            }

            // Get the leaders target
            var leaderObj = botHandler.BotOwner.GetPlayerByGuid(botHandler.Group.Leader.Guid);
            if (leaderObj == null) return;
            var target = botHandler.BotOwner.GetWorldObjectByGuid(leaderObj.TargetGuid);
            if (target == null)
            {
                botHandler.BotOwner.ChatParty("Target what you would like me to complete a quest from.");
                return;
            }

            // Get the object that we were told to loot.
            var questGiver = botHandler.BotOwner.GetWorldObjectByGuid(target.Guid);
            if (questGiver == null)
            {
                botHandler.BotOwner.ChatParty($"That target does not exist, I can't complete quests from that that.");
                return;
            }

            // If the target is not a questgiver, we can't accept quests from them
            if ((target is Unit && !((target as Unit).IsQuestGiver)) ||
                (target is GameObject && !((target as GameObject).IsQuestGiver)))
            {
                botHandler.BotOwner.ChatParty($"That target is not a quest giver.");
                return;
            }

            // Make sure we are close enough to the object to accept quests from them
            float dist = botHandler.BotOwner.DistanceFrom(questGiver.Position);
            if (dist > MAX_QUESTGIVER_DISTANCE)
            {
                botHandler.BotOwner.ChatParty($"The questgiver is too far away. I can only complete quests within {MAX_QUESTGIVER_DISTANCE} yards. The questgiver is {dist.ToNearestInt()} yards away.");
                return;
            }

            // If the questgiver is dead, we can't interact withit
            if (target is Unit && (target as Unit).IsDead)
            {
                botHandler.BotOwner.ChatParty($"The questgiver is dead, I can't complete quests from them.");
                return;
            }

            // Accept quests from the quest giver
            AcceptQuestFromObject(botHandler, target);
        }

        /// <summary>
        /// Lists all quests the bot currently has
        /// </summary>
        /// <param name="botHandler"></param>
        private void ListQuests(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            if (botHandler.BotOwner.Quests.Count() == 0)
            {
                botHandler.BotOwner.ChatParty("I no quests in my log.");
                return;
            }

            botHandler.BotOwner.ChatParty("I have the following quests in my log:");
            foreach (var questLogItem in botHandler.BotOwner.Quests)
            {
                var quest = QuestManager.Instance.Get(questLogItem.QuestId);
                if (quest != null)
                    botHandler.BotOwner.ChatParty($"[{quest.QuestLevel}] {quest.QuestName} {questLogItem.QuestStatusDescription}");
            }
        }

        /// <summary>
        /// Starts the process to accept a quest from a world object
        /// </summary>
        /// <param name="botHandler"></param>
        /// <param name="wo"></param>
        private void AcceptQuestFromObject(GroupBotHandler botHandler, WorldObject wo)
        {
            //var actionQueue = ActionMgr.GetActionQueue(botHandler.BotOwner.Guid);
            //actionQueue.Add(new MoveTowardsObject(botHandler.BotOwner, wo, 1.0f));
            //actionQueue.Add(new AcceptQuests(botHandler.BotOwner, wo));
        }

        /// <summary>
        /// Starts the process to complete a quest from a world object
        /// </summary>
        /// <param name="botHandler"></param>
        /// <param name="wo"></param>
        private void CompleteQuestFromObject(GroupBotHandler botHandler, WorldObject wo)
        {
            //var actionQueue = ActionMgr.GetActionQueue(botHandler.BotOwner.Guid);
            //actionQueue.Add(new MoveTowardsObject(botHandler.BotOwner, wo, 1.0f));
            //actionQueue.Add(new AcceptQuests(botHandler.BotOwner, wo));
        }
    }
}
