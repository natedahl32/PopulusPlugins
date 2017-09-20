using Populus.Core.World.Objects;
using System.Collections.Generic;

namespace Populus.GroupBot.Combat.Hunter
{
    public class HunterCombatLogic : CombatLogicHandler
    {
        #region Declarations

        protected uint PET_SUMMON,
           PET_DISMISS,
           PET_REVIVE,
           PET_MEND,
           PET_FEED,
           BESTIAL_WRATH,
           BAD_ATTITUDE,
           SONIC_BLAST,
           DEMORALIZING_SCREECH,
           INTIMIDATION;

        protected uint AUTO_SHOT,
               HUNTERS_MARK,
               ARCANE_SHOT,
               CONCUSSIVE_SHOT,
               DISTRACTING_SHOT,
               MULTI_SHOT,
               EXPLOSIVE_SHOT,
               SERPENT_STING,
               SCORPID_STING,
               VIPER_STING,
               WYVERN_STING,
               AIMED_SHOT,
               VOLLEY,
               BLACK_ARROW,
               TRANQ_SHOT,
               SCATTER_SHOT;

        protected uint RAPTOR_STRIKE,
               WING_CLIP,
               MONGOOSE_BITE,
               DISENGAGE,
               DETERRENCE;

        protected uint FREEZING_TRAP,
               IMMOLATION_TRAP,
               FROST_TRAP,
               EXPLOSIVE_TRAP;

        protected uint ASPECT_OF_THE_HAWK,
               ASPECT_OF_THE_MONKEY,
               ASPECT_OF_THE_BEAST,
               ASPECT_OF_THE_CHEETAH,
               ASPECT_OF_THE_PACK,
               ASPECT_OF_THE_WILD,
               RAPID_FIRE,
               TRUESHOT_AURA,
               BEAST_LORE,
               EAGLE_EYE,
               EYES_OF_THE_BEAST,
               FEIGN_DEATH,
               FLARE,
               SCARE_BEAST,
               TAME_BEAST,
               TRACK_BEASTS,
               TRACK_DEMONS,
               TRACK_DRAGONKIN,
               TRACK_ELEMENTALS,
               TRACK_GIANTS,
               TRACK_HIDDEN,
               TRACK_HUMANOIDS,
               TRACK_UNDEAD,
               COUNTERATTACK,
               READINESS;

        #endregion

        #region Constructors

        public HunterCombatLogic(GroupBotHandler botHandler) : base(botHandler)
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether or not this class is primarily a melee class
        /// </summary>
        public override bool IsMelee
        {
            get { return false; }
        }

        /// <summary>
        /// Gets all hunter spells and abilities available by level
        /// </summary>
        protected override Dictionary<int, List<uint>> SpellsByLevel => mSpellsByLevel;

        #endregion

        #region Public Methods

