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
using Populus.GroupBot.Talents;
using Populus.Core.DBC;
using System.Collections.Generic;
using Populus.Core.World.Spells;
using System.Linq;

namespace Populus.GroupBot.Combat
{
    /// <summary>
    /// An abstract class that handles combat logic for bots. All combat logic handlers are based off this class.
    /// </summary>
    public abstract class CombatLogicHandler
    {
        #region Declarations

        public const uint RECENTLY_BANDAGED = 11196;
        public const uint WAND_SHOOT = 5019;

        // racial
        protected uint STONEFORM,
            ESCAPE_ARTIST,
            PERCEPTION,
            SHADOWMELD,
            BLOOD_FURY,
            WAR_STOMP,
            BERSERKING,
            WILL_OF_THE_FORSAKEN;

        private readonly GroupBotHandler mBotHandler;
        private bool mIsAttacking = false;
        private bool mIsFirstCombatActionDone = false;

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

        /// <summary>
        /// Gets whether or not the bot is currently attacking a target with either melee or ranged
        /// </summary>
        public bool IsAttacking { get { return mIsAttacking; } }



        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes all spells the player currently has.
        /// </summary>
        public virtual void InitializeSpells()
        {
            // Racial abilities
            STONEFORM = InitSpell(RacialTraits.STONEFORM_ALL);
            ESCAPE_ARTIST = InitSpell(RacialTraits.ESCAPE_ARTIST_ALL);
            PERCEPTION = InitSpell(RacialTraits.PERCEPTION_ALL);
            SHADOWMELD = InitSpell(RacialTraits.SHADOWMELD_ALL);
            BLOOD_FURY = InitSpell(RacialTraits.BLOOD_FURY_ALL);
            WAR_STOMP = InitSpell(RacialTraits.WAR_STOMP_ALL);
            BERSERKING = InitSpell(RacialTraits.BERSERKING_ALL);
            WILL_OF_THE_FORSAKEN = InitSpell(RacialTraits.WILL_OF_THE_FORSAKEN_ALL);
        }

        /// <summary>
        /// Ordered to start an attack on a unit
        /// </summary>
        /// <param name="unit"></param>
        public void StartAttack(Unit unit)
        {
            // Set the target since we were ordered to attack this unit
            BotHandler.CombatState.SetTarget(unit);
            DoCombatAction(unit);
        }

        /// <summary>
        /// Update combat logic
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            // if we are dead, we can't do anything
            if (BotHandler.BotOwner.IsDead) return;

            // if we are no longer in combat, don't do anything
            if (!BotHandler.CombatState.IsInCombat)
            {
                mIsFirstCombatActionDone = false;
                return;
            }

            // if our current target is dead, stop attacking
            if (BotHandler.CombatState.CurrentTarget.IsDead)
                BotHandler.BotOwner.StopAttack();

            // if we are currently casting something, wait until we are done with that
            if (BotHandler.CombatState.IsCasting) return;

            DoCombatAction(BotHandler.CombatState.CurrentTarget);
        }

