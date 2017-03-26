using Populus.Core.Shared;
using System.Collections.Concurrent;
using System.Linq;

namespace Populus.GroupManager
{
    public class GroupsCollection
    {
        #region Declarations

        // Thread safe dictionary to store groups based on character guid. Group references are shared across characters (if Character A and Character B are in the same group, they will point to the same group reference)
        private ConcurrentDictionary<WoWGuid, Group> mGroups;

        #endregion

        #region Constructors

        internal GroupsCollection()
        {
            mGroups = new ConcurrentDictionary<WoWGuid, Group>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the group this character belongs to
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Group GetCharacterGroup(WoWGuid guid)
        {
            Group group = null;
            mGroups.TryGetValue(guid, out group);
            return group;
        }

        /// <summary>
        /// Gets the group member by guid
        /// </summary>
        /// <param name="guid">Guid of group member to retrieve</param>
        /// <returns></returns>
        public GroupMember GetGroupMember(WoWGuid guid)
        {
            // Since the same group should be referenced in all intances and a group member cannot belong to more than one group. We just need to pull the first instance of the group member
            return mGroups.Values.SelectMany(m => m.Members).Where(m => m.Guid.GetOldGuid() == guid.GetOldGuid()).FirstOrDefault();
        }

        #endregion
    }
}
