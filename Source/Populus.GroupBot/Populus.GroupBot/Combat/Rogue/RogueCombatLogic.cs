using Populus.Core.World.Objects;
using System.Collections.Generic;

namespace Populus.GroupBot.Combat.Rogue
{
    public class RogueCombatLogic : CombatLogicHandler
    {
        #region Declarations

        // COMBAT
        protected uint ADRENALINE_RUSH,
               SINISTER_STRIKE,
               BACKSTAB,
               GOUGE,
               EVASION,
               SPRINT,
               KICK,
               FEINT;

        // SUBTLETY
        protected uint STEALTH,
               VANISH,
               HEMORRHAGE,
               BLIND,
               PICK_POCKET,
               CRIPPLING_POISON,
               DEADLY_POISON,
               MIND_NUMBING_POISON,
               GHOSTLY_STRIKE,
               DISTRACT,
               PREPARATION,
               PREMEDITATION;

        // ASSASSINATION
        protected uint COLD_BLOOD,
               EVISCERATE,
               SLICE_DICE,
               GARROTE,
               EXPOSE_ARMOR,
               AMBUSH,
               RUPTURE,
               CHEAP_SHOT,
               KIDNEY_SHOT,
               BLADE_FURRY,
               DISARM_TRAP,
               PICK_LOCK,
               RIPOSTE,
               SAP;

        // Poisons
        protected static IList<uint> INSTANT_POISONS = new List<uint> { Poisons.INSTANT_POISON_6,
                                                                 Poisons.INSTANT_POISON_5,
                                                                 Poisons.INSTANT_POISON_4,
                                                                 Poisons.INSTANT_POISON_3,
                                                                 Poisons.INSTANT_POISON_2,
                                                                 Poisons.INSTANT_POISON_1};
        protected static IList<uint> DEADLY_POISONS = new List<uint> { Poisons.DEADLY_POISON_5,
                                                                 Poisons.DEADLY_POISON_4,
                                                                 Poisons.DEADLY_POISON_3,
                                                                 Poisons.DEADLY_POISON_2,
                                                                 Poisons.DEADLY_POISON_1};
        protected static IList<uint> CRIPPLING_POISONS = new List<uint> { Poisons.CRIPPLING_POISON_2,
                                                                 Poisons.CRIPPLING_POISON_1};
        protected static IList<uint> MIND_NUMBING_POISONS = new List<uint> { Poisons.MIND_NUMBING_POISON_3,
                                                                 Poisons.MIND_NUMBING_POISON_2,
                                                                 Poisons.MIND_NUMBING_POISON_1};
        protected static IList<uint> WOUND_POISONS = new List<uint> { Poisons.WOUND_POISON_4,
                                                                 Poisons.WOUND_POISON_3,
                                                                 Poisons.WOUND_POISON_2,
                                                                 Poisons.WOUND_POISON_1};

        #endregion

        #region Constructors

        public RogueCombatLogic(GroupBotHandler botHandler) : base(botHandler)
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

        #endregion

        #region Public Methods

        public override void InitializeSpells()
        {
            base.InitializeSpells();

            // Spells
            ADRENALINE_RUSH = InitSpell(Spells.ADRENALINE_RUSH_1);
            AMBUSH = InitSpell(Spells.AMBUSH_1);
            BACKSTAB = InitSpell(Spells.BACKSTAB_1);
            BLADE_FURRY = InitSpell(Spells.BLADE_FLURRY_1);
            BLIND = InitSpell(Spells.BLIND_1);
            CHEAP_SHOT = InitSpell(Spells.CHEAP_SHOT_1);
            COLD_BLOOD = InitSpell(Spells.COLD_BLOOD_1);
            DISARM_TRAP = InitSpell(Spells.DISARM_TRAP_1);
            DISTRACT = InitSpell(Spells.DISTRACT_1);
            EVASION = InitSpell(Spells.EVASION_1);
            EVISCERATE = InitSpell(Spells.EVISCERATE_1);
            EXPOSE_ARMOR = InitSpell(Spells.EXPOSE_ARMOR_1);
            FEINT = InitSpell(Spells.FEINT_1);
            GARROTE = InitSpell(Spells.GARROTE_1);
            GHOSTLY_STRIKE = InitSpell(Spells.GHOSTLY_STRIKE_1);
            GOUGE = InitSpell(Spells.GOUGE_1);
            HEMORRHAGE = InitSpell(Spells.HEMORRHAGE_1);
            KICK = InitSpell(Spells.KICK_1);
            KIDNEY_SHOT = InitSpell(Spells.KIDNEY_SHOT_1);
            PICK_LOCK = InitSpell(Spells.PICK_LOCK_1);
            PICK_POCKET = InitSpell(Spells.PICK_POCKET_1);
            PREMEDITATION = InitSpell(Spells.PREMEDITATION_1);
            PREPARATION = InitSpell(Spells.PREPARATION_1);
            RIPOSTE = InitSpell(Spells.RIPOSTE_1);
            RUPTURE = InitSpell(Spells.RUPTURE_1);
            SAP = InitSpell(Spells.SAP_1);
            SINISTER_STRIKE = InitSpell(Spells.SINISTER_STRIKE_1);
            SLICE_DICE = InitSpell(Spells.SLICE_AND_DICE_1);
            SPRINT = InitSpell(Spells.SPRINT_1);
            STEALTH = InitSpell(Spells.STEALTH_1);
            VANISH = InitSpell(Spells.VANISH_1);
        }

