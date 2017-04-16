using Populus.ActionManager;
using Populus.ActionManager.Actions;
using Populus.CombatManager.Actions;
using Populus.Core.Constants;
using Populus.Core.DBC;
using Populus.Core.Shared;
using Populus.Core.World.Objects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Populus.CombatManager
{
    public class BotCombatState
    {
        #region Declarations

        private const float MAX_CAST_DISTANCE = 80.0f;
        private const float CAST_RANGE_BUFFER = 1.5f;

        // holds the current objects marked with raid targets
        private ConcurrentDictionary<RaidTargetMarkers, WoWGuid> mMarkedTargets = new ConcurrentDictionary<RaidTargetMarkers, WoWGuid>();

        private readonly Bot mBotOwner;
        private ActionQueue mActionQueue;
        private readonly AggroList mAggroList = new AggroList();
        private Unit mTarget;

        // Id of the spell/ability that is currently being used (only set if the ability has a cast time)
        private uint mCastingSpellId = 0;
        // Id of the spell/ability that will be used next after the current spell/ability is completed. Can still be set if no cast time, because it still has to wait
        // for the current spell to be cast before it can be used.
        private uint mQueuedSpellId = 0;

        #endregion

        #region Constructors

        internal BotCombatState(Bot botOwner)
        {
            if (botOwner == null) throw new ArgumentNullException("botOwner");
            this.mBotOwner = botOwner;

            IsInCombat = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the action queue for this bot combat state
        /// </summary>
        private ActionQueue ActionQueue
        {
            get
            {
                if (mActionQueue == null)
                    mActionQueue = ActionManager.ActionManager.GetActionQueue(mBotOwner.Guid);
                return mActionQueue;
            }
        }

        /// <summary>
        /// Whether or not the bot is in combat
        /// </summary>
        public bool IsInCombat { get; private set; }

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
        /// Gets whether or not a spell is currently queued up to cast after the current one is finished
        /// </summary>
        public bool HasSpellQueued
        {
            get { return mQueuedSpellId > 0; }
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
                return mBotOwner.GetUnitByGuid(target);
            // Unable to get, return null
            return null;
        }

        /// <summary>
        /// Sends commands to attack the specified target with melee attacks. This will move the bot into melee range and keep the target on
        /// follow while continuing to attack the target until another command is given or the target is dead.
        /// </summary>
        public void AttackMelee(Unit target)
        {
            // First of all, set our combat flag
            IsInCombat = true;

            // Set our target and it to our aggro list
            mTarget = target;
            mAggroList.AddOrUpdate(target.Guid, target);
            mBotOwner.SetTarget(target.Guid);

            // Follow the target to move to it
            mBotOwner.SetFollow(target.Guid);

            // Attack the target with melee!
            mBotOwner.Attack(target.Guid);
        }

        /// <summary>
        /// Sends commands to cast a spell on the current target. If no current target exists, the spell will not be cast.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="spellId"></param>
        public void SpellCast(uint spellId)
        {
            SpellCast(CurrentTarget, spellId);
        }

        /// <summary>
        /// Sends commands to cast a spell on a target. If that target is hostile, this will place the bot into combat and add the target
        /// to the bots aggro list.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="spellId"></param>
        public void SpellCast(Unit target, uint spellId)
        {
            // If we have a spell we are casting and a spell that is queued up, we can't handle any more spell casts
            if (mCastingSpellId > 0 && mQueuedSpellId > 0) return;

            // Get the spell
            var spell = SpellTable.Instance.getSpell(spellId);
            if (spell == null)
                return;

            // Always set the target
            mTarget = target;
            mBotOwner.SetTarget(target.Guid);

            // If not within distance to cast, move to distance
            var dist = mBotOwner.DistanceFrom(target.Position);
            if (spell.MaximumRange.HasValue && dist > spell.MaximumRange.Value)
            {
                // If we are too far away, stop the cast
                if (dist > MAX_CAST_DISTANCE)
                    return;

                // Move to a spot that is within range
                ActionQueue.Add(new MoveTowardsObject(mBotOwner, mTarget, spell.MaximumRange.Value - CAST_RANGE_BUFFER));
            }

            // Check if hostile or not friendly (handles neutral enemy factions, like boars)
            if (mBotOwner.IsHostileTo(target) ||
                !mBotOwner.IsFriendlyTo(target))
            {
                // Set combat flag since this target is hostile
                IsInCombat = true;

                // Add the target to the aggro list
                mAggroList.AddOrUpdate(target.Guid, target);
            }

            // Cast the spell
            if (mCastingSpellId > 0)
                mQueuedSpellId = spellId;
            else
            {
                if (spell.CastTime > 0 || spell.CastingTimeIndex > 1)
                    mCastingSpellId = spellId;
            }
            ActionQueue.Add(new CastSpellAbility(mBotOwner, target, spellId));
        }

        /// <summary>
        /// Cancels the current spell being cast
        /// </summary>
        public void CancelSpellCast()
        {
            if (!IsCasting) return;
            mBotOwner.CancelCast(mCastingSpellId);
        }

        /// <summary>
        /// Adds a unit to the aggro list if they are not already on it
        /// </summary>
        /// <param name="unit"></param>
        public void AddToAggroList(Unit unit)
        {
            if (!mAggroList.Contains(unit.Guid))
                mAggroList.AddOrUpdate(unit.Guid, unit);
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
            // If this was our target, remove our target
            if (mTarget != null && mTarget.Guid == guid)
            {
                CancelSpellCast();
                mTarget = null;
            }
        }

        /// <summary>
        /// Updates the combat state each tick
        /// </summary>
        /// <param name="deltaTime"></param>
        internal void UpdateState(float deltaTime)
        {
            // Remove any dead units from aggro
            mAggroList.RemoveDeadUnits();
            if (mAggroList.AggroUnits.Count() <= 0)
                IsInCombat = false;
            else
                IsInCombat = true;
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
            {
                if (mQueuedSpellId > 0)
                {
                    mCastingSpellId = mQueuedSpellId;
                    mQueuedSpellId = 0;
                }
                else
                    mCastingSpellId = 0;
                return;
            }

            // If the queued spell cast is complete... huh? Maybe we are getting multiple completion events?
            if (mQueuedSpellId == spellId)
            {
                mQueuedSpellId = 0;
            }
        }

        /// <summary>
        /// Cancels all combat
        /// </summary>
        internal void CancelCombat()
        {
            IsInCombat = false;
            mAggroList.Clear();

            if (mTarget != null)
            {
                // If our current target is dead cancel any casts.
                if (mTarget.IsDead)
                    CancelSpellCast();
                mTarget = null;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Selects the next target based on raid markers
        /// </summary>
        /// <returns></returns>
        private Unit GetTargetByRaidMarker()
        {
            var target = GetPrimaryDpsTarget();
            if (target == null)
                target = GetMarkedUnit(RaidTargetMarkers.CROSS);
            if (target == null)
                target = GetMarkedUnit(RaidTargetMarkers.STAR);
            if (target == null)
                target = GetMarkedUnit(RaidTargetMarkers.CIRCLE);
            if (target == null)
                target = GetMarkedUnit(RaidTargetMarkers.SQUARE);
            return target;
        }

        #endregion
    }
}
