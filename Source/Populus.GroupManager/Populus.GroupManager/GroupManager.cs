using Populus.Core.Plugins;
using Populus.Core.World.Objects;

namespace Populus.GroupManager
{
    public class GroupManager : IPlugin
    {
        #region Declarations

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
            // TODO: Based on config value
            // Accept an invite from anyone
            Bot.GroupInvite += (bot, args) =>
            {
                bot.AcceptGroupInvite();
            };

            // Handle group member updates
            Bot.GroupMemberUpdate += (bot, args) =>
            {

            };
        }

        public void Tick(Bot bot)
        {

        }

        public void Unload()
        {

        }

        #endregion
    }
}
