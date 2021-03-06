﻿using Populus.Core.Constants;
using Populus.Core.World.Objects;
using Populus.Core.Utils;
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
using Populus.Core.World.Objects.Events;
using FluentBehaviourTree;
using Populus.Core.Shared;

namespace Populus.GroupBot.Combat
{
    /// <summary>
    /// An abstract class that handles combat logic for bots. All combat logic handlers are based off this class.
    /// </summary>
    public abstract class CombatLogicHandler
    {
        #region Declarations

        public const float MELEE_RANGE_DISTANCE = 4.0f;
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
        private Unit mAttackingUnit = null;
        private bool mIsFirstCombatActionDone = false;

        private uint? mCombatStartedTick;

        // Behavior trees
        private IBehaviourTreeNode mOutOfCombatBehavior;
        private IBehaviourTreeNode mCombatBehavior;

        private static class ConjuredFood
        {
            public const uint FOOD_LVL_5 = 5349;
            public const uint FOOD_LVL_15 = 1113;
            public const uint FOOD_LVL_25 = 1114;
            public const uint FOOD_LVL_35 = 1487;
            public const uint FOOD_LVL_45 = 8075;
            public const uint FOOD_LVL_55 = 8076;
            public const uint FOOD_LVL_65 = 22895;
        }

        private static class ConjuredWater
        {
            public const uint WATER_LVL_5 = 5350;
            public const uint WATER_LVL_15 = 2288;
            public const uint WATER_LVL_25 = 2136;
            public const uint WATER_LVL_35 = 3772;
            public const uint WATER_LVL_45 = 8077;
            public const uint WATER_LVL_55 = 8078;
            public const uint WATER_LVL_65 = 8079;
        }

        private static List<uint> Healthstones = new List<uint>
        {
            5512,
            19004,
            19005,
            5511,
            19006,
            19007,
            5509,
            19008,
            19009,
            5510,
            19010,
            19011,
            9412,
            19012,
            19013
        };

        #endregion

        #region Constructors

