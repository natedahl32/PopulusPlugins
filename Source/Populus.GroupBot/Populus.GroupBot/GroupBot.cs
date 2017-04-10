using Populus.Core.Plugins;
using Populus.Core.World.Objects;
using Populus.Core.World.Objects.Events;
using static Populus.Core.World.Objects.Bot;
using GroupMgr = Populus.GroupManager.GroupManager;

namespace Populus.GroupBot
{
    /// <summary>
    /// Plugin that allows a bot to perform functions while in a group by responding to commands from the group leader. This plugin
    /// will also cause the bot to accept a group invite from anyone.
    /// 
    /// This plugin depends on:
    ///     ActionManager
    ///     CombatManager
    ///     GroupManager
    /// </summary>
    public class GroupBot : PluginBase
    {
        #region Declarations

        // handlers
        BotEventDelegate<GroupInviteEventArgs> inviteHandler = null;
        BotEventDelegate<ChatEventArgs> chatHandler = null;

        // static instance of our bot handlers collection
        private static WoWGuidCollection<GroupBotHandler> mBotHandlerCollection = new WoWGuidCollection<GroupBotHandler>();

        #endregion

        #region Constructors

        public GroupBot()
        {
            // On load we need to register all chat commands we have in this plugin
            GroupBotChatHandler.RegisterChatCommands();
        }

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

            // Incoming chat handler
            chatHandler = (bot, args) =>
            {
                var handler = mBotHandlerCollection.Get(bot.Guid);
                if (handler != null)
                    handler.ChatHandler.HandleChatMessage(args);
            };
            Bot.ChatMessageReceived += chatHandler;
        }

        public override void Unload()
        {
            // Remove handlers to avoid memory leaks
            Bot.ChatMessageReceived -= chatHandler;
            Bot.GroupInvite -= inviteHandler;
        }

        public override void OnTick(Bot bot, float deltaTime)
        {
            // If the bot is in a group, get the handler or add one if it does not have one yet
            if (GroupMgr.Groups.CharacterHasGroup(bot.Guid))
            {
                var handler = mBotHandlerCollection.Get(bot.Guid);
                if (handler == null)
                {
                    handler = new GroupBotHandler(bot);
                    mBotHandlerCollection.AddOrUpdate(bot.Guid, handler);
                }

                // Update the handler
                handler.Update(deltaTime);
            }
            else
            {
                var handler = mBotHandlerCollection.Remove(bot.Guid);
                if (handler != null)
                    handler.GroupDisbanded();
            }

            base.OnTick(bot, deltaTime);
        }

        #endregion
    }
}
