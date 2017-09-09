using Populus.SinglePlayerBot.Goals.Leveling;

namespace Populus.SinglePlayerBot.States
{
    internal class LevelingState : State
    {
        #region Constructors

        public LevelingState() : base("Leveling")
        {
            AddGoal(new FindAvailableQuests());
            AddGoal(new FinishCompletedQuests());
        }

        #endregion
    }
}
