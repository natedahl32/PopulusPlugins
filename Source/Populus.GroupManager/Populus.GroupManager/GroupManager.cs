using Populus.Core.Plugins;
using Populus.Core.World.Objects;
using Populus.Core.World.Objects.Events;
using static Populus.Core.World.Objects.Bot;

namespace Populus.GroupManager
{
    public class GroupManager : IPlugin
    {
        #region Declarations

        // handlers
        BotEventDelegate<GroupInviteEventArgs> inviteHandler = null;
        BotEventDelegate<GroupMemberUpdateEventArgs> groupMemberUpdateHandler = null;
        BotEventDelegate<GroupListEventArgs> groupListHandler = null;

        // static instance of our groups collection
        private static GroupsCollection mGroupsCollection = new GroupsCollection();

        #endregion

        #region Properties

        /// <summary>
        /// Plugin name
        /// </summary>
        public string Name => "Group Manager";

        /// <summary>
        /// Plugin author
        /// </summary>
        public string Author => "Kazadoom";

        /// <summary>
        /// Plugin website
        /// </summary>
        public string Website => "";

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection that stores all known groups
        /// </summary>
        public GroupsCollection Groups
        {
            get { return mGroupsCollection; }
        }

        #endregion

        #region Public Methods

        public void Initialize()
        {
            // TODO: Based on config value. This actually might not even belong in this plugin????
            // Accept an invite from anyone
            inviteHandler = (bot, args) =>
            {
                bot.AcceptGroupInvite();
            };
            Bot.GroupInvite += inviteHandler;


            // Handle group member updates
            groupMemberUpdateHandler = (bot, args) =>
            {

            };
            Bot.GroupMemberUpdate += groupMemberUpdateHandler;


            // Handle group member list update
            groupListHandler = (bot, args) =>
            {
                // If this bot does not have a group assigned, either create a new group if one doesn't exist yet
                // or find the group reference from one of the other group members. This is how we keep group references the same
                // for each bot.
                Group group = mGroupsCollection.GetOrCreateGroupForPlayer(bot, args.GroupMembersData);
                group.UpdateFromGroupList(args);
            };
            Bot.GroupListUpdate += groupListHandler;
        }

        public void Tick(Bot bot)
        {

        }

        public void Unload()
        {
            // Remove handlers to avoid memory leaks
            Bot.GroupListUpdate -= groupListHandler;
            Bot.GroupMemberUpdate -= groupMemberUpdateHandler;
            Bot.GroupInvite -= inviteHandler;
        }

        #endregion
    }
}
