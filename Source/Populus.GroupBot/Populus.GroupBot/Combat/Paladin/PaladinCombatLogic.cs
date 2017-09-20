using System.Collections.Generic;

namespace Populus.GroupBot.Combat.Paladin
{
    public class PaladinCombatLogic : CombatLogicHandler
    {
        #region Declarations

        // Retribution
        protected uint RETRIBUTION_AURA,
               SEAL_OF_COMMAND,
               GREATER_BLESSING_OF_WISDOM,
               GREATER_BLESSING_OF_MIGHT,
               BLESSING_OF_WISDOM,
               BLESSING_OF_MIGHT,
               HAMMER_OF_JUSTICE,
               RIGHTEOUS_FURY,
               JUDGEMENT;

        // Holy
        protected uint FLASH_OF_LIGHT,
               HOLY_LIGHT,
               DIVINE_SHIELD,
               HAMMER_OF_WRATH,
               CONSECRATION,
               CONCENTRATION_AURA,
               DIVINE_FAVOR,
               HOLY_SHOCK,
               HOLY_WRATH,
               LAY_ON_HANDS,
               EXORCISM,
               REDEMPTION,
               SEAL_OF_JUSTICE,
               SEAL_OF_LIGHT,
               SEAL_OF_RIGHTEOUSNESS,
               SEAL_OF_WISDOM,
               SEAL_OF_THE_CRUSADER,
               PURIFY,
               CLEANSE;

        // Protection
        protected uint GREATER_BLESSING_OF_KINGS,
               BLESSING_OF_KINGS,
               BLESSING_OF_PROTECTION,
               SHADOW_RESISTANCE_AURA,
               DEVOTION_AURA,
               FIRE_RESISTANCE_AURA,
               FROST_RESISTANCE_AURA,
               DEFENSIVE_STANCE,
               BERSERKER_STANCE,
               BATTLE_STANCE,
               DIVINE_SACRIFICE,
               DIVINE_PROTECTION,
               DIVINE_INTERVENTION,
               HOLY_SHIELD,
               AVENGERS_SHIELD,
               RIGHTEOUS_DEFENSE,
               BLESSING_OF_SANCTUARY,
               GREATER_BLESSING_OF_SANCTUARY,
               BLESSING_OF_SACRIFICE,
               SHIELD_OF_RIGHTEOUSNESS,
               HAND_OF_RECKONING,
               HAMMER_OF_THE_RIGHTEOUS,
               HAND_OF_FREEDOM,
               HAND_OF_SALVATION,
               REPENTANCE,
               SENSE_UNDEAD;

        // cannot be protected
        protected uint FORBEARANCE;

        //Non-Stacking buffs
        protected uint PRAYER_OF_SHADOW_PROTECTION;

        #endregion

        #region Constructors

        public PaladinCombatLogic(GroupBotHandler botHandler) : base(botHandler)
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
        /// Gets all paladin spells and abilities available by level
        /// </summary>
        protected override Dictionary<int, List<uint>> SpellsByLevel => mSpellsByLevel;

        #endregion

        #region Public Methods

