using Populus.Core.World.Objects;
using System.Linq;

namespace Populus.GroupBot.Combat.Priest
{
    public class PriestCombatLogic : CombatLogicHandler
    {
        #region Declarations

        // holy
        protected uint CLEARCASTING,
               DESPERATE_PRAYER,
               FLASH_HEAL,
               GREATER_HEAL,
               HEAL,
               HOLY_FIRE,
               HOLY_NOVA,
               LESSER_HEAL,
               MANA_BURN,
               PRAYER_OF_HEALING,
               RENEW,
               RESURRECTION,
               SHACKLE_UNDEAD,
               SMITE,
               CURE_DISEASE,
               ABOLISH_DISEASE,
               PRIEST_DISPEL_MAGIC;

        // ranged
        protected uint SHOOT;

        // shadowmagic
        protected uint FADE,
               SHADOW_WORD_PAIN,
               MIND_BLAST,
               SCREAM,
               MIND_FLAY,
               DEVOURING_PLAGUE,
               SHADOW_PROTECTION,
               PRAYER_OF_SHADOW_PROTECTION,
               SHADOWFORM,
               VAMPIRIC_EMBRACE;

        // discipline
        protected uint POWER_WORD_SHIELD,
               INNER_FIRE,
               POWER_WORD_FORTITUDE,
               PRAYER_OF_FORTITUDE,
               FEAR_WARD,
               POWER_INFUSION,
               MASS_DISPEL,
               DIVINE_SPIRIT,
               PRAYER_OF_SPIRIT,
               INNER_FOCUS,
               ELUNES_GRACE,
               LEVITATE,
               LIGHTWELL,
               MIND_CONTROL,
               MIND_SOOTHE,
               MIND_VISION,
               PSYCHIC_SCREAM,
               SILENCE;

        #endregion

        #region Constructors

        public PriestCombatLogic(GroupBotHandler botHandler) : base(botHandler)
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

        #endregion

        #region Public Methods

        public override void InitializeSpells()
        {
            base.InitializeSpells();

            // Spells
            ABOLISH_DISEASE = InitSpell(Spells.ABOLISH_DISEASE_1);
            CURE_DISEASE = InitSpell(Spells.CURE_DISEASE_1);
            DESPERATE_PRAYER = InitSpell(Spells.DESPERATE_PRAYER_1);
            DEVOURING_PLAGUE = InitSpell(Spells.DEVOURING_PLAGUE_1);
            PRIEST_DISPEL_MAGIC = InitSpell(Spells.DISPEL_MAGIC_1);
            DIVINE_SPIRIT = InitSpell(Spells.DIVINE_SPIRIT_1);
            ELUNES_GRACE = InitSpell(Spells.ELUNES_GRACE_1);
            FADE = InitSpell(Spells.FADE_1);
            FEAR_WARD = InitSpell(Spells.FEAR_WARD_1);
            FLASH_HEAL = InitSpell(Spells.FLASH_HEAL_1);
            GREATER_HEAL = InitSpell(Spells.GREATER_HEAL_1);
            HEAL = InitSpell(Spells.HEAL_1);
            HOLY_FIRE = InitSpell(Spells.HOLY_FIRE_1);
            HOLY_NOVA = InitSpell(Spells.HOLY_NOVA_1);
            INNER_FIRE = InitSpell(Spells.INNER_FIRE_1);
            INNER_FOCUS = InitSpell(Spells.INNER_FOCUS_1);
            LESSER_HEAL = InitSpell(Spells.LESSER_HEAL_1);
            LEVITATE = InitSpell(Spells.LEVITATE_1);
            LIGHTWELL = InitSpell(Spells.LIGHTWELL_1);
            MANA_BURN = InitSpell(Spells.MANA_BURN_1);
            MIND_BLAST = InitSpell(Spells.MIND_BLAST_1);
            MIND_CONTROL = InitSpell(Spells.MIND_CONTROL_1);
            MIND_FLAY = InitSpell(Spells.MIND_FLAY_1);
            MIND_SOOTHE = InitSpell(Spells.MIND_SOOTHE_1);
            MIND_VISION = InitSpell(Spells.MIND_VISION_1);
            POWER_INFUSION = InitSpell(Spells.POWER_INFUSION_1);
            POWER_WORD_FORTITUDE = InitSpell(Spells.POWER_WORD_FORTITUDE_1);
            POWER_WORD_SHIELD = InitSpell(Spells.POWER_WORD_SHIELD_1);
            PRAYER_OF_FORTITUDE = InitSpell(Spells.PRAYER_OF_FORTITUDE_1);
            PRAYER_OF_HEALING = InitSpell(Spells.PRAYER_OF_HEALING_1);
            PRAYER_OF_SHADOW_PROTECTION = InitSpell(Spells.PRAYER_OF_SHADOW_PROTECTION_1);
            PRAYER_OF_SPIRIT = InitSpell(Spells.PRAYER_OF_SPIRIT_1);
            PSYCHIC_SCREAM = InitSpell(Spells.PSYCHIC_SCREAM_1);
            RENEW = InitSpell(Spells.RENEW_1);
            RESURRECTION = InitSpell(Spells.RESURRECTION_1);
            SHACKLE_UNDEAD = InitSpell(Spells.SHACKLE_UNDEAD_1);
            SHADOW_PROTECTION = InitSpell(Spells.SHADOW_PROTECTION_1);
            SHADOW_WORD_PAIN = InitSpell(Spells.SHADOW_WORD_PAIN_1);
            SHADOWFORM = InitSpell(Spells.SHADOWFORM_1);
            SHOOT = InitSpell(Spells.SHOOT_1);
            SMITE = InitSpell(Spells.SMITE_1);
            SILENCE = InitSpell(Spells.SILENCE_1);
            VAMPIRIC_EMBRACE = InitSpell(Spells.VAMPIRIC_EMBRACE_1);
        }

