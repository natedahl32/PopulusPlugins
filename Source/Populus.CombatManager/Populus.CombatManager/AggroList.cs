using Populus.Core.Plugins;
using Populus.Core.World.Objects;
using System.Collections.Generic;
using System.Linq;

namespace Populus.CombatManager
{
    /// <summary>
    /// Maintains a list of units that are currently on a threat list for a bot
    /// </summary>
    public class AggroList : WoWGuidCollection<Unit>
    {
        #region Properties

        /// <summary>
        /// Gets all units that are currently on the threat/aggro list for the bot
        /// </summary>
        public IEnumerable<Unit> AggroUnits
        {
            get { return Data.Values.ToList(); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Removes all dead units from the aggro list
        /// </summary>
        internal void RemoveDeadUnits()
        {
            var copy = Data.Values.ToList();
            foreach (var mob in copy)
                if (mob.IsDead)
                    Remove(mob.Guid);
        }

        /// <summary>
        /// Clears the entire aggro list
        /// </summary>
        internal void Clear()
        {
            Data.Clear();
        }

        #endregion
    }
}
