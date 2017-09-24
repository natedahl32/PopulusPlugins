using Populus.Core.Shared;

namespace Populus.GroupManager
{
    public partial class Group
    {
        #region Event Delegates

        public delegate void GroupEventDelegate<T>(Group group, T eventArgs);

        #endregion

        #region Events

        /// <summary>
        /// Event fired when a member is added to a group
        /// </summary>
        public static event GroupEventDelegate<WoWGuid> MemberAddedToGroup;
        internal void OnMemberAddedToGroup(Group group, WoWGuid guid)
        {
            MemberAddedToGroup?.Invoke(group, guid);
        }

        #endregion
    }
}