        #endregion

        #region Private Methods

        protected override CombatActionResult DoFirstCombatAction(Unit unit)
        {
            AttackMelee(unit);
            return base.DoFirstCombatAction(unit);
        }

        protected override CombatActionResult DoNextCombatAction(Unit unit)
        {
            // TODO: Build up rogue combat logic
            if (BotHandler.BotOwner.ComboPoints >= 4 && HasSpellAndCanCast(EVISCERATE))
            {
                BotHandler.CombatState.SpellCast(EVISCERATE);
                return CombatActionResult.ACTION_OK;
            }

            if (HasSpellAndCanCast(SINISTER_STRIKE))
            {
                BotHandler.CombatState.SpellCast(SINISTER_STRIKE);
                return CombatActionResult.ACTION_OK;
            }

            return base.DoNextCombatAction(unit);
        }

        #endregion

        #region Rogue Constants

        public static class Poisons
        {
            public const uint INSTANT_POISON_1 = 6947;
            public const uint INSTANT_POISON_2 = 6949;
            public const uint INSTANT_POISON_3 = 6950;
            public const uint INSTANT_POISON_4 = 8926;
            public const uint INSTANT_POISON_5 = 8927;
            public const uint INSTANT_POISON_6 = 8928;

            public const uint CRIPPLING_POISON_1 = 3775;
            public const uint CRIPPLING_POISON_2 = 3776;

            public const uint DEADLY_POISON_1 = 2892;
            public const uint DEADLY_POISON_2 = 2893;
            public const uint DEADLY_POISON_3 = 8984;
            public const uint DEADLY_POISON_4 = 8985;
            public const uint DEADLY_POISON_5 = 20844;

            public const uint MIND_NUMBING_POISON_1 = 5237;
            public const uint MIND_NUMBING_POISON_2 = 6951;
            public const uint MIND_NUMBING_POISON_3 = 9186;

            public const uint WOUND_POISON_1 = 10918;
            public const uint WOUND_POISON_2 = 10920;
            public const uint WOUND_POISON_3 = 10921;
            public const uint WOUND_POISON_4 = 10922;
        }

        public static class Spells
        {
            public const uint ADRENALINE_RUSH_1 = 13750;
            public const uint AMBUSH_1 = 8676;
            public const uint BACKSTAB_1 = 53;
            public const uint BLADE_FLURRY_1 = 13877;
            public const uint BLIND_1 = 2094;
            public const uint CHEAP_SHOT_1 = 1833;
            public const uint COLD_BLOOD_1 = 14177;
            public const uint DISARM_TRAP_1 = 1842;
            public const uint DISTRACT_1 = 1725;
            public const uint EVASION_1 = 5277;
            public const uint EVISCERATE_1 = 2098;
            public const uint EXPOSE_ARMOR_1 = 8647;
            public const uint FEINT_1 = 1966;
            public const uint GARROTE_1 = 703;
            public const uint GHOSTLY_STRIKE_1 = 14278;
            public const uint GOUGE_1 = 1776;
            public const uint HEMORRHAGE_1 = 16511;
            public const uint KICK_1 = 1766;
            public const uint KIDNEY_SHOT_1 = 408;
            public const uint PICK_LOCK_1 = 1804;
            public const uint PICK_POCKET_1 = 921;
            public const uint PREMEDITATION_1 = 14183;
            public const uint PREPARATION_1 = 14185;
            public const uint RIPOSTE_1 = 14251;
            public const uint RUPTURE_1 = 1943;
            public const uint SAP_1 = 6770;
            public const uint SINISTER_STRIKE_1 = 1752;
            public const uint SLICE_AND_DICE_1 = 5171;
            public const uint SPRINT_1 = 2983;
            public const uint STEALTH_1 = 1784;
            public const uint VANISH_1 = 1856;
        }

        #endregion
    }
}
