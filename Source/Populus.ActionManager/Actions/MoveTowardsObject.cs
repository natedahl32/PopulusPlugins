using Populus.Core.World.Objects;
using System;

namespace Populus.ActionManager.Actions
{
    public class MoveTowardsObject : Action
    {
        #region Declarations

        private const float NON_MOVE_BUFFER = 0.5f;
        private const float MOVE_BUFFER = 2.0f;

        private WorldObject mMoveTowardsObject;
        private float? mMaxDistance;

        #endregion

        #region Constructors

        public MoveTowardsObject(Bot bot, WorldObject wo) : this(bot, wo, null)
        {
        }

        public MoveTowardsObject(Bot bot, WorldObject wo, float? maxDistance) : base(bot)
        {
            if (wo == null) throw new ArgumentNullException("wo");

            mMoveTowardsObject = wo;
            mMaxDistance = maxDistance;
        }

        #endregion

        #region Properties

        // TOOD: This is dangerous. What if the bot gets stuck behind and object/wall. We should check to make sure they are always reducing the
        // distance between them and their target and if they aren't making progress after a certain length of time, trigger the timeout manually.
        public override int Timeout => -1;

        public override bool IsComplete
        {
            get
            {
                var distance = BotOwner.DistanceFrom(mMoveTowardsObject.Position);
                return (distance < mMaxDistance && !BotOwner.IsMoving);
            }
        }

        #endregion

        #region Public Methods

        public override void Completed()
        {
            base.Completed();
            // Stop following the target
            BotOwner.RemoveFollow();
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            if (!IsComplete)
                BotOwner.SetFollow(mMoveTowardsObject.Guid, mMaxDistance);
        }

        #endregion
    }
}
