using Populus.ActionManager.Actions;
using Populus.Core.Shared;
using Populus.Core.World.Objects;
using System.Linq;

namespace Populus.SinglePlayerBot.States
{
    public class LevelingState : State
    {
        #region Constructors

        public LevelingState() : base("Leveling")
        {
        }

        #endregion

        #region Public Methods

        internal override void OnTick(SpBotHandler handler)
        {
            // If we have actions to process, finish those first
            if (!handler.ActionQueue.IsEmpty) return;

            // Find a quest to accept
            // TODO: Must be in range
            var quest = handler.BotOwner.QuestGiverStatuses.FirstOrDefault(s => s.Status == Core.Constants.QuestGiverStatus.DIALOG_STATUS_AVAILABLE);
            if (quest != null)
            {
                var target = handler.BotOwner.GetWorldObjectByGuid(new WoWGuid(quest.Guid));
                handler.BotOwner.Logger.Log($"Accepting a quest from {target.Name}");
                handler.ActionQueue.Add(new MoveTowardsObject(handler.BotOwner, target, 1.0f));
                handler.ActionQueue.Add(new AcceptQuests(handler.BotOwner, target));
            }
        }

        #endregion
    }
}