        public override CombatActionResult DoOutOfCombatAction()
        {
            // Inner Fire if not on self
            if (HasSpellAndCanCast(INNER_FIRE) && !BotHandler.BotOwner.HasAura(INNER_FIRE))
            {
                BotHandler.CombatState.SpellCast(BotHandler.BotOwner, INNER_FIRE);
                return CombatActionResult.ACTION_OK;
            }

            // Power Word Fortitude if not on self
            if (HasSpellAndCanCast(POWER_WORD_FORTITUDE) && !BotHandler.BotOwner.HasAura(POWER_WORD_FORTITUDE))
            {
                BotHandler.CombatState.SpellCast(BotHandler.BotOwner, POWER_WORD_FORTITUDE);
                return CombatActionResult.ACTION_OK;
            }

            // Handle group checks. More efficient to do these all at once
            foreach (var member in BotHandler.Group.Members)
            {
                var unit = BotHandler.BotOwner.GetUnitByGuid(member.Guid);
                if (unit == null) continue;

                // Check Power Word Fortitude
                if (!unit.HasAura(POWER_WORD_FORTITUDE) && HasSpellAndCanCast(POWER_WORD_FORTITUDE))
                {
                    BotHandler.CombatState.SpellCast(unit, POWER_WORD_FORTITUDE);
                    return CombatActionResult.ACTION_OK;
                }

                // Check if group member needs a moderate heal
                if (HasSpellAndCanCast(LESSER_HEAL) && unit.HealthPercentage < 70.0f)
                {
                    BotHandler.CombatState.SpellCast(unit, LESSER_HEAL);
                    return CombatActionResult.ACTION_OK;
                }

                // Check if group member needs a small hot
                if (HasSpellAndCanCast(RENEW) && unit.HealthPercentage < 90.0f)
                {
                    BotHandler.CombatState.SpellCast(unit, RENEW);
                    return CombatActionResult.ACTION_OK;
                }
            }

            // Power Word Fortitude if not on a member of the group
            if (HasSpellAndCanCast(POWER_WORD_FORTITUDE))
            {
                foreach (var member in BotHandler.Group.Members)
                {
                    var unit = BotHandler.BotOwner.GetUnitByGuid(member.Guid);
                    if (unit != null && !unit.HasAura(POWER_WORD_FORTITUDE))
                    {
                        BotHandler.CombatState.SpellCast(unit, POWER_WORD_FORTITUDE);
                        return CombatActionResult.ACTION_OK;
                    }
                }
            }

            // Heal group members that are low on health
            if (HasSpellAndCanCast(LESSER_HEAL))
            {

            }

            return base.DoOutOfCombatAction();
        }

