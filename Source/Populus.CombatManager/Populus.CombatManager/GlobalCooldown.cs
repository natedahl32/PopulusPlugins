using Populus.Core.DBC;
using Populus.Core.Shared;
using Populus.Core.World.Objects;

namespace Populus.CombatManager
{
    public class GlobalCooldown
    {
        #region Declarations

        private readonly Bot mBotOwner;
        private float mGCDTime;
        private uint? mGCDStartTime;

        #endregion

        #region Constructors

        internal GlobalCooldown(Bot bot)
        {
            mBotOwner = bot;
            GCDTime = 1500f;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether or not the global cooldown is currently active
        /// </summary>
        public bool IsGCDActive
        {
            get
            {
                // If we do not have a value for GCD start time, it is not active
                if (!mGCDStartTime.HasValue)
                    return false;
                // If the time has exceeded, it is not active
                if ((Time.MM_GetTime() - mGCDStartTime.Value) > mGCDTime)
                    return false;

                // We are on the GCD
                return true;
            }
        }

        /// <summary>
        /// Gets the amount of time to wait for the GCD to finish
        /// </summary>
        private float GCDTime
        {
            get { return mGCDTime; }
            set
            {
                mGCDTime = value;
                if (mGCDTime < 1000)
                    mGCDTime = 1000f;
                if (mGCDTime > 1500)
                    mGCDTime = 1500f;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the global cooldown
        /// </summary>
        internal void TriggerGCD(SpellEntry spell)
        {
            // Spells that don't have a start recovery time do not trigger the GCD
            if (spell.StartRecoveryTime == 0) return;
            GCDTime = spell.StartRecoveryTime * mBotOwner.CastSpeedMod;
            mGCDStartTime = Time.MM_GetTime();
        }

        /// <summary>
        /// Resets the GCD
        /// </summary>
        internal void ResetGCD()
        {
            mGCDStartTime = null;
        }

        #endregion
    }
}