        public CombatLogicHandler(GroupBotHandler botHandler)
        {
            mBotHandler = botHandler ?? throw new ArgumentNullException("botHandler");
            this.mOutOfCombatBehavior = InitializeOutOfCombatBehavior();
            this.mCombatBehavior = InitializeCombatBehaivor();
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
        /// Gets whether or not this class is primarily a range class (does not specify a caster, ie. hunter)
        /// </summary>
        public bool IsRange
        {
            get { return !IsMelee; }
        }

        /// <summary>
        /// Gets whether or not this class is a caster
        /// </summary>
        public abstract bool IsCaster { get; }

        /// <summary>
        /// Gets whether or not combat logic for the bot is a Tank
        /// </summary>
        public virtual bool IsTank { get { return false; } }

        /// <summary>
        /// Gets whether or not combat logic for the bot is a Healer
        /// </summary>
        public virtual bool IsHealer { get { return false; } }

        /// <summary>
        /// Gets all spells available to the class for all levels
        /// </summary>
        protected abstract Dictionary<int, List<uint>> SpellsByLevel { get; }

        /// <summary>
        /// Health level percentage where we regenerate health out of combat
        /// </summary>
        public virtual float OutOfCombatHealthRegenLevel { get { return 50.0f; } }

        /// <summary>
        /// Mana level percentage where we regenerate mana out of combat
        /// </summary>
        public virtual float OutOfCombatManaRegenLevel { get { return 30.0f; } }

        /// <summary>
        /// Does the bot have food in inventory
        /// </summary>
        public bool HasFood
        {
            get { return BotHandler.BotOwner.HasItemInInventory(GetFood()); }
        }

        /// <summary>
        /// Does the bot have water in inventory
        /// </summary>
        public bool HasWater
        {
            get { return GetWater() > 0 && BotHandler.BotOwner.HasItemInInventory(GetWater()); }
        }

        /// <summary>
        /// Gets or sets whether or not logic debugging should be turned on or off (true = on, false = off)
        /// </summary>
        public bool DebugLogic
        {
            get; set;
        }

        /// <summary>
        /// Gets the number of seconds the bot will wait before attacking
        /// </summary>
        public int WaitSecondsForAttack
        {
            get;
            private set;
        }

        /// <summary>
        /// Total time (in MS) that the bot has been in combat
        /// </summary>
        public uint TimeInCombat
        {
            get
            {
                if (!mCombatStartedTick.HasValue)
                    return 0;
                return Time.MM_GetTime() - mCombatStartedTick.Value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Update combat logic
        /// </summary>
        /// <param name="deltaTime"></param>
        public void CombatUpdate(float deltaTime)
        {
            if (!mCombatStartedTick.HasValue) mCombatStartedTick = Time.MM_GetTime();
            // Wait a certain amount of time before starting to attack. Tanks don't wait ever though, need to build that threat
            if (WaitSecondsForAttack > 0 && TimeInCombat < (WaitSecondsForAttack * 1000) && !IsTank)
                return;
            if (DebugLogic) mBotHandler.BotOwner.Logger.Log("--------------------- START DEBUG LOGIC ---------------------------");
            if (DebugLogic) mBotHandler.BotOwner.Logger.Log("--------------------- COMBAT LOGIC --------------------------------");
            this.mCombatBehavior?.Tick(new TimeData(deltaTime));
            if (DebugLogic) mBotHandler.BotOwner.Logger.Log("--------------------- END DEBUG LOGIC -----------------------------");
        }

        /// <summary>
        /// Update out of combat logic
        /// </summary>
        /// <param name="deltaTime"></param>
        public void OutOfCombatUpdate(float deltaTime)
        {
            if (mCombatStartedTick.HasValue) mCombatStartedTick = null;
            WaitSecondsForAttack = 0;
            if (DebugLogic) mBotHandler.BotOwner.Logger.Log("--------------------- START DEBUG LOGIC ---------------------------");
            if (DebugLogic) mBotHandler.BotOwner.Logger.Log("--------------------- OOC LOGIC --------------------------------");
            this.mOutOfCombatBehavior?.Tick(new TimeData(deltaTime));
            if (DebugLogic) mBotHandler.BotOwner.Logger.Log("--------------------- END DEBUG LOGIC -----------------------------");
        }

        /// <summary>
        /// Starts an attack against a unit (we may already be attacking another unit). We leave it up to the combat logic
        /// to determine how to attack.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="waitSeconds">Optionally, wait X seconds before attacking</param>
        public void Attack(Unit unit, int waitSeconds = 0)
        {
            WaitSecondsForAttack = waitSeconds;
            // Add this unit to the aggro list and set it as the current target. This will start combat
            // if it's not already started. And also immediately start attacking the unit.
            BotHandler.StopFollow();
            BotHandler.CombatState.SetTarget(unit);
            BotHandler.CombatState.AddToAggroList(unit);
        }

        /// <summary>
        /// Gets all spells up to a given level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public IEnumerable<uint> GetSpellsUpToLevel(uint level)
        {
            var result = SpellsByLevel.Where(kvp => kvp.Key <= level).ToList();
            return result.SelectMany(s => s.Value).Select(v => v);
        }

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
        /// Received not facing target update
        /// </summary>
        /// <param name="angle"></param>
        internal virtual void NotFacingTarget(float angle)
        {
            if (BotHandler.CombatState.CurrentTarget != null)
                BotHandler.BotOwner.FaceTarget(BotHandler.CombatState.CurrentTarget.Guid);
        }

        /// <summary>
        /// Gets the id of level appropriate food for the bot
        /// </summary>
        public uint GetFood()
        {
            var level = BotHandler.BotOwner.Level;
            if (level >= 60)
                return ConjuredFood.FOOD_LVL_65;
            if (level >= 55)
                return ConjuredFood.FOOD_LVL_55;
            if (level >= 45)
                return ConjuredFood.FOOD_LVL_45;
            if (level >= 35)
                return ConjuredFood.FOOD_LVL_35;
            if (level >= 25)
                return ConjuredFood.FOOD_LVL_25;
            if (level >= 15)
                return ConjuredFood.FOOD_LVL_15;
            return ConjuredFood.FOOD_LVL_5;
        }

        /// <summary>
        /// Gets the id of level appropriate water for the bot
        /// </summary>
        public uint GetWater()
        {
            // If bot does not use mana, return 0 for water
            if (BotHandler.BotOwner.PowerType != Powers.POWER_MANA) return 0;
            var level = BotHandler.BotOwner.Level;
            if (level >= 60)
                return ConjuredWater.WATER_LVL_65;
            if (level >= 55)
                return ConjuredWater.WATER_LVL_55;
            if (level >= 45)
                return ConjuredWater.WATER_LVL_45;
            if (level >= 35)
                return ConjuredWater.WATER_LVL_35;
            if (level >= 25)
                return ConjuredWater.WATER_LVL_25;
            if (level >= 15)
                return ConjuredWater.WATER_LVL_15;
            return ConjuredWater.WATER_LVL_5;
        }

        /// <summary>
        /// Tells the bot to pull a target
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public abstract bool Pull(Unit unit);

        /// <summary>
        /// Checks for required skills for a class
        /// </summary>
        internal virtual void CheckForSkills()
        {
            // Do nothing in base
        }

        /// <summary>
        /// Called when combat is over so any combat state can be reset
        /// </summary>
        public virtual void ResetCombatState()
        {
            WaitSecondsForAttack = 0;
        }

        /// <summary>
        /// Update combat attacks
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="eventArgs"></param>
        public virtual void CombatAttackUpdate(Bot bot, Core.World.Objects.Events.CombatAttackUpdateArgs eventArgs)
        {
            // If we are in a group, add anything that attacks someone in our group to our aggro list
            if (BotHandler.Group != null)
            {
                if (BotHandler.Group.ContainsMember(eventArgs.VictimGuid))
                {
                    var unit = BotHandler.BotOwner.GetUnitByGuid(eventArgs.AttackerGuid);
                    if (unit != null)
                        BotHandler.CombatState.AddToAggroList(unit);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets whether or not the bot is in melee range of it's target
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected bool IsInMeleeRange(Unit target)
        {
            return mBotHandler.BotOwner.DistanceFrom(target.Position) <= MELEE_RANGE_DISTANCE;
        }

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

            // Do we even have this spell?
            if (!BotHandler.BotOwner.HasSpell((ushort)spellId)) return false;

            // Is it on cooldown?
            if (BotHandler.BotOwner.SpellIsOnCooldown(spellId)) return false;

            // Are we on the GCD?
            if (BotHandler.CombatState.IsGCDActive) return false;

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
            uint spell = 0;
            List<SpellChainNode> nodes = SpellManager.Instance.GetSpellsInLine(spellId).ToList();

            // If there are no nodes, check if they have this spell or not
            if (nodes.Count == 0)
            {
                // If nothing, just use the spell id passed in
                if (spell == 0)
                    if (BotHandler.BotOwner.HasSpell((ushort)spellId))
                        return spellId;
            }
                

            // Loop the nodes (they are ordered by rank) until we find the spell we do not currently have
            // We need to work backwards. Melee spells don't keep previos ranks.
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                if (mBotHandler.BotOwner.HasSpell((ushort)nodes[i].SpellId))
                {
                    spell = nodes[i].SpellId;
                    break;
                }
            }

            return spell;
        }

        /// <summary>
        /// Initialize out of combat behavior
        /// </summary>
        protected abstract IBehaviourTreeNode InitializeOutOfCombatBehavior();

        /// <summary>
        /// Initialize combat related behavior
        /// </summary>
        protected abstract IBehaviourTreeNode InitializeCombatBehaivor();

        /// <summary>
        /// Prints a debug message returns success
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected BehaviourTreeStatus DebugSuccess(string message)
        {
            if (DebugLogic) mBotHandler.BotOwner.Logger.Log(message);
            return BehaviourTreeStatus.Success;
        }

        /// <summary>
        /// Prints a debug message returns failure
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected BehaviourTreeStatus DebugFailure(string message)
        {
            if (DebugLogic) mBotHandler.BotOwner.Logger.Log(message);
            return BehaviourTreeStatus.Failure;
        }

        /// <summary>
        /// Prints a debug message returns running
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected BehaviourTreeStatus DebugRunning(string message)
        {
            if (DebugLogic) mBotHandler.BotOwner.Logger.Log(message);
            return BehaviourTreeStatus.Running;
        }

        #endregion

        #region Common Behaviors

        /// <summary>
        /// Creates a behavior tree node that melee attacks the current target
        /// </summary>
        /// <returns></returns>
        internal IBehaviourTreeNode MeleeAttack(GroupBotHandler handler)
        {
            // Should fail if we do not or can not regenreate health
            var builder = new BehaviourTreeBuilder();
            builder.Sequence("Melee Attack")
                        .Do("Start Melee Attack", t => StartMeleeSwing(handler))
                        .Do("Move to target", t => MoveToTarget(handler))
                   .End();
            return builder.Build();
        }

        private BehaviourTreeStatus StartMeleeSwing(GroupBotHandler handler)
        {
            if (handler.CombatState.CurrentTarget == null) return DebugFailure("Current target is null, cannot start melee swing");

            // If we are not already melee attacking, start
            if (!handler.CombatState.IsAttacking)
                handler.BotOwner.Attack(handler.CombatState.CurrentTarget.Guid);

            return DebugSuccess("Starting melee swing");
        }

        private BehaviourTreeStatus MoveToTarget(GroupBotHandler handler)
        {
            if (handler.CombatState.CurrentTarget == null) return DebugFailure("Current target is null, cannot move towards target");
            
            // Make sure the target is our follow target
            if (handler.BotOwner.FollowTarget == null || handler.BotOwner.FollowTarget.Guid != handler.CombatState.CurrentTarget.Guid)
            {
                handler.BotOwner.SetFollow(handler.CombatState.CurrentTarget.Guid);
                return DebugRunning("Moving to target by settingit as my follow target");
            }

            // If we are not close enough to follow target then we are still running
            var distance = MathUtility.CalculateDistance(handler.BotOwner.Position, handler.CombatState.CurrentTarget.Position);
            if (distance > MELEE_RANGE_DISTANCE)
                return DebugRunning($"I am not close enough to my target. I am {distance} yards away. Still moving towards them.");

            return DebugSuccess("I have reached my target!");
        }

        /// <summary>
        /// Creates a behavior tree that wand attacks the current target
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        internal IBehaviourTreeNode WandAttack(GroupBotHandler handler)
        {
            // Should fail if we do not have a wand to attack with
            var builder = new BehaviourTreeBuilder();
            builder.Sequence("Wand Attack")
                        .Do("Has Wand to Attack With", t => HasWandToAttackWith(handler))
                        .Do("Start Wand Attack", t => StartWanding(handler))
                   .End();
            return builder.Build();
        }

        private BehaviourTreeStatus HasWandToAttackWith(GroupBotHandler handler)
        {
            if (!handler.BotOwner.CanUseWands)
                return DebugFailure("I cannot use wands");
            if (!handler.BotOwner.HasWandEquipped)
                return DebugFailure("I do not have a wand equipped");
            return BehaviourTreeStatus.Success;
        }

        private BehaviourTreeStatus StartWanding(GroupBotHandler handler)
        {
            if (handler.CombatState.CurrentTarget == null) return DebugFailure("Current target is null, cannot start attacking with wand");

            // If we are not already attacking, start
            if (!handler.CombatState.IsAttacking)
                handler.CombatState.SpellCast(WAND_SHOOT);

            return DebugSuccess("I am attacking with my wand");
        }

        /// <summary>
        /// Behavior that casts a spell that requires melee range
        /// </summary>
        /// <param name="spellId"></param>
        /// <returns></returns>
        protected BehaviourTreeStatus CastMeleeSpell(uint spellId)
        {
            var spell = SpellTable.Instance.getSpell(spellId);
            return CastMeleeSpell(spell);
        }

        /// <summary>
        /// Behavior that casts a spell that requires melee range
        /// </summary>
        /// <param name="spell"></param>
        /// <returns></returns>
        protected BehaviourTreeStatus CastMeleeSpell(SpellEntry spell)
        {
            if (spell == null) return BehaviourTreeStatus.Failure;
            // If we are not within range
            if (!IsInMeleeRange(BotHandler.CombatState.CurrentTarget))
                return DebugFailure($"I am not in melee range to cast spell {spell.SpellName}");
            return CastSpell(spell);
        }

        /// <summary>
        /// Behavior that casts a spell on current target
        /// </summary>
        /// <param name="spellId"></param>
        /// <returns></returns>
        protected BehaviourTreeStatus CastSpell(uint spellId)
        {
            var spell = SpellTable.Instance.getSpell(spellId);
            return CastSpell(spell);
        }

        /// <summary>
        /// Behavior that casts a spell on current target
        /// </summary>
        /// <param name="spell"></param>
        /// <returns></returns>
        protected BehaviourTreeStatus CastSpell(SpellEntry spell)
        {
            if (spell == null) return BehaviourTreeStatus.Failure;
            // If we can't cast it, fail
            if (!HasSpellAndCanCast(spell.SpellId))
                return DebugFailure($"I cannot cast spell {spell.SpellName}");

            if (!BotHandler.CombatState.SpellCast(spell.SpellId))
                return DebugFailure($"I was not able to cast spell {spell.SpellName}");
            return DebugSuccess($"I cast spell {spell.SpellName}");
        }

        /// <summary>
        /// Behavior that casts a spell on a target
        /// </summary>
        /// <param name="spellId"></param>
        /// <returns></returns>
        protected BehaviourTreeStatus CastSpell(uint spellId, Unit target)
        {
            var spell = SpellTable.Instance.getSpell(spellId);
            return CastSpell(spell, target);
        }

        /// <summary>
        /// Behavior that casts a spell on a target
        /// </summary>
        /// <param name="spell"></param>
        /// <returns></returns>
        protected BehaviourTreeStatus CastSpell(SpellEntry spell, Unit target)
        {
            if (spell == null) return BehaviourTreeStatus.Failure;
            // If we can't cast it, fail
            if (!HasSpellAndCanCast(spell.SpellId))
                return DebugFailure($"I cannot cast spell {spell.SpellName}");

            if (!BotHandler.CombatState.SpellCast(target, spell.SpellId))
                return DebugFailure($"I was not able to cast spell {spell.SpellName}");
            return DebugSuccess($"I cast spell {spell.SpellName}");
        }

        /// <summary>
        /// Behavior that casts a DOT spell on current target if it is not already present
        /// </summary>
        /// <param name="spellId"></param>
        /// <returns></returns>
        protected BehaviourTreeStatus CastDOT(uint spellId)
        {
            return CastDOT(spellId, 0f);
        }

        /// <summary>
        /// Behavior that casts a DOT spell on current target if it is not already present and if
        /// the targets health is above a certain percentage
        /// </summary>
        /// <param name="spellId"></param>
        /// <returns></returns>
        protected BehaviourTreeStatus CastDOT(uint spellId, float aboveHealth)
        {
            var spell = SpellTable.Instance.getSpell(spellId);
            return CastDOT(spell, aboveHealth);
        }

        /// <summary>
        /// Behavior that casts a DOT spell on current target if it is not already present and if
        /// the targets health is above a certain percentage
        /// </summary>
        /// <param name="spell"></param>
        /// <returns></returns>
        protected BehaviourTreeStatus CastDOT(SpellEntry spell, float aboveHealth)
        {
            if (spell == null) return BehaviourTreeStatus.Failure;
            // Make sure health percentage is at or above threshold for the dot spell
            if (BotHandler.CombatState.CurrentTarget.HealthPercentage < aboveHealth)
                return DebugFailure($"I cannot cast DoT {spell.SpellName} because current health is below threshold of {aboveHealth}");

            // Make sure the target does not already have the dot aura
            if (BotHandler.CombatState.CurrentTarget.HasAura(spell.SpellId))
                return DebugFailure($"My target already has the dot aura {spell.SpellName}");

            return CastSpell(spell);
        }

        /// <summary>
        /// Checks that a buff can be cast and is not already on the bot
        /// </summary>
        /// <param name="spellAndAura">Spell and aura to check</param>
        /// <returns></returns>
        protected BehaviourTreeStatus SelfBuff(uint spellAndAura)
        {
            var spell = SpellTable.Instance.getSpell(spellAndAura);
            return SelfBuff(spell);
        }

        /// <summary>
        /// Checks that a buff can be cast and is not already on the bot
        /// </summary>
        /// <param name="spellAndAura">Spell and aura to check</param>
        /// <returns></returns>
        protected BehaviourTreeStatus SelfBuff(SpellEntry spell)
        {
            if (spell == null) return BehaviourTreeStatus.Failure;
            // If does not have spell or cannot cast, fail
            if (!HasSpellAndCanCast(spell.SpellId))
                return DebugFailure($"I cannot cast spell {spell.SpellName}");
            // If already has the aura, fail
            if (BotHandler.BotOwner.HasAura(spell.SpellId))
                return DebugFailure($"I cannot buff spell {spell.SpellName} because I already have that buff/aura");

            if (!BotHandler.CombatState.SpellCast(BotHandler.BotOwner, spell.SpellId))
                return DebugFailure($"I was not able to cast spell {spell.SpellName}");
            return DebugSuccess($"I buffed myself with spell {spell.SpellName}");
        }

        /// <summary>
        /// Checks that a buff can be cast and someone in the group needs it
        /// </summary>
        /// <param name="spellAndAura">Spell and aura to check</param>
        /// <returns></returns>
        protected BehaviourTreeStatus GroupBuff(uint spellAndAura)
        {
            var spell = SpellTable.Instance.getSpell(spellAndAura);
            return GroupBuff(spell);
        }

        /// <summary>
        /// Checks that a buff can be cast and someone in the group needs it
        /// </summary>
        /// <param name="spell">Spell and aura to check</param>
        /// <returns></returns>
        protected BehaviourTreeStatus GroupBuff(SpellEntry spell)
        {
            if (spell == null) return BehaviourTreeStatus.Failure;
            // If we are not in a group, self buff instead
            if (BotHandler.Group == null) return SelfBuff(spell);

            // If does not have spell or cannot cast, fail
            if (!HasSpellAndCanCast(spell.SpellId))
                return DebugFailure($"I cannot cast spell {spell.SpellName}");
            // If everyone in the group already has the aura
            if (!BotHandler.Group.Members.Any(m => !m.HasAura((ushort)spell.SpellId)))
                return DebugFailure($"Everyone in group already has buff for spell {spell.SpellName}");

            // Get the first member in the group that needs the aura
            var needs = BotHandler.Group.Members.Where(m => !m.HasAura((ushort)spell.SpellId)).FirstOrDefault();
            if (needs == null)
                return DebugFailure($"Could not found group member that needs buff for spell {spell.SpellName}");

            if (!BotHandler.CombatState.SpellCast(BotHandler.BotOwner.GetPlayerByGuid(needs.Guid), spell.SpellId))
                return DebugFailure($"I was not able to cast spell {spell.SpellName}");
            return DebugSuccess($"I buffed {needs.Name} with buff for spell {spell.SpellName}");
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Create a combat logic handler based on class
        /// </summary>
        /// <param name="botHandler"></param>
        /// <param name="botClass"></param>
        /// <returns></returns>
        public static CombatLogicHandler Create(GroupBotHandler botHandler, ClassType botClass)
        {
            switch (botClass)
            {
                case ClassType.Druid:
                    return new DruidCombatLogic(botHandler);
                case ClassType.Hunter:
                    return new HunterCombatLogic(botHandler);
                case ClassType.Mage:
                    return new MageCombatLogic(botHandler);
                case ClassType.Paladin:
                    return new PaladinCombatLogic(botHandler);
                case ClassType.Priest:
                    return new PriestCombatLogic(botHandler);
                case ClassType.Rogue:
                    return new RogueCombatLogic(botHandler);
                case ClassType.Shaman:
                    return new ShamanCombatLogic(botHandler);
                case ClassType.Warlock:
                    return new WarlockCombatLogic(botHandler);
                case ClassType.Warrior:
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
        public static CombatLogicHandler Create(GroupBotHandler botHandler, ClassType botClass, TalentSpec spec)
        {
            if (spec == null) return Create(botHandler, botClass);

            switch (botClass)
            {
                case ClassType.Druid:
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
                case ClassType.Hunter:
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
                case ClassType.Mage:
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
                case ClassType.Paladin:
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
                case ClassType.Priest:
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
                case ClassType.Rogue:
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
                case ClassType.Shaman:
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
                case ClassType.Warlock:
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
                case ClassType.Warrior:
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
        public static MainSpec GetSpecFromTalentTab(ClassType @class, uint tab)
        {
            switch (@class)
            {
                case ClassType.Druid:
                    if (tab == 0) return MainSpec.DRUID_SPEC_BALANCE;
                    if (tab == 1) return MainSpec.DRUID_SPEC_FERAL;
                    if (tab == 2) return MainSpec.DRUID_SPEC_RESTORATION;
                    break;
                case ClassType.Hunter:
                    if (tab == 0) return MainSpec.HUNTER_SPEC_BEASTMASTERY;
                    if (tab == 1) return MainSpec.HUNTER_SPEC_MARKSMANSHIP;
                    if (tab == 2) return MainSpec.HUNTER_SPEC_SURVIVAL;
                    break;
                case ClassType.Mage:
                    if (tab == 0) return MainSpec.MAGE_SPEC_ARCANE;
                    if (tab == 1) return MainSpec.MAGE_SPEC_FIRE;
                    if (tab == 2) return MainSpec.MAGE_SPEC_FROST;
                    break;
                case ClassType.Paladin:
                    if (tab == 0) return MainSpec.PALADIN_SPEC_HOLY;
                    if (tab == 1) return MainSpec.PALADIN_SPEC_PROTECTION;
                    if (tab == 2) return MainSpec.PALADIN_SPEC_RETRIBUTION;
                    break;
                case ClassType.Priest:
                    if (tab == 0) return MainSpec.PRIEST_SPEC_DISCIPLINE;
                    if (tab == 1) return MainSpec.PRIEST_SPEC_HOLY;
                    if (tab == 2) return MainSpec.PRIEST_SPEC_SHADOW;
                    break;
                case ClassType.Rogue:
                    if (tab == 0) return MainSpec.ROGUE_SPEC_ASSASSINATION;
                    if (tab == 1) return MainSpec.ROGUE_SPEC_COMBAT;
                    if (tab == 2) return MainSpec.ROGUE_SPEC_SUBTELTY;
                    break;
                case ClassType.Shaman:
                    if (tab == 0) return MainSpec.SHAMAN_SPEC_ELEMENTAL;
                    if (tab == 1) return MainSpec.SHAMAN_SPEC_ENHANCEMENT;
                    if (tab == 2) return MainSpec.SHAMAN_SPEC_RESTORATION;
                    break;
                case ClassType.Warlock:
                    if (tab == 0) return MainSpec.WARLOCK_SPEC_AFFLICTION;
                    if (tab == 1) return MainSpec.WARLOCK_SPEC_DEMONOLOGY;
                    if (tab == 2) return MainSpec.WARLOCK_SPEC_DESTRUCTION;
                    break;
                case ClassType.Warrior:
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
