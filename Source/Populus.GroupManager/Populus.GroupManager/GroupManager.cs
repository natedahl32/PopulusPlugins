using Populus.Core.Plugins;
using Populus.Core.World.Objects;
using Populus.Core.World.Objects.Events;
using System.Linq;
using static Populus.Core.World.Objects.Bot;

namespace Populus.GroupManager
{
    public class GroupManager : PluginBase
    {
        #region Declarations

        // handlers
        BotEventDelegate<MovementUpdateEventArgs> objectMovementHandler = null;
        BotEventDelegate<GroupInviteEventArgs> inviteHandler = null;
        BotEventDelegate<GroupMemberUpdateEventArgs> groupMemberUpdateHandler = null;
        BotEventDelegate<GroupListEventArgs> groupListHandler = null;
        BotEventDelegate<GroupLeaderChangeEventArgs> groupLeaderChangeHandler = null;
        EmptyEventDelegate disbandHandler = null;

        // static instance of our groups collection
        private static GroupsCollection mGroupsCollection = new GroupsCollection();

        #endregion

        #region Properties

        /// <summary>
        /// Plugin name
        /// </summary>
        public override string Name => "Group Manager";

        /// <summary>
        /// Plugin author
        /// </summary>
        public override string Author => "Kazadoom";

        /// <summary>
        /// Plugin website
        /// </summary>
        public override string Website => "";

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

        public override void Initialize()
        {
            // TODO: Based on config value. This actually might not even belong in this plugin????
            // Accept an invite from anyone
            inviteHandler = (bot, args) =>
            {
                bot.AcceptGroupInvite();
            };
            Bot.GroupInvite += inviteHandler;


            // Handle any object movement
            objectMovementHandler = (bot, args) =>
            {
                // Get this bots group and if the object belongs to the group update it's position
                Group group = mGroupsCollection.GetCharacterGroup(bot.Guid);
                if (group != null)
                {
                    var member = group.GetMember(args.ObjectGuid);
                    if (member != null)
                        member.UpdatePosition(args.Position);
                }
            };
            Bot.ObjectMovement += objectMovementHandler;

            // Handle group member updates
            groupMemberUpdateHandler = (bot, args) =>
            {
                // This updates a single member of the group. It is assumed the bot sending this event already has a group.
                Group group = mGroupsCollection.GetCharacterGroup(bot.Guid);
                if (group != null)
                    group.UpdateGroupMember(args);
            };
            Bot.GroupMemberUpdate += groupMemberUpdateHandler;


            // Handle group member list update
            groupListHandler = (bot, args) =>
            {
                // If the list contains only one group member and that member is the bot, disband this bots group
                var memberList = args.GroupMembersData.ToList();
                if ((args.MemberCount == 1 && memberList[0].Guid.GetOldGuid() == bot.Guid.GetOldGuid()) || args.MemberCount == 0)
                {
                    mGroupsCollection.RemoveGroupForPlayer(bot.Guid);
                    return;
                }

                // If this bot does not have a group assigned, either create a new group if one doesn't exist yet
                // or find the group reference from one of the other group members. This is how we keep group references the same
                // for each bot.
                Group group = mGroupsCollection.GetOrCreateGroupForPlayer(bot, args.GroupMembersData);
                group.UpdateFromGroupList(args);

                // Follow the first member of the group that isn't us
                var firstMember = group.Members.Where(m => m.Guid.GetOldGuid() != bot.Guid.GetOldGuid()).FirstOrDefault();
                if (firstMember != null)
                    bot.SetFollow(firstMember.Guid);
            };
            Bot.GroupListUpdate += groupListHandler;

            // Handle group disbanding or being removed from group
            disbandHandler = bot =>
            {
                // We were removed from our group, empty our group instance
                mGroupsCollection.RemoveGroupForPlayer(bot.Guid);
            };
            Bot.GroupDisbanded += disbandHandler;
            Bot.GroupUninvite += disbandHandler;

            // Handle group leader changing
            groupLeaderChangeHandler = (bot, args) =>
            {
                // Get this bots group and update the leader
                Group group = mGroupsCollection.GetCharacterGroup(bot.Guid);
                if (group != null)
                    group.ChangeLeader(args.LeaderName);
            };
            Bot.GroupLeaderChange += groupLeaderChangeHandler;
        }

        public override void Unload()
        {
            // Remove handlers to avoid memory leaks
            Bot.GroupLeaderChange -= groupLeaderChangeHandler;
            Bot.GroupDisbanded -= disbandHandler;
            Bot.GroupUninvite -= disbandHandler;
            Bot.GroupListUpdate -= groupListHandler;
            Bot.GroupMemberUpdate -= groupMemberUpdateHandler;
            Bot.ObjectMovement -= objectMovementHandler;
            Bot.GroupInvite -= inviteHandler;
        }

        #endregion
    }
}
