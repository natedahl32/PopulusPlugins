using FluentBehaviourTree;
using Populus.Core.World.Objects;
using System.Collections.Generic;

namespace Populus.GroupBot.Combat.Warrior
{
    public class WarriorCombatLogic : CombatLogicHandler
    {
        #region Declarations

        // ARMS
        protected uint BATTLE_STANCE,
            CHARGE,
            HEROIC_STRIKE,
            REND,
            THUNDER_CLAP,
            HAMSTRING,
            MOCKING_BLOW,
            RETALIATION,
            SWEEPING_STRIKES,
            MORTAL_STRIKE,
            TASTE_FOR_BLOOD,
            SUDDEN_DEATH;

        // PROTECTION
        protected uint DEFENSIVE_STANCE,
            BLOODRAGE,
            SUNDER_ARMOR,
            TAUNT,
            SHIELD_BASH,
            REVENGE,
            SHIELD_BLOCK,
            DISARM,
            SHIELD_WALL,
            SHIELD_SLAM,
            CONCUSSION_BLOW,
            LAST_STAND;

        // FURY
        protected uint BERSERKER_STANCE,
            BATTLE_SHOUT,
            DEMORALIZING_SHOUT,
            OVERPOWER,
            CLEAVE,
            INTIMIDATING_SHOUT,
            EXECUTE,
            CHALLENGING_SHOUT,
            SLAM,
            INTERCEPT,
            DEATH_WISH,
            BERSERKER_RAGE,
            WHIRLWIND,
            PUMMEL,
            BLOODTHIRST,
            RECKLESSNESS,
            PIERCING_HOWL,
            SLAM_PROC,
            BLOODSURGE;

        // general
        protected uint SHOOT,
            SHOOT_BOW,
            SHOOT_GUN,
            SHOOT_XBOW;

        // flag that determines if heroic strike was used or not
        protected bool mHeroicStrikePrepared = false;

        // holds the threshold of rage we will use to determine if we can cast heroic strike or not
        private int mUseHeroicStrikeRageThreshold = 30;

        #endregion

        #region Constructors

        public WarriorCombatLogic(GroupBotHandler botHandler) : base(botHandler)
        {
            Bot.CombatAttackUpdate += CombatAttackUpdate;
        }

