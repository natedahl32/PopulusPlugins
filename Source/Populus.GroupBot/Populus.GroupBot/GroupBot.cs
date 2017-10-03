using Populus.Core.Plugins;
using Populus.Core.Shared;
using Populus.Core.World.Objects;
using Populus.Core.World.Objects.Events;
using Populus.GroupBot.Talents;
using Populus.GroupManager;
using System;
using System.Linq;
using System.Timers;
using static Populus.Core.World.Objects.Bot;
using static Populus.GroupManager.Group;
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
        EmptyEventDelegate groupUninvite = null;
        BotEventDelegate<ChatEventArgs> chatHandler = null;
        BotEventDelegate<uint> levelUpHandler = null;
        EmptyEventDelegate loginHandler = null;
        BotEventDelegate<InitialSpellsArgs> initialSpellsHandler = null;
        BotEventDelegate<uint> spellLearnedHandler = null;
        BotEventDelegate<uint> spellRemovedHandler = null;
        BotEventDelegate<CombatBadFacingArgs> notFacingTargetHandler = null;
        BotEventDelegate<CombatAttackStopEventArgs> attackStopHandler = null;
        BotEventDelegate<CombatAttackUpdateArgs> attackUpdateHandler = null;
        BotEventDelegate<SpellCastCompleteArgs> spellCastCompleteHandler = null;
        BotEventDelegate<LootRollStartArgs> lootRollStartHandler = null;

        // static instance of our bot handlers collection
        private static WoWGuidCollection<GroupBotHandler> mBotHandlerCollection = new WoWGuidCollection<GroupBotHandler>();

        #endregion

        #region Constructors

        public GroupBot()
        {
            // On load we need to register all chat commands we have in this plugin
            GroupBotChatHandler.RegisterChatCommands();
            // Load all talent specs that have been created as well
            TalentManager.Instance.LoadTalents();
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

            // Remove the follow target when we are uninvited from a group
            groupUninvite = (bot) =>
            {
                bot.RemoveFollow();
            };
            Bot.GroupUninvite += groupUninvite;

            // Incoming chat handler
            chatHandler = (bot, args) =>
            {
                var handler = mBotHandlerCollection.Get(bot.Guid);
                if (handler != null)
                    handler.ChatHandler.HandleChatMessage(args);
            };
            Bot.ChatMessageReceived += chatHandler;

            // Level up handler
            levelUpHandler = (bot, level) =>
            {
                var handler = mBotHandlerCollection.Get(bot.Guid);
                if (handler != null)
                    handler.LearnLevelSpells(level);
            };
            Bot.LevelUp += levelUpHandler;

            // Login handler
            loginHandler = bot =>
            {
                var timer = new Timer()
                {
                    AutoReset = false,
                    Interval = 3000,
                    Enabled = true
                };
                // Elapsed handler for timer
                timer.Elapsed += (s, e) =>
                {
                    if (GroupMgr.Groups.CharacterHasGroup(bot.Guid))
                    {
                        var handler = mBotHandlerCollection.Get(bot.Guid);
                        if (handler != null)
                        {
                            if (!handler.Group.Members.Any(m => m.IsOnline))
                                bot.LeaveGroup();
                        }
                    }
                };
                // Start the timer
                timer.Start();
            };
            Bot.LoggedIn += loginHandler;

            // Handles spells being learned and unlearned
            initialSpellsHandler = (bot, args) =>
            {
                var handler = mBotHandlerCollection.Get(bot.Guid);
                if (handler != null)
                    handler.CombatHandler.InitializeSpells();
            };
            Bot.InitialSpells += initialSpellsHandler;

            spellLearnedHandler = (bot, args) =>
            {
                var handler = mBotHandlerCollection.Get(bot.Guid);
                if (handler != null)
                    handler.CombatHandler.InitializeSpells();
            };
            Bot.LearnedSpell += spellLearnedHandler;

            spellRemovedHandler = (bot, args) =>
            {
                var handler = mBotHandlerCollection.Get(bot.Guid);
                if (handler != null)
                    handler.CombatHandler.InitializeSpells();
            };
            Bot.RemovedSpell += spellRemovedHandler;

            // Handles attack stop event
            attackStopHandler = (bot, args) =>
            {
                //var handler = mBotHandlerCollection.Get(bot.Guid);
                //if (handler != null)
                //    handler.CombatHandler.StopAttack();
            };
            Bot.AttackStopped += attackStopHandler;

            // Handles attack update event
            attackUpdateHandler = (bot, args) =>
            {
                var handler = mBotHandlerCollection.Get(bot.Guid);
                if (handler != null)
                    handler.CombatHandler.CombatAttackUpdate(bot, args);
            };
            Bot.CombatAttackUpdate += attackUpdateHandler;

            // Handle spell cast update event
            spellCastCompleteHandler = (bot, args) =>
            {
                //var handler = mBotHandlerCollection.Get(bot.Guid);
                //if (handler != null)
                //    handler.CombatHandler.SpellCastCompleteUpdate(args);
            };
            Bot.SpellCastCompleted += spellCastCompleteHandler;

            // Handle not facing target while attacking
            notFacingTargetHandler = (bot, args) =>
            {
                var handler = mBotHandlerCollection.Get(bot.Guid);
                if (handler != null)
                    handler.CombatHandler.NotFacingTarget(args.CorrectAngle);
            };
            Bot.MeleeNotFacingTowardsTarget += notFacingTargetHandler;

            // Handle loot roll start
            lootRollStartHandler = (bot, args) =>
            {
                var handler = mBotHandlerCollection.Get(bot.Guid);
                if (handler != null)
                    handler.HandleLootRoll(args);
            };
            Bot.LootRollStart += lootRollStartHandler;
        }

        public override void Unload()
        {
            // Save bot data
            foreach (var handler in mBotHandlerCollection.GetAll())
                handler.SaveBotData();

            // Remove handlers to avoid memory leaks
            Bot.ChatMessageReceived -= chatHandler;
            Bot.GroupInvite -= inviteHandler;
            Bot.GroupUninvite -= groupUninvite;
            Bot.LevelUp -= levelUpHandler;
            Bot.LoggedIn -= loginHandler;
            Bot.InitialSpells -= initialSpellsHandler;
            Bot.LearnedSpell -= spellLearnedHandler;
            Bot.RemovedSpell -= spellRemovedHandler;
            Bot.AttackStopped -= attackStopHandler;
            Bot.CombatAttackUpdate -= attackUpdateHandler;
            Bot.SpellCastCompleted -= spellCastCompleteHandler;
            Bot.MeleeNotFacingTowardsTarget -= notFacingTargetHandler;
            Bot.LootRollStart -= lootRollStartHandler;
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
                    handler.LearnLevelSpells(bot.Level);
                    handler.AssignGroupRole();
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
