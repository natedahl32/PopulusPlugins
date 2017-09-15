using System.Collections.Generic;
using Populus.Core.World.Objects;

namespace Populus.GroupBot.Combat.Warlock
{
    public class WarlockCombatLogic : CombatLogicHandler
    {
        #region Declarations

        // CURSES
        protected uint CURSE_OF_WEAKNESS,
               CURSE_OF_AGONY,
               CURSE_OF_EXHAUSTION,
               CURSE_OF_RECKLESSNESS,
               CURSE_OF_SHADOW,
               CURSE_OF_TONGUES,
               CURSE_OF_THE_ELEMENTS,
               CURSE_OF_DOOM;

        // RANGED
        protected uint SHOOT;

        // AFFLICTION
        protected uint AMPLIFY_CURSE,
               CORRUPTION,
               DRAIN_SOUL,
               DRAIN_LIFE,
               DRAIN_MANA,
               LIFE_TAP,
               DARK_PACT,
               HOWL_OF_TERROR,
               FEAR,
               SIPHON_LIFE,
               DEATH_COIL,
               EYE_OF_KILROG,
               INFERNO,
               RITUAL_OF_DOOM,
               RITUAL_OF_SUMMONING,
               SENSE_DEMONS,
               UNENDING_BREATH;

        // DESTRUCTION
        protected uint SHADOW_BOLT,
               IMMOLATE,
               SEARING_PAIN,
               CONFLAGRATE,
               SOUL_FIRE,
               HELLFIRE,
               RAIN_OF_FIRE,
               SHADOWBURN;

        // DEMONOLOGY
        protected uint BANISH,
               DEMON_SKIN,
               DEMON_ARMOR,
               SHADOW_WARD,
               ENSLAVE_DEMON,
               SOUL_LINK,
               SOUL_LINK_AURA,
               HEALTH_FUNNEL,
               DETECT_INVISIBILITY,
               CREATE_FIRESTONE,
               CREATE_SOULSTONE,
               CREATE_HEALTHSTONE,
               CREATE_SPELLSTONE;

        // DEMON SUMMON
        protected uint SUMMON_IMP,
               SUMMON_VOIDWALKER,
               SUMMON_SUCCUBUS,
               SUMMON_FELHUNTER;

        // DEMON SKILLS
        protected uint BLOOD_PACT,
               FIREBOLT,
               FIRE_SHIELD,
               ANGUISH,
               INTERCEPT,
               DEVOUR_MAGIC,
               SPELL_LOCK,
               LASH_OF_PAIN,
               SEDUCTION,
               SOOTHING_KISS,
               CONSUME_SHADOWS,
               SACRIFICE,
               SUFFERING,
               TORMENT,
               FEL_DOMINATION;

        #endregion

        #region Constructors

        public WarlockCombatLogic(GroupBotHandler botHandler) : base(botHandler)
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
        /// Gets all warlock spells and abilities available by level
        /// </summary>
        protected override Dictionary<int, List<uint>> SpellsByLevel => mSpellsByLevel;

        #endregion

        #region Public Methods

