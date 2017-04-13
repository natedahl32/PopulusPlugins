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

        /// <summary>
        /// Gets the spec associated with the talent tab for a class
        /// </summary>
        /// <param name="class"></param>
        /// <param name="tab"></param>
        /// <returns></returns>
        public static MainSpec GetSpecFromTalentTab(ClassName @class, uint tab)
        {
            switch (@class)
            {
                case ClassName.Druid:
                    if (tab == 0) return MainSpec.DRUID_SPEC_BALANCE;
                    if (tab == 1) return MainSpec.DRUID_SPEC_FERAL;
                    if (tab == 2) return MainSpec.DRUID_SPEC_RESTORATION;
                    break;
                case ClassName.Hunter:
                    if (tab == 0) return MainSpec.HUNTER_SPEC_BEASTMASTERY;
                    if (tab == 1) return MainSpec.HUNTER_SPEC_MARKSMANSHIP;
                    if (tab == 2) return MainSpec.HUNTER_SPEC_SURVIVAL;
                    break;
                case ClassName.Mage:
                    if (tab == 0) return MainSpec.MAGE_SPEC_ARCANE;
                    if (tab == 1) return MainSpec.MAGE_SPEC_FIRE;
                    if (tab == 2) return MainSpec.MAGE_SPEC_FROST;
                    break;
                case ClassName.Paladin:
                    if (tab == 0) return MainSpec.PALADIN_SPEC_HOLY;
                    if (tab == 1) return MainSpec.PALADIN_SPEC_PROTECTION;
                    if (tab == 2) return MainSpec.PALADIN_SPEC_RETRIBUTION;
                    break;
                case ClassName.Priest:
                    if (tab == 0) return MainSpec.PRIEST_SPEC_DISCIPLINE;
                    if (tab == 1) return MainSpec.PRIEST_SPEC_HOLY;
                    if (tab == 2) return MainSpec.PRIEST_SPEC_SHADOW;
                    break;
                case ClassName.Rogue:
                    if (tab == 0) return MainSpec.ROGUE_SPEC_ASSASSINATION;
                    if (tab == 1) return MainSpec.ROGUE_SPEC_COMBAT;
                    if (tab == 2) return MainSpec.ROGUE_SPEC_SUBTELTY;
                    break;
                case ClassName.Shaman:
                    if (tab == 0) return MainSpec.SHAMAN_SPEC_ELEMENTAL;
                    if (tab == 1) return MainSpec.SHAMAN_SPEC_ENHANCEMENT;
                    if (tab == 2) return MainSpec.SHAMAN_SPEC_RESTORATION;
                    break;
                case ClassName.Warlock:
                    if (tab == 0) return MainSpec.WARLOCK_SPEC_AFFLICTION;
                    if (tab == 1) return MainSpec.WARLOCK_SPEC_DEMONOLOGY;
                    if (tab == 2) return MainSpec.WARLOCK_SPEC_DESTRUCTION;
                    break;
                case ClassName.Warrior:
                    if (tab == 0) return MainSpec.WARRIOR_SPEC_ARMS;
                    if (tab == 1) return MainSpec.WARRIOR_SPEC_FURY;
                    if (tab == 2) return MainSpec.WARRIOR_SPEC_PROTECTION;
                    break;
            }

            return MainSpec.NONE;
        }

        #endregion
    }
}
