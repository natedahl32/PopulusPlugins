using Populus.Core.Constants;
using Populus.Core.World.Objects;
using Populus.GroupBot.Combat.Druid;
using Populus.GroupBot.Combat.Hunter;
using Populus.GroupBot.Combat.Mage;
using Populus.GroupBot.Combat.Paladin;
using Populus.GroupBot.Combat.Priest;
using Populus.GroupBot.Combat.Rogue;
using Populus.GroupBot.Combat.Shaman;
using Populus.GroupBot.Combat.Warlock;
using Populus.GroupBot.Combat.Warrior;
using CombatMgr = Populus.CombatManager.CombatManager;
using System;

namespace Populus.GroupBot.Combat
{
    /// <summary>
    /// An abstract class that handles combat logic for bots. All combat logic handlers are based off this class.
    /// </summary>
    public abstract class CombatLogicHandler
    {
        #region Declarations

        private readonly GroupBotHandler mBotHandler;

        #endregion

        #region Constructors

        public CombatLogicHandler(GroupBotHandler botHandler)
        {
            mBotHandler = botHandler ?? throw new ArgumentNullException("botHandler");
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the bot handler that owns this combat logic
        /// </summary>
        public GroupBotHandler BotHandler { get { return mBotHandler; } }

        /// <summary>
        /// Gets whether or not this class is primarily a melee class
        /// </summary>
        public abstract bool IsMelee { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts an attack on a unit
        /// </summary>
        /// <param name="unit"></param>
        public void StartAttack(Unit unit)
        {
            // If we are level 5 or under, just melee attack
            if (mBotHandler.BotOwner.Level <= 5)
                CombatMgr.GetCombatState(mBotHandler.BotOwner.Guid).AttackMelee(unit);
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Create a combat logic handler based on class
        /// </summary>
        /// <param name="botHandler"></param>
        /// <param name="botClass"></param>
        /// <returns></returns>
        public static CombatLogicHandler Create(GroupBotHandler botHandler, ClassName botClass)
        {
            switch (botClass)
            {
                case ClassName.Druid:
                    return new DruidCombatLogic(botHandler);
                case ClassName.Hunter:
                    return new HunterCombatLogic(botHandler);
                case ClassName.Mage:
                    return new MageCombatLogic(botHandler);
                case ClassName.Paladin:
                    return new PaladinCombatLogic(botHandler);
                case ClassName.Priest:
                    return new PriestCombatLogic(botHandler);
                case ClassName.Rogue:
                    return new RogueCombatLogic(botHandler);
                case ClassName.Shaman:
                    return new ShamanCombatLogic(botHandler);
                case ClassName.Warlock:
                    return new WarlockCombatLogic(botHandler);
                case ClassName.Warrior:
                    return new WarriorCombatLogic(botHandler);
                default:
                    throw new ArgumentException($"Class {botClass} is not defined. Unable to create combat logic handler for this class.");
            }
        }

        #endregion
    }
}