        /// <summary>
        /// Received an event to stop attacking
        /// </summary>
        internal virtual void StopAttack()
        {
            mIsAttacking = false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Convenience method to get spell from DBC
        /// </summary>
        /// <param name="spellId"></param>
        /// <returns></returns>
        protected SpellEntry Spell(uint spellId)
        {
            return SpellTable.Instance.getSpell(spellId);
        }

        /// <summary>
        /// Checks if the player owns the spell and can cast it
        /// </summary>
        /// <param name="spellId"></param>
        /// <returns></returns>
        protected bool HasSpellAndCanCast(uint spellId)
        {
            // No spell? Can't cast it
            if (spellId == 0) return false;

            var spell = Spell(spellId);
            if (spell == null)
                return false;

            // TODO: Might need this
            // Is it on cooldown?
            //if (Player.SpellCooldowns.HasCooldown(spellId)) return false;

            // Have enough power to cast it?
            if (!BotHandler.BotOwner.CanCastSpell(spell)) return false;

            // TODO: More checks needed. Have reagents? for example

            return true;
        }

        /// <summary>
        /// Initializes a spell by getting the current rank of the spell the bot currently has
        /// </summary>
        /// <param name="spellId"></param>
        /// <returns></returns>
        protected uint InitSpell(uint spellId)
        {
            // If the player does not have the spell
            if (!mBotHandler.BotOwner.HasSpell((ushort)spellId))
                return 0;

            var spell = spellId;
            List<SpellChainNode> nodes = SpellManager.Instance.GetSpellsInLine(spellId).ToList();

            // Loop the nodes (they are ordered by rank) until we find the spell we do not currently have
            for (int i = 1; i < nodes.Count; i++)
            {
                if (mBotHandler.BotOwner.HasSpell((ushort)nodes[i].SpellId))
                    spell = nodes[i].SpellId;
                else
                    break;
            }

            return spell;
        }

        /// <summary>
        /// Attacks a unit with melee attacks
        /// </summary>
        /// <param name="unit"></param>
        protected void AttackMelee(Unit unit)
        {
            if (unit == null) return;
            BotHandler.CombatState.AttackMelee(unit);
            mIsAttacking = true;
        }

        /// <summary>
        /// Attacks a unit with wand attacks
        /// </summary>
        /// <param name="unit"></param>
        protected void AttackWand(Unit unit)
        {
            if (unit == null) return;
            if (BotHandler.BotOwner.CanUseWands && BotHandler.BotOwner.GetEquippedItemsByInventoryType(InventoryType.INVTYPE_RANGED) != null)
            {
                BotHandler.CombatState.SpellCast(unit, WAND_SHOOT);
                mIsAttacking = true;
            }
        }

        /// <summary>
        /// Performs a combat action
        /// </summary>
        /// <param name="unit"></param>
        private void DoCombatAction(Unit target)
        {
            CombatActionResult result;
            // now we need to get a combat action. get the first one if needed
            if (!mIsFirstCombatActionDone)
            {
                result = DoFirstCombatAction(target);
                if (result == CombatActionResult.ACTION_OK || result == CombatActionResult.NO_ACTION_OK)
                {
                    mIsFirstCombatActionDone = true;
                    // If we did an action, don't continue
                    if (result == CombatActionResult.ACTION_OK)
                        return;
                }
            }

            // need an action as part of our rotation
            result = DoNextCombatAction(target);
        }

        /// <summary>
        /// Performs the first combat action
        /// </summary>
        /// <param name="unit"></param>
        protected virtual CombatActionResult DoFirstCombatAction(Unit unit)
        {
            // If we are a melee class, start melee attacks. Otherwise start ranged attacks
            if (IsMelee)
                AttackMelee(unit);
            else
                AttackWand(unit);

            return CombatActionResult.ACTION_OK;
        }

        /// <summary>
        /// Performs the next combat action
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        protected virtual CombatActionResult DoNextCombatAction(Unit unit)
        {
            // base does nothing
            return CombatActionResult.NO_ACTION_OK;
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
        /// Create a combat logic handler based on class and spec
        /// </summary>
        /// <param name="botHandler"></param>
        /// <param name="botClass"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        public static CombatLogicHandler Create(GroupBotHandler botHandler, ClassName botClass, TalentSpec spec)
        {
            if (spec == null) return Create(botHandler, botClass);

            switch (botClass)
            {
                case ClassName.Druid:
                    switch (spec.Spec)
                    {
                        case MainSpec.DRUID_SPEC_BALANCE:
                            return new Druid.BalanceCombatLogic(botHandler);
                        case MainSpec.DRUID_SPEC_FERAL:
                            return new Druid.FeralCombatLogic(botHandler);
                        case MainSpec.DRUID_SPEC_RESTORATION:
                            return new Druid.RestorationCombatLogic(botHandler);
                        default:
                            return new DruidCombatLogic(botHandler);
                    }
                case ClassName.Hunter:
                    switch (spec.Spec)
                    {
                        case MainSpec.HUNTER_SPEC_BEASTMASTERY:
                            return new Hunter.BeastMasteryCombatLogic(botHandler);
                        case MainSpec.HUNTER_SPEC_MARKSMANSHIP:
                            return new Hunter.MarksmanshipCombatLogic(botHandler);
                        case MainSpec.HUNTER_SPEC_SURVIVAL:
                            return new Hunter.SurvivalCombatLogic(botHandler);
                        default:
                            return new HunterCombatLogic(botHandler);
                    }
                case ClassName.Mage:
                    switch(spec.Spec)
                    {
                        case MainSpec.MAGE_SPEC_ARCANE:
                            return new Mage.ArcaneCombatLogic(botHandler);
                        case MainSpec.MAGE_SPEC_FIRE:
                            return new Mage.FireCombatLogic(botHandler);
                        case MainSpec.MAGE_SPEC_FROST:
                            return new Mage.FrostCombatLogic(botHandler);
                        default:
                            return new MageCombatLogic(botHandler);
                    }
                case ClassName.Paladin:
                    switch(spec.Spec)
                    {
                        case MainSpec.PALADIN_SPEC_HOLY:
                            return new Paladin.HolyCombatLogic(botHandler);
                        case MainSpec.PALADIN_SPEC_PROTECTION:
                            return new Paladin.ProtectionCombatLogic(botHandler);
                        case MainSpec.PALADIN_SPEC_RETRIBUTION:
                            return new Paladin.RetributionCombatLogic(botHandler);
                        default:
                            return new PaladinCombatLogic(botHandler);
                    }
                case ClassName.Priest:
                    switch(spec.Spec)
                    {
                        case MainSpec.PRIEST_SPEC_DISCIPLINE:
                            return new Priest.DisciplineCombatLogic(botHandler);
                        case MainSpec.PRIEST_SPEC_HOLY:
                            return new Priest.HolyCombatLogic(botHandler);
                        case MainSpec.PRIEST_SPEC_SHADOW:
                            return new Priest.ShadowCombatLogic(botHandler);
                        default:
                            return new PriestCombatLogic(botHandler);
                    }
                case ClassName.Rogue:
                    switch(spec.Spec)
                    {
                        case MainSpec.ROGUE_SPEC_ASSASSINATION:
                            return new Rogue.AssassinationCombatLogic(botHandler);
                        case MainSpec.ROGUE_SPEC_COMBAT:
                            return new Rogue.CombatCombatLogic(botHandler);
                        case MainSpec.ROGUE_SPEC_SUBTELTY:
                            return new Rogue.SubtletyCombatLogic(botHandler);
                        default:
                            return new RogueCombatLogic(botHandler);
                    }
                case ClassName.Shaman:
                    switch (spec.Spec)
                    {
                        case MainSpec.SHAMAN_SPEC_ELEMENTAL:
                            return new Shaman.ElementalCombatLogic(botHandler);
                        case MainSpec.SHAMAN_SPEC_ENHANCEMENT:
                            return new Shaman.EnhancementCombatLogic(botHandler);
                        case MainSpec.SHAMAN_SPEC_RESTORATION:
                            return new Shaman.RestorationCombatLogic(botHandler);
                        default:
                            return new ShamanCombatLogic(botHandler);
                    }
                case ClassName.Warlock:
                    switch(spec.Spec)
                    {
                        case MainSpec.WARLOCK_SPEC_AFFLICTION:
                            return new Warlock.AfflictionCombatLogic(botHandler);
                        case MainSpec.WARLOCK_SPEC_DEMONOLOGY:
                            return new Warlock.DemonologyCombatLogic(botHandler);
                        case MainSpec.WARLOCK_SPEC_DESTRUCTION:
                            return new Warlock.DestructionCombatLogic(botHandler);
                        default:
                            return new WarlockCombatLogic(botHandler);
                    }
                case ClassName.Warrior:
                    switch (spec.Spec)
                    {
                        case MainSpec.WARRIOR_SPEC_PROTECTION:
                            return new Warrior.ProtectionCombatLogic(botHandler);
                        case MainSpec.WARRIOR_SPEC_ARMS:
                            return new Warrior.ArmsCombatLogic(botHandler);
                        case MainSpec.WARRIOR_SPEC_FURY:
                            return new Warrior.FuryCombatLogic(botHandler);
                        default:
                            return new WarriorCombatLogic(botHandler);
                    }
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
