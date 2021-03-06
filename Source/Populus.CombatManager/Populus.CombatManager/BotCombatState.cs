﻿using Populus.Core.Constants;
using Populus.Core.DBC;
using Populus.Core.Shared;
using Populus.Core.World.Objects;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Populus.CombatManager
{
    public class BotCombatState
    {
        #region Declarations

        private const float MAX_CAST_DISTANCE = 80.0f;
        private const float CAST_RANGE_BUFFER = 1.5f;
        public const uint WAND_SHOOT = 5019;

        // holds the current objects marked with raid targets
        private ConcurrentDictionary<RaidTargetMarkers, WoWGuid> mMarkedTargets = new ConcurrentDictionary<RaidTargetMarkers, WoWGuid>();

        private readonly Bot mBotOwner;
        private readonly AggroList mAggroList = new AggroList();
        private readonly GlobalCooldown mGlobalCooldown;
        private Unit mTarget;

        // Id of the spell/ability that is currently being used (only set if the ability has a cast time)
        private uint mCastingSpellId = 0;

        // Flag that holds whether or not the bot is attacking
        private bool mIsAttacking = false;

        #endregion

        #region Constructors

        internal BotCombatState(Bot botOwner)
        {
            if (botOwner == null) throw new ArgumentNullException("botOwner");
            this.mBotOwner = botOwner;
            this.mGlobalCooldown = new GlobalCooldown(botOwner);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the bot owner that owns this combat state
        /// </summary>
        internal Bot BotOwner { get { return mBotOwner; } }

        /// <summary>
        /// Whether or not the bot is in combat
        /// </summary>
        public bool IsInCombat { get { return mAggroList.AggroUnits.Count() > 0; } }

        /// <summary>
        /// Gets the current target for the bot
        /// </summary>
        public Unit CurrentTarget
        {
            get
            {
                // If target is null, get a target based on marker
                if (mTarget == null)
                    mTarget = GetTargetByRaidMarker();
                // If target is still null, get first target in aggro list
                if (mTarget == null)
                    mTarget = mAggroList.First;
                return mTarget;
            }
        }

        /// <summary>
        /// Gets whether or not a spell/ability is currently being casted
        /// </summary>
        public bool IsCasting
        {
            get { return mCastingSpellId > 0; }
        }

        /// <summary>
        /// Gets the id of the spell currently being cast
        /// </summary>
        public uint CastingSpell
        {
            get { return mCastingSpellId; }
        }

        /// <summary>
        /// Gets whether or not the bot is currently attacking
        /// </summary>
        public bool IsAttacking
        {
            get { return mIsAttacking; }
        }

        /// <summary>
        /// Gets whether or not the GCD is currently active
        /// </summary>
        public bool IsGCDActive
        {
            get { return mGlobalCooldown.IsGCDActive; }
        }

        /// <summary>
        /// Gets the aggro list for this bot
        /// </summary>
        public AggroList AggroList
        {
            get { return mAggroList; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the combat target to a specific target instead of letting the combat state determine. A null target is not valid. Cannot clear using this method.
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(Unit target)
        {
            if (target == null) return;
            mTarget = target;
            // Clear the attacking flag when we manually set a target
            StopAttack();
        }

        /// <summary>
        /// Gets the primary dps target for this bot. This is globally considered to be SKULL.
        /// </summary>
        /// <returns></returns>
        public Unit GetPrimaryDpsTarget()
        {
            return GetMarkedUnit(RaidTargetMarkers.SKULL);
        }

        /// <summary>
        /// Gets the unit that is marked by a raid target marker.
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        public Unit GetMarkedUnit(RaidTargetMarkers icon)
        {
            WoWGuid target = null;
            if (mMarkedTargets.TryGetValue(icon, out target))
                if (target != null)
                    return mBotOwner.GetUnitByGuid(target);
            // Unable to get, return null
            return null;
        }

        /// <summary>
        /// Sends commands to cast a spell on the current target. If no current target exists, the spell will not be cast.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="spellId"></param>
        public bool SpellCast(uint spellId)
        {
            return SpellCast(CurrentTarget, spellId);
        }

        /// <summary>
        /// Sends commands to cast a spell on a target. If that target is hostile, this will place the bot into combat and add the target
        /// to the bots aggro list.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="spellId"></param>
        /// <returns>Whether or not the cast was successful</returns>
        public bool SpellCast(Unit target, uint spellId)
        {
            // If we do not have a target, use ourselves as a target
            if (target == null) target = mBotOwner;

            // If we have a spell we are casting we can't cast another one
            if (mCastingSpellId > 0) return false;

            // Get the spell
            var spell = SpellTable.Instance.getSpell(spellId);
            if (spell == null)
                return false;

            // If not within distance to cast, move to distance
            var dist = mBotOwner.DistanceFrom(target.Position);
            if (spell.MaximumRange.HasValue && dist > spell.MaximumRange.Value)
            {
                // If we are too far away, stop the cast
                if (dist > MAX_CAST_DISTANCE)
                    return false;

                // Move to a spot that is within range
                if (mBotOwner.FollowTarget == null || mBotOwner.FollowTarget.Guid != target.Guid)
                    mBotOwner.SetFollow(target.Guid, spell.MaximumRange.Value - 0.5f);
            }
            else
            {
                // If we are now within range, remove our target from follow
                if (mBotOwner.FollowTarget != null && mBotOwner.FollowTarget.Guid == target.Guid)
                    mBotOwner.RemoveFollow();
            }

            // Set the casting spell if there is a cast time
            if (spell.CastTime > 0 || spell.CastingTimeIndex > 1)
                mCastingSpellId = spellId;
            // If the spell we are casting is WAND_SHOOT, start attacking flag (this does not cast the SHOOT spell!)
            if (spellId == WAND_SHOOT)
                StartAttack();

            // Cast the spell
            CastSpell(target, spellId);
            return true;
        }

        /// <summary>
        /// Cancels the current spell being cast
        /// </summary>
        public void CancelSpellCast()
        {
            if (!IsCasting) return;
            mBotOwner.CancelCast(mCastingSpellId);
            mBotOwner.Logger.Log($"Casting of spell {mCastingSpellId} was cancelled");
            mCastingSpellId = 0;
        }

        /// <summary>
        /// Adds a unit to the aggro list if they are not already on it
        /// </summary>
        /// <param name="unit"></param>
        public void AddToAggroList(Unit unit)
        {
            // Log if this starts combat
            mBotOwner.Logger.Log($"Unit guid {unit.Guid} added to aggro.");
            if (!IsInCombat)
                mBotOwner.Logger.Log("Combat is now started due to mob in my aggro list!");
            // Shouldn't need to check if already exists, will update if it does
            mAggroList.AddOrUpdate(unit.Guid, unit);
        }

        public void StopCombat()
        {
            // Clear our aggro list
            mAggroList.Clear();
            // Clear our current target
            mTarget = null;
            // Stop attacking anything
            StopAttack();
        }

        /// <summary>
        /// Adds all marked NPC targets that are alive to the aggro list
        /// </summary>
        public void AddMarkedTargetsToAggroList()
        {
            AddMarkedTargetToAggroList(RaidTargetMarkers.SKULL);
            AddMarkedTargetToAggroList(RaidTargetMarkers.CROSS);
            AddMarkedTargetToAggroList(RaidTargetMarkers.STAR);
            AddMarkedTargetToAggroList(RaidTargetMarkers.CIRCLE);
            AddMarkedTargetToAggroList(RaidTargetMarkers.SQUARE);
            AddMarkedTargetToAggroList(RaidTargetMarkers.DIAMOND);
            AddMarkedTargetToAggroList(RaidTargetMarkers.MOON);
            AddMarkedTargetToAggroList(RaidTargetMarkers.TRIANGLE);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Reports a unit that was killed
        /// </summary>
        /// <param name="guid"></param>
        internal void UnitKilled(WoWGuid guid)
        {
            // Remove from our aggro list
            mAggroList.Remove(guid);
            mBotOwner.Logger.Log($"Removed unit with guid {guid} from aggro list. Aggro list currently contains {mAggroList.Count} mobs");
        }

        /// <summary>
        /// Updates the combat state each tick
        /// </summary>
        /// <param name="deltaTime"></param>
        internal void UpdateState(float deltaTime)
        {
            var inCombat = IsInCombat;

            // Remove any dead units from aggro, or units that are no longer in our objects or too far away
            mAggroList.RemoveUnits(mBotOwner);

            // If our current target is dead, remove it
            if (CurrentTarget != null && CurrentTarget.IsDead)
                ClearTarget();

            // TODO: If these are resurrection spells, we don't want to stop
            // If we are currently casting and our target is dead, stop casting
            if (IsCasting && (CurrentTarget == null || CurrentTarget.IsDead))
                CancelSpellCast();

            // If we were in combat, but we are no longer.
            if (inCombat && !IsInCombat) mBotOwner.Logger.Log("Combat has ended");
        }

        /// <summary>
        /// Updates the raid target assigned to the object/guid
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="guid"></param>
        internal void UpdateRaidTarget(RaidTargetMarkers icon, WoWGuid guid)
        {
            // Check if the icon key exists yet, if not add it
            if (!mMarkedTargets.ContainsKey(icon))
                mMarkedTargets.TryAdd(icon, null);

            // Always try get the current in case we need to update
            WoWGuid current = null;
            mMarkedTargets.TryGetValue(icon, out current);

            // If guid is 0, clear the marker
            if (guid.GetOldGuid() == 0)
            {
                mMarkedTargets.TryUpdate(icon, null, current);
                return;
            }

            // We have a guid so it is being assigned the icon
            mMarkedTargets.TryUpdate(icon, guid, current);
        }

        /// <summary>
        /// Notification when a spell cast has completed
        /// </summary>
        /// <param name="spellId"></param>
        internal void SpellCastComplete(uint spellId)
        {
            // If the current spell cast is complete
            if (mCastingSpellId == spellId)
                mCastingSpellId = 0;
        }

        /// <summary>
        /// Notification when a spell cast is interrupted
        /// </summary>
        /// <param name="spellId"></param>
        internal void SpellCastInterrupted(uint spellId)
        {
            // If the current spell cast is interrupted
            if (mCastingSpellId == spellId)
            {
                mCastingSpellId = 0;
                // Also reset the GCD
                mGlobalCooldown.ResetGCD();
            }
        }

        /// <summary>
        /// Turns on the attacking flag
        /// </summary>
        internal void StartAttack()
        {
            mIsAttacking = true;
        }

        /// <summary>
        /// Sets the attacking flag to false
        /// </summary>
        internal void StopAttack()
        {
            mIsAttacking = false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds a unit that is marked with a raid target to the aggro list
        /// </summary>
        /// <param name="marker"></param>
        private void AddMarkedTargetToAggroList(RaidTargetMarkers marker)
        {
            var unit = GetMarkedUnit(marker);
            if (unit != null && !unit.IsDead && unit.IsNPC && !unit.IsFriendlyTo(BotOwner))
                AddToAggroList(unit);
        }

        /// <summary>
        /// Selects the next target based on raid markers
        /// </summary>
        /// <returns></returns>
        private Unit GetTargetByRaidMarker()
        {
            var target = GetPrimaryDpsTarget();
            if (target == null || target.IsDead || target.IsFriendlyTo(BotOwner) || !target.IsNPC)
                target = GetMarkedUnit(RaidTargetMarkers.CROSS);
            if (target == null || target.IsDead || target.IsFriendlyTo(BotOwner) || !target.IsNPC)
                target = GetMarkedUnit(RaidTargetMarkers.STAR);
            if (target == null || target.IsDead || target.IsFriendlyTo(BotOwner) || !target.IsNPC)
                target = GetMarkedUnit(RaidTargetMarkers.CIRCLE);
            if (target == null || target.IsDead || target.IsFriendlyTo(BotOwner) || !target.IsNPC)
                target = GetMarkedUnit(RaidTargetMarkers.SQUARE);
            if (target == null || target.IsDead || target.IsFriendlyTo(BotOwner) || !target.IsNPC)
                return null;
            return target;
        }

        /// <summary>
        /// Cast a spell at a target
        /// </summary>
        /// <param name="target"></param>
        /// <param name="spellId"></param>
        private void CastSpell(Unit target, uint spellId)
        {
            var spell = SpellTable.Instance.getSpell(spellId);
            if (spell == null)
                return;

            // Face the target before we cast
            if (target.Guid != mBotOwner.Guid)
                mBotOwner.FaceTarget(target.Guid);

            // Cast the spell
            mBotOwner.CastSpellAbility(target.Guid, spellId);

            // Trigger the GCD
            mGlobalCooldown.TriggerGCD(spell);

            // Log what we are casting
            mBotOwner.Logger.Log($"Casting spell {spell.SpellName}");
        }

        /// <summary>
        /// Clears the current target
        /// </summary>
        private void ClearTarget()
        {
            mTarget = null;
            // Clear the attacking flag when we clear a target
            StopAttack();
        }

        #endregion
    }
}
