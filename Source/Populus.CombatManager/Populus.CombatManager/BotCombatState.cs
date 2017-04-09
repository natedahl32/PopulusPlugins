using Populus.Core.World.Objects;
using System;
using System.Linq;

namespace Populus.CombatManager
{
    public class BotCombatState
    {
        #region Declarations

        private readonly Bot mBotOwner;
        private readonly AggroList mAggroList = new AggroList();
        private Unit mTarget;

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
        /// Whether or not the bot is in combat
        /// </summary>
        public bool IsInCombat { get; private set; }

        /// <summary>
        /// Gets the current target for the bot
        /// </summary>
        public Unit CurrentTarget
        {
            get { return mTarget; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the combat state each tick
        /// </summary>
        /// <param name="deltaTime"></param>
        internal void UpdateState(float deltaTime)
        {
            mAggroList.RemoveDeadUnits();
            if (mAggroList.AggroUnits.Count() <= 0)
                IsInCombat = false;
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

        #endregion
    }
}