        public override void InitializeSpells()
        {
            base.InitializeSpells();

            // Spells
            ARCANE_SHOT = InitSpell(Spells.ARCANE_SHOT_1);
            ASPECT_OF_THE_BEAST = InitSpell(Spells.ASPECT_OF_THE_BEAST_1);
            ASPECT_OF_THE_CHEETAH = InitSpell(Spells.ASPECT_OF_THE_CHEETAH_1);
            ASPECT_OF_THE_HAWK = InitSpell(Spells.ASPECT_OF_THE_HAWK_1);
            ASPECT_OF_THE_MONKEY = InitSpell(Spells.ASPECT_OF_THE_MONKEY_1);
            ASPECT_OF_THE_PACK = InitSpell(Spells.ASPECT_OF_THE_PACK_1);
            ASPECT_OF_THE_WILD = InitSpell(Spells.ASPECT_OF_THE_WILD_1);
            AUTO_SHOT = InitSpell(Spells.AUTO_SHOT_1);
            BEAST_LORE = InitSpell(Spells.BEAST_LORE_1);
            PET_SUMMON = InitSpell(Spells.CALL_PET_1);
            CONCUSSIVE_SHOT = InitSpell(Spells.CONCUSSIVE_SHOT_1);
            DETERRENCE = InitSpell(Spells.DETERRENCE_1);
            DISENGAGE = InitSpell(Spells.DISENGAGE_1);
            PET_DISMISS = InitSpell(Spells.DISMISS_PET_1);
            DISTRACTING_SHOT = InitSpell(Spells.DISTRACTING_SHOT_1);
            EAGLE_EYE = InitSpell(Spells.EAGLE_EYE_1);
            EXPLOSIVE_TRAP = InitSpell(Spells.EXPLOSIVE_TRAP_1);
            EYES_OF_THE_BEAST = InitSpell(Spells.EYES_OF_THE_BEAST_1);
            PET_FEED = InitSpell(Spells.FEED_PET_1);
            FEIGN_DEATH = InitSpell(Spells.FEIGN_DEATH_1);
            FLARE = InitSpell(Spells.FLARE_1);
            FREEZING_TRAP = InitSpell(Spells.FREEZING_TRAP_1);
            FROST_TRAP = InitSpell(Spells.FROST_TRAP_1);
            HUNTERS_MARK = InitSpell(Spells.HUNTERS_MARK_1);
            IMMOLATION_TRAP = InitSpell(Spells.IMMOLATION_TRAP_1);
            PET_MEND = InitSpell(Spells.MEND_PET_1);
            MONGOOSE_BITE = InitSpell(Spells.MONGOOSE_BITE_1);
            MULTI_SHOT = InitSpell(Spells.MULTISHOT_1);
            RAPID_FIRE = InitSpell(Spells.RAPID_FIRE_1);
            RAPTOR_STRIKE = InitSpell(Spells.RAPTOR_STRIKE_1);
            PET_REVIVE = InitSpell(Spells.REVIVE_PET_1);
            SCARE_BEAST = InitSpell(Spells.SCARE_BEAST_1);
            SCORPID_STING = InitSpell(Spells.SCORPID_STING_1);
            SERPENT_STING = InitSpell(Spells.SERPENT_STING_1);
            TAME_BEAST = InitSpell(Spells.TAME_BEAST_1);
            TRACK_BEASTS = InitSpell(Spells.TRACK_BEASTS_1);
            TRACK_DEMONS = InitSpell(Spells.TRACK_DEMONS_1);
            TRACK_DRAGONKIN = InitSpell(Spells.TRACK_DRAGONKIN_1);
            TRACK_ELEMENTALS = InitSpell(Spells.TRACK_ELEMENTALS_1);
            TRACK_GIANTS = InitSpell(Spells.TRACK_GIANTS_1);
            TRACK_HIDDEN = InitSpell(Spells.TRACK_HIDDEN_1);
            TRACK_HUMANOIDS = InitSpell(Spells.TRACK_HUMANOIDS_1);
            TRACK_UNDEAD = InitSpell(Spells.TRACK_UNDEAD_1);
            TRANQ_SHOT = InitSpell(Spells.TRANQUILIZING_SHOT_1);
            VIPER_STING = InitSpell(Spells.VIPER_STING_1);
            VOLLEY = InitSpell(Spells.VOLLEY_1);
            WING_CLIP = InitSpell(Spells.WING_CLIP_1);
            AIMED_SHOT = InitSpell(Spells.AIMED_SHOT_1);
            BESTIAL_WRATH = InitSpell(Spells.BESTIAL_WRATH_1);
            BLACK_ARROW = InitSpell(Spells.BLACK_ARROW_1);
            COUNTERATTACK = InitSpell(Spells.COUNTERATTACK_1);
            INTIMIDATION = InitSpell(Spells.INTIMIDATION_1);
            READINESS = InitSpell(Spells.READINESS_1);
            SCATTER_SHOT = InitSpell(Spells.SCATTER_SHOT_1);
            TRUESHOT_AURA = InitSpell(Spells.TRUESHOT_AURA_1);
            WYVERN_STING = InitSpell(Spells.WYVERN_STING_1);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Commands the hunters pet to attack, if one is summoned
        /// </summary>
        protected void PetAttack(Unit unit)
        {
            if (BotHandler.BotOwner.Pet != null)
            {
                // TODO: Need to figure this one out
            }
        }

        #endregion

        #region Hunter Constants

        // Key = Level, Values = List of spells attained at that level
        private static Dictionary<int, List<uint>> mSpellsByLevel = new Dictionary<int, List<uint>>
        {
            { 4, new List<uint> {   } }
        };

        public static class Spells
        {
            public const uint ARCANE_SHOT_1 = 3044;
            public const uint ARCANE_SHOT_2 = 14281;
            public const uint ARCANE_SHOT_3 = 14282;
            public const uint ARCANE_SHOT_4 = 14283;
            public const uint ARCANE_SHOT_5 = 14284;
            public const uint ARCANE_SHOT_6 = 14285;
            public const uint ARCANE_SHOT_7 = 14286;
            public const uint ARCANE_SHOT_8 = 14287;
            public const uint ASPECT_OF_THE_BEAST_1 = 13161;
            public const uint ASPECT_OF_THE_CHEETAH_1 = 5118;
            public const uint ASPECT_OF_THE_HAWK_1 = 13165;
            public const uint ASPECT_OF_THE_HAWK_2 = 14318;
            public const uint ASPECT_OF_THE_HAWK_3 = 14319;
            public const uint ASPECT_OF_THE_HAWK_4 = 14320;
            public const uint ASPECT_OF_THE_HAWK_5 = 14321;
            public const uint ASPECT_OF_THE_HAWK_6 = 14322;
            public const uint ASPECT_OF_THE_HAWK_7 = 25296;
            public const uint ASPECT_OF_THE_MONKEY_1 = 13163;
            public const uint ASPECT_OF_THE_PACK_1 = 13159;
            public const uint ASPECT_OF_THE_WILD_1 = 20043;
            public const uint ASPECT_OF_THE_WILD_2 = 20190;
            public const uint AUTO_SHOT_1 = 75;
            public const uint BEAST_LORE_1 = 1462;
            public const uint BLACK_ARROW_1 = 3674;
            public const uint BLACK_ARROW_2 = 14296;
            public const uint CALL_PET_1 = 883;
            public const uint CONCUSSIVE_SHOT_1 = 5116;
            public const uint COUNTERATTACK_1 = 19306;
            public const uint COUNTERATTACK_2 = 20909;
            public const uint COUNTERATTACK_3 = 20910;
            public const uint DETERRENCE_1 = 19263;
            public const uint DISENGAGE_1 = 781;
            public const uint DISMISS_PET_1 = 2641;
            public const uint DISTRACTING_SHOT_1 = 20736;
            public const uint EAGLE_EYE_1 = 6197;
            public const uint EXPLOSIVE_TRAP_1 = 13813;
            public const uint EXPLOSIVE_TRAP_2 = 14316;
            public const uint EXPLOSIVE_TRAP_3 = 14317;
            public const uint EXPLOSIVE_TRAP_EFFECT_1 = 13812;
            public const uint EXPLOSIVE_TRAP_EFFECT_2 = 14314;
            public const uint EXPLOSIVE_TRAP_EFFECT_3 = 14315;
            public const uint EYES_OF_THE_BEAST_1 = 1002;
            public const uint FEED_PET_1 = 6991;
            public const uint FEIGN_DEATH_1 = 5384;
            public const uint FLARE_1 = 1543;
            public const uint FREEZING_TRAP_1 = 1499;
            public const uint FREEZING_TRAP_2 = 14310;
            public const uint FREEZING_TRAP_3 = 14311;
            public const uint FROST_TRAP_1 = 13809;
            public const uint HUNTERS_MARK_1 = 1130;
            public const uint HUNTERS_MARK_2 = 14323;
            public const uint HUNTERS_MARK_3 = 14324;
            public const uint HUNTERS_MARK_4 = 14325;
            public const uint IMMOLATION_TRAP_1 = 13795;
            public const uint IMMOLATION_TRAP_2 = 14302;
            public const uint IMMOLATION_TRAP_3 = 14303;
            public const uint IMMOLATION_TRAP_4 = 14304;
            public const uint IMMOLATION_TRAP_5 = 14305;
            public const uint IMMOLATION_TRAP_EFFECT_1 = 13797;
            public const uint IMMOLATION_TRAP_EFFECT_2 = 14298;
            public const uint IMMOLATION_TRAP_EFFECT_3 = 14299;
            public const uint IMMOLATION_TRAP_EFFECT_4 = 14300;
            public const uint IMMOLATION_TRAP_EFFECT_5 = 14301;
            public const uint MEND_PET_1 = 136;
            public const uint MEND_PET_2 = 3111;
            public const uint MEND_PET_3 = 3661;
            public const uint MEND_PET_4 = 3662;
            public const uint MEND_PET_5 = 13542;
            public const uint MEND_PET_6 = 13543;
            public const uint MEND_PET_7 = 13544;
            public const uint MONGOOSE_BITE_1 = 1495;
            public const uint MONGOOSE_BITE_2 = 14269;
            public const uint MONGOOSE_BITE_3 = 14270;
            public const uint MONGOOSE_BITE_4 = 14271;
            public const uint MULTISHOT_1 = 2643;
            public const uint MULTISHOT_2 = 14288;
            public const uint MULTISHOT_3 = 14289;
            public const uint MULTISHOT_4 = 14290;
            public const uint MULTISHOT_5 = 25294;
            public const uint PARRY_1 = 3127;
            public const uint RAPID_FIRE_1 = 3045;
            public const uint RAPTOR_STRIKE_1 = 2973;
            public const uint RAPTOR_STRIKE_2 = 14260;
            public const uint RAPTOR_STRIKE_3 = 14261;
            public const uint RAPTOR_STRIKE_4 = 14262;
            public const uint RAPTOR_STRIKE_5 = 14263;
            public const uint RAPTOR_STRIKE_6 = 14264;
            public const uint RAPTOR_STRIKE_7 = 14265;
            public const uint RAPTOR_STRIKE_8 = 14266;
            public const uint REVIVE_PET_1 = 982;
            public const uint RIDING_1 = 33391;
            public const uint SCARE_BEAST_1 = 1513;
            public const uint SCARE_BEAST_2 = 14326;
            public const uint SCARE_BEAST_3 = 14327;
            public const uint SCATTER_SHOT_1 = 19503;
            public const uint SCORPID_STING_1 = 3043;
            public const uint SERPENT_STING_1 = 1978;
            public const uint SERPENT_STING_2 = 13549;
            public const uint SERPENT_STING_3 = 13550;
            public const uint SERPENT_STING_4 = 13551;
            public const uint SERPENT_STING_5 = 13552;
            public const uint SERPENT_STING_6 = 13553;
            public const uint SERPENT_STING_7 = 13554;
            public const uint SERPENT_STING_8 = 13555;
            public const uint SERPENT_STING_9 = 25295;
            public const uint TAME_BEAST_1 = 1515;
            public const uint TRACK_BEASTS_1 = 1494;
            public const uint TRACK_DEMONS_1 = 19878;
            public const uint TRACK_DRAGONKIN_1 = 19879;
            public const uint TRACK_ELEMENTALS_1 = 19880;
            public const uint TRACK_GIANTS_1 = 19882;
            public const uint TRACK_HIDDEN_1 = 19885;
            public const uint TRACK_HUMANOIDS_1 = 19883;
            public const uint TRACK_UNDEAD_1 = 19884;
            public const uint TRANQUILIZING_SHOT_1 = 19801;
            public const uint VIPER_STING_1 = 3034;
            public const uint VOLLEY_1 = 1510;
            public const uint VOLLEY_2 = 14294;
            public const uint VOLLEY_3 = 14295;
            public const uint WING_CLIP_1 = 2974;
            public const uint WYVERN_STING_1 = 19386;
            public const uint WYVERN_STING_2 = 24132;
            public const uint WYVERN_STING_3 = 24133;

            public const uint AIMED_SHOT_1 = 19434;
            public const uint BESTIAL_WRATH_1 = 19574;
            public const uint INTIMIDATION_1 = 19577;
            public const uint READINESS_1 = 23989;
            public const uint TRUESHOT_AURA_1 = 19506;
        }

        #endregion
    }
}