        public override void InitializeSpells()
        {
            base.InitializeSpells();

            // Spells
            BLESSING_OF_KINGS = InitSpell(Spells.BLESSING_OF_KINGS_1);
            BLESSING_OF_MIGHT = InitSpell(Spells.BLESSING_OF_MIGHT_1);
            BLESSING_OF_SANCTUARY = InitSpell(Spells.BLESSING_OF_SANCTUARY_1);
            BLESSING_OF_WISDOM = InitSpell(Spells.BLESSING_OF_WISDOM_1);
            CLEANSE = InitSpell(Spells.CLEANSE_1);
            CONCENTRATION_AURA = InitSpell(Spells.CONCENTRATION_AURA_1);
            CONSECRATION = InitSpell(Spells.CONSECRATION_1);
            DEVOTION_AURA = InitSpell(Spells.DEVOTION_AURA_1);
            DIVINE_FAVOR = InitSpell(Spells.DIVINE_FAVOR_1);
            DIVINE_INTERVENTION = InitSpell(Spells.DIVINE_INTERVENTION_1);
            DIVINE_PROTECTION = InitSpell(Spells.DIVINE_PROTECTION_1);
            DIVINE_SHIELD = InitSpell(Spells.DIVINE_SHIELD_1);
            EXORCISM = InitSpell(Spells.EXORCISM_1);
            FIRE_RESISTANCE_AURA = InitSpell(Spells.FIRE_RESISTANCE_AURA_1);
            FLASH_OF_LIGHT = InitSpell(Spells.FLASH_OF_LIGHT_1);
            FROST_RESISTANCE_AURA = InitSpell(Spells.FROST_RESISTANCE_AURA_1);
            GREATER_BLESSING_OF_KINGS = InitSpell(Spells.GREATER_BLESSING_OF_KINGS_1);
            GREATER_BLESSING_OF_MIGHT = InitSpell(Spells.GREATER_BLESSING_OF_MIGHT_1);
            GREATER_BLESSING_OF_SANCTUARY = InitSpell(Spells.GREATER_BLESSING_OF_SANCTUARY_1);
            GREATER_BLESSING_OF_WISDOM = InitSpell(Spells.GREATER_BLESSING_OF_WISDOM_1);
            HAMMER_OF_JUSTICE = InitSpell(Spells.HAMMER_OF_JUSTICE_1);
            HAMMER_OF_WRATH = InitSpell(Spells.HAMMER_OF_WRATH_1);
            HAND_OF_FREEDOM = InitSpell(Spells.HAND_OF_FREEDOM_1);
            BLESSING_OF_PROTECTION = InitSpell(Spells.BLESSING_OF_PROTECTION_1);
            BLESSING_OF_SACRIFICE = InitSpell(Spells.BLESSING_OF_SACRIFICE_1);
            HAND_OF_SALVATION = InitSpell(Spells.HAND_OF_SALVATION_1);
            HOLY_LIGHT = InitSpell(Spells.HOLY_LIGHT_1);
            HOLY_SHIELD = InitSpell(Spells.HOLY_SHIELD_1);
            HOLY_SHOCK = InitSpell(Spells.HOLY_SHOCK_1);
            HOLY_WRATH = InitSpell(Spells.HOLY_WRATH_1);
            JUDGEMENT = InitSpell(Spells.JUDGEMENT_1);
            LAY_ON_HANDS = InitSpell(Spells.LAY_ON_HANDS_1);
            PURIFY = InitSpell(Spells.PURIFY_1);
            REDEMPTION = InitSpell(Spells.REDEMPTION_1);
            REPENTANCE = InitSpell(Spells.REPENTANCE_1);
            RETRIBUTION_AURA = InitSpell(Spells.RETRIBUTION_AURA_1);
            RIGHTEOUS_FURY = InitSpell(Spells.RIGHTEOUS_FURY_1);
            SEAL_OF_COMMAND = InitSpell(Spells.SEAL_OF_COMMAND_1);
            SEAL_OF_JUSTICE = InitSpell(Spells.SEAL_OF_JUSTICE_1);
            SEAL_OF_LIGHT = InitSpell(Spells.SEAL_OF_LIGHT_1);
            SEAL_OF_RIGHTEOUSNESS = InitSpell(Spells.SEAL_OF_RIGHTEOUSNESS_1);
            SEAL_OF_WISDOM = InitSpell(Spells.SEAL_OF_WISDOM_1);
            SEAL_OF_THE_CRUSADER = InitSpell(Spells.SEAL_OF_THE_CRUSADER_1);
            SENSE_UNDEAD = InitSpell(Spells.SENSE_UNDEAD_1);
            SHADOW_RESISTANCE_AURA = InitSpell(Spells.SHADOW_RESISTANCE_AURA_1);
        }

        #endregion

        #region Paladin Constants

        // Key = Level, Values = List of spells attained at that level
        private static Dictionary<int, List<uint>> mSpellsByLevel = new Dictionary<int, List<uint>>
        {
            { 4, new List<uint> {   } }
        };

