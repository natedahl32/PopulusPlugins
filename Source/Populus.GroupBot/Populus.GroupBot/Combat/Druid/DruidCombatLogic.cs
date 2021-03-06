﻿using FluentBehaviourTree;
using Populus.Core.World.Objects;
using System.Collections.Generic;

namespace Populus.GroupBot.Combat.Druid
{
    public class DruidCombatLogic : CombatLogicHandler
    {
        #region Declarations

        // druid cat/bear/dire bear/moonkin/tree of life forms
        protected uint CAT_FORM,
               BEAR_FORM,
               DIRE_BEAR_FORM,
               MOONKIN_FORM,
               TRAVEL_FORM,
               AQUATIC_FORM,
               TREE_FORM;

        // druid cat attacks
        protected uint CLAW,
               COWER,
               TIGERS_FURY,
               RAKE,
               RIP,
               SHRED,
               FEROCIOUS_BITE;

        // druid bear/dire bear attacks & buffs
        protected uint BASH,
               MAUL,
               SWIPE,
               DEMORALIZING_ROAR,
               CHALLENGING_ROAR,
               GROWL,
               ENRAGE,
               FAERIE_FIRE_FERAL,
               FERAL_CHARGE_BEAR,
               POUNCE,
               PROWL,
               RAVAGE;

        // druid caster DPS attacks & debuffs
        protected uint MOONFIRE,
               ROOTS,
               WRATH,
               OMEN_OF_CLARITY,
               STARFIRE,
               INSECT_SWARM,
               FAERIE_FIRE,
               HIBERNATE,
               ENTANGLING_ROOTS,
               HURRICANE,
               NATURES_GRASP,
               SOOTHE_ANIMAL;

        // druid buffs
        protected uint MARK_OF_THE_WILD,
               GIFT_OF_THE_WILD,
               THORNS,
               INNERVATE,
               NATURES_SWIFTNESS,
               BARKSKIN,
               DASH,
               FRENZIED_GENERATION;

        // druid heals
        protected uint REJUVENATION,
               REGROWTH,
               HEALING_TOUCH,
               SWIFTMEND,
               TRANQUILITY,
               REBIRTH,
               REMOVE_CURSE,
               ABOLISH_POISON,
               CURE_POISON;

        // procs
        protected uint ECLIPSE,
            ECLIPSE_SOLAR,
            ECLIPSE_LUNAR;

        #endregion

        #region Constructors

        public DruidCombatLogic(GroupBotHandler botHandler) : base(botHandler)
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
        /// Gets whether or not this class is a caster
        /// </summary>
        public override bool IsCaster
        {
            get { return false; }
        }

        /// <summary>
        /// Gets all druid spells and abilities available by level
        /// </summary>
        protected override Dictionary<int, List<uint>> SpellsByLevel => mSpellsByLevel;

        #endregion

        #region Public Methods

