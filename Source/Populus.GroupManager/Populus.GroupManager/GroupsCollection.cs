using Populus.Core.Shared;
using Populus.Core.World.Objects;
using Populus.Core.World.Objects.Events;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        /// <summary>
        /// Gets whether or not the character has a group established
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool CharacterHasGroup(WoWGuid guid)
        {
            return mGroups.ContainsKey(guid);
        }

        /// <summary>
        /// Gets an existing group or creates a new group for a player
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        internal Group GetOrCreateGroupForPlayer(Bot bot, IEnumerable<GroupListEventArgs.GroupMemberListData> members)
        {
            Group group = null;
            if (!CharacterHasGroup(bot.Guid))
            {
                foreach (var groupMember in members)
                {
                    group = GetCharacterGroup(groupMember.Guid);
                    if (group != null)
                        break;
                }

                if (group == null)
                    group = new Group();

                // Add the group for this bot
                mGroups.TryAdd(bot.Guid, group);
            }
            else
            {
                group = GetCharacterGroup(bot.Guid);
            }
            return group;
        }

        #endregion
    }
}