        public static class Spells
        {
            public const uint ANTICIPATION_1 = 20096;
            public const uint ANTICIPATION_2 = 20097;
            public const uint ANTICIPATION_3 = 20098;
            public const uint ANTICIPATION_4 = 20099;
            public const uint ANTICIPATION_5 = 20100;
            public const uint BLESSING_OF_FREEDOM_1 = 1044;
            public const uint BLESSING_OF_KINGS_1 = 20217;
            public const uint BLESSING_OF_MIGHT_1 = 19740;
            public const uint BLESSING_OF_MIGHT_2 = 19834;
            public const uint BLESSING_OF_MIGHT_3 = 19835;
            public const uint BLESSING_OF_MIGHT_4 = 19836;
            public const uint BLESSING_OF_MIGHT_5 = 19837;
            public const uint BLESSING_OF_MIGHT_6 = 19838;
            public const uint BLESSING_OF_MIGHT_7 = 25291;
            public const uint BLESSING_OF_PROTECTION_1 = 1022;
            public const uint BLESSING_OF_PROTECTION_2 = 5599;
            public const uint BLESSING_OF_PROTECTION_3 = 10278;
            public const uint BLESSING_OF_SACRIFICE_1 = 6940;
            public const uint BLESSING_OF_SALVATION_1 = 1038;
            public const uint BLESSING_OF_SANCTUARY_1 = 20911;
            public const uint BLESSING_OF_WISDOM_1 = 19742;
            public const uint BLESSING_OF_WISDOM_2 = 19850;
            public const uint BLESSING_OF_WISDOM_3 = 19852;
            public const uint BLESSING_OF_WISDOM_4 = 19853;
            public const uint BLESSING_OF_WISDOM_5 = 19854;
            public const uint BLESSING_OF_WISDOM_6 = 25290;
            public const uint BLOCK_1 = 107;
            public const uint CLEANSE_1 = 4987;
            public const uint CONCENTRATION_AURA_1 = 19746;
            public const uint CONSECRATION_1 = 26573;
            public const uint CONSECRATION_2 = 20116;
            public const uint CONSECRATION_3 = 20922;
            public const uint CONSECRATION_4 = 20923;
            public const uint CONSECRATION_5 = 20924;
            public const uint DEVOTION_AURA_1 = 465;
            public const uint DEVOTION_AURA_2 = 10290;
            public const uint DEVOTION_AURA_3 = 643;
            public const uint DEVOTION_AURA_4 = 10291;
            public const uint DEVOTION_AURA_5 = 1032;
            public const uint DEVOTION_AURA_6 = 10292;
            public const uint DEVOTION_AURA_7 = 10293;
            public const uint DIVINE_FAVOR_1 = 20216;
            public const uint DIVINE_INTERVENTION_1 = 19752;
            public const uint DIVINE_PROTECTION_1 = 498;
            public const uint DIVINE_SHIELD_1 = 642;
            public const uint EXORCISM_1 = 879;
            public const uint EXORCISM_2 = 5614;
            public const uint EXORCISM_3 = 5615;
            public const uint EXORCISM_4 = 10312;
            public const uint EXORCISM_5 = 10313;
            public const uint EXORCISM_6 = 10314;
            public const uint EYE_FOR_AN_EYE_1 = 25997;
            public const uint FIRE_RESISTANCE_AURA_1 = 19891;
            public const uint FIRE_RESISTANCE_AURA_2 = 19899;
            public const uint FIRE_RESISTANCE_AURA_3 = 19900;
            public const uint FLASH_OF_LIGHT_1 = 19750;
            public const uint FLASH_OF_LIGHT_2 = 19939;
            public const uint FLASH_OF_LIGHT_3 = 19940;
            public const uint FLASH_OF_LIGHT_4 = 19941;
            public const uint FLASH_OF_LIGHT_5 = 19942;
            public const uint FLASH_OF_LIGHT_6 = 19943;
            public const uint FROST_RESISTANCE_AURA_1 = 19888;
            public const uint FROST_RESISTANCE_AURA_2 = 19897;
            public const uint FROST_RESISTANCE_AURA_3 = 19898;
            public const uint GREATER_BLESSING_OF_KINGS_1 = 25898;
            public const uint GREATER_BLESSING_OF_MIGHT_1 = 25782;
            public const uint GREATER_BLESSING_OF_MIGHT_2 = 25916;
            public const uint GREATER_BLESSING_OF_SANCTUARY_1 = 25899;
            public const uint GREATER_BLESSING_OF_WISDOM_1 = 25894;
            public const uint GREATER_BLESSING_OF_WISDOM_2 = 25918;
            public const uint HAMMER_OF_JUSTICE_1 = 853;
            public const uint HAMMER_OF_JUSTICE_2 = 5588;
            public const uint HAMMER_OF_JUSTICE_3 = 5589;
            public const uint HAMMER_OF_JUSTICE_4 = 10308;
            public const uint HAMMER_OF_WRATH_1 = 24275;
            public const uint HAMMER_OF_WRATH_2 = 24274;
            public const uint HAMMER_OF_WRATH_3 = 24239;
            public const uint HAND_OF_FREEDOM_1 = 1044;
            public const uint HAND_OF_SALVATION_1 = 1038;
            public const uint HOLY_LIGHT_1 = 635;
            public const uint HOLY_LIGHT_2 = 639;
            public const uint HOLY_LIGHT_3 = 647;
            public const uint HOLY_LIGHT_4 = 1026;
            public const uint HOLY_LIGHT_6 = 3472;
            public const uint HOLY_LIGHT_7 = 10328;
            public const uint HOLY_LIGHT_8 = 10329;
            public const uint HOLY_LIGHT_9 = 25292;
            public const uint HOLY_SHIELD_1 = 20925;
            public const uint HOLY_SHOCK_1 = 20473;
            public const uint HOLY_SHOCK_2 = 20929;
            public const uint HOLY_SHOCK_3 = 20930;
            public const uint HOLY_WRATH_1 = 2812;
            public const uint HOLY_WRATH_2 = 10318;
            public const uint JUDGEMENT_1 = 20271;
            public const uint JUDGEMENT_OF_JUSTICE_1 = 20184;
            public const uint JUDGEMENT_OF_LIGHT_1 = 20185;
            public const uint JUDGEMENT_OF_RIGHTEOUSNESS_1 = 20187;
            public const uint JUDGEMENT_OF_WISDOM_1 = 20186;
            public const uint LAY_ON_HANDS_1 = 633;
            public const uint LAY_ON_HANDS_2 = 2800;
            public const uint LAY_ON_HANDS_3 = 10310;
            public const uint PARRY_1 = 3127;
            public const uint PURIFY_1 = 1152;
            public const uint REDEMPTION_1 = 7328;
            public const uint REDEMPTION_2 = 10322;
            public const uint REDEMPTION_3 = 10324;
            public const uint REDEMPTION_4 = 20772;
            public const uint REDEMPTION_5 = 20773;
            public const uint REPENTANCE_1 = 20066;
            public const uint RETRIBUTION_AURA_1 = 7294;
            public const uint RETRIBUTION_AURA_2 = 10298;
            public const uint RETRIBUTION_AURA_3 = 10299;
            public const uint RETRIBUTION_AURA_4 = 10300;
            public const uint RETRIBUTION_AURA_5 = 10301;
            public const uint RIDING_1 = 33391;
            public const uint RIGHTEOUS_FURY_1 = 25780;
            public const uint SEAL_OF_COMMAND_1 = 20375;
            public const uint SEAL_OF_THE_CRUSADER_1 = 21082;
            public const uint SEAL_OF_JUSTICE_1 = 20164;
            public const uint SEAL_OF_LIGHT_1 = 20165;
            public const uint SEAL_OF_RIGHTEOUSNESS_1 = 21084;
            public const uint SEAL_OF_WISDOM_1 = 20166;
            public const uint SENSE_UNDEAD_1 = 5502;
            public const uint SHADOW_RESISTANCE_AURA_1 = 19876;
            public const uint SHADOW_RESISTANCE_AURA_2 = 19895;
            public const uint SHADOW_RESISTANCE_AURA_3 = 19896;
            public const uint SUMMON_CHARGER_1 = 23214;
            public const uint SUMMON_WARHORSE_1 = 13819;
            public const uint TURN_EVIL_1 = 10326;
            public const uint TURN_UNDEAD_3 = 10326;

            // Judgement auras on target
            public const uint JUDGEMENT_OF_WISDOM = 20355; // rank 2: 20354; rank 1: 20186
            public const uint JUDGEMENT_OF_JUSTICE = 20184;
            public const uint JUDGEMENT_OF_THE_CRUSADER = 20303;  // rank 5: 20302; rank 4: 20301; rank 3: 20300; rank 2: 20188; rank 1: 21183
        }

        #endregion
    }
}
