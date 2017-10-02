namespace Populus.GroupBot.States
{
    public class Combat : State
    {
        #region Singleton

        private static readonly Combat instance = new Combat();

        static Combat() { }

        private Combat() { }

        public static Combat Instance
        {
            get { return instance; }
        }

        #endregion

        public override void Update(GroupBotHandler handler, float deltaTime)
        {
            // If we are dead, trigger dead state
            if (handler.BotOwner.IsDead)
            {
                handler.TriggerState(Triggers.StateTriggers.Died);
                return;
            }

            // If we are no longer in combat, trigger the idle state
            if (!handler.CombatState.IsInCombat)
            {
                handler.CombatHandler.ResetCombatState();
                handler.TriggerState(Triggers.StateTriggers.Idle);
                return;
            }

            // Handle combat actions
            handler.CombatHandler.CombatUpdate(deltaTime);
        }
    }
}
