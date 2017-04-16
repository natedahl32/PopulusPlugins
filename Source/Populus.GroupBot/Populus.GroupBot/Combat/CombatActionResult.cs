namespace Populus.GroupBot.Combat
{
    public enum CombatActionResult
    {
        NO_ACTION_OK,               // Used when no action is taken, but another action could be used
        NO_ACTION_ERROR,
        NO_ACTION_WAIT,             // Used when we should take no action because we are waiting
        ACTION_OK,                  // Used an action
        ACTION_OK_CONTINUE_FIRST    // Used an action but continue with the first combat action (same as ACTION_OK when used for non-first combat action)
    }
}
