using FluentBehaviourTree;
using Populus.Core.World.Objects;
using System.Collections.Generic;

namespace Populus.GroupBot.Combat.Shaman
{
    public class ShamanCombatLogic : CombatLogicHandler
    {
        #region Declarations

        // ENHANCEMENT
        protected uint ROCKBITER_WEAPON,
               STONESKIN_TOTEM,
               LIGHTNING_SHIELD,
               FLAMETONGUE_WEAPON,
               STRENGTH_OF_EARTH_TOTEM,
               FOCUSED,
               FROSTBRAND_WEAPON,
               FROST_RESISTANCE_TOTEM,
               FLAMETONGUE_TOTEM,
               FIRE_RESISTANCE_TOTEM,
               WINDFURY_WEAPON,
               GROUNDING_TOTEM,
               NATURE_RESISTANCE_TOTEM,
               WIND_FURY_TOTEM,
               STORMSTRIKE,
               WRATH_OF_AIR_TOTEM,
               EARTH_ELEMENTAL_TOTEM,
               BLOODLUST;

        // RESTORATION
        protected uint HEALING_WAVE,
               LESSER_HEALING_WAVE,
               ANCESTRAL_SPIRIT,
               TREMOR_TOTEM,
               HEALING_STREAM_TOTEM,
               MANA_SPRING_TOTEM,
               CHAIN_HEAL,
               MANA_TIDE_TOTEM,
               EARTH_SHIELD,
               CURE_DISEASE_SHAMAN,
               CURE_POISON_SHAMAN,
               NATURES_SWIFTNESS_SHAMAN;

        // ELEMENTAL
        protected uint LIGHTNING_BOLT,
               EARTH_SHOCK,
               STONECLAW_TOTEM,
               FLAME_SHOCK,
               SEARING_TOTEM,
               PURGE,
               FIRE_NOVA_TOTEM,
               FROST_SHOCK,
               MAGMA_TOTEM,
               CHAIN_LIGHTNING,
               FIRE_ELEMENTAL_TOTEM,
               EARTHBIND_TOTEM,
               ELEMENTAL_MASTERY,
               ASTRAL_RECALL,
               CLEANSING_TOTEM,
               GHOST_WOLF,
               SENTRY_TOTEM,
               WATER_BREATHING;

        // totem buffs
        protected uint STRENGTH_OF_EARTH_EFFECT,
               FLAMETONGUE_EFFECT,
               MAGMA_TOTEM_EFFECT,
               STONECLAW_EFFECT,
               FIRE_RESISTANCE_EFFECT,
               FROST_RESISTANCE_EFFECT,
               GROUDNING_EFFECT,
               NATURE_RESISTANCE_EFFECT,
               STONESKIN_EFFECT,
               WINDFURY_EFFECT,
               WRATH_OF_AIR_EFFECT,
               CLEANSING_TOTEM_EFFECT,
               HEALING_STREAM_EFFECT,
               MANA_SPRING_EFFECT,
               TREMOR_TOTEM_EFFECT,
               EARTHBIND_EFFECT;

        #endregion

        #region Constructors

