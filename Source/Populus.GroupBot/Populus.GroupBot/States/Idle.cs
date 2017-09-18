using Populus.GroupBot.Combat;

namespace Populus.GroupBot.States
{
    public class Idle : State
    {
        #region Singleton

        private static readonly Idle instance = new Idle();

        static Idle() { }

        private Idle() { }

        public static Idle Instance
        {
            get { return instance; }
        }

        #endregion

        public override void Update(GroupBotHandler handler, float deltaTime)
        {
            // Check for out of combat actions to be performed
            if (handler.CombatHandler.DoOutOfCombatAction() == CombatActionResult.ACTION_OK)
                return;

            // Follow the group leader if we aren't already and we can
            handler.FollowGroupLeader();
        }
    }
}