        public override void InitializeSpells()
        {
            base.InitializeSpells();

            // Spells
            AMPLIFY_CURSE = InitSpell(Spells.AMPLIFY_CURSE_1);
            BANISH = InitSpell(Spells.BANISH_1);
            CONFLAGRATE = InitSpell(Spells.CONFLAGRATE_1);
            CORRUPTION = InitSpell(Spells.CORRUPTION_1);
            CREATE_FIRESTONE = InitSpell(Spells.CREATE_FIRESTONE_LESSER);
            CREATE_HEALTHSTONE = InitSpell(Spells.CREATE_HEALTHSTONE_MINOR);
            CREATE_SOULSTONE = InitSpell(Spells.CREATE_SOULSTONE_MINOR);
            CREATE_SPELLSTONE = InitSpell(Spells.CREATE_SPELLSTONE);
            CURSE_OF_AGONY = InitSpell(Spells.CURSE_OF_AGONY_1);
            CURSE_OF_DOOM = InitSpell(Spells.CURSE_OF_DOOM_1);
            CURSE_OF_EXHAUSTION = InitSpell(Spells.CURSE_OF_EXHAUSTION_1);
            CURSE_OF_RECKLESSNESS = InitSpell(Spells.CURSE_OF_RECKLESSNESS_1);
            CURSE_OF_SHADOW = InitSpell(Spells.CURSE_OF_SHADOW_1);
            CURSE_OF_THE_ELEMENTS = InitSpell(Spells.CURSE_OF_THE_ELEMENTS_1);
            CURSE_OF_TONGUES = InitSpell(Spells.CURSE_OF_TONGUES_1);
            CURSE_OF_WEAKNESS = InitSpell(Spells.CURSE_OF_WEAKNESS_1);
            DARK_PACT = InitSpell(Spells.DARK_PACT_1);
            DEATH_COIL = InitSpell(Spells.DEATH_COIL_WARLOCK_1);
            DEMON_ARMOR = InitSpell(Spells.DEMON_ARMOR_1);
            DEMON_SKIN = InitSpell(Spells.DEMON_SKIN_1);
            DETECT_INVISIBILITY = InitSpell(Spells.DETECT_INVISIBILITY_1);
            DRAIN_LIFE = InitSpell(Spells.DRAIN_LIFE_1);
            DRAIN_MANA = InitSpell(Spells.DRAIN_MANA_1);
            DRAIN_SOUL = InitSpell(Spells.DRAIN_SOUL_1);
            ENSLAVE_DEMON = InitSpell(Spells.ENSLAVE_DEMON_1);
            EYE_OF_KILROG = InitSpell(Spells.EYE_OF_KILROGG_1);
            FEAR = InitSpell(Spells.FEAR_1);
            FEL_DOMINATION = InitSpell(Spells.FEL_DOMINATION_1);
            HEALTH_FUNNEL = InitSpell(Spells.HEALTH_FUNNEL_1);
            HELLFIRE = InitSpell(Spells.HELLFIRE_1);
            HOWL_OF_TERROR = InitSpell(Spells.HOWL_OF_TERROR_1);
            IMMOLATE = InitSpell(Spells.IMMOLATE_1);
            INFERNO = InitSpell(Spells.INFERNO_1);
            LIFE_TAP = InitSpell(Spells.LIFE_TAP_1);
            RAIN_OF_FIRE = InitSpell(Spells.RAIN_OF_FIRE_1);
            RITUAL_OF_DOOM = InitSpell(Spells.RITUAL_OF_DOOM_1);
            RITUAL_OF_SUMMONING = InitSpell(Spells.RITUAL_OF_SUMMONING_1);
            SEARING_PAIN = InitSpell(Spells.SEARING_PAIN_1);
            SENSE_DEMONS = InitSpell(Spells.SENSE_DEMONS_1);
            SHADOW_BOLT = InitSpell(Spells.SHADOW_BOLT_1);
            SHADOW_WARD = InitSpell(Spells.SHADOW_WARD_1);
            SHADOWBURN = InitSpell(Spells.SHADOWBURN_1);
            SHOOT = InitSpell(Spells.SHOOT_3);
            SIPHON_LIFE = InitSpell(Spells.SIPHON_LIFE_1);
            SOUL_FIRE = InitSpell(Spells.SOUL_FIRE_1);
            SOUL_LINK = InitSpell(Spells.SOUL_LINK_1);
            SUMMON_FELHUNTER = InitSpell(Spells.SUMMON_FELHUNTER_1);
            SUMMON_IMP = InitSpell(Spells.SUMMON_IMP_1);
            SUMMON_SUCCUBUS = InitSpell(Spells.SUMMON_SUCCUBUS_1);
            SUMMON_VOIDWALKER = InitSpell(Spells.SUMMON_VOIDWALKER_1);
            UNENDING_BREATH = InitSpell(Spells.UNENDING_BREATH_1);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Commands the warlocks pet to attack, if one is summoned
        /// </summary>
        private void PetAttack(Unit unit)
        {
            if(BotHandler.BotOwner.Pet != null)
            {
                // TODO: Need to figure this one out
            }
        }

        protected override CombatActionResult DoFirstCombatAction(Unit unit)
        {
            return CombatActionResult.NO_ACTION_OK;
        }

        protected override CombatActionResult DoNextCombatAction(Unit unit)
        {
            // Always pet attack this unit
            PetAttack(unit);

            // TODO: Build up warlock combat logic
            if (HasSpellAndCanCast(CURSE_OF_AGONY) && !unit.HasAura(CURSE_OF_AGONY) && unit.HealthPercentage > 50.0f)
            {
                BotHandler.CombatState.SpellCast(CURSE_OF_AGONY);
                return CombatActionResult.ACTION_OK;
            }

            if (HasSpellAndCanCast(CORRUPTION) && !unit.HasAura(CORRUPTION) && unit.HealthPercentage > 40.0f)
            {
                BotHandler.CombatState.SpellCast(CORRUPTION);
                return CombatActionResult.ACTION_OK;
            }

            if (HasSpellAndCanCast(SHADOW_BOLT))
            {
                BotHandler.CombatState.SpellCast(SHADOW_BOLT);
                return CombatActionResult.ACTION_OK;
            }

            return CombatActionResult.NO_ACTION_OK;
        }

        #endregion

        #region Warlock Constants

        // Key = Level, Values = List of spells attained at that level
        private static Dictionary<int, List<uint>> mSpellsByLevel = new Dictionary<int, List<uint>>
        {
            { 4, new List<uint> { Spells.SUMMON_IMP_1, Spells.CURSE_OF_WEAKNESS_1, Spells.CORRUPTION_1, DemonSpells.BLOOD_PACT_1  } },
            { 6, new List<uint> { Spells.LIFE_TAP_1, Spells.SHADOW_BOLT_2 } },
            { 8, new List<uint> { Spells.CURSE_OF_AGONY_1, Spells.FEAR_1, DemonSpells.FIREBOLT_2 } },
            { 10, new List<uint> { Spells.DEMON_SKIN_2, Spells.SUMMON_VOIDWALKER_1, Spells.CREATE_HEALTHSTONE_MINOR, Spells.DRAIN_SOUL_1, Spells.IMMOLATE_2, DemonSpells.TORMENT_1 } },
            { 12, new List<uint> { Spells.HEALTH_FUNNEL_1, Spells.CURSE_OF_WEAKNESS_2, Spells.SHADOW_BOLT_3 } },
            { 14, new List<uint> { Spells.CORRUPTION_2, Spells.DRAIN_LIFE_1, DemonSpells.BLOOD_PACT_2, DemonSpells.FIRE_SHIELD_1 } },
            { 16, new List<uint> { Spells.UNENDING_BREATH_1, Spells.LIFE_TAP_2 } },
            { 18, new List<uint> { Spells.CREATE_SOULSTONE_MINOR, Spells.SEARING_PAIN_1, Spells.CURSE_OF_AGONY_2, DemonSpells.FIREBOLT_3 } },
            { 20, new List<uint> { Spells.DEMON_ARMOR_1, Spells.SUMMON_SUCCUBUS_1, Spells.HEALTH_FUNNEL_2, Spells.RITUAL_OF_SUMMONING_1, Spells.SHADOW_BOLT_4, Spells.IMMOLATE_3, Spells.RAIN_OF_FIRE_1, Spells.SHADOWBURN_1, DemonSpells.TORMENT_2 } },
            { 22, new List<uint> { Spells.CREATE_HEALTHSTONE_LESSER, Spells.EYE_OF_KILROGG_1, Spells.CURSE_OF_WEAKNESS_3, Spells.DRAIN_LIFE_2, DemonSpells.SOOTHING_KISS_1 } },
            { 24, new List<uint> { Spells.SENSE_DEMONS_1, Spells.CORRUPTION_3, Spells.DRAIN_MANA_1, Spells.DRAIN_SOUL_2, Spells.SHADOWBURN_2, DemonSpells.FIRE_SHIELD_2, DemonSpells.SUFFERING_1 } },
            { 26, new List<uint> { Spells.DETECT_INVISIBILITY_1, Spells.CURSE_OF_TONGUES_1, Spells.LIFE_TAP_3, Spells.SEARING_PAIN_2, DemonSpells.BLOOD_PACT_3 } },
            { 28, new List<uint> { Spells.HEALTH_FUNNEL_3, Spells.CREATE_FIRESTONE_LESSER, Spells.BANISH_1, Spells.CURSE_OF_AGONY_3, Spells.SHADOW_BOLT_5, DemonSpells.FIREBOLT_4 } },
            { 30, new List<uint> { Spells.ENSLAVE_DEMON_1, Spells.DEMON_ARMOR_2, Spells.SUMMON_FELHUNTER_1, Spells.CREATE_SOULSTONE_LESSER, Spells.DRAIN_LIFE_3, Spells.IMMOLATE_4, Spells.HELLFIRE_1, DemonSpells.DEVOUR_MAGIC_1, DemonSpells.TORMENT_3 } },
            { 32, new List<uint> { Spells.SHADOW_WARD_1, Spells.CURSE_OF_THE_ELEMENTS_1, Spells.CURSE_OF_WEAKNESS_4, Spells.FEAR_2, Spells.SHADOWBURN_3 } },
            { 34, new List<uint> { Spells.CREATE_HEALTHSTONE, Spells.CORRUPTION_4, Spells.RAIN_OF_FIRE_2, Spells.SEARING_PAIN_3, DemonSpells.FIRE_SHIELD_3, DemonSpells.SOOTHING_KISS_2 } },
            { 36, new List<uint> { Spells.HEALTH_FUNNEL_4, Spells.CREATE_SPELLSTONE, Spells.CREATE_FIRESTONE, Spells.LIFE_TAP_4, Spells.SHADOW_BOLT_6, DemonSpells.SPELL_LOCK_1, DemonSpells.SUFFERING_2 } },
            { 38, new List<uint> { Spells.CURSE_OF_AGONY_4, Spells.DRAIN_LIFE_4, Spells.DRAIN_SOUL_3, DemonSpells.FIREBOLT_5, DemonSpells.BLOOD_PACT_4, DemonSpells.DEVOUR_MAGIC_2 } },
            { 40, new List<uint> { Spells.DEMON_ARMOR_3, Spells.SUMMON_FELSTEED_1, Spells.CREATE_SOULSTONE, Spells.HOWL_OF_TERROR_1, Spells.DARK_PACT_1, Spells.IMMOLATE_5, Spells.CONFLAGRATE_1, Spells.SHADOWBURN_4, DemonSpells.TORMENT_4 } },
            { 42, new List<uint> { Spells.SHADOW_WARD_2, Spells.CURSE_OF_WEAKNESS_5, Spells.DEATH_COIL_WARLOCK_1, Spells.HELLFIRE_2, Spells.SEARING_PAIN_4 } },
            { 44, new List<uint> { Spells.ENSLAVE_DEMON_1, Spells.HEALTH_FUNNEL_5, Spells.CORRUPTION_5, Spells.SHADOW_BOLT_7, DemonSpells.FIRE_SHIELD_4 } },
            { 46, new List<uint> { Spells.CREATE_HEALTHSTONE_GREATER, Spells.CREATE_FIRESTONE_GREATER, Spells.CURSE_OF_THE_ELEMENTS_2, Spells.DRAIN_LIFE_5, Spells.LIFE_TAP_5, Spells.RAIN_OF_FIRE_3, DemonSpells.DEVOUR_MAGIC_3, DemonSpells.SOOTHING_KISS_3 } },
            { 48, new List<uint> { Spells.CREATE_SPELLSTONE_GREATER, Spells.BANISH_2, Spells.CURSE_OF_AGONY_5, Spells.SOUL_FIRE_1, Spells.SHADOWBURN_5, DemonSpells.FIREBOLT_6, DemonSpells.SUFFERING_3 } },
            { 50, new List<uint> { Spells.DEMON_ARMOR_4, Spells.SUMMON_INFERNO_1, Spells.CREATE_SOULSTONE_GREATER, Spells.CURSE_OF_TONGUES_2, Spells.DEATH_COIL_WARLOCK_2, Spells.DARK_PACT_2, Spells.IMMOLATE_6, Spells.SEARING_PAIN_5, DemonSpells.BLOOD_PACT_5, DemonSpells.TORMENT_5 } },
            { 52, new List<uint> { Spells.HEALTH_FUNNEL_6, Spells.SHADOW_WARD_3, Spells.CURSE_OF_WEAKNESS_6, Spells.DRAIN_SOUL_4, Spells.SHADOW_BOLT_8, DemonSpells.SPELL_LOCK_2 } },
            { 54, new List<uint> { Spells.HOWL_OF_TERROR_2, Spells.CORRUPTION_6, Spells.DRAIN_LIFE_6, Spells.HELLFIRE_3, DemonSpells.FIRE_SHIELD_5, DemonSpells.DEVOUR_MAGIC_4 } },
            { 56, new List<uint> { Spells.CREATE_FIRESTONE_MAJOR, Spells.FEAR_3, Spells.LIFE_TAP_6, Spells.SOUL_FIRE_2, Spells.SHADOWBURN_6 } },
            { 58, new List<uint> { Spells.ENSLAVE_DEMON_3, Spells.CREATE_HEALTHSTONE_MAJOR, Spells.CURSE_OF_AGONY_6, Spells.DEATH_COIL_WARLOCK_3, Spells.RAIN_OF_FIRE_4, Spells.SEARING_PAIN_6, DemonSpells.FIREBOLT_7, DemonSpells.SOOTHING_KISS_4 } },
            { 60, new List<uint> { Spells.DEMON_ARMOR_5, Spells.HEALTH_FUNNEL_7, Spells.CREATE_SPELLSTONE_MAJOR, Spells.RITUAL_OF_DOOM_1, Spells.CREATE_SOULSTONE_MAJOR, Spells.SUMMON_DREADSTEED_1, Spells.SHADOW_WARD_4, Spells.CURSE_OF_THE_ELEMENTS_3, Spells.CURSE_OF_DOOM_1, Spells.DARK_PACT_3, Spells.CORRUPTION_7, Spells.SHADOW_BOLT_10, Spells.IMMOLATE_8, DemonSpells.SUFFERING_4, DemonSpells.TORMENT_6 } }
        };

        public static class Stones
        {
            public const uint FIRESTONE = 13699;
            public const uint LESSER_FIRESTONE = 1254;
            public const uint GREATER_FIRESTONE = 13700;
            public const uint MAJOR_FIRESTONE = 13701;

            public const uint SPELLSTONE = 5522;
            public const uint GREATER_SPELLSTONE = 13602;
            public const uint MAJOR_SPELLSTONE = 13603;

            public const uint MINOR_SOULSTONE = 5232;
            public const uint LESSER_SOULSTONE = 16892;
            public const uint SOULSTONE = 16893;
            public const uint GREATER_SOULSTONE = 16895;
            public const uint MAJOR_SOULSTONE = 16896;
        }

        public static class Demons
        {
            public const uint DEMON_IMP = 416;
            public const uint DEMON_VOIDWALKER = 1860;
            public const uint DEMON_SUCCUBUS = 1863;
            public const uint DEMON_FELHUNTER = 417;
        }

        public static class DemonSpells
        {
            // Imp
            public const uint BLOOD_PACT_1 = 6307;
            public const uint BLOOD_PACT_2 = 7804;
            public const uint BLOOD_PACT_3 = 7805;
            public const uint BLOOD_PACT_4 = 11766;
            public const uint BLOOD_PACT_5 = 11767;
            public const uint FIREBOLT_1 = 3110;
            public const uint FIREBOLT_2 = 7799;
            public const uint FIREBOLT_3 = 7800;
            public const uint FIREBOLT_4 = 7801;
            public const uint FIREBOLT_5 = 7802;
            public const uint FIREBOLT_6 = 11762;
            public const uint FIREBOLT_7 = 11763;
            public const uint FIRE_SHIELD_1 = 2947;
            public const uint FIRE_SHIELD_2 = 8316;
            public const uint FIRE_SHIELD_3 = 8317;
            public const uint FIRE_SHIELD_4 = 11770;
            public const uint FIRE_SHIELD_5 = 11771;
            // Felhunter
            public const uint DEVOUR_MAGIC_1 = 19505;
            public const uint DEVOUR_MAGIC_2 = 19731;
            public const uint DEVOUR_MAGIC_3 = 19734;
            public const uint DEVOUR_MAGIC_4 = 19736;
            public const uint SPELL_LOCK_1 = 19244;
            public const uint SPELL_LOCK_2 = 19647;
            // Succubus
            public const uint LASH_OF_PAIN_1 = 7814;
            public const uint SEDUCTION_1 = 6358;
            public const uint SOOTHING_KISS_1 = 6360;
            public const uint SOOTHING_KISS_2 = 7813;
            public const uint SOOTHING_KISS_3 = 11784;
            public const uint SOOTHING_KISS_4 = 11785;
            // Voidwalker
            public const uint CONSUME_SHADOWS_1 = 17767;
            public const uint SACRIFICE_1 = 7812;
            public const uint SUFFERING_1 = 17735;
            public const uint SUFFERING_2 = 17750;
            public const uint SUFFERING_3 = 17751;
            public const uint SUFFERING_4 = 17752;
            public const uint TORMENT_1 = 3716;
            public const uint TORMENT_2 = 7809;
            public const uint TORMENT_3 = 7810;
            public const uint TORMENT_4 = 7811;
            public const uint TORMENT_5 = 11774;
            public const uint TORMENT_6 = 11775;
        }

        public static class Spells
        {
            public const uint AMPLIFY_CURSE_1 = 18288;
            public const uint BANISH_1 = 710;
            public const uint BANISH_2 = 18647;
            public const uint CONFLAGRATE_1 = 17962;
            public const uint CORRUPTION_1 = 172;
            public const uint CORRUPTION_2 = 6222;
            public const uint CORRUPTION_3 = 6223;
            public const uint CORRUPTION_4 = 7648;
            public const uint CORRUPTION_5 = 11671;
            public const uint CORRUPTION_6 = 11672;
            public const uint CORRUPTION_7 = 25311;
            public const uint CREATE_FIRESTONE_LESSER = 6366;
            public const uint CREATE_FIRESTONE = 17951;
            public const uint CREATE_FIRESTONE_GREATER = 17952;
            public const uint CREATE_FIRESTONE_MAJOR = 17953;
            public const uint CREATE_HEALTHSTONE_MINOR = 6201;
            public const uint CREATE_HEALTHSTONE_LESSER = 6202;
            public const uint CREATE_HEALTHSTONE = 5699;
            public const uint CREATE_HEALTHSTONE_GREATER = 11729;
            public const uint CREATE_HEALTHSTONE_MAJOR = 11730;
            public const uint CREATE_SOULSTONE_MINOR = 693;
            public const uint CREATE_SOULSTONE_LESSER = 20752;
            public const uint CREATE_SOULSTONE = 20755;
            public const uint CREATE_SOULSTONE_GREATER = 20756;
            public const uint CREATE_SOULSTONE_MAJOR = 20757;
            public const uint CREATE_SPELLSTONE = 2362;
            public const uint CREATE_SPELLSTONE_GREATER = 17727;
            public const uint CREATE_SPELLSTONE_MAJOR = 17728;
            public const uint CURSE_OF_AGONY_1 = 980;
            public const uint CURSE_OF_AGONY_2 = 1014;
            public const uint CURSE_OF_AGONY_3 = 6217;
            public const uint CURSE_OF_AGONY_4 = 11711;
            public const uint CURSE_OF_AGONY_5 = 11712;
            public const uint CURSE_OF_AGONY_6 = 11713;
            public const uint CURSE_OF_DOOM_1 = 603;
            public const uint CURSE_OF_EXHAUSTION_1 = 18223;
            public const uint CURSE_OF_RECKLESSNESS_1 = 704;
            public const uint CURSE_OF_SHADOW_1 = 17862;
            public const uint CURSE_OF_THE_ELEMENTS_1 = 1490;
            public const uint CURSE_OF_THE_ELEMENTS_2 = 11721;
            public const uint CURSE_OF_THE_ELEMENTS_3 = 11722;
            public const uint CURSE_OF_TONGUES_1 = 1714;
            public const uint CURSE_OF_TONGUES_2 = 11719;
            public const uint CURSE_OF_WEAKNESS_1 = 702;
            public const uint CURSE_OF_WEAKNESS_2 = 1108;
            public const uint CURSE_OF_WEAKNESS_3 = 6205;
            public const uint CURSE_OF_WEAKNESS_4 = 7646;
            public const uint CURSE_OF_WEAKNESS_5 = 11707;
            public const uint CURSE_OF_WEAKNESS_6 = 11708;
            public const uint DARK_PACT_1 = 18220;
            public const uint DARK_PACT_2 = 18937;
            public const uint DARK_PACT_3 = 18938;
            public const uint DEATH_COIL_WARLOCK_1 = 6789;
            public const uint DEATH_COIL_WARLOCK_2 = 17925;
            public const uint DEATH_COIL_WARLOCK_3 = 17926;
            public const uint DEMON_ARMOR_1 = 706;
            public const uint DEMON_ARMOR_2 = 1086;
            public const uint DEMON_ARMOR_3 = 11733;
            public const uint DEMON_ARMOR_4 = 11734;
            public const uint DEMON_ARMOR_5 = 11735;
            public const uint DEMON_SKIN_1 = 687;
            public const uint DEMON_SKIN_2 = 696;
            public const uint DETECT_INVISIBILITY_1 = 132;
            public const uint DRAIN_LIFE_1 = 689;
            public const uint DRAIN_LIFE_2 = 699;
            public const uint DRAIN_LIFE_3 = 709;
            public const uint DRAIN_LIFE_4 = 7651;
            public const uint DRAIN_LIFE_5 = 11699;
            public const uint DRAIN_LIFE_6 = 11700;
            public const uint DRAIN_MANA_1 = 5138;
            public const uint DRAIN_SOUL_1 = 1120;
            public const uint DRAIN_SOUL_2 = 8288;
            public const uint DRAIN_SOUL_3 = 8289;
            public const uint DRAIN_SOUL_4 = 11675;
            public const uint ENSLAVE_DEMON_1 = 1098;
            public const uint ENSLAVE_DEMON_3 = 11726;
            public const uint EYE_OF_KILROGG_1 = 126;
            public const uint FEAR_1 = 5782;
            public const uint FEAR_2 = 6213;
            public const uint FEAR_3 = 6215;
            public const uint FEL_DOMINATION_1 = 18708;
            public const uint HEALTH_FUNNEL_1 = 755;
            public const uint HEALTH_FUNNEL_2 = 3698;
            public const uint HEALTH_FUNNEL_3 = 3699;
            public const uint HEALTH_FUNNEL_4 = 3700;
            public const uint HEALTH_FUNNEL_5 = 11693;
            public const uint HEALTH_FUNNEL_6 = 11694;
            public const uint HEALTH_FUNNEL_7 = 11695;
            public const uint HELLFIRE_1 = 1949;
            public const uint HELLFIRE_2 = 11683;
            public const uint HELLFIRE_3 = 11684;
            public const uint HOWL_OF_TERROR_1 = 5484;
            public const uint HOWL_OF_TERROR_2 = 17928;
            public const uint IMMOLATE_1 = 348;
            public const uint IMMOLATE_2 = 707;
            public const uint IMMOLATE_3 = 1094;
            public const uint IMMOLATE_4 = 2941;
            public const uint IMMOLATE_5 = 11665;
            public const uint IMMOLATE_6 = 11667;
            public const uint IMMOLATE_7 = 11668;
            public const uint IMMOLATE_8 = 25309;
            public const uint INFERNO_1 = 1122;
            public const uint LIFE_TAP_1 = 1454;
            public const uint LIFE_TAP_2 = 1455;
            public const uint LIFE_TAP_3 = 1456;
            public const uint LIFE_TAP_4 = 11687;
            public const uint LIFE_TAP_5 = 11688;
            public const uint LIFE_TAP_6 = 11689;
            public const uint RAIN_OF_FIRE_1 = 5740;
            public const uint RAIN_OF_FIRE_2 = 6219;
            public const uint RAIN_OF_FIRE_3 = 11677;
            public const uint RAIN_OF_FIRE_4 = 11678;
            public const uint RITUAL_OF_DOOM_1 = 18540;
            public const uint RITUAL_OF_SUMMONING_1 = 698;
            public const uint SEARING_PAIN_1 = 5676;
            public const uint SEARING_PAIN_2 = 17919;
            public const uint SEARING_PAIN_3 = 17920;
            public const uint SEARING_PAIN_4 = 17921;
            public const uint SEARING_PAIN_5 = 17922;
            public const uint SEARING_PAIN_6 = 17923;
            public const uint SENSE_DEMONS_1 = 5500;
            public const uint SHADOW_BOLT_1 = 686;
            public const uint SHADOW_BOLT_2 = 695;
            public const uint SHADOW_BOLT_3 = 705;
            public const uint SHADOW_BOLT_4 = 1088;
            public const uint SHADOW_BOLT_5 = 1106;
            public const uint SHADOW_BOLT_6 = 7641;
            public const uint SHADOW_BOLT_7 = 11659;
            public const uint SHADOW_BOLT_8 = 11660;
            public const uint SHADOW_BOLT_9 = 11661;
            public const uint SHADOW_BOLT_10 = 25307;
            public const uint SHADOW_WARD_1 = 6229;
            public const uint SHADOW_WARD_2 = 11739;
            public const uint SHADOW_WARD_3 = 11740;
            public const uint SHADOW_WARD_4 = 28610;
            public const uint SHADOWBURN_1 = 17877;
            public const uint SHADOWBURN_2 = 18867;
            public const uint SHADOWBURN_3 = 18868;
            public const uint SHADOWBURN_4 = 18869;
            public const uint SHADOWBURN_5 = 18870;
            public const uint SHADOWBURN_6 = 18871;
            public const uint SHOOT_3 = 5019;
            public const uint SIPHON_LIFE_1 = 18265;
            public const uint SOUL_FIRE_1 = 6353;
            public const uint SOUL_FIRE_2 = 17924;
            public const uint SOUL_LINK_1 = 19028;
            public const uint SUMMON_DREADSTEED_1 = 23161;
            public const uint SUMMON_FELHUNTER_1 = 691;
            public const uint SUMMON_FELSTEED_1 = 5784;
            public const uint SUMMON_IMP_1 = 688;
            public const uint SUMMON_INFERNO_1 = 1122;
            public const uint SUMMON_SUCCUBUS_1 = 712;
            public const uint SUMMON_VOIDWALKER_1 = 697;
            public const uint UNENDING_BREATH_1 = 5697;
        }

        #endregion
    }
}
