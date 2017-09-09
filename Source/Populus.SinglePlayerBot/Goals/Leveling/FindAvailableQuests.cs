using Populus.ActionManager.Actions;
using Populus.Core.Shared;
using System.Linq;

namespace Populus.SinglePlayerBot.Goals.Leveling
{
    /// <summary>
    /// This goal finds all available quests within the near vicinity and accepts them if the quest giver is within range
    /// </summary>
    internal class FindAvailableQuests : Goal
    {
        #region Declarations

        private const float MAX_DISTANCE = 30.0f;

        #endregion

        #region Properties

        internal override int Priority => 1000;

        #endregion

        #region Public Methods

        internal override bool ProcessGoal(SpBotHandler handler)
        {
            // Find a quest to accept that is in range of the bot
            var quest = handler.BotOwner.QuestGiverStatuses.FirstOrDefault(s => s.Status == Core.Constants.QuestGiverStatus.DIALOG_STATUS_AVAILABLE);
            if (quest != null)
            {
                var target = handler.BotOwner.GetWorldObjectByGuid(new WoWGuid(quest.Guid));
                if (handler.BotOwner.DistanceFrom(target.Position) <= MAX_DISTANCE)
                {
                    handler.BotOwner.Logger.Log($"Accepting a quest from {target.Name}");
                    handler.ActionQueue.Add(new MoveTowardsObject(handler.BotOwner, target, 1.0f));
                    handler.ActionQueue.Add(new AcceptQuests(handler.BotOwner, target));
                    return true;
                }   
            }

            return base.ProcessGoal(handler);
        }

        #endregion
    }
}