        ~WarriorCombatLogic()
        {
            Bot.CombatAttackUpdate -= CombatAttackUpdate;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether or not this class is primarily a melee class
        /// </summary>
        public override bool IsMelee
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets the threshold value of rage that will determine if we cast heroic strike or not
        /// </summary>
        protected int UseHeroicStrikeRageThreshold
        {
            get { return mUseHeroicStrikeRageThreshold; }
            set { mUseHeroicStrikeRageThreshold = value; }
        }

        /// <summary>
        /// Gets all warrior spells and abilities available by level
        /// </summary>
        protected override Dictionary<int, List<uint>> SpellsByLevel => mSpellsByLevel;

        #endregion

        #region Public Methods

        public override void InitializeSpells()
        {
            base.InitializeSpells();

            // Abilities
            SHOOT_BOW = InitSpell(Spells.SHOOT_BOW_1);
            SHOOT_GUN = InitSpell(Spells.SHOOT_GUN_1);
            SHOOT_XBOW = InitSpell(Spells.SHOOT_XBOW_1);

            // Spells
            BATTLE_STANCE = InitSpell(Spells.BATTLE_STANCE_1);
            CHARGE = InitSpell(Spells.CHARGE_1);
            OVERPOWER = InitSpell(Spells.OVERPOWER_1);
            HEROIC_STRIKE = InitSpell(Spells.HEROIC_STRIKE_1);
            REND = InitSpell(Spells.REND_1);
            THUNDER_CLAP = InitSpell(Spells.THUNDER_CLAP_1);
            HAMSTRING = InitSpell(Spells.HAMSTRING_1);
            MOCKING_BLOW = InitSpell(Spells.MOCKING_BLOW_1);
            RETALIATION = InitSpell(Spells.RETALIATION_1);
            SWEEPING_STRIKES = InitSpell(Spells.SWEEPING_STRIKES_1);
            MORTAL_STRIKE = InitSpell(Spells.MORTAL_STRIKE_1);
            BLOODRAGE = InitSpell(Spells.BLOODRAGE_1);
            DEFENSIVE_STANCE = InitSpell(Spells.DEFENSIVE_STANCE_1);
            SUNDER_ARMOR = InitSpell(Spells.SUNDER_ARMOR_1);
            TAUNT = InitSpell(Spells.TAUNT_1);
            SHIELD_BASH = InitSpell(Spells.SHIELD_BASH_1);
            REVENGE = InitSpell(Spells.REVENGE_1);
            SHIELD_BLOCK = InitSpell(Spells.SHIELD_BLOCK_1);
            DISARM = InitSpell(Spells.DISARM_1);
            SHIELD_WALL = InitSpell(Spells.SHIELD_WALL_1);
            SHIELD_SLAM = InitSpell(Spells.SHIELD_SLAM_1);
            CONCUSSION_BLOW = InitSpell(Spells.CONCUSSION_BLOW_1);
            LAST_STAND = InitSpell(Spells.LAST_STAND_1);
            BATTLE_SHOUT = InitSpell(Spells.BATTLE_SHOUT_1);
            DEMORALIZING_SHOUT = InitSpell(Spells.DEMORALIZING_SHOUT_1);
            CLEAVE = InitSpell(Spells.CLEAVE_1);
            INTIMIDATING_SHOUT = InitSpell(Spells.INTIMIDATING_SHOUT_1);
            EXECUTE = InitSpell(Spells.EXECUTE_1);
            CHALLENGING_SHOUT = InitSpell(Spells.CHALLENGING_SHOUT_1);
            SLAM = InitSpell(Spells.SLAM_1);
            BERSERKER_STANCE = InitSpell(Spells.BERSERKER_STANCE_1);
            INTERCEPT = InitSpell(Spells.INTERCEPT_1);
            DEATH_WISH = InitSpell(Spells.DEATH_WISH_1);
            BERSERKER_RAGE = InitSpell(Spells.BERSERKER_RAGE_1);
            WHIRLWIND = InitSpell(Spells.WHIRLWIND_1);
            PUMMEL = InitSpell(Spells.PUMMEL_1);
            BLOODTHIRST = InitSpell(Spells.BLOODTHIRST_1);
            RECKLESSNESS = InitSpell(Spells.RECKLESSNESS_1);
            PIERCING_HOWL = InitSpell(Spells.PIERCING_HOWL_1);

            // Procs
            SLAM_PROC = InitSpell(Procs.SLAM_PROC_1);
            BLOODSURGE = InitSpell(Procs.BLOODSURGE_1);
            TASTE_FOR_BLOOD = InitSpell(Procs.TASTE_FOR_BLOOD_1);
            SUDDEN_DEATH = InitSpell(Procs.SUDDEN_DEATH_1);
        }

        #endregion

        #region Private Methods

        protected override IBehaviourTreeNode InitializeCombatBehaivor()
        {
            var builder = new BehaviourTreeBuilder();
            builder.Selector("Combat Behavior")
                        .Inverter("If Melee Attack Succeeds, Move On")
                            .Splice(MeleeAttack(BotHandler))
                        .End()
                        .Condition("Not casting", t => BotHandler.CombatState.IsCasting)
                        .Splice(CombatRotationTree())
                   .End();
            return builder.Build();
        }

        protected override IBehaviourTreeNode InitializeOutOfCombatBehavior()
        {
            var builder = new BehaviourTreeBuilder();
            builder.Selector("OOC Behavior")
                        .Splice(OutOfCombatLogic.OutOfCombatHealthRegen(BotHandler))
                        .Splice(OutOfCombatBuffsTree())
                        .Do("Follow Group Leader", t => OutOfCombatLogic.FollowGroupLeader(BotHandler))
                   .End();
            return builder.Build();
        }

        /// <summary>
        /// Creates a behavior tree with the combat rotation for the warrior class
        /// </summary>
        /// <returns></returns>
        protected virtual IBehaviourTreeNode CombatRotationTree()
        {
            var builder = new BehaviourTreeBuilder();
            builder.Selector("Warrior Rotation")
                        .Do("Battle Stance", t => SelfBuff(BATTLE_STANCE))
                        .Do("Battle Shout", t => SelfBuff(BATTLE_SHOUT))        // We might not have the rage to do this until combat
                        .Do("Heroic Strike", t => HeroicStrike(40f))
                   .End();
            return builder.Build();
        }

        /// <summary>
        /// Creates a behavior tree that handles out of combat buffs for the warrior class
        /// </summary>
        /// <returns></returns>
        protected virtual IBehaviourTreeNode OutOfCombatBuffsTree()
        {
            var builder = new BehaviourTreeBuilder();
            builder.Selector("Warrior Buffs")
                        .Do("Battle Stance", t => SelfBuff(BATTLE_STANCE))
                        .Do("Battle Shout", t => GroupBuff(BATTLE_SHOUT))
                   .End();
            return builder.Build();
        }

        /// <summary>
        /// Casts heroic strike at a certain rage level
        /// </summary>
        /// <param name="atRageLevel">At or above the rage level to cast Heroic Strike</param>
        /// <returns></returns>
        protected BehaviourTreeStatus HeroicStrike(float atRageLevel)
        {
            // If it's already prepared for the next melee swing, fail
            if (mHeroicStrikePrepared)
                return BehaviourTreeStatus.Failure;
            // If we are not within range
            if (!IsInMeleeRange(BotHandler.CombatState.CurrentTarget))
                return BehaviourTreeStatus.Failure;
            // If we can't cast it, fail
            if (!HasSpellAndCanCast(HEROIC_STRIKE))
                return BehaviourTreeStatus.Failure;
            // If we are not at or above the specified rage level, fail
            if (BotHandler.BotOwner.PowerPercentage < atRageLevel)
                return BehaviourTreeStatus.Failure;

            mHeroicStrikePrepared = true;
            BotHandler.CombatState.SpellCast(HEROIC_STRIKE);
            return BehaviourTreeStatus.Success;
        }

        protected virtual void CombatAttackUpdate(Bot bot, Core.World.Objects.Events.CombatAttackUpdateArgs eventArgs)
        {
            // Reset heroic strike flag on each attack
            if (bot.Guid == BotHandler.BotOwner.Guid && eventArgs.AttackerGuid == BotHandler.BotOwner.Guid && mHeroicStrikePrepared)
                mHeroicStrikePrepared = false;
        }

        #endregion

        #region Warrior Constants

        // Key = Level, Values = List of spells attained at that level
        private static Dictionary<int, List<uint>> mSpellsByLevel = new Dictionary<int, List<uint>>
        {
            { 4, new List<uint> { Spells.REND_1, Spells.CHARGE_1  } },
            { 6, new List<uint> { Spells.THUNDER_CLAP_1 } },
            { 8, new List<uint> { Spells.HEROIC_STRIKE_2, Spells.HAMSTRING_1 } },
            { 10, new List<uint> { Spells.REND_2, Spells.BLOODRAGE_1, Spells.DEFENSIVE_STANCE_1, Spells.TAUNT_1, Spells.SUNDER_ARMOR_1 } },
            { 12, new List<uint> { Spells.OVERPOWER_1, Spells.BATTLE_SHOUT_2, Spells.SHIELD_BASH_1 } },
            { 14, new List<uint> { Spells.DEMORALIZING_SHOUT_1, Spells.REVENGE_1 } },
            { 16, new List<uint> { Spells.HEROIC_STRIKE_3, Spells.MOCKING_BLOW_1, Spells.SHIELD_BLOCK_1 } },
            { 18, new List<uint> { Spells.THUNDER_CLAP_2, Spells.DISARM_1 } },
            { 20, new List<uint> { Spells.REND_3, Spells.RETALIATION_1, Spells.CLEAVE_1 } },
            { 22, new List<uint> { Spells.BATTLE_SHOUT_3, Spells.INTIMIDATING_SHOUT_1 } },
            { 24, new List<uint> { Spells.HEROIC_STRIKE_4, Spells.DEMORALIZING_SHOUT_2, Spells.EXECUTE_1, Spells.REVENGE_2 } },
            { 26, new List<uint> { Spells.CHARGE_2, Spells.CHALLENGING_SHOUT_1 } },
            { 28, new List<uint> { Spells.THUNDER_CLAP_3, Spells.SHIELD_WALL_1 } },
            { 30, new List<uint> { Spells.REND_4, Spells.SLAM_1, Spells.BERSERKER_STANCE_1, Spells.CLEAVE_2, Spells.INTERCEPT_1 } },
            { 32, new List<uint> { Spells.HEROIC_STRIKE_5, Spells.BATTLE_SHOUT_4, Spells.BERSERKER_RAGE_1, Spells.EXECUTE_2 } },
            { 34, new List<uint> { Spells.DEMORALIZING_SHOUT_3, Spells.REVENGE_3 } },
            { 36, new List<uint> { Spells.WHIRLWIND_1 } },
            { 38, new List<uint> { Spells.THUNDER_CLAP_4, Spells.SLAM_2, Spells.PUMMEL_1 } },
            { 40, new List<uint> { Spells.REND_5, Spells.HEROIC_STRIKE_6, Spells.MORTAL_STRIKE_1, Spells.CLEAVE_3, Spells.EXECUTE_3, Spells.SHIELD_SLAM_1 } },
            { 42, new List<uint> { Spells.BATTLE_SHOUT_5 } },
            { 44, new List<uint> { Spells.DEMORALIZING_SHOUT_4, Spells.REVENGE_4 } },
            { 46, new List<uint> { Spells.CHARGE_3, Spells.SLAM_3 } },
            { 48, new List<uint> { Spells.HEROIC_STRIKE_7, Spells.THUNDER_CLAP_5, Spells.MORTAL_STRIKE_2, Spells.EXECUTE_4, Spells.SHIELD_SLAM_2 } },
            { 50, new List<uint> { Spells.REND_6, Spells.CLEAVE_4, Spells.RECKLESSNESS_1 } },
            { 52, new List<uint> { Spells.BATTLE_SHOUT_6 } },
            { 54, new List<uint> { Spells.MORTAL_STRIKE_3, Spells.SLAM_4, Spells.DEMORALIZING_SHOUT_5, Spells.REVENGE_5, Spells.SHIELD_SLAM_3 } },
            { 56, new List<uint> { Spells.HEROIC_STRIKE_8, Spells.EXECUTE_5 } },
            { 58, new List<uint> { Spells.THUNDER_CLAP_6 } },
            { 60, new List<uint> { Spells.REND_7, Spells.MORTAL_STRIKE_4, Spells.HEROIC_STRIKE_9, Spells.CLEAVE_5, Spells.BATTLE_SHOUT_7, Spells.SHIELD_SLAM_4, Spells.REVENGE_6 } }
        };

        public static class Procs
        {
            public const uint SLAM_PROC_1 = 46916;
            public const uint BLOODSURGE_1 = 46915;
            public const uint TASTE_FOR_BLOOD_1 = 56638;
            public const uint SUDDEN_DEATH_1 = 52437;
        }

        public static class Spells
        {
            public const uint BATTLE_SHOUT_1 = 6673;
            public const uint BATTLE_SHOUT_2 = 5242;
            public const uint BATTLE_SHOUT_3 = 6192;
            public const uint BATTLE_SHOUT_4 = 11549;
            public const uint BATTLE_SHOUT_5 = 11550;
            public const uint BATTLE_SHOUT_6 = 11551;
            public const uint BATTLE_SHOUT_7 = 25289;
            public const uint BATTLE_STANCE_1 = 2457;
            public const uint BERSERKER_RAGE_1 = 18499;
            public const uint BERSERKER_STANCE_1 = 2458;
            public const uint BLOODRAGE_1 = 2687;
            public const uint BLOODTHIRST_1 = 23881;
            public const uint CHALLENGING_SHOUT_1 = 1161;
            public const uint CHARGE_1 = 100;
            public const uint CHARGE_2 = 6178;
            public const uint CHARGE_3 = 11578;
            public const uint CLEAVE_1 = 845;
            public const uint CLEAVE_2 = 7369;
            public const uint CLEAVE_3 = 11608;
            public const uint CLEAVE_4 = 11609;
            public const uint CLEAVE_5 = 20569;
            public const uint CONCUSSION_BLOW_1 = 12809;
            public const uint DEATH_WISH_1 = 12292;
            public const uint DEFENSIVE_STANCE_1 = 71;
            public const uint DEMORALIZING_SHOUT_1 = 1160;
            public const uint DEMORALIZING_SHOUT_2 = 6190;
            public const uint DEMORALIZING_SHOUT_3 = 11554;
            public const uint DEMORALIZING_SHOUT_4 = 11555;
            public const uint DEMORALIZING_SHOUT_5 = 11556;
            public const uint DISARM_1 = 676;
            public const uint EXECUTE_1 = 5308;
            public const uint EXECUTE_2 = 20658;
            public const uint EXECUTE_3 = 20660;
            public const uint EXECUTE_4 = 20661;
            public const uint EXECUTE_5 = 20662;
            public const uint HAMSTRING_1 = 1715;
            public const uint HEROIC_STRIKE_1 = 78;
            public const uint HEROIC_STRIKE_2 = 284;
            public const uint HEROIC_STRIKE_3 = 285;
            public const uint HEROIC_STRIKE_4 = 1608;
            public const uint HEROIC_STRIKE_5 = 11564;
            public const uint HEROIC_STRIKE_6 = 11565;
            public const uint HEROIC_STRIKE_7 = 11566;
            public const uint HEROIC_STRIKE_8 = 11567;
            public const uint HEROIC_STRIKE_9 = 25286;
            public const uint INTERCEPT_1 = 20252;
            public const uint INTERVENE_1 = 3411;
            public const uint INTIMIDATING_SHOUT_1 = 5246;
            public const uint LAST_STAND_1 = 12975;
            public const uint MOCKING_BLOW_1 = 694;
            public const uint MORTAL_STRIKE_1 = 12294;
            public const uint MORTAL_STRIKE_2 = 21551;
            public const uint MORTAL_STRIKE_3 = 21552;
            public const uint MORTAL_STRIKE_4 = 21553;
            public const uint OVERPOWER_1 = 7384;
            public const uint PIERCING_HOWL_1 = 12323;
            public const uint PUMMEL_1 = 6552;
            public const uint RECKLESSNESS_1 = 1719;
            public const uint REND_1 = 772;
            public const uint REND_2 = 6546;
            public const uint REND_3 = 6547;
            public const uint REND_4 = 6548;
            public const uint REND_5 = 11572;
            public const uint REND_6 = 11573;
            public const uint REND_7 = 11574;
            public const uint RETALIATION_1 = 20230;
            public const uint REVENGE_1 = 6572;
            public const uint REVENGE_2 = 6574;
            public const uint REVENGE_3 = 7379;
            public const uint REVENGE_4 = 11600;
            public const uint REVENGE_5 = 11601;
            public const uint REVENGE_6 = 25288;
            public const uint SHIELD_BASH_1 = 72;
            public const uint SHIELD_BLOCK_1 = 2565;
            public const uint SHIELD_SLAM_1 = 23922;
            public const uint SHIELD_SLAM_2 = 23923;
            public const uint SHIELD_SLAM_3 = 23924;
            public const uint SHIELD_SLAM_4 = 23925;
            public const uint SHIELD_WALL_1 = 871;
            public const uint SHOOT_BOW_1 = 2480;
            public const uint SHOOT_GUN_1 = 7918;
            public const uint SHOOT_XBOW_1 = 7919;
            public const uint SLAM_1 = 1464;
            public const uint SLAM_2 = 8820;
            public const uint SLAM_3 = 11604;
            public const uint SLAM_4 = 11605;
            public const uint SUNDER_ARMOR_1 = 7386;
            public const uint SWEEPING_STRIKES_1 = 12328;
            public const uint TAUNT_1 = 355;
            public const uint THUNDER_CLAP_1 = 6343;
            public const uint THUNDER_CLAP_2 = 8198;
            public const uint THUNDER_CLAP_3 = 8204;
            public const uint THUNDER_CLAP_4 = 8205;
            public const uint THUNDER_CLAP_5 = 11580;
            public const uint THUNDER_CLAP_6 = 11581;
            public const uint WHIRLWIND_1 = 1680;
        }

        #endregion
    }
}
