using Populus.Core.Plugins;
using Populus.Core.Shared;
using Populus.Core.Utils;
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
        #region Declarations

        private const float MAX_AGGRO_DISTANCE = 100f;

        #endregion

        #region Properties

        /// <summary>
        /// Gets all units that are currently on the threat/aggro list for the bot
        /// </summary>
        public IEnumerable<Unit> AggroUnits
        {
            get { return Data.Values.ToList(); }
        }

        /// <summary>
        /// Gets the first unit in the aggro list
        /// </summary>
        public Unit First
        {
            get { return Data.Values.FirstOrDefault(); }
        }

        /// <summary>
        /// Gets the number of mobs currently on the aggro list
        /// </summary>
        public int Count
        {
            get { return Data.Values.ToList().Count; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Removes all units from the aggro list that should no longer be there.
        /// - Units that are dead
        /// - Units that are no longer found in objects
        /// - Units that are too far away
        /// </summary>
        internal void RemoveUnits(Bot bot)
        {
            var copy = Data.Values.ToList();
            foreach (var mob in copy)
            {
                var obj = bot.GetUnitByGuid(mob.Guid);
                if (obj == null)
                    Remove(mob.Guid);
                else
                {
                    var dist = MathUtility.CalculateDistance(bot.Position, obj.Position);
                    if (mob.IsDead || dist > MAX_AGGRO_DISTANCE)
                        Remove(mob.Guid);
                }
            }
        }

        /// <summary>
        /// Clears the entire aggro list
        /// </summary>
        internal void Clear()
        {
            Data.Clear();
        }

        /// <summary>
        /// Gets whether or not the aggro list contains an object with the guid
        /// </summary>
        /// <param name="guid"></param>
        internal bool Contains(WoWGuid guid)
        {
            return Data.ContainsKey(guid);
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
