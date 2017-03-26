using Populus.Core.Constants;
using Populus.Core.Shared;
using Populus.Core.World.Objects.Events;
using System.Collections.Generic;
using System.Linq;

namespace Populus.GroupManager
{
    /// <summary>
    /// Represents a player group in the world
    /// </summary>
    public class Group
    {
        #region Declarations

        private object mGroupMembersLock = new object();
        private List<GroupMember> mGroupMembers;
        private WoWGuid mLeaderGuid = null;
        private WoWGuid mMasterLooterGuid = null;
        private LootMethod? mLootMethod = null;
        private ItemQualities? mLootThreshold = null;
        private GroupType mGroupType;

        #endregion

        #region Constructors

        public Group()
        {
            mGroupMembers = new List<GroupMember>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of members in the group
        /// </summary>
        public byte MemberCount
        {
            get { return (byte)mGroupMembers.Count; }
        }

        /// <summary>
        /// Gets a list of all members in the group
        /// </summary>
        public IEnumerable<GroupMember> Members { get { return mGroupMembers.ToList(); } }

        /// <summary>
        /// Gets the group member that is the leader of the group
        /// </summary>
        public GroupMember Leader
        {
            get
            {
                if (mLeaderGuid == null) return null; // Possible?
                return mGroupMembers.ToList().Where(m => m.Guid.GetOldGuid() == mLeaderGuid.GetOldGuid()).SingleOrDefault();
            }
        }

        /// <summary>
        /// Gets the group member that is the master looter of the group
        /// </summary>
        public GroupMember MasterLooter
        {
            get
            {
                if (mMasterLooterGuid == null) return null;
                return mGroupMembers.ToList().Where(m => m.Guid.GetOldGuid() == mMasterLooterGuid.GetOldGuid()).SingleOrDefault();
            }
        }

        /// <summary>
        /// Gets the loot method currentyl set for the group, if any
        /// </summary>
        public LootMethod? LootMethod
        {
            get
            {
                return mLootMethod;
            }
        }

        /// <summary>
        /// Gets the loot threshold set for the group, if any
        /// </summary>
        public ItemQualities? LootThreshold
        {
            get { return mLootThreshold; }
        }

        /// <summary>
        /// Gets the type of group this is
        /// </summary>
        public GroupType GroupType
        {
            get { return mGroupType; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets a member of the group by their guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public GroupMember GetMember(WoWGuid guid)
        {
            return mGroupMembers.ToList().Where(m => m.Guid.GetOldGuid() == guid.GetOldGuid()).SingleOrDefault();
        }

        /// <summary>
        /// Adds or updates a group member
        /// </summary>
        internal void AddOrUpdateGroupMember()
        {
            lock (mGroupMembersLock)
            {
                // Add stuff
            }
        }

        /// <summary>
        /// Removes the member from the group
        /// </summary>
        /// <param name="guid">Guid of member to remove</param>
        internal void RemoveGroupMember(WoWGuid guid)
        {
            lock (mGroupMembersLock)
            {
                var member = mGroupMembers.Where(m => m.Guid.GetOldGuid() == guid.GetOldGuid()).SingleOrDefault();
                if (member != null)
                    mGroupMembers.Remove(member);
            }
        }

        /// <summary>
        /// Changes the group leader to the character with the given name
        /// </summary>
        /// <param name="leaderName"></param>
        internal void ChangeLeader(string leaderName)
        {
            var leader = mGroupMembers.ToList().Where(m => m.Name.ToLower() == leaderName.ToLower()).SingleOrDefault();
            if (leader != null)
                mLeaderGuid = leader.Guid;
        }

        /// <summary>
        /// Updates group data from the group list event args
        /// </summary>
        /// <param name="args"></param>
        internal void UpdateFromGroupList(GroupListEventArgs args)
        {
            // Update group information
            mLeaderGuid = args.LeaderGuid;
            mGroupType = args.GroupType;
            if (args.MasterLooterGuid != null) mMasterLooterGuid = args.MasterLooterGuid;
            if (args.LootMethod.HasValue) mLootMethod = args.LootMethod.Value;
            if (args.LootThreshold.HasValue) mLootThreshold = args.LootThreshold.Value;

            // Add or update members
            foreach (var memberData in args.GroupMembersData)
            {
                // Get member in the group if they exist
                var member = Members.Where(m => m.Guid.GetOldGuid() == memberData.Guid.GetOldGuid()).SingleOrDefault();
                if (member == null)
                    member = new GroupMember();

                // Update the group member
                member.Update(memberData);
            }
        }

        /// <summary>
        /// Updates a group members data
        /// </summary>
        /// <param name="args"></param>
        internal void UpdateGroupMember(GroupMemberUpdateEventArgs args)
        {
            var member = mGroupMembers.ToList().Where(m => m.Guid.GetOldGuid() == args.Guid.GetOldGuid()).SingleOrDefault();
            if (member != null)
                member.Update(args);
        }

        #endregion
    }
}
