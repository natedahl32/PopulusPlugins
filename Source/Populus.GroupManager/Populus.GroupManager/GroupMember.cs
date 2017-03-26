using Populus.Core.Constants;
using Populus.Core.Shared;
using Populus.Core.World.Objects.Events;
using System.Collections.Generic;

namespace Populus.GroupManager
{
    /// <summary>
    /// Represents a member in a group
    /// </summary>
    public class GroupMember
    {
        #region Declarations

        // Contains pet data for this group member
        private GroupMember mPet;

        #endregion

        #region Constructors

        public GroupMember()
        {
            AuraIds = new List<ushort>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the guid of the group member
        /// </summary>
        public WoWGuid Guid { get; private set; }

        /// <summary>
        /// Gets the name of the group member
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets whether or not this group member is online
        /// </summary>
        public bool IsOnline { get; private set; }

        /// <summary>
        /// Gets the display/model id (pets only)
        /// </summary>
        public ushort DisplayId { get; private set; }

        /// <summary>
        /// Gets the current health of the group member
        /// </summary>
        public ushort CurrentHealth { get; private set; }

        /// <summary>
        /// Gets the maximum health of the group member
        /// </summary>
        public ushort MaxHealth { get; private set; }

        /// <summary>
        /// Gets the power type of the group member
        /// </summary>
        public Powers PowerType { get; private set; }

        /// <summary>
        /// Gets the current power level of the group member
        /// </summary>
        public ushort CurrentPower { get; private set; }

        /// <summary>
        /// Gets the maixmum power level of the group member
        /// </summary>
        public ushort MaxPower { get; private set; }

        /// <summary>
        /// Gets the level of the group member
        /// </summary>
        public ushort Level { get; private set; }

        /// <summary>
        /// Gets the zone id of the group member
        /// </summary>
        public ushort ZoneId { get; private set; }

        /// <summary>
        /// Gets the zone name of the group member
        /// </summary>
        public string Zone { get; private set; }

        /// <summary>
        /// Gets the position of the group member
        /// </summary>
        public Coordinate Position { get; private set; }

        /// <summary>
        /// List of all aura ids this group member has
        /// </summary>
        public IEnumerable<ushort> AuraIds { get; private set; }

        /// <summary>
        /// Gets the pet this group member controls. Null if no pet
        /// </summary>
        public GroupMember Pet { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the group member data from an event
        /// </summary>
        /// <param name="args"></param>
        public void Update(GroupMemberUpdateEventArgs args)
        {
            Guid = args.Guid;
            if (args.IsOnline.HasValue)
                IsOnline = args.IsOnline.Value;
            if (args.CurrentHealth.HasValue)
                CurrentHealth = args.CurrentHealth.Value;
            if (args.MaxHealth.HasValue)
                MaxHealth = args.MaxHealth.Value;
            if (args.PowerType.HasValue)
                PowerType = args.PowerType.Value;
            if (args.CurrentPower.HasValue)
                CurrentPower = args.CurrentPower.Value;
            if (args.MaxPower.HasValue)
                MaxPower = args.MaxPower.Value;
            if (args.Level.HasValue)
                Level = args.Level.Value;
            if (args.ZoneId.HasValue)
                ZoneId = args.ZoneId.Value;
            if (!string.IsNullOrEmpty(args.Zone))
                Zone = args.Zone;
            if (args.Position != null)
                Position = args.Position;
            if (args.AuraIds != null)
                AuraIds = args.AuraIds;

            // Pets
            if (args.PetGuid != null)
            {
                if (mPet == null)
                    mPet = new GroupMember();
                mPet.Guid = args.PetGuid;
            }

            // Only if we have a pet (we would know if we do at this point because we were passed guid)
            if (mPet != null)
            {
                if (!string.IsNullOrEmpty(args.PetName))
                    mPet.Name = args.PetName;
                if (args.PetDisplayId.HasValue)
                    mPet.DisplayId = args.PetDisplayId.Value;
                if (args.PetCurrentHealth.HasValue)
                    mPet.CurrentHealth = args.PetCurrentHealth.Value;
                if (args.PetMaximumHealth.HasValue)
                    mPet.MaxHealth = args.PetMaximumHealth.Value;
                if (args.PetPowerType.HasValue)
                    mPet.PowerType = args.PetPowerType.Value;
                if (args.PetCurrentPower.HasValue)
                    mPet.CurrentPower = args.PetCurrentPower.Value;
                if (args.PetMaximumPower.HasValue)
                    mPet.MaxPower = args.PetMaximumPower.Value;
                if (args.PetAuraIds != null)
                    mPet.AuraIds = args.PetAuraIds;
            }
        }

        #endregion
    }
}
