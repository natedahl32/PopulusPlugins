using System;
using Populus.Core.Plugins;

namespace Populus.ActionManager
{
    /// <summary>
    /// This class manages complex actions for bots using an action queue. Queued ations run one at a time and the next action
    /// in the queue is started when the previous action completes. 
    /// </summary>
    public class ActionManager : PluginBase
    {
        #region Properties

        public override string Name => "Action Manager";

        public override string Author => "Kazadoom";

        public override string Website => "";

        #endregion
    }
}
