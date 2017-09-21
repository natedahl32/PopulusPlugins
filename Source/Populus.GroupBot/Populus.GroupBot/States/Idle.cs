using Populus.GroupBot.Triggers;

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
            // If we are in combat, transition to the combat state
            if (handler.CombatState.IsInCombat)
            {
                handler.TriggerState(StateTriggers.Combat);
                return;
            }

            // Check for out of combat actions to be performed
            handler.CombatHandler.OutOfCombatUpdate(deltaTime);

            // Follow the group leader if we aren't already and we can
            //handler.FollowGroupLeader();
        }
    }
}