        #endregion

        #region Private Methods

        protected override CombatActionResult DoFirstCombatAction(Unit unit)
        {
            return CombatActionResult.NO_ACTION_OK;
        }

        protected override CombatActionResult DoNextCombatAction(Unit unit)
        {
            // Handle group health checks.
            foreach (var member in BotHandler.Group.Members)
            {
                var memberUnit = BotHandler.BotOwner.GetUnitByGuid(member.Guid);
                if (memberUnit == null) continue;

                // Check if group member needs a moderate heal
                if (HasSpellAndCanCast(LESSER_HEAL) && memberUnit.HealthPercentage < 50.0f)
                {
                    BotHandler.CombatState.SpellCast(memberUnit, LESSER_HEAL);
                    return CombatActionResult.ACTION_OK;
                }
            }

            // TODO: Build up priest combat logic
            if (HasSpellAndCanCast(SHADOW_WORD_PAIN) && !unit.HasAura(SHADOW_WORD_PAIN) && unit.HealthPercentage > 30.0f)
            {
                BotHandler.CombatState.SpellCast(SHADOW_WORD_PAIN);
                return CombatActionResult.ACTION_OK;
            }

            // Wand if the mob is less than 40% health
            if (unit.HealthPercentage < 40.0f && AttackWand(unit))
                return CombatActionResult.NO_ACTION_OK;

            if (HasSpellAndCanCast(SMITE))
            {
                BotHandler.CombatState.SpellCast(SMITE);
                return CombatActionResult.ACTION_OK;
            }

            return CombatActionResult.NO_ACTION_OK;
        }

        #endregion

        #region Priest Constants

        public static class Reagents
        {
            public const uint SACRED_CANDLE = 17029;
        }