        public ShamanCombatLogic(GroupBotHandler botHandler) : base(botHandler)
        {

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
        /// Gets all shaman spells and abilities available by level
        /// </summary>
        protected override Dictionary<int, List<uint>> SpellsByLevel => mSpellsByLevel;

        #endregion

        #region Public Methods

        public override void InitializeSpells()
        {
            base.InitializeSpells();

            // Spells
            ANCESTRAL_SPIRIT = InitSpell(Spells.ANCESTRAL_SPIRIT_1);
            ASTRAL_RECALL = InitSpell(Spells.ASTRAL_RECALL_1);
            BLOODLUST = InitSpell(Spells.BLOODLUST_1);
            CHAIN_HEAL = InitSpell(Spells.CHAIN_HEAL_1);
            CHAIN_LIGHTNING = InitSpell(Spells.CHAIN_LIGHTNING_1);
            CLEANSING_TOTEM = InitSpell(Spells.CLEANSING_TOTEM_1);
            CURE_DISEASE_SHAMAN = InitSpell(Spells.CURE_DISEASE_SHAMAN_1);
            CURE_POISON_SHAMAN = InitSpell(Spells.CURE_POISON_SHAMAN_1);
            EARTH_ELEMENTAL_TOTEM = InitSpell(Spells.EARTH_ELEMENTAL_TOTEM_1);
            EARTH_SHIELD = InitSpell(Spells.EARTH_SHIELD_1);
            EARTH_SHOCK = InitSpell(Spells.EARTH_SHOCK_1);
            EARTHBIND_TOTEM = InitSpell(Spells.EARTHBIND_TOTEM_1);
            ELEMENTAL_MASTERY = InitSpell(Spells.ELEMENTAL_MASTERY_1);
            FIRE_ELEMENTAL_TOTEM = InitSpell(Spells.FIRE_ELEMENTAL_TOTEM_1);
            FIRE_NOVA_TOTEM = InitSpell(Spells.FIRE_NOVA_1);
            FIRE_RESISTANCE_TOTEM = InitSpell(Spells.FIRE_RESISTANCE_TOTEM_1);
            FLAME_SHOCK = InitSpell(Spells.FLAME_SHOCK_1);
            FLAMETONGUE_TOTEM = InitSpell(Spells.FLAMETONGUE_TOTEM_1);
            FLAMETONGUE_WEAPON = InitSpell(Spells.FLAMETONGUE_WEAPON_1);
            FROST_RESISTANCE_TOTEM = InitSpell(Spells.FROST_RESISTANCE_TOTEM_1);
            FROST_SHOCK = InitSpell(Spells.FROST_SHOCK_1);
            FROSTBRAND_WEAPON = InitSpell(Spells.FROSTBRAND_WEAPON_1);
            GHOST_WOLF = InitSpell(Spells.GHOST_WOLF_1);
            GROUNDING_TOTEM = InitSpell(Spells.GROUNDING_TOTEM_1);
            HEALING_STREAM_TOTEM = InitSpell(Spells.HEALING_STREAM_TOTEM_1);
            HEALING_WAVE = InitSpell(Spells.HEALING_WAVE_1);
            LESSER_HEALING_WAVE = InitSpell(Spells.LESSER_HEALING_WAVE_1);
            LIGHTNING_BOLT = InitSpell(Spells.LIGHTNING_BOLT_1);
            LIGHTNING_SHIELD = InitSpell(Spells.LIGHTNING_SHIELD_1);
            MAGMA_TOTEM = InitSpell(Spells.MAGMA_TOTEM_1);
            MANA_SPRING_TOTEM = InitSpell(Spells.MANA_SPRING_TOTEM_1);
            MANA_TIDE_TOTEM = InitSpell(Spells.MANA_TIDE_TOTEM_1);
            NATURE_RESISTANCE_TOTEM = InitSpell(Spells.NATURE_RESISTANCE_TOTEM_1);
            NATURES_SWIFTNESS_SHAMAN = InitSpell(Spells.NATURES_SWIFTNESS_SHAMAN_1);
            PURGE = InitSpell(Spells.PURGE_1);
            ROCKBITER_WEAPON = InitSpell(Spells.ROCKBITER_WEAPON_1);
            SEARING_TOTEM = InitSpell(Spells.SEARING_TOTEM_1);
            SENTRY_TOTEM = InitSpell(Spells.SENTRY_TOTEM_1);
            STONECLAW_TOTEM = InitSpell(Spells.STONECLAW_TOTEM_1);
            STONESKIN_TOTEM = InitSpell(Spells.STONESKIN_TOTEM_1);
            STORMSTRIKE = InitSpell(Spells.STORMSTRIKE_1);
            STRENGTH_OF_EARTH_TOTEM = InitSpell(Spells.STRENGTH_OF_EARTH_TOTEM_1);
            TREMOR_TOTEM = InitSpell(Spells.TREMOR_TOTEM_1);
            WATER_BREATHING = InitSpell(Spells.WATER_BREATHING_1);
            WIND_FURY_TOTEM = InitSpell(Spells.WINDFURY_TOTEM_1);
            WINDFURY_WEAPON = InitSpell(Spells.WINDFURY_WEAPON_1);
            WRATH_OF_AIR_TOTEM = InitSpell(Spells.WRATH_OF_AIR_TOTEM_1);

            // Totem effects
            STRENGTH_OF_EARTH_EFFECT = InitSpell(Spells.STRENGTH_OF_EARTH_EFFECT_1);
            FLAMETONGUE_EFFECT = InitSpell(Spells.FLAMETONGUE_EFFECT_1);
            MAGMA_TOTEM_EFFECT = InitSpell(Spells.MAGMA_TOTEM_EFFECT_1);
            STONECLAW_EFFECT = InitSpell(Spells.STONECLAW_EFFECT_1);
            FIRE_RESISTANCE_EFFECT = InitSpell(Spells.FIRE_RESISTANCE_EFFECT_1);
            FROST_RESISTANCE_EFFECT = InitSpell(Spells.FROST_RESISTANCE_EFFECT_1);
            GROUDNING_EFFECT = InitSpell(Spells.GROUDNING_EFFECT_1);
            NATURE_RESISTANCE_EFFECT = InitSpell(Spells.NATURE_RESISTANCE_EFFECT_1);
            STONESKIN_EFFECT = InitSpell(Spells.STONESKIN_EFFECT_1);
            WINDFURY_EFFECT = InitSpell(Spells.WINDFURY_EFFECT_1);
            WRATH_OF_AIR_EFFECT = InitSpell(Spells.WRATH_OF_AIR_EFFECT_1);
            CLEANSING_TOTEM_EFFECT = InitSpell(Spells.CLEANSING_TOTEM_EFFECT_1);
            MANA_SPRING_EFFECT = InitSpell(Spells.MANA_SPRING_EFFECT_1);
            TREMOR_TOTEM_EFFECT = InitSpell(Spells.TREMOR_TOTEM_EFFECT_1);
            EARTHBIND_EFFECT = InitSpell(Spells.EARTHBIND_EFFECT_1);
        }

        public override bool Pull(Unit unit)
        {
            // TODO: Pull logic
            return false;
        }

        /// <summary>
        /// Checks for required shaman skills and if not found, learns them
        /// </summary>
        internal override void CheckForSkills()
        {
            if (!BotHandler.BotOwner.HasSkill(Core.Constants.SkillType.SKILL_2H_AXES))
                BotHandler.BotOwner.ChatSay(".learn 197");
            if (!BotHandler.BotOwner.HasSkill(Core.Constants.SkillType.SKILL_AXES))
                BotHandler.BotOwner.ChatSay(".learn 196");
            if (!BotHandler.BotOwner.HasSkill(Core.Constants.SkillType.SKILL_2H_MACES))
                BotHandler.BotOwner.ChatSay(".learn 199");
            if (!BotHandler.BotOwner.HasSkill(Core.Constants.SkillType.SKILL_MACES))
                BotHandler.BotOwner.ChatSay(".learn 198");
            if (!BotHandler.BotOwner.HasSkill(Core.Constants.SkillType.SKILL_STAVES))
                BotHandler.BotOwner.ChatSay(".learn 227");
            if (!BotHandler.BotOwner.HasSkill(Core.Constants.SkillType.SKILL_DAGGERS))
                BotHandler.BotOwner.ChatSay(".learn 1180");
            if (!BotHandler.BotOwner.HasSkill(Core.Constants.SkillType.SKILL_FIST_WEAPONS))
                BotHandler.BotOwner.ChatSay(".learn 15590");
            if (BotHandler.BotOwner.Level >= 40 && !BotHandler.BotOwner.HasSkill(Core.Constants.SkillType.SKILL_MAIL))
                BotHandler.BotOwner.ChatSay(".learn 8737");

            base.CheckForSkills();
        }

        #endregion

        #region Private Methods

        protected override IBehaviourTreeNode InitializeCombatBehaivor()
        {
            return null;
        }

        protected override IBehaviourTreeNode InitializeOutOfCombatBehavior()
        {
            return null;
        }

        #endregion

        #region Shaman Constants

        // Key = Level, Values = List of spells attained at that level
        private static Dictionary<int, List<uint>> mSpellsByLevel = new Dictionary<int, List<uint>>
        {
            { 4, new List<uint> { Spells.STONESKIN_TOTEM_1, Spells.EARTH_SHOCK_1 } },
            { 6, new List<uint> { Spells.HEALING_WAVE_2, Spells.EARTHBIND_TOTEM_1 } },
            { 8, new List<uint> { Spells.LIGHTNING_SHIELD_1, Spells.ROCKBITER_WEAPON_2, Spells.STONECLAW_TOTEM_1, Spells.EARTH_SHOCK_2, Spells.LIGHTNING_BOLT_2 } },
            { 10, new List<uint> { Spells.FLAMETONGUE_WEAPON_1, Spells.STRENGTH_OF_EARTH_TOTEM_1, Spells.FLAME_SHOCK_1, Spells.SEARING_TOTEM_1 } },
            { 12, new List<uint> { Spells.HEALING_WAVE_3, Spells.ANCESTRAL_SPIRIT_1, Spells.PURGE_1, Spells.FIRE_NOVA_1 } },
            { 14, new List<uint> { Spells.STONESKIN_TOTEM_2, Spells.EARTH_SHOCK_3, Spells.LIGHTNING_BOLT_3 } },
            { 16, new List<uint> { Spells.LIGHTNING_SHIELD_2, Spells.ROCKBITER_WEAPON_3 } },
            { 18, new List<uint> { Spells.FLAMETONGUE_WEAPON_2, Spells.HEALING_WAVE_4, Spells.TREMOR_TOTEM_1, Spells.STONECLAW_TOTEM_2, Spells.FLAME_SHOCK_2 } },
            { 20, new List<uint> { Spells.GHOST_WOLF_1, Spells.FROSTBRAND_WEAPON_1, Spells.LESSER_HEALING_WAVE_1, Spells.HEALING_STREAM_TOTEM_1, Spells.FROST_SHOCK_1, Spells.LIGHTNING_BOLT_4, Spells.SEARING_TOTEM_2 } },
            { 22, new List<uint> { Spells.WATER_BREATHING_1, Spells.FIRE_NOVA_2 } },
            { 24, new List<uint> { Spells.LIGHTNING_SHIELD_3, Spells.ROCKBITER_WEAPON_4, Spells.FROST_RESISTANCE_TOTEM_1, Spells.STONESKIN_TOTEM_3, Spells.STRENGTH_OF_EARTH_TOTEM_2, Spells.HEALING_WAVE_5, Spells.ANCESTRAL_SPIRIT_2, Spells.EARTH_SHOCK_4 } },
            { 26, new List<uint> { Spells.FAR_SIGHT_1, Spells.FLAMETONGUE_WEAPON_3, Spells.MANA_SPRING_TOTEM_1, Spells.LIGHTNING_BOLT_5, Spells.MAGMA_TOTEM_1 } },
            { 28, new List<uint> { Spells.FLAMETONGUE_TOTEM_1, Spells.FROSTBRAND_WEAPON_2, Spells.FIRE_RESISTANCE_TOTEM_1, Spells.WATER_WALKING_1, Spells.LESSER_HEALING_WAVE_2, Spells.STONECLAW_TOTEM_3, Spells.FLAME_SHOCK_3 } },
            { 30, new List<uint> { Spells.WINDFURY_WEAPON_1, Spells.GROUNDING_TOTEM_1, Spells.NATURE_RESISTANCE_TOTEM_1, Spells.ASTRAL_RECALL_1, Spells.HEALING_STREAM_TOTEM_2, Spells.REINCARNATION_1, Spells.SEARING_TOTEM_3 } },
            { 32, new List<uint> { Spells.LIGHTNING_SHIELD_4, Spells.WINDFURY_TOTEM_1, Spells.HEALING_WAVE_6, Spells.PURGE_2, Spells.LIGHTNING_BOLT_6, Spells.CHAIN_LIGHTNING_1, Spells.FIRE_NOVA_3 } },
            { 34, new List<uint> { Spells.SENTRY_TOTEM_1, Spells.STONESKIN_TOTEM_4, Spells.FROST_SHOCK_2 } },
            { 36, new List<uint> { Spells.FLAMETONGUE_WEAPON_4, Spells.LESSER_HEALING_WAVE_3, Spells.MANA_SPRING_TOTEM_2, Spells.ANCESTRAL_SPIRIT_3, Spells.EARTH_SHOCK_5, Spells.MAGMA_TOTEM_2 } },
            { 38, new List<uint> { Spells.FROST_RESISTANCE_TOTEM_2, Spells.FLAMETONGUE_TOTEM_2, Spells.STRENGTH_OF_EARTH_TOTEM_3, Spells.FROSTBRAND_WEAPON_3, Spells.DISEASE_CLEANSING_TOTEM_1, Spells.STONECLAW_TOTEM_4, Spells.LIGHTNING_BOLT_7 } },
            { 40, new List<uint> { Spells.LIGHTNING_SHIELD_5, Spells.WINDFURY_WEAPON_2, Spells.STORMSTRIKE_1, Spells.HEALING_WAVE_7, Spells.HEALING_STREAM_TOTEM_3, Spells.CHAIN_HEAL_1, Spells.MANA_TIDE_TOTEM_1, Spells.BLOODLUST_1, Spells.FLAME_SHOCK_4, Spells.CHAIN_LIGHTNING_2, Spells.SEARING_TOTEM_4 } },
            { 42, new List<uint> { Spells.FIRE_RESISTANCE_TOTEM_2, Spells.FIRE_NOVA_4, } },
            { 44, new List<uint> { Spells.NATURE_RESISTANCE_TOTEM_2, Spells.STONESKIN_TOTEM_5, Spells.LESSER_HEALING_WAVE_4, Spells.LIGHTNING_BOLT_8  } },
            { 46, new List<uint> { Spells.FLAMETONGUE_WEAPON_5, Spells.MANA_SPRING_TOTEM_3, Spells.CHAIN_HEAL_2, Spells.FROST_SHOCK_3, Spells.MAGMA_TOTEM_3 } },
            { 48, new List<uint> { Spells.LIGHTNING_SHIELD_6, Spells.FROSTBRAND_WEAPON_4, Spells.FLAMETONGUE_TOTEM_3, Spells.HEALING_WAVE_8, Spells.ANCESTRAL_SPIRIT_4, Spells.EARTH_SHOCK_6, Spells.STONECLAW_TOTEM_5, Spells.CHAIN_LIGHTNING_3 } },
            { 50, new List<uint> { Spells.WINDFURY_WEAPON_3, Spells.HEALING_STREAM_TOTEM_4, Spells.LIGHTNING_BOLT_9, Spells.SEARING_TOTEM_5 } },
            { 52, new List<uint> { Spells.STRENGTH_OF_EARTH_TOTEM_4, Spells.LESSER_HEALING_WAVE_5, Spells.FLAME_SHOCK_5, Spells.FIRE_NOVA_5 } },
            { 54, new List<uint> { Spells.FROST_RESISTANCE_TOTEM_3, Spells.STONESKIN_TOTEM_6, Spells.CHAIN_HEAL_3 } },
            { 56, new List<uint> { Spells.LIGHTNING_SHIELD_7, Spells.FLAMETONGUE_WEAPON_6, Spells.HEALING_WAVE_9, Spells.MANA_SPRING_TOTEM_4, Spells.LIGHTNING_BOLT_10, Spells.CHAIN_LIGHTNING_4, Spells.MAGMA_TOTEM_4 } },
            { 58, new List<uint> { Spells.FIRE_RESISTANCE_TOTEM_3, Spells.FROSTBRAND_WEAPON_5, Spells.FLAMETONGUE_TOTEM_4, Spells.STONECLAW_TOTEM_6, Spells.FROST_SHOCK_4 } },
            { 60, new List<uint> { Spells.NATURE_RESISTANCE_TOTEM_3, Spells.WINDFURY_WEAPON_4, Spells.STRENGTH_OF_EARTH_TOTEM_5, Spells.HEALING_STREAM_TOTEM_5, Spells.LESSER_HEALING_WAVE_6, Spells.ANCESTRAL_SPIRIT_5, Spells.HEALING_WAVE_10, Spells.EARTH_SHOCK_7, Spells.SEARING_TOTEM_6, Spells.FLAME_SHOCK_6 } }
        };

        public static class Reagents
        {
            public const uint ANKH = 17030;
        }

        public static class Spells
        {
            public const uint ANCESTRAL_SPIRIT_1 = 2008;
            public const uint ANCESTRAL_SPIRIT_2 = 20609;
            public const uint ANCESTRAL_SPIRIT_3 = 20610;
            public const uint ANCESTRAL_SPIRIT_4 = 20776;
            public const uint ANCESTRAL_SPIRIT_5 = 20777;
            public const uint ASTRAL_RECALL_1 = 556;
            public const uint BLOODLUST_1 = 2825;
            public const uint CHAIN_HEAL_1 = 1064;
            public const uint CHAIN_HEAL_2 = 10622;
            public const uint CHAIN_HEAL_3 = 10623;
            public const uint CHAIN_LIGHTNING_1 = 421;
            public const uint CHAIN_LIGHTNING_2 = 930;
            public const uint CHAIN_LIGHTNING_3 = 2860;
            public const uint CHAIN_LIGHTNING_4 = 10605;
            public const uint CLEANSING_TOTEM_1 = 8170;
            public const uint CURE_DISEASE_SHAMAN_1 = 2870;
            public const uint CURE_POISON_SHAMAN_1 = 526;
            public const uint DISEASE_CLEANSING_TOTEM_1 = 8170;
            public const uint EARTH_ELEMENTAL_TOTEM_1 = 2062;
            public const uint EARTH_SHIELD_1 = 974;
            public const uint EARTH_SHOCK_1 = 8042;
            public const uint EARTH_SHOCK_2 = 8044;
            public const uint EARTH_SHOCK_3 = 8045;
            public const uint EARTH_SHOCK_4 = 8046;
            public const uint EARTH_SHOCK_5 = 10412;
            public const uint EARTH_SHOCK_6 = 10413;
            public const uint EARTH_SHOCK_7 = 10414;
            public const uint EARTHBIND_TOTEM_1 = 2484;
            public const uint ELEMENTAL_MASTERY_1 = 16166;
            public const uint FAR_SIGHT_1 = 6196;
            public const uint FIRE_ELEMENTAL_TOTEM_1 = 2894;
            public const uint FIRE_NOVA_1 = 1535;
            public const uint FIRE_NOVA_2 = 8498;
            public const uint FIRE_NOVA_3 = 8499;
            public const uint FIRE_NOVA_4 = 11314;
            public const uint FIRE_NOVA_5 = 11315;
            public const uint FIRE_RESISTANCE_TOTEM_1 = 8184;
            public const uint FIRE_RESISTANCE_TOTEM_2 = 10537;
            public const uint FIRE_RESISTANCE_TOTEM_3 = 10538;
            public const uint FLAME_SHOCK_1 = 8050;
            public const uint FLAME_SHOCK_2 = 8052;
            public const uint FLAME_SHOCK_3 = 8053;
            public const uint FLAME_SHOCK_4 = 10447;
            public const uint FLAME_SHOCK_5 = 10448;
            public const uint FLAME_SHOCK_6 = 29228;
            public const uint FLAMETONGUE_TOTEM_1 = 8227;
            public const uint FLAMETONGUE_TOTEM_2 = 8249;
            public const uint FLAMETONGUE_TOTEM_3 = 10526;
            public const uint FLAMETONGUE_TOTEM_4 = 16387;
            public const uint FLAMETONGUE_WEAPON_1 = 8024;
            public const uint FLAMETONGUE_WEAPON_2 = 8027;
            public const uint FLAMETONGUE_WEAPON_3 = 8030;
            public const uint FLAMETONGUE_WEAPON_4 = 16339;
            public const uint FLAMETONGUE_WEAPON_5 = 16341;
            public const uint FLAMETONGUE_WEAPON_6 = 16342;
            public const uint FROST_RESISTANCE_TOTEM_1 = 8181;
            public const uint FROST_RESISTANCE_TOTEM_2 = 10478;
            public const uint FROST_RESISTANCE_TOTEM_3 = 10479;
            public const uint FROST_SHOCK_1 = 8056;
            public const uint FROST_SHOCK_2 = 8058;
            public const uint FROST_SHOCK_3 = 10472;
            public const uint FROST_SHOCK_4 = 10473;
            public const uint FROSTBRAND_WEAPON_1 = 8033;
            public const uint FROSTBRAND_WEAPON_2 = 8038;
            public const uint FROSTBRAND_WEAPON_3 = 10456;
            public const uint FROSTBRAND_WEAPON_4 = 16355;
            public const uint FROSTBRAND_WEAPON_5 = 16356;
            public const uint GHOST_WOLF_1 = 2645;
            public const uint GROUNDING_TOTEM_1 = 8177;
            public const uint HEALING_STREAM_TOTEM_1 = 5394;
            public const uint HEALING_STREAM_TOTEM_2 = 6375;
            public const uint HEALING_STREAM_TOTEM_3 = 6377;
            public const uint HEALING_STREAM_TOTEM_4 = 10462;
            public const uint HEALING_STREAM_TOTEM_5 = 10463;
            public const uint HEALING_WAVE_1 = 331;
            public const uint HEALING_WAVE_2 = 332;
            public const uint HEALING_WAVE_3 = 547;
            public const uint HEALING_WAVE_4 = 913;
            public const uint HEALING_WAVE_5 = 939;
            public const uint HEALING_WAVE_6 = 959;
            public const uint HEALING_WAVE_7 = 8005;
            public const uint HEALING_WAVE_8 = 10395;
            public const uint HEALING_WAVE_9 = 10396;
            public const uint HEALING_WAVE_10 = 25357;
            public const uint LESSER_HEALING_WAVE_1 = 8004;
            public const uint LESSER_HEALING_WAVE_2 = 8008;
            public const uint LESSER_HEALING_WAVE_3 = 8010;
            public const uint LESSER_HEALING_WAVE_4 = 10466;
            public const uint LESSER_HEALING_WAVE_5 = 10467;
            public const uint LESSER_HEALING_WAVE_6 = 10468;
            public const uint LIGHTNING_BOLT_1 = 403;
            public const uint LIGHTNING_BOLT_2 = 529;
            public const uint LIGHTNING_BOLT_3 = 548;
            public const uint LIGHTNING_BOLT_4 = 915;
            public const uint LIGHTNING_BOLT_5 = 943;
            public const uint LIGHTNING_BOLT_6 = 6041;
            public const uint LIGHTNING_BOLT_7 = 10391;
            public const uint LIGHTNING_BOLT_8 = 10392;
            public const uint LIGHTNING_BOLT_9 = 15207;
            public const uint LIGHTNING_BOLT_10 = 15208;
            public const uint LIGHTNING_SHIELD_1 = 324;
            public const uint LIGHTNING_SHIELD_2 = 325;
            public const uint LIGHTNING_SHIELD_3 = 905;
            public const uint LIGHTNING_SHIELD_4 = 945;
            public const uint LIGHTNING_SHIELD_5 = 8134;
            public const uint LIGHTNING_SHIELD_6 = 10431;
            public const uint LIGHTNING_SHIELD_7 = 10432;
            public const uint MAGMA_TOTEM_1 = 8190;
            public const uint MAGMA_TOTEM_2 = 10585;
            public const uint MAGMA_TOTEM_3 = 10586;
            public const uint MAGMA_TOTEM_4 = 10587;
            public const uint MANA_SPRING_TOTEM_1 = 5675;
            public const uint MANA_SPRING_TOTEM_2 = 10495;
            public const uint MANA_SPRING_TOTEM_3 = 10496;
            public const uint MANA_SPRING_TOTEM_4 = 10497;
            public const uint MANA_TIDE_TOTEM_1 = 16190;
            public const uint NATURE_RESISTANCE_TOTEM_1 = 10595;
            public const uint NATURE_RESISTANCE_TOTEM_2 = 10600;
            public const uint NATURE_RESISTANCE_TOTEM_3 = 10601;
            public const uint NATURES_SWIFTNESS_SHAMAN_1 = 16188;
            public const uint PURGE_1 = 370;
            public const uint PURGE_2 = 8012;
            public const uint REINCARNATION_1 = 21169;
            public const uint ROCKBITER_WEAPON_1 = 8017;
            public const uint ROCKBITER_WEAPON_2 = 8018;
            public const uint ROCKBITER_WEAPON_3 = 8019;
            public const uint ROCKBITER_WEAPON_4 = 10399;
            public const uint SEARING_TOTEM_1 = 3599;
            public const uint SEARING_TOTEM_2 = 6363;
            public const uint SEARING_TOTEM_3 = 6364;
            public const uint SEARING_TOTEM_4 = 6365;
            public const uint SEARING_TOTEM_5 = 10437;
            public const uint SEARING_TOTEM_6 = 10438;
            public const uint SENTRY_TOTEM_1 = 6495;
            public const uint STONECLAW_TOTEM_1 = 5730;
            public const uint STONECLAW_TOTEM_2 = 6390;
            public const uint STONECLAW_TOTEM_3 = 6391;
            public const uint STONECLAW_TOTEM_4 = 6392;
            public const uint STONECLAW_TOTEM_5 = 10427;
            public const uint STONECLAW_TOTEM_6 = 10428;
            public const uint STONESKIN_TOTEM_1 = 8071;
            public const uint STONESKIN_TOTEM_2 = 8154;
            public const uint STONESKIN_TOTEM_3 = 8155;
            public const uint STONESKIN_TOTEM_4 = 10406;
            public const uint STONESKIN_TOTEM_5 = 10407;
            public const uint STONESKIN_TOTEM_6 = 10408;
            public const uint STORMSTRIKE_1 = 17364;
            public const uint STRENGTH_OF_EARTH_TOTEM_1 = 8075;
            public const uint STRENGTH_OF_EARTH_TOTEM_2 = 8160;
            public const uint STRENGTH_OF_EARTH_TOTEM_3 = 8161;
            public const uint STRENGTH_OF_EARTH_TOTEM_4 = 10442;
            public const uint STRENGTH_OF_EARTH_TOTEM_5 = 25361;
            public const uint TREMOR_TOTEM_1 = 8143;
            public const uint WATER_BREATHING_1 = 131;
            public const uint WATER_WALKING_1 = 546;
            public const uint WINDFURY_TOTEM_1 = 8512;
            public const uint WINDFURY_WEAPON_1 = 8232;
            public const uint WINDFURY_WEAPON_2 = 8235;
            public const uint WINDFURY_WEAPON_3 = 10486;
            public const uint WINDFURY_WEAPON_4 = 16362;
            public const uint WRATH_OF_AIR_TOTEM_1 = 3738;

            //Totem Buffs
            public const uint STRENGTH_OF_EARTH_EFFECT_1 = 8076;
            public const uint FLAMETONGUE_EFFECT_1 = 8026;
            public const uint MAGMA_TOTEM_EFFECT_1 = 8188;
            public const uint STONECLAW_EFFECT_1 = 5728;
            public const uint FIRE_RESISTANCE_EFFECT_1 = 8185;
            public const uint FROST_RESISTANCE_EFFECT_1 = 8182;
            public const uint GROUDNING_EFFECT_1 = 8178;
            public const uint NATURE_RESISTANCE_EFFECT_1 = 10596;
            public const uint STONESKIN_EFFECT_1 = 8072;
            public const uint WINDFURY_EFFECT_1 = 8515;
            public const uint WRATH_OF_AIR_EFFECT_1 = 2895;
            public const uint CLEANSING_TOTEM_EFFECT_1 = 8172;
            public const uint MANA_SPRING_EFFECT_1 = 5677;
            public const uint TREMOR_TOTEM_EFFECT_1 = 8145;
            public const uint EARTHBIND_EFFECT_1 = 6474;
        }

        #endregion
    }
}
