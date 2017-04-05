using Populus.Core.Plugins;
using Populus.Core.World.Objects;
using Populus.Core.World.Objects.Events;
using static Populus.Core.World.Objects.Bot;

namespace Populus.GroupBot
{
    /// <summary>
    /// Plugin that allows a bot to perform functions while in a group by responding to commands from the group leader. This plugin
    /// will also cause the bot to accept a group invite from anyone.
    /// 
    /// This plugin depends on:
    ///     GroupManager
    /// </summary>
    public class GroupBot : PluginBase
    {
        #region Declarations

        // handlers
        BotEventDelegate<GroupInviteEventArgs> inviteHandler = null;

        #endregion

        #region Properties

        public override string Name => "Group Bot";

        public override string Author => "Kazadoom";

        public override string Website => "";

        #endregion

        #region Public Methods

        public override void Initialize()
        {
            // Accept an invite from anyone
            inviteHandler = (bot, args) =>
            {
                bot.AcceptGroupInvite();
            };
            Bot.GroupInvite += inviteHandler;
        }

        public override void Unload()
        {
            // Remove handlers to avoid memory leaks
            Bot.GroupInvite -= inviteHandler;
        }

        #endregion
    }
}
