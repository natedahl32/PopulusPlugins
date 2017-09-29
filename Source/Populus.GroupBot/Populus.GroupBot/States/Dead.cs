namespace Populus.GroupBot.States
{
    public class Dead : State
    {
        #region Singleton

        private static readonly Dead instance = new Dead();

        static Dead() { }

        private Dead() { }

        public static Dead Instance
        {
            get { return instance; }
        }

        #endregion

        public override void Update(GroupBotHandler handler, float deltaTime)
        {
            // If we are no longer dead, go back to idle
            if (!handler.BotOwner.IsDead)
                handler.TriggerState(Triggers.StateTriggers.Resurrected);
        }
    }
}