        public override void InitializeSpells()
        {
            base.InitializeSpells();

            // Spells
            ABOLISH_POISON = InitSpell(Spells.ABOLISH_POISON_1);
            AQUATIC_FORM = InitSpell(Spells.AQUATIC_FORM_1);
            BARKSKIN = InitSpell(Spells.BARKSKIN_1);
            BASH = InitSpell(Spells.BASH_1);
            BEAR_FORM = InitSpell(Spells.BEAR_FORM_1);
            CAT_FORM = InitSpell(Spells.CAT_FORM_1);
            CHALLENGING_ROAR = InitSpell(Spells.CHALLENGING_ROAR_1);
            CLAW = InitSpell(Spells.CLAW_1);
            COWER = InitSpell(Spells.COWER_1);
            CURE_POISON = InitSpell(Spells.CURE_POISON_1);
            DASH = InitSpell(Spells.DASH_1);
            DEMORALIZING_ROAR = InitSpell(Spells.DEMORALIZING_ROAR_1);
            DIRE_BEAR_FORM = InitSpell(Spells.DIRE_BEAR_FORM_1);
            ENRAGE = InitSpell(Spells.ENRAGE_1);
            ENTANGLING_ROOTS = InitSpell(Spells.ENTANGLING_ROOTS_1);
            FAERIE_FIRE = InitSpell(Spells.FAERIE_FIRE_1);
            FAERIE_FIRE_FERAL = InitSpell(Spells.FAERIE_FIRE_FERAL_1);
            FERAL_CHARGE_BEAR = InitSpell(Spells.FERAL_CHARGE_BEAR_1);
            FEROCIOUS_BITE = InitSpell(Spells.FEROCIOUS_BITE_1);
            FRENZIED_GENERATION = InitSpell(Spells.FRENZIED_REGENERATION_1);
            GIFT_OF_THE_WILD = InitSpell(Spells.GIFT_OF_THE_WILD_1);
            GROWL = InitSpell(Spells.GROWL_1);
            HEALING_TOUCH = InitSpell(Spells.HEALING_TOUCH_1);
            HIBERNATE = InitSpell(Spells.HIBERNATE_1);
            HURRICANE = InitSpell(Spells.HURRICANE_1);
            INNERVATE = InitSpell(Spells.INNERVATE_1);
            INSECT_SWARM = InitSpell(Spells.INSECT_SWARM_1);
            MARK_OF_THE_WILD = InitSpell(Spells.MARK_OF_THE_WILD_1);
            MAUL = InitSpell(Spells.MAUL_1);
            MOONFIRE = InitSpell(Spells.MOONFIRE_1);
            MOONKIN_FORM = InitSpell(Spells.MOONKIN_FORM_1);
            NATURES_GRASP = InitSpell(Spells.NATURES_GRASP_1);
            NATURES_SWIFTNESS = InitSpell(Spells.NATURES_SWIFTNESS_DRUID_1);
            OMEN_OF_CLARITY = InitSpell(Spells.OMEN_OF_CLARITY_1);
            POUNCE = InitSpell(Spells.POUNCE_1);
            PROWL = InitSpell(Spells.PROWL_1);
            RAKE = InitSpell(Spells.RAKE_1);
            RAVAGE = InitSpell(Spells.RAVAGE_1);
            REBIRTH = InitSpell(Spells.REBIRTH_1);
            REGROWTH = InitSpell(Spells.REGROWTH_1);
            REJUVENATION = InitSpell(Spells.REJUVENATION_1);
            REMOVE_CURSE = InitSpell(Spells.REMOVE_CURSE_1);
            RIP = InitSpell(Spells.RIP_1);
            SHRED = InitSpell(Spells.SHRED_1);
            SOOTHE_ANIMAL = InitSpell(Spells.SOOTHE_ANIMAL_1);
            STARFIRE = InitSpell(Spells.STARFIRE_1);
            SWIFTMEND = InitSpell(Spells.SWIFTMEND_1);
            SWIPE = InitSpell(Spells.SWIPE_BEAR_1);
            THORNS = InitSpell(Spells.THORNS_1);
            TIGERS_FURY = InitSpell(Spells.TIGERS_FURY_1);
            TRANQUILITY = InitSpell(Spells.TRANQUILITY_1);
            TRAVEL_FORM = InitSpell(Spells.TRAVEL_FORM_1);
            WRATH = InitSpell(Spells.WRATH_1);
            ECLIPSE = InitSpell(Spells.ECLIPSE_1);

            // Procs
            ECLIPSE_LUNAR = InitSpell(Procs.ECLIPSE_LUNAR_1);
            ECLIPSE_SOLAR = InitSpell(Procs.ECLIPSE_SOLAR_1);

        }

        public override bool Pull(Unit unit)
        {
            // TODO: Pull logic
            return false;
        }

