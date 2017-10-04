using Populus.Core.Constants;
using Populus.Core.Shared;
using Populus.Core.World.Objects;
using Populus.Core.World.Objects.Events;
using Populus.Core.World.Spells;
using System.Collections.Generic;
using System.Linq;

namespace Populus.GroupManager
{
    /// <summary>
    /// Represents a member in a group
    /// </summary>
    public class GroupMember
    {
        #region Declarations

        // Role constants
        public const string ROLE_TANK = "TANK";
        public const string ROLE_HEALER = "HEALER";
        public const string ROLE_ASSIST = "ASSIST";

        // Contains pet data for this group member
        private GroupMember mPet;
        // Contains aura data for this group member
        private List<ushort> mAuraIds = new List<ushort>();
        private object mAuraLock = new object();

        // Role of the group member
        private string mRoleName;

        #endregion

        #region Constructors

        public GroupMember()
        {
            AuraIds = new List<ushort>();
        }

        public GroupMember(Bot bot) : this()
        {
            AuraIds = bot.Auras.Select(a => (ushort)a.Spell.SpellId).ToList();
            Guid = bot.Guid;
            Name = bot.Name;
            IsOnline = true;
            CurrentHealth = (ushort)bot.CurrentHealth;
            MaxHealth = (ushort)bot.MaxHealth;
            PowerType = bot.PowerType;
            CurrentPower = (ushort)bot.CurrentPower;
            MaxPower = (ushort)bot.MaximumPower;
            Level = (ushort)bot.Level;
            MapId = bot.MapId;
            Position = bot.Position;
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
        /// Gets the group id/number that this group member belongs to (useful for raids with multiple groups)
        /// </summary>
        public byte? GroupId { get; private set; }

        /// <summary>
        /// Gets whether or not this group member is online
        /// </summary>
        public bool IsOnline { get; private set; }

        /// <summary>
        /// Gets whether or not this group member is PvP flagged
        /// </summary>
        public bool IsPvPFlagged { get; private set; }

        /// <summary>
        /// Gets whether or not this group member is dead
        /// </summary>
        public bool IsDead { get; private set; }

        /// <summary>
        /// Gets whether or not this group member is a ghost
        /// </summary>
        public bool IsGhost { get; private set; }

        /// <summary>
        /// Gets whether or not this group member is AFK
        /// </summary>
        public bool IsAFK { get; private set; }

        /// <summary>
        /// Gets whether or not this group member is DND
        /// </summary>
        public bool IsDND { get; private set; }

        /// <summary>
        /// Gets ehther or not the group members is an assistant
        /// </summary>
        public bool? IsAssistant { get; private set; }

        /// <summary>
        /// Gets the display/model id (pets only)
        /// </summary>
        public ushort? DisplayId { get; private set; }

        /// <summary>
        /// Gets the current health of the group member
        /// </summary>
        public ushort? CurrentHealth { get; private set; }

        /// <summary>
        /// Gets the maximum health of the group member
        /// </summary>
        public ushort? MaxHealth { get; private set; }

        /// <summary>
        /// Gets the power type of the group member
        /// </summary>
        public Powers? PowerType { get; private set; }

        /// <summary>
        /// Gets the current power level of the group member
        /// </summary>
        public ushort? CurrentPower { get; private set; }

        /// <summary>
        /// Gets the maixmum power level of the group member
        /// </summary>
        public ushort? MaxPower { get; private set; }

        /// <summary>
        /// Gets the level of the group member
        /// </summary>
        public ushort? Level { get; private set; }

        /// <summary>
        /// Gets the zone id of the group member
        /// </summary>
        public ushort? ZoneId { get; private set; }

        /// <summary>
        /// Gets the zone name of the group member
        /// </summary>
        public string Zone { get; private set; }

        /// <summary>
        /// Gets the id of the map this group member is on
        /// </summary>
        public uint MapId { get; private set; }

        /// <summary>
        /// Gets the name of the map this group member is on
        /// </summary>
        public string MapName { get; private set; }

        /// <summary>
        /// Gets the position of the group member
        /// </summary>
        public Coordinate Position { get; private set; }

        /// <summary>
        /// Gets whether or not this group member is in a healer role
        /// </summary>
        public bool IsHealer
        {
            get { return Role == ROLE_HEALER; }
        }

        /// <summary>
        /// Gets whether or not this group member is in a tank role
        /// </summary>
        public bool IsTank
        {
            get { return Role == ROLE_TANK; }
        }

        /// <summary>
        /// Gets the role this group member is in
        /// </summary>
        public string Role
        {
            get
            {
                if (string.IsNullOrEmpty(mRoleName))
                    return "None";
                return mRoleName.ToUpper();
            }
        }

        /// <summary>
        /// List of all aura ids this group member has
        /// </summary>
        public IEnumerable<ushort> AuraIds
        {
            get { return mAuraIds; }
            private set
            {
                lock(mAuraLock)
                    mAuraIds = value.ToList();
            }
        }

        /// <summary>
        /// Gets the pet this group member controls. Null if no pet
        /// </summary>
        public GroupMember Pet { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns whether or not the group member has the spell aura or not
        /// </summary>
        /// <param name="auraId"></param>
        /// <returns></returns>
        public bool HasAura(ushort auraId)
        {
            lock (mAuraLock)
                return AuraIds.Contains(auraId);
        }

        /// <summary>
        /// Updates an aura from a holder
        /// </summary>
        /// <param name="aura"></param>
        public void UpdateAura(AuraHolder aura)
        {
            lock (mAuraLock)
            {
                // If we do not have this aura, add it if the duration is greater than 0
                if (!mAuraIds.Contains((ushort)aura.Spell.SpellId))
                {
                    if (aura.Duration > 0)
                        mAuraIds.Add((ushort)aura.Spell.SpellId);
                }
                else
                {
                    if (aura.Duration <= 0)
                        mAuraIds.Remove((ushort)aura.Spell.SpellId);
                }
            }
        }

        /// <summary>
        /// Assigns a role to the group member
        /// </summary>
        /// <param name="role"></param>
        public void AssignRole(string role)
        {
            mRoleName = role;
        }

        /// <summary>
        /// Updates the group members position
        /// </summary>
        /// <param name="position"></param>
        internal void UpdatePosition(Coordinate position)
        {
            if (position != null)
                Position = position;
        }

        /// <summary>
        /// Updates the group membeer data from a group list event
        /// </summary>
        /// <param name="data"></param>
        internal void Update(GroupListEventArgs.GroupMemberListData data)
        {
            Name = data.Name;
            Guid = data.Guid;
            GroupId = data.GroupId;
            IsAssistant = data.IsAssistant;
            
            // Statuses
            if (data.Status.HasValue)
            {
                IsAFK = data.Status.Value.HasFlag(GroupMemberStatus.MEMBER_STATUS_AFK);
                IsDead = data.Status.Value.HasFlag(GroupMemberStatus.MEMBER_STATUS_DEAD);
                IsDND = data.Status.Value.HasFlag(GroupMemberStatus.MEMBER_STATUS_DND);
                IsGhost = data.Status.Value.HasFlag(GroupMemberStatus.MEMBER_STATUS_GHOST);
                IsOnline = data.Status.Value.HasFlag(GroupMemberStatus.MEMBER_STATUS_ONLINE);
                IsPvPFlagged = data.Status.Value.HasFlag(GroupMemberStatus.MEMBER_STATUS_PVP);
            }
        }

        /// <summary>
        /// Objects the group member data for this bot from an object. Called when an object is updated
        /// </summary>
        internal void UpdateFromObject(Bot bot)
        {
            var player = bot.GetPlayerByGuid(Guid);
            CurrentHealth = (ushort)player.CurrentHealth;
            MaxHealth = (ushort)player.MaxHealth;
            PowerType = player.PowerType;
            CurrentPower = (ushort)player.CurrentPower;
            MaxPower = (ushort)player.MaximumPower;
            Level = (ushort)player.Level;
            // Zone id and MapId ????
            Position = player.Position;
            AuraIds = player.Auras.Select(a => (ushort)a.Spell.SpellId).ToList();

            // Pet data ????
        }

        /// <summary>
        /// Updates the group member data from an event
        /// </summary>
        /// <param name="args"></param>
        internal void Update(GroupMemberUpdateEventArgs args)
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
            if (args.MapId.HasValue)
                MapId = args.MapId.Value;
            if (!string.IsNullOrEmpty(args.MapName))
                MapName = args.MapName;
            if (args.Position != null)
                Position = args.Position;
            if (args.AuraIds != null)
                lock(mAuraLock)
                    AuraIds = args.AuraIds;

            // Pets
            if (args.PetGuid != null)
            {
                if (mPet == null)
                    mPet = new GroupMember();
                mPet.Guid = args.PetGuid;
            }
            else
            {
                // No pet
                mPet = null;
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