        public static class Spells
        {
            public const uint ABOLISH_DISEASE_1 = 552;
            public const uint CURE_DISEASE_1 = 528;
            public const uint DESPERATE_PRAYER_1 = 13908;
            public const uint DESPERATE_PRAYER_2 = 19236;
            public const uint DESPERATE_PRAYER_3 = 19238;
            public const uint DESPERATE_PRAYER_4 = 19240;
            public const uint DESPERATE_PRAYER_5 = 19241;
            public const uint DESPERATE_PRAYER_6 = 19242;
            public const uint DESPERATE_PRAYER_7 = 19243;
            public const uint DEVOURING_PLAGUE_1 = 2944;
            public const uint DEVOURING_PLAGUE_2 = 19276;
            public const uint DEVOURING_PLAGUE_3 = 19277;
            public const uint DEVOURING_PLAGUE_4 = 19278;
            public const uint DEVOURING_PLAGUE_5 = 19279;
            public const uint DEVOURING_PLAGUE_6 = 19280;
            public const uint DISPEL_MAGIC_1 = 527;
            public const uint DISPEL_MAGIC_2 = 988;
            public const uint DIVINE_SPIRIT_1 = 14752;
            public const uint DIVINE_SPIRIT_2 = 14818;
            public const uint DIVINE_SPIRIT_3 = 14819;
            public const uint DIVINE_SPIRIT_4 = 27841;
            public const uint ELUNES_GRACE_1 = 2651;
            public const uint FADE_1 = 586;
            public const uint FEAR_WARD_1 = 6346;
            public const uint FLASH_HEAL_1 = 2061;
            public const uint FLASH_HEAL_2 = 9472;
            public const uint FLASH_HEAL_3 = 9473;
            public const uint FLASH_HEAL_4 = 9474;
            public const uint FLASH_HEAL_5 = 10915;
            public const uint FLASH_HEAL_6 = 10916;
            public const uint FLASH_HEAL_7 = 10917;
            public const uint GREATER_HEAL_1 = 2060;
            public const uint GREATER_HEAL_2 = 10963;
            public const uint GREATER_HEAL_3 = 10964;
            public const uint GREATER_HEAL_4 = 10965;
            public const uint GREATER_HEAL_5 = 25314;
            public const uint HEAL_1 = 2054;
            public const uint HEAL_2 = 2055;
            public const uint HEAL_3 = 6063;
            public const uint HEAL_4 = 6064;
            public const uint HOLY_FIRE_1 = 14914;
            public const uint HOLY_FIRE_2 = 15262;
            public const uint HOLY_FIRE_3 = 15263;
            public const uint HOLY_FIRE_4 = 15264;
            public const uint HOLY_FIRE_5 = 15265;
            public const uint HOLY_FIRE_6 = 15266;
            public const uint HOLY_FIRE_7 = 15267;
            public const uint HOLY_FIRE_8 = 15261;
            public const uint HOLY_NOVA_1 = 15237;
            public const uint HOLY_NOVA_2 = 15430;
            public const uint HOLY_NOVA_3 = 15431;
            public const uint HOLY_NOVA_4 = 27799;            
            public const uint HOLY_NOVA_5 = 27800;
            public const uint HOLY_NOVA_6 = 27801;
            public const uint INNER_FIRE_1 = 588;
            public const uint INNER_FIRE_2 = 7128;
            public const uint INNER_FIRE_3 = 602;
            public const uint INNER_FIRE_4 = 1006;
            public const uint INNER_FIRE_5 = 10951;
            public const uint INNER_FIRE_6 = 10952;
            public const uint LESSER_HEAL_1 = 2050;
            public const uint LESSER_HEAL_2 = 2052;
            public const uint LESSER_HEAL_3 = 2053;
            public const uint LEVITATE_1 = 1706;
            public const uint LIGHTWELL_1 = 724;
            public const uint LIGHTWELL_2 = 27870;
            public const uint LIGHTWELL_3 = 27871;
            public const uint LIGHTWELL_RENEW_1 = 7001;
            public const uint LIGHTWELL_RENEW_2 = 27873;
            public const uint LIGHTWELL_RENEW_3 = 27874;
            public const uint MANA_BURN_1 = 8129;
            public const uint MIND_BLAST_1 = 8092;
            public const uint MIND_BLAST_2 = 8102;
            public const uint MIND_BLAST_3 = 8103;
            public const uint MIND_BLAST_4 = 8104;
            public const uint MIND_BLAST_5 = 8105;
            public const uint MIND_BLAST_6 = 8106;
            public const uint MIND_BLAST_7 = 10945;
            public const uint MIND_BLAST_8 = 10946;
            public const uint MIND_BLAST_9 = 10947;
            public const uint MIND_CONTROL_1 = 605;
            public const uint MIND_FLAY_1 = 15407;
            public const uint MIND_FLAY_2 = 17311;
            public const uint MIND_FLAY_3 = 17312;
            public const uint MIND_FLAY_4 = 17313;
            public const uint MIND_FLAY_5 = 17314;
            public const uint MIND_FLAY_6 = 18807;
            public const uint MIND_SOOTHE_1 = 453;
            public const uint MIND_VISION_1 = 2096;
            public const uint MIND_VISION_2 = 10909;
            public const uint POWER_INFUSION_1 = 10060;
            public const uint POWER_WORD_FORTITUDE_1 = 1243;
            public const uint POWER_WORD_FORTITUDE_2 = 1244;
            public const uint POWER_WORD_FORTITUDE_3 = 1245;
            public const uint POWER_WORD_FORTITUDE_4 = 2791;
            public const uint POWER_WORD_FORTITUDE_5 = 10937;
            public const uint POWER_WORD_FORTITUDE_6 = 10938;
            public const uint POWER_WORD_SHIELD_1 = 17;
            public const uint POWER_WORD_SHIELD_2 = 592;
            public const uint POWER_WORD_SHIELD_3 = 600;
            public const uint POWER_WORD_SHIELD_4 = 3747;
            public const uint POWER_WORD_SHIELD_5 = 6065;
            public const uint POWER_WORD_SHIELD_6 = 6066;
            public const uint POWER_WORD_SHIELD_7 = 10898;
            public const uint POWER_WORD_SHIELD_8 = 10899;
            public const uint POWER_WORD_SHIELD_9 = 10900;
            public const uint POWER_WORD_SHIELD_10 = 10901;
            public const uint PRAYER_OF_FORTITUDE_1 = 21562;
            public const uint PRAYER_OF_FORTITUDE_2 = 21564;
            public const uint PRAYER_OF_HEALING_1 = 596;
            public const uint PRAYER_OF_HEALING_2 = 996;
            public const uint PRAYER_OF_HEALING_3 = 10960;
            public const uint PRAYER_OF_HEALING_4 = 10961;
            public const uint PRAYER_OF_HEALING_5 = 25316;
            public const uint PRAYER_OF_SHADOW_PROTECTION_1 = 27683;
            public const uint PRAYER_OF_SPIRIT_1 = 27681;
            public const uint PSYCHIC_SCREAM_1 = 8122;
            public const uint PSYCHIC_SCREAM_2 = 8124;
            public const uint PSYCHIC_SCREAM_3 = 10888;
            public const uint PSYCHIC_SCREAM_4 = 10890;
            public const uint RENEW_1 = 139;
            public const uint RENEW_2 = 6074;
            public const uint RENEW_3 = 6075;
            public const uint RENEW_4 = 6076;
            public const uint RENEW_5 = 6077;
            public const uint RENEW_6 = 6078;
            public const uint RENEW_7 = 10927;
            public const uint RENEW_8 = 10928;
            public const uint RENEW_9 = 10929;
            public const uint RENEW_10 = 25315;
            public const uint RESURRECTION_1 = 2006;
            public const uint RESURRECTION_2 = 2010;
            public const uint RESURRECTION_3 = 10880;
            public const uint RESURRECTION_4 = 10881;
            public const uint RESURRECTION_5 = 20770;
            public const uint RIDING_1 = 33391;
            public const uint SHACKLE_UNDEAD_1 = 9484;
            public const uint SHACKLE_UNDEAD_2 = 9485;
            public const uint SHACKLE_UNDEAD_3 = 10955;
            public const uint SHADOWFORM_1 = 15473;
            public const uint SHADOW_PROTECTION_1 = 976;
            public const uint SHADOW_PROTECTION_2 = 10957;
            public const uint SHADOW_PROTECTION_3 = 10958;
            public const uint SHADOW_WORD_PAIN_1 = 589;
            public const uint SHADOW_WORD_PAIN_2 = 594;
            public const uint SHADOW_WORD_PAIN_3 = 970;
            public const uint SHADOW_WORD_PAIN_4 = 992;
            public const uint SHADOW_WORD_PAIN_5 = 2767;
            public const uint SHADOW_WORD_PAIN_6 = 10892;
            public const uint SHADOW_WORD_PAIN_7 = 10893;
            public const uint SHADOW_WORD_PAIN_8 = 10894;
            public const uint SHOOT_1 = 5019;
            public const uint SILENCE_1 = 15487;
            public const uint SMITE_1 = 585;
            public const uint SMITE_2 = 591;
            public const uint SMITE_3 = 598;
            public const uint SMITE_4 = 984;
            public const uint SMITE_5 = 1004;
            public const uint SMITE_6 = 6060;
            public const uint SMITE_7 = 10933;
            public const uint SMITE_8 = 10934;
            public const uint VAMPIRIC_EMBRACE_1 = 15286;
            public const uint WEAKENED_SOUL = 6788;
            public const uint INNER_FOCUS_1 = 14751;
        }

        #endregion
    }
}