        /// <summary>
        /// Checks for required druid skills and if not found, learns them
        /// </summary>
        internal override void CheckForSkills()
        {
            if (!BotHandler.BotOwner.HasSkill(Core.Constants.SkillType.SKILL_2H_MACES))
                BotHandler.BotOwner.ChatSay(".learn 199");
            if (!BotHandler.BotOwner.HasSkill(Core.Constants.SkillType.SKILL_MACES))
                BotHandler.BotOwner.ChatSay(".learn 198");
            if (!BotHandler.BotOwner.HasSkill(Core.Constants.SkillType.SKILL_POLEARMS))
                BotHandler.BotOwner.ChatSay(".learn 200");
            if (!BotHandler.BotOwner.HasSkill(Core.Constants.SkillType.SKILL_STAVES))
                BotHandler.BotOwner.ChatSay(".learn 227");
            if (!BotHandler.BotOwner.HasSkill(Core.Constants.SkillType.SKILL_DAGGERS))
                BotHandler.BotOwner.ChatSay(".learn 1180");
            if (!BotHandler.BotOwner.HasSkill(Core.Constants.SkillType.SKILL_FIST_WEAPONS))
                BotHandler.BotOwner.ChatSay(".learn 15590");

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

        #region Druid Constants

        // Key = Level, Values = List of spells attained at that level
        private static Dictionary<int, List<uint>> mSpellsByLevel = new Dictionary<int, List<uint>>
        {
            { 4, new List<uint> { Spells.REJUVENATION_1, Spells.MOONFIRE_1 } },
            { 6, new List<uint> { Spells.MANGLE_1, Spells.THORNS_1, Spells.WRATH_2 } },
            { 8, new List<uint> { Spells.HEALING_TOUCH_2, Spells.ENTANGLING_ROOTS_1 } },
            { 10, new List<uint> { Spells.DEMORALIZING_ROAR_1, Spells.BEAR_FORM_1, Spells.MAUL_1, Spells.GROWL_1, Spells.MARK_OF_THE_WILD_2, Spells.REJUVENATION_2, Spells.MOONFIRE_2, Spells.TELEPORT_MOONGLADE_1 } },
            { 12, new List<uint> { Spells.ENRAGE_1, Spells.REGROWTH_1 } },
            { 14, new List<uint> { Spells.BASH_1, Spells.HEALING_TOUCH_3, Spells.CURE_POISON_1, Spells.THORNS_2, Spells.WRATH_3 } },
            { 16, new List<uint> { Spells.SWIPE_1, Spells.AQUATIC_FORM_1, Spells.REJUVENATION_3, Spells.MOONFIRE_3 } },
            { 18, new List<uint> { Spells.MAUL_2, Spells.REGROWTH_2, Spells.FAERIE_FIRE_1, Spells.ENTANGLING_ROOTS_2, Spells.HIBERNATE_1 } },
            { 20, new List<uint> { Spells.CAT_FORM_1, Spells.DEMORALIZING_ROAR_2, Spells.RIP_1, Spells.PROWL_1, Spells.HEALING_TOUCH_4, Spells.MARK_OF_THE_WILD_3, Spells.REBIRTH_1, Spells.STARFIRE_1, Spells.INSECT_SWARM_1 } },
            { 22, new List<uint> { Spells.SHRED_1, Spells.REJUVENATION_4, Spells.WRATH_4, Spells.MOONFIRE_4, Spells.SOOTHE_ANIMAL_1 } },
            { 24, new List<uint> { Spells.SWIPE_2, Spells.TIGERS_FURY_1, Spells.RAKE_1, Spells.REGROWTH_3, Spells.REMOVE_CURSE_1, Spells.THORNS_3 } },
            { 26, new List<uint> { Spells.MAUL_3, Spells.DASH_1, Spells.HEALING_TOUCH_5, Spells.ABOLISH_POISON_1, Spells.STARFIRE_2 } },
            { 28, new List<uint> { Spells.CLAW_2, Spells.CHALLENGING_ROAR_1, Spells.COWER_1, Spells.RIP_2, Spells.REJUVENATION_5, Spells.MOONFIRE_5, Spells.ENTANGLING_ROOTS_3 } },
            { 30, new List<uint> { Spells.SHRED_2, Spells.BASH_2, Spells.TRAVEL_FORM_1, Spells.MARK_OF_THE_WILD_4, Spells.REGROWTH_4, Spells.TRANQUILITY_1, Spells.REBIRTH_2, Spells.WRATH_5, Spells.INSECT_SWARM_2 } },
            { 32, new List<uint> { Spells.TRACK_HUMANOIDS_1, Spells.RAVAGE_1, Spells.DEMORALIZING_ROAR_3, Spells.FEROCIOUS_BITE_1, Spells.HEALING_TOUCH_6 } },
            { 34, new List<uint> { Spells.RAKE_2, Spells.MAUL_4, Spells.SWIPE_3, Spells.REJUVENATION_6, Spells.THORNS_4, Spells.STARFIRE_3, Spells.MOONFIRE_6 } },
            { 36, new List<uint> { Spells.TIGERS_FURY_2, Spells.POUNCE_1, Spells.RIP_3, Spells.FRENZIED_REGENERATION_1, Spells.REGROWTH_5 } },
            { 38, new List<uint> { Spells.CLAW_3, Spells.SHRED_3, Spells.HEALING_TOUCH_7, Spells.WRATH_6, Spells.ENTANGLING_ROOTS_4, Spells.SOOTHE_ANIMAL_2, Spells.HIBERNATE_2 } },
            { 40, new List<uint> { Spells.COWER_2, Spells.DIRE_BEAR_FORM_1, Spells.FEROCIOUS_BITE_2, Spells.MARK_OF_THE_WILD_5, Spells.REJUVENATION_7, Spells.TRANQUILITY_2, Spells.REBIRTH_3, Spells.MOONFIRE_7, Spells.HURRICANE_1, Spells.INNERVATE_1, Spells.INSECT_SWARM_3 } },
            { 42, new List<uint> { Spells.RAVAGE_2, Spells.MAUL_5, Spells.DEMORALIZING_ROAR_4, Spells.REGROWTH_6, Spells.STARFIRE_4 } },
            { 44, new List<uint> { Spells.RAKE_3, Spells.RIP_4, Spells.SWIPE_4, Spells.HEALING_TOUCH_8, Spells.THORNS_5, Spells.BARKSKIN_1 } },
            { 46, new List<uint> { Spells.BASH_3, Spells.DASH_2, Spells.POUNCE_2, Spells.SHRED_4, Spells.REJUVENATION_8, Spells.WRATH_7, Spells.MOONFIRE_8 } },
            { 48, new List<uint> { Spells.TIGERS_FURY_3, Spells.CLAW_4, Spells.FEROCIOUS_BITE_3, Spells.REGROWTH_7, Spells.ENTANGLING_ROOTS_5 } },
            { 50, new List<uint> { Spells.RAVAGE_3, Spells.MAUL_6, Spells.HEALING_TOUCH_9, Spells.MARK_OF_THE_WILD_6, Spells.TRANQUILITY_3, Spells.REBIRTH_4, Spells.GIFT_OF_THE_WILD_1, Spells.STARFIRE_5, Spells.HURRICANE_2, Spells.INSECT_SWARM_4 } },
            { 52, new List<uint> { Spells.COWER_3, Spells.RIP_5, Spells.DEMORALIZING_ROAR_5, Spells.REJUVENATION_9, Spells.MOONFIRE_9 } },
            { 54, new List<uint> { Spells.SHRED_5, Spells.RAKE_4, Spells.SWIPE_5, Spells.REGROWTH_8, Spells.THORNS_6, Spells.WRATH_8, Spells.SOOTHE_ANIMAL_3 } },
            { 56, new List<uint> { Spells.POUNCE_3, Spells.FEROCIOUS_BITE_4, Spells.HEALING_TOUCH_10 } },
            { 58, new List<uint> { Spells.CLAW_5, Spells.RAVAGE_4, Spells.MAUL_7, Spells.REJUVENATION_10, Spells.STARFIRE_6, Spells.MOONFIRE_10, Spells.ENTANGLING_ROOTS_6, Spells.HIBERNATE_3 } },
            { 60, new List<uint> { Spells.TIGERS_FURY_4, Spells.RIP_6, Spells.FEROCIOUS_BITE_5, Spells.MARK_OF_THE_WILD_7, Spells.REGROWTH_9, Spells.TRANQUILITY_4, Spells.REBIRTH_5, Spells.GIFT_OF_THE_WILD_2, Spells.REJUVENATION_11, Spells.HEALING_TOUCH_11 } }
        };

        public static class Reagents
        {
            public const uint WILD_THORNROOT = 17026;
        }

        public static class Procs
        {
            public const uint ECLIPSE_SOLAR_1 = 48517;
            public const uint ECLIPSE_LUNAR_1 = 48518;
        }

        public static class Spells
        {
            public const uint ABOLISH_POISON_1 = 2893;
            public const uint ABOLISH_POISON_EFFECT_1 = 3137;
            public const uint AQUATIC_FORM_1 = 1066;
            public const uint BARKSKIN_1 = 22812;
            public const uint BASH_1 = 5211;
            public const uint BASH_2 = 6798;
            public const uint BASH_3 = 8983;
            public const uint BEAR_FORM_1 = 5487;
            public const uint CAT_FORM_1 = 768;
            public const uint CHALLENGING_ROAR_1 = 5209;
            public const uint CLAW_1 = 1082;
            public const uint CLAW_2 = 3029;
            public const uint CLAW_3 = 5201;
            public const uint CLAW_4 = 9849;
            public const uint CLAW_5 = 9850;
            public const uint COPY_OF_FEROCIOUS_BITE_4 = 24248;
            public const uint COWER_1 = 8998;
            public const uint COWER_2 = 9000;
            public const uint COWER_3 = 9892;
            public const uint CURE_POISON_1 = 8946;
            public const uint DASH_1 = 1850;
            public const uint DASH_2 = 9821;
            public const uint DEMORALIZING_ROAR_1 = 99;
            public const uint DEMORALIZING_ROAR_2 = 1735;
            public const uint DEMORALIZING_ROAR_3 = 9490;
            public const uint DEMORALIZING_ROAR_4 = 9747;
            public const uint DEMORALIZING_ROAR_5 = 9898;
            public const uint DIRE_BEAR_FORM_1 = 9634;
            public const uint ENRAGE_1 = 5229;
            public const uint ENTANGLING_ROOTS_1 = 339;
            public const uint ENTANGLING_ROOTS_2 = 1062;
            public const uint ENTANGLING_ROOTS_3 = 5195;
            public const uint ENTANGLING_ROOTS_4 = 5196;
            public const uint ENTANGLING_ROOTS_5 = 9852;
            public const uint ENTANGLING_ROOTS_6 = 9853;
            public const uint FAERIE_FIRE_1 = 770;
            public const uint FAERIE_FIRE_FERAL_1 = 16857;
            public const uint FELINE_GRACE_1 = 20719;
            public const uint FERAL_CHARGE_BEAR_1 = 16979;
            public const uint FEROCIOUS_BITE_1 = 22568;
            public const uint FEROCIOUS_BITE_2 = 22827;
            public const uint FEROCIOUS_BITE_3 = 22828;
            public const uint FEROCIOUS_BITE_4 = 22829;
            public const uint FEROCIOUS_BITE_5 = 31018;
            public const uint FRENZIED_REGENERATION_1 = 22842;
            public const uint GIFT_OF_THE_WILD_1 = 21849;
            public const uint GIFT_OF_THE_WILD_2 = 21850;
            public const uint GROWL_1 = 6795;
            public const uint HEALING_TOUCH_1 = 5185;
            public const uint HEALING_TOUCH_2 = 5186;
            public const uint HEALING_TOUCH_3 = 5187;
            public const uint HEALING_TOUCH_4 = 5188;
            public const uint HEALING_TOUCH_5 = 5189;
            public const uint HEALING_TOUCH_6 = 6778;
            public const uint HEALING_TOUCH_7 = 8903;
            public const uint HEALING_TOUCH_8 = 9758;
            public const uint HEALING_TOUCH_9 = 9888;
            public const uint HEALING_TOUCH_10 = 9889;
            public const uint HEALING_TOUCH_11 = 25297;
            public const uint HIBERNATE_1 = 2637;
            public const uint HIBERNATE_2 = 18657;
            public const uint HIBERNATE_3 = 18658;
            public const uint HURRICANE_1 = 16914;
            public const uint HURRICANE_2 = 17401;
            public const uint HURRICANE_3 = 17402;
            public const uint INNERVATE_1 = 29166;
            public const uint INSECT_SWARM_1 = 5570;
            public const uint INSECT_SWARM_2 = 24974;
            public const uint INSECT_SWARM_3 = 24975;
            public const uint INSECT_SWARM_4 = 24976;
            public const uint INSECT_SWARM_5 = 24977;
            public const uint MANGLE_1 = 22570;
            public const uint MARK_OF_THE_WILD_1 = 1126;
            public const uint MARK_OF_THE_WILD_2 = 5232;
            public const uint MARK_OF_THE_WILD_3 = 6756;
            public const uint MARK_OF_THE_WILD_4 = 5234;
            public const uint MARK_OF_THE_WILD_5 = 8907;
            public const uint MARK_OF_THE_WILD_6 = 9884;
            public const uint MARK_OF_THE_WILD_7 = 9885;
            public const uint MAUL_1 = 6807;
            public const uint MAUL_2 = 6808;
            public const uint MAUL_3 = 6809;
            public const uint MAUL_4 = 8972;
            public const uint MAUL_5 = 9745;
            public const uint MAUL_6 = 9880;
            public const uint MAUL_7 = 9881;
            public const uint MOONFIRE_1 = 8921;
            public const uint MOONFIRE_2 = 8924;
            public const uint MOONFIRE_3 = 8925;
            public const uint MOONFIRE_4 = 8926;
            public const uint MOONFIRE_5 = 8927;
            public const uint MOONFIRE_6 = 8928;
            public const uint MOONFIRE_7 = 8929;
            public const uint MOONFIRE_8 = 9833;
            public const uint MOONFIRE_9 = 9834;
            public const uint MOONFIRE_10 = 9835;
            public const uint MOONKIN_AURA_1 = 24907;
            public const uint MOONKIN_FORM_1 = 24858;
            public const uint NATURES_GRASP_1 = 16689;
            public const uint NATURES_GRASP_2 = 16810;
            public const uint NATURES_GRASP_3 = 16811;
            public const uint NATURES_GRASP_4 = 16812;
            public const uint NATURES_GRASP_5 = 16813;
            public const uint NATURES_GRASP_6 = 17329;
            public const uint NATURES_SWIFTNESS_DRUID_1 = 17116;
            public const uint OMEN_OF_CLARITY_1 = 16864;
            public const uint POUNCE_1 = 9005;
            public const uint POUNCE_2 = 9823;
            public const uint POUNCE_3 = 9827;
            public const uint POUNCE_BLEED_1 = 9007;
            public const uint POUNCE_BLEED_2 = 9824;
            public const uint POUNCE_BLEED_3 = 9826;
            public const uint PROWL_1 = 5215;
            public const uint RAKE_1 = 1822;
            public const uint RAKE_2 = 1823;
            public const uint RAKE_3 = 1824;
            public const uint RAKE_4 = 9904;
            public const uint RAVAGE_1 = 6785;
            public const uint RAVAGE_2 = 6787;
            public const uint RAVAGE_3 = 9866;
            public const uint RAVAGE_4 = 9867;
            public const uint REBIRTH_1 = 20484;
            public const uint REBIRTH_2 = 20739;
            public const uint REBIRTH_3 = 20742;
            public const uint REBIRTH_4 = 20747;
            public const uint REBIRTH_5 = 20748;
            public const uint REGROWTH_1 = 8936;
            public const uint REGROWTH_2 = 8938;
            public const uint REGROWTH_3 = 8939;
            public const uint REGROWTH_4 = 8940;
            public const uint REGROWTH_5 = 8941;
            public const uint REGROWTH_6 = 9750;
            public const uint REGROWTH_7 = 9856;
            public const uint REGROWTH_8 = 9857;
            public const uint REGROWTH_9 = 9858;
            public const uint REJUVENATION_1 = 774;
            public const uint REJUVENATION_2 = 1058;
            public const uint REJUVENATION_3 = 1430;
            public const uint REJUVENATION_4 = 2090;
            public const uint REJUVENATION_5 = 2091;
            public const uint REJUVENATION_6 = 3627;
            public const uint REJUVENATION_7 = 8910;
            public const uint REJUVENATION_8 = 9839;
            public const uint REJUVENATION_9 = 9840;
            public const uint REJUVENATION_10 = 9841;
            public const uint REJUVENATION_11 = 25299;
            public const uint REMOVE_CURSE_1 = 2782;
            public const uint RIDING_1 = 33391;
            public const uint RIP_1 = 1079;
            public const uint RIP_2 = 9492;
            public const uint RIP_3 = 9493;
            public const uint RIP_4 = 9752;
            public const uint RIP_5 = 9894;
            public const uint RIP_6 = 9896;
            public const uint SHRED_1 = 5221;
            public const uint SHRED_2 = 6800;
            public const uint SHRED_3 = 8992;
            public const uint SHRED_4 = 9829;
            public const uint SHRED_5 = 9830;
            public const uint SOOTHE_ANIMAL_1 = 2908;
            public const uint SOOTHE_ANIMAL_2 = 8955;
            public const uint SOOTHE_ANIMAL_3 = 9901;
            public const uint STARFIRE_1 = 2912;
            public const uint STARFIRE_2 = 8949;
            public const uint STARFIRE_3 = 8950;
            public const uint STARFIRE_4 = 8951;
            public const uint STARFIRE_5 = 9875;
            public const uint STARFIRE_6 = 9876;
            public const uint STARFIRE_7 = 25298;
            public const uint SWIFTMEND_1 = 18562;
            public const uint SWIPE_BEAR_1 = 779;
            public const uint SWIPE_1 = 779;
            public const uint SWIPE_2 = 780;
            public const uint SWIPE_3 = 769;
            public const uint SWIPE_4 = 9754;
            public const uint SWIPE_5 = 9908;
            public const uint TELEPORT_MOONGLADE_1 = 18960;
            public const uint THORNS_1 = 467;
            public const uint THORNS_2 = 782;
            public const uint THORNS_3 = 1075;
            public const uint THORNS_4 = 8914;
            public const uint THORNS_5 = 9756;
            public const uint THORNS_6 = 9910;
            public const uint TIGERS_FURY_1 = 5217;
            public const uint TIGERS_FURY_2 = 6793;
            public const uint TIGERS_FURY_3 = 9845;
            public const uint TIGERS_FURY_4 = 9846;
            public const uint TRACK_HUMANOIDS_1 = 5225;
            public const uint TRANQUILITY_1 = 740;
            public const uint TRANQUILITY_2 = 8918;
            public const uint TRANQUILITY_3 = 9862;
            public const uint TRANQUILITY_4 = 9863;
            public const uint TRAVEL_FORM_1 = 783;
            public const uint WRATH_1 = 5176;
            public const uint WRATH_2 = 5177;
            public const uint WRATH_3 = 5178;
            public const uint WRATH_4 = 5179;
            public const uint WRATH_5 = 5180;
            public const uint WRATH_6 = 6780;
            public const uint WRATH_7 = 8905;
            public const uint WRATH_8 = 9912;


            public const uint ECLIPSE_1 = 48525;
        }

        #endregion
    }
}
