using Populus.Core.Utils;

namespace Populus.GroupBot.States
{
    public class Teleport : State
    {
        #region Singleton

        private static readonly Teleport instance = new Teleport();

        static Teleport() { }

        private Teleport() { }

        public static Teleport Instance
        {
            get { return instance; }
        }

        #endregion

        public override void Update(GroupBotHandler handler, float deltaTime)
        {
            // If we are within 5 yards of our target position, change back to idle
            var distance = Populus.Core.Utils.MathUtility.CalculateDistance(handler.BotOwner.Position, handler.TeleportingTo);
            if (distance <= 5.0f)
                handler.TriggerState(Triggers.StateTriggers.Idle);
        }
    }
}
