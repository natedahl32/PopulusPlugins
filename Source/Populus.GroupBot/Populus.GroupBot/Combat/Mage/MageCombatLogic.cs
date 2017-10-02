using FluentBehaviourTree;
using Populus.Core.World.Objects;
using System.Collections.Generic;
using System.Linq;

namespace Populus.GroupBot.Combat.Mage
{
    public class MageCombatLogic : CombatLogicHandler
    {
        #region Declarations

        // ARCANE
        protected uint ARCANE_MISSILES,
               ARCANE_EXPLOSION,
               COUNTERSPELL,
               SLOW,
               POLYMORPH,
               ARCANE_POWER;

        // RANGED
        protected uint SHOOT;

        // FIRE
        protected uint FIREBALL,
               FIRE_BLAST,
               FLAMESTRIKE,
               SCORCH,
               PYROBLAST,
               BLAST_WAVE,
               COMBUSTION,
               FIRE_WARD;

        // FROST
        protected uint FROSTBOLT,
               FROST_NOVA,
               BLIZZARD,
               CONE_OF_COLD,
               ICE_BARRIER,
               FROST_WARD,
               ICE_BLOCK,
               COLD_SNAP;

        // buffs
        protected uint FROST_ARMOR,
               ICE_ARMOR,
               MAGE_ARMOR,
               ARCANE_INTELLECT,
               ARCANE_BRILLIANCE,
               MANA_SHIELD,
               DAMPEN_MAGIC,
               AMPLIFY_MAGIC;

        protected uint CONJURE_WATER,
               CONJURE_FOOD,
               CONJURE_MANA_GEM,
               BLINK,
               EVOCATION,
               INVISIBILITY,
               PRESENCE_OF_MIND,
               REMOVE_CURSE,
               SLOW_FALL;

        #endregion

        #region Constructors

        public MageCombatLogic(GroupBotHandler botHandler) : base(botHandler)
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
        /// Gets all mage spells and abilities available by level
        /// </summary>
        protected override Dictionary<int, List<uint>> SpellsByLevel => mSpellsByLevel;

        #endregion

        #region Public Methods

        public override void InitializeSpells()
        {
            base.InitializeSpells();

            // Spells
            AMPLIFY_MAGIC = InitSpell(Spells.AMPLIFY_MAGIC_1);
            ARCANE_BRILLIANCE = InitSpell(Spells.ARCANE_BRILLIANCE_1);
            ARCANE_EXPLOSION = InitSpell(Spells.ARCANE_EXPLOSION_1);
            ARCANE_INTELLECT = InitSpell(Spells.ARCANE_INTELLECT_1);
            ARCANE_MISSILES = InitSpell(Spells.ARCANE_MISSILES_1);
            ARCANE_POWER = InitSpell(Spells.ARCANE_POWER_1);
            BLAST_WAVE = InitSpell(Spells.BLAST_WAVE_1);
            BLINK = InitSpell(Spells.BLINK_1);
            BLIZZARD = InitSpell(Spells.BLIZZARD_1);
            COLD_SNAP = InitSpell(Spells.COLD_SNAP_1);
            COMBUSTION = InitSpell(Spells.COMBUSTION_1);
            CONE_OF_COLD = InitSpell(Spells.CONE_OF_COLD_1);
            CONJURE_FOOD = InitSpell(Spells.CONJURE_FOOD_1);
            CONJURE_WATER = InitSpell(Spells.CONJURE_WATER_1);
            CONJURE_MANA_GEM = InitSpell(Spells.CONJURE_MANA_GEM_1);
            COUNTERSPELL = InitSpell(Spells.COUNTERSPELL_1);
            DAMPEN_MAGIC = InitSpell(Spells.DAMPEN_MAGIC_1);
            EVOCATION = InitSpell(Spells.EVOCATION_1);
            FIRE_BLAST = InitSpell(Spells.FIRE_BLAST_1);
            FIRE_WARD = InitSpell(Spells.FIRE_WARD_1);
            FIREBALL = InitSpell(Spells.FIREBALL_1);
            FLAMESTRIKE = InitSpell(Spells.FLAMESTRIKE_1);
            FROST_ARMOR = InitSpell(Spells.FROST_ARMOR_1);
            FROST_NOVA = InitSpell(Spells.FROST_NOVA_1);
            FROST_WARD = InitSpell(Spells.FROST_WARD_1);
            FROSTBOLT = InitSpell(Spells.FROSTBOLT_1);
            ICE_ARMOR = InitSpell(Spells.ICE_ARMOR_1);
            ICE_BARRIER = InitSpell(Spells.ICE_BARRIER_1);
            ICE_BLOCK = InitSpell(Spells.ICE_BLOCK_1);
            INVISIBILITY = InitSpell(Spells.INVISIBILITY_1);
            MAGE_ARMOR = InitSpell(Spells.MAGE_ARMOR_1);
            MANA_SHIELD = InitSpell(Spells.MANA_SHIELD_1);
            POLYMORPH = InitSpell(Spells.POLYMORPH_1);
            PRESENCE_OF_MIND = InitSpell(Spells.PRESENCE_OF_MIND_1);
            PYROBLAST = InitSpell(Spells.PYROBLAST_1);
            REMOVE_CURSE = InitSpell(Spells.REMOVE_CURSE_MAGE_1);
            SCORCH = InitSpell(Spells.SCORCH_1);
            SHOOT = InitSpell(Spells.SHOOT_1);
            SLOW_FALL = InitSpell(Spells.SLOW_FALL_1);
            SLOW = InitSpell(Spells.SLOW_1);
        }

        public override bool Pull(Unit unit)
        {
            // TODO: Pull logic
            return false;
        }

        #endregion

        #region Private Methods

        protected override IBehaviourTreeNode InitializeCombatBehaivor()
        {
            var builder = new BehaviourTreeBuilder();
            builder.Selector("Combat Behavior")
                        .Do("Is Dead", t => BotHandler.BotOwner.IsDead ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure)
                        .Do("Is Casting", t => BotHandler.CombatState.IsCasting ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure)
                        .Do("Cast Frostbolt", t => CastSpell(FROSTBOLT))
                        .Do("Cast Fireball", t => CastSpell(FIREBALL))
                        .Splice(WandAttack(BotHandler))
                   .End();
            return builder.Build();
        }

        protected override IBehaviourTreeNode InitializeOutOfCombatBehavior()
        {
            var builder = new BehaviourTreeBuilder();
            builder.Selector("OOC Behavior")
                        .Do("Is Dead", t => BotHandler.BotOwner.IsDead ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure)
                        .Do("Is Casting", t => BotHandler.CombatState.IsCasting ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure)
                        .Parallel("Eat and Drink", 2, 2)    // Run eat and drink in paralell until both fail
                            .Do("Eat", t => OutOfCombatLogic.OutOfCombatHealthRegen(BotHandler))
                            .Do("Drink", t => OutOfCombatLogic.OutOfCombatManaRegen(BotHandler))
                        .End()
                        .Splice(OutOfCombatBuffsTree())
                        .Do("Spend Talent Points", t => OutOfCombatLogic.UseFreeTalentPoints(BotHandler))
                        .Do("Follow Group Leader", t => OutOfCombatLogic.FollowGroupLeader(BotHandler))
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
            builder.Selector("Mage Buffs")
                        .Do("Arcane Brilliance", t => GroupBuff(ARCANE_BRILLIANCE))
                        .Do("Arcane Intellect", t => ArcaneIntellect())
                        .Splice(ArmorBuff())
                        .Splice(SummonManaGems())
                   .End();
            return builder.Build();
        }

        /// <summary>
        /// Creates a behavior tree that handles the armor buff for a mage
        /// </summary>
        /// <returns></returns>
        protected virtual IBehaviourTreeNode ArmorBuff()
        {
            var builder = new BehaviourTreeBuilder();
            builder.Selector("Armor Buff")
                        .Do("Mage Armor", t => CheckForAndCastArmor(MAGE_ARMOR))
                        .Do("Ice Armor", t => CheckForAndCastArmor(ICE_ARMOR))
                        .Do("Frost Armor", t => CheckForAndCastArmor(FROST_ARMOR))
                   .End();
            return builder.Build();
        }

        /// <summary>
        /// Creates a behavior tree that handles the mage summoning mana gems
        /// </summary>
        /// <returns></returns>
        protected virtual IBehaviourTreeNode SummonManaGems()
        {
            var builder = new BehaviourTreeBuilder();
            builder.Selector("Summon Mana Gems")
                        .Do("Mana Ruby", t => CheckForAndCastManaGem(Spells.CONJURE_MANA_GEM_4))
                        .Do("Mana Citrine", t => CheckForAndCastManaGem(Spells.CONJURE_MANA_GEM_3))
                        .Do("Mana Jade", t => CheckForAndCastManaGem(Spells.CONJURE_MANA_GEM_2))
                        .Do("Mana Agate", t => CheckForAndCastManaGem(Spells.CONJURE_MANA_GEM_1))
                   .End();
            return builder.Build();
        }

        /// <summary>
        /// Checks for existance of a mana gem in inventory and cast the spell if it is not found
        /// </summary>
        /// <param name="manaGemSpell"></param>
        /// <returns></returns>
        protected BehaviourTreeStatus CheckForAndCastManaGem(uint manaGemSpell)
        {
            // If we can't cast the spell, fail
            if (!HasSpellAndCanCast(manaGemSpell))
                return BehaviourTreeStatus.Failure;

            // If we can't find the mana gem spell, fail
            var spell = Spell(manaGemSpell);
            if (spell == null)
                return BehaviourTreeStatus.Failure;

            // If we can't find the mana gem in inventory, cast and succeed
            var itemInInv = BotHandler.BotOwner.GetInventoryItem(spell.CreatesItemId);
            if (itemInInv == null)
            {
                if (!BotHandler.CombatState.SpellCast(BotHandler.BotOwner, manaGemSpell))
                    return BehaviourTreeStatus.Failure;
                return BehaviourTreeStatus.Success;
            }

            // Otherwise, we have the mana gem in inventory, so succeed
            return BehaviourTreeStatus.Failure;
        }

        /// <summary>
        /// Checks for an armor spell buff and if has the aura, returns success. Fails if
        /// the buff is not found and we cannot cast the buff.
        /// </summary>
        /// <param name="armorSpell">Armor spell to check</param>
        /// <returns></returns>
        protected BehaviourTreeStatus CheckForAndCastArmor(uint armorSpell)
        {
            // If we do not have the armor aura and we have the spell and can cast it, succeed
            if (!BotHandler.BotOwner.HasAura(armorSpell) && HasSpellAndCanCast(armorSpell))
            {
                if (!BotHandler.CombatState.SpellCast(BotHandler.BotOwner, armorSpell))
                    return BehaviourTreeStatus.Failure;
                return BehaviourTreeStatus.Success;
            }

            // If we can't cast we fail
            return BehaviourTreeStatus.Failure;
        }

        private BehaviourTreeStatus ArcaneIntellect()
        {
            // If we are not in a group, self buff instead
            if (BotHandler.Group == null)
                if (!BotHandler.BotOwner.HasAura(Spells.ARCANE_BRILLIANCE_1))
                    return SelfBuff(ARCANE_INTELLECT);

            // If does not have spell or cannot cast, fail
            if (!HasSpellAndCanCast(ARCANE_INTELLECT))
                return BehaviourTreeStatus.Failure;

            // Get the first member in the group that needs the aura
            var needs = BotHandler.Group.Members.Where(m => !m.HasAura((ushort)ARCANE_INTELLECT) && !m.HasAura((ushort)Spells.ARCANE_BRILLIANCE_1)).FirstOrDefault();
            if (needs == null)
                return BehaviourTreeStatus.Failure;
            // If the member is not a mana user, they don't need this buff
            if (needs.PowerType != Core.Constants.Powers.POWER_MANA)
                return BehaviourTreeStatus.Failure;

            var player = BotHandler.BotOwner.GetPlayerByGuid(needs.Guid);
            if (player == null)
                return BehaviourTreeStatus.Failure;

            if (!BotHandler.CombatState.SpellCast(player, ARCANE_INTELLECT))
                return BehaviourTreeStatus.Failure;
            return BehaviourTreeStatus.Success;
        }

        #endregion

        #region Mage Constants

        // Key = Level, Values = List of spells attained at that level
        private static Dictionary<int, List<uint>> mSpellsByLevel = new Dictionary<int, List<uint>>
        {
            { 4, new List<uint> { Spells.FROSTBOLT_1, Spells.CONJURE_WATER_1 } },
            { 6, new List<uint> { Spells.FIREBALL_2, Spells.FIRE_BLAST_1, Spells.CONJURE_FOOD_1 } },
            { 8, new List<uint> { Spells.FROSTBOLT_2, Spells.ARCANE_MISSILES_1, Spells.POLYMORPH_1 } },
            { 10, new List<uint> { Spells.FROST_NOVA_1, Spells.FROST_ARMOR_1, Spells.CONJURE_WATER_2 } },
            { 12, new List<uint> { Spells.FIREBALL_3, Spells.DAMPEN_MAGIC_1, Spells.SLOW_FALL_1, Spells.CONJURE_FOOD_2 } },
            { 14, new List<uint> { Spells.FROSTBOLT_3, Spells.FIRE_BLAST_2, Spells.ARCANE_INTELLECT_2, Spells.ARCANE_EXPLOSION_1 } },
            { 16, new List<uint> { Spells.FLAMESTRIKE_1, Spells.ARCANE_MISSILES_2 } },
            { 18, new List<uint> { Spells.FIREBALL_4, Spells.REMOVE_CURSE_MAGE_1, Spells.AMPLIFY_MAGIC_1 } },
            { 20, new List<uint> { Spells.BLIZZARD_1, Spells.FROST_ARMOR_3, Spells.FROSTBOLT_4, Spells.FIRE_WARD_1, Spells.TELEPORT_IRONFORGE_1, Spells.TELEPORT_ORGRIMMAR_1, Spells.TELEPORT_STORMWIND_1, Spells.TELEPORT_UNDERCITY_1, Spells.MANA_SHIELD_1, Spells.CONJURE_WATER_3, Spells.POLYMORPH_2, Spells.BLINK_1 } },
            { 22, new List<uint> { Spells.FROST_WARD_1, Spells.FIRE_BLAST_3, Spells.SCORCH_1, Spells.ARCANE_EXPLOSION_2, Spells.CONJURE_FOOD_3 } },
            { 24, new List<uint> { Spells.FLAMESTRIKE_2, Spells.FIREBALL_5, Spells.ARCANE_MISSILES_3, Spells.COUNTERSPELL_1, Spells.DAMPEN_MAGIC_2 } },
            { 26, new List<uint> { Spells.FROST_NOVA_2, Spells.FROSTBOLT_5, Spells.CONE_OF_COLD_1 } },
            { 28, new List<uint> { Spells.BLIZZARD_2, Spells.SCORCH_2, Spells.ARCANE_INTELLECT_3, Spells.MANA_SHIELD_2, Spells.CONJURE_MANA_GEM_1 } },
            { 30, new List<uint> { Spells.ICE_ARMOR_1, Spells.FIREBALL_6, Spells.FIRE_BLAST_4, Spells.FIRE_WARD_2, Spells.TELEPORT_DARNASSUS_1, Spells.TELEPORT_THUNDER_BLUFF_1, Spells.ARCANE_EXPLOSION_3, Spells.AMPLIFY_MAGIC_2, Spells.CONJURE_WATER_4 } },
            { 32, new List<uint> { Spells.FROSTBOLT_6, Spells.FROST_WARD_2, Spells.FLAMESTRIKE_3, Spells.ARCANE_MISSILES_4, Spells.CONJURE_FOOD_4 } },
            { 34, new List<uint> { Spells.CONE_OF_COLD_2, Spells.SCORCH_3, Spells.MAGE_ARMOR_1 } },
            { 36, new List<uint> { Spells.BLIZZARD_3, Spells.FIREBALL_7, Spells.DAMPEN_MAGIC_3, Spells.MANA_SHIELD_3 } },
            { 38, new List<uint> { Spells.FROSTBOLT_7, Spells.FIRE_BLAST_5, Spells.ARCANE_EXPLOSION_4, Spells.CONJURE_MANA_GEM_2 } },
            { 40, new List<uint> { Spells.FROST_NOVA_3, Spells.ICE_ARMOR_2, Spells.FLAMESTRIKE_4, Spells.SCORCH_4, Spells.FIRE_WARD_3, Spells.ARCANE_MISSILES_5, Spells.PORTAL_IRONFORGE_1, Spells.PORTAL_ORGRIMMAR_1, Spells.PORTAL_STORMWIND_1, Spells.PORTAL_UNDERCITY_1, Spells.CONJURE_WATER_5, Spells.POLYMORPH_3 } },
            { 42, new List<uint> { Spells.CONE_OF_COLD_3, Spells.FROST_WARD_3, Spells.FIREBALL_8, Spells.ARCANE_INTELLECT_4, Spells.AMPLIFY_MAGIC_3, Spells.CONJURE_FOOD_5 } },
            { 44, new List<uint> { Spells.FROSTBOLT_8, Spells.BLIZZARD_4, Spells.MANA_SHIELD_4 } },
            { 46, new List<uint> { Spells.FIRE_BLAST_6, Spells.SCORCH_5, Spells.ARCANE_EXPLOSION_5, Spells.MAGE_ARMOR_2 } },
            { 48, new List<uint> { Spells.FIREBALL_9, Spells.FLAMESTRIKE_5, Spells.DAMPEN_MAGIC_4, Spells.ARCANE_MISSILES_6, Spells.CONJURE_MANA_GEM_3 } },
            { 50, new List<uint> { Spells.CONE_OF_COLD_4, Spells.FROSTBOLT_9, Spells.ICE_ARMOR_3, Spells.FIRE_WARD_4, Spells.PORTAL_DARNASSUS_1, Spells.PORTAL_THUNDER_BLUFF_1, Spells.CONJURE_WATER_6 } },
            { 52, new List<uint> { Spells.FROST_WARD_4, Spells.BLIZZARD_5, Spells.SCORCH_6, Spells.MANA_SHIELD_5, Spells.CONJURE_FOOD_6 } },
            { 54, new List<uint> { Spells.FROST_NOVA_4, Spells.FIREBALL_10, Spells.FIRE_BLAST_7, Spells.AMPLIFY_MAGIC_4, Spells.ARCANE_EXPLOSION_6 } },
            { 56, new List<uint> { Spells.FROSTBOLT_10, Spells.FLAMESTRIKE_6, Spells.ARCANE_INTELLECT_5, Spells.ARCANE_MISSILES_7, Spells.ARCANE_BRILLIANCE_1, Spells.ARCANE_MISSILES_8 } },
            { 58, new List<uint> { Spells.CONE_OF_COLD_5, Spells.SCORCH_7, Spells.CONJURE_MANA_GEM_4, Spells.MAGE_ARMOR_3 } },
            { 60, new List<uint> { Spells.BLIZZARD_6, Spells.ICE_ARMOR_4, Spells.FROSTBOLT_11, Spells.FROST_WARD_5, Spells.FIREBALL_11, Spells.FIRE_WARD_5, Spells.FIREBALL_12, Spells.DAMPEN_MAGIC_5, Spells.MANA_SHIELD_6, Spells.POLYMORPH_4, Spells.CONJURE_FOOD_7, Spells.CONJURE_WATER_7 } }
        };

        public static class Spells
        {
            public const uint AMPLIFY_MAGIC_1 = 1008;
            public const uint AMPLIFY_MAGIC_2 = 8455;
            public const uint AMPLIFY_MAGIC_3 = 10169;
            public const uint AMPLIFY_MAGIC_4 = 10170;
            public const uint ARCANE_BRILLIANCE_1 = 23028;
            public const uint ARCANE_EXPLOSION_1 = 1449;
            public const uint ARCANE_EXPLOSION_2 = 8437;
            public const uint ARCANE_EXPLOSION_3 = 8438;
            public const uint ARCANE_EXPLOSION_4 = 8439;
            public const uint ARCANE_EXPLOSION_5 = 10201;
            public const uint ARCANE_EXPLOSION_6 = 10202;
            public const uint ARCANE_INTELLECT_1 = 1459;
            public const uint ARCANE_INTELLECT_2 = 1460;
            public const uint ARCANE_INTELLECT_3 = 1461;
            public const uint ARCANE_INTELLECT_4 = 10156;
            public const uint ARCANE_INTELLECT_5 = 10157;
            public const uint ARCANE_MISSILES_1 = 5143;
            public const uint ARCANE_MISSILES_2 = 5144;
            public const uint ARCANE_MISSILES_3 = 5145;
            public const uint ARCANE_MISSILES_4 = 8416;
            public const uint ARCANE_MISSILES_5 = 8417;
            public const uint ARCANE_MISSILES_6 = 10211;
            public const uint ARCANE_MISSILES_7 = 10212;
            public const uint ARCANE_MISSILES_8 = 25345;
            public const uint ARCANE_POWER_1 = 12042;
            public const uint ARCANE_SUBTLETY_1 = 11210;
            public const uint ARCANE_SUBTLETY_2 = 12592;
            public const uint BLAST_WAVE_1 = 11113;
            public const uint BLAST_WAVE_2 = 13018;
            public const uint BLAST_WAVE_3 = 13019;
            public const uint BLAST_WAVE_4 = 13020;
            public const uint BLAST_WAVE_5 = 13021;
            public const uint BLINK_1 = 1953;
            public const uint BLIZZARD_1 = 10;
            public const uint BLIZZARD_2 = 6141;
            public const uint BLIZZARD_3 = 8427;
            public const uint BLIZZARD_4 = 10185;
            public const uint BLIZZARD_5 = 10186;
            public const uint BLIZZARD_6 = 10187;
            public const uint BURNING_SOUL_1 = 11083;
            public const uint BURNING_SOUL_2 = 12351;
            public const uint CHILLED_1 = 12484;
            public const uint CHILLED_2 = 12485;
            public const uint CHILLED_3 = 12486;
            public const uint COLD_SNAP_1 = 12472;
            public const uint COMBUSTION_1 = 11129;
            public const uint CONE_OF_COLD_1 = 120;
            public const uint CONE_OF_COLD_2 = 8492;
            public const uint CONE_OF_COLD_3 = 10159;
            public const uint CONE_OF_COLD_4 = 10160;
            public const uint CONE_OF_COLD_5 = 10161;
            public const uint COUNTERSPELL_1 = 2139;
            public const uint CRITICAL_MASS_1 = 11115;
            public const uint CRITICAL_MASS_2 = 11367;
            public const uint CRITICAL_MASS_3 = 11368;
            public const uint DAMPEN_MAGIC_1 = 604;
            public const uint DAMPEN_MAGIC_2 = 8450;
            public const uint DAMPEN_MAGIC_3 = 8451;
            public const uint DAMPEN_MAGIC_4 = 10173;
            public const uint DAMPEN_MAGIC_5 = 10174;
            public const uint ELEMENTAL_PRECISION_1 = 29438;
            public const uint ELEMENTAL_PRECISION_2 = 29439;
            public const uint ELEMENTAL_PRECISION_3 = 29440;
            public const uint EVOCATION_1 = 12051;
            public const uint FIRE_BLAST_1 = 2136;
            public const uint FIRE_BLAST_2 = 2137;
            public const uint FIRE_BLAST_3 = 2138;
            public const uint FIRE_BLAST_4 = 8412;
            public const uint FIRE_BLAST_5 = 8413;
            public const uint FIRE_BLAST_6 = 10197;
            public const uint FIRE_BLAST_7 = 10199;
            public const uint FIRE_POWER_1 = 11124;
            public const uint FIRE_POWER_2 = 12378;
            public const uint FIRE_POWER_3 = 12398;
            public const uint FIRE_POWER_4 = 12399;
            public const uint FIRE_POWER_5 = 12400;
            public const uint FIRE_WARD_1 = 543;
            public const uint FIRE_WARD_2 = 8457;
            public const uint FIRE_WARD_3 = 8458;
            public const uint FIRE_WARD_4 = 10223;
            public const uint FIRE_WARD_5 = 10225;
            public const uint FIREBALL_1 = 133;
            public const uint FIREBALL_2 = 143;
            public const uint FIREBALL_3 = 145;
            public const uint FIREBALL_4 = 3140;
            public const uint FIREBALL_5 = 8400;
            public const uint FIREBALL_6 = 8401;
            public const uint FIREBALL_7 = 8402;
            public const uint FIREBALL_8 = 10148;
            public const uint FIREBALL_9 = 10149;
            public const uint FIREBALL_10 = 10150;
            public const uint FIREBALL_11 = 10151;
            public const uint FIREBALL_12 = 25306;
            public const uint FLAME_THROWING_1 = 11100;
            public const uint FLAME_THROWING_2 = 12353;
            public const uint FLAMESTRIKE_1 = 2120;
            public const uint FLAMESTRIKE_2 = 2121;
            public const uint FLAMESTRIKE_3 = 8422;
            public const uint FLAMESTRIKE_4 = 8423;
            public const uint FLAMESTRIKE_5 = 10215;
            public const uint FLAMESTRIKE_6 = 10216;
            public const uint FROST_ARMOR_1 = 168;
            public const uint FROST_ARMOR_2 = 7300;
            public const uint FROST_ARMOR_3 = 7301;
            public const uint FROST_CHANNELING_1 = 11160;
            public const uint FROST_CHANNELING_2 = 12518;
            public const uint FROST_CHANNELING_3 = 12519;
            public const uint FROST_NOVA_1 = 122;
            public const uint FROST_NOVA_2 = 865;
            public const uint FROST_NOVA_3 = 6131;
            public const uint FROST_NOVA_4 = 10230;
            public const uint FROST_WARD_1 = 6143;
            public const uint FROST_WARD_2 = 8461;
            public const uint FROST_WARD_3 = 8462;
            public const uint FROST_WARD_4 = 10177;
            public const uint FROST_WARD_5 = 28609;
            public const uint FROST_WARDING_1 = 11189;
            public const uint FROST_WARDING_2 = 28332;
            public const uint FROSTBITE_1 = 11071;
            public const uint FROSTBITE_2 = 12496;
            public const uint FROSTBITE_3 = 12497;
            public const uint FROSTBOLT_1 = 116;
            public const uint FROSTBOLT_2 = 205;
            public const uint FROSTBOLT_3 = 837;
            public const uint FROSTBOLT_4 = 7322;
            public const uint FROSTBOLT_5 = 8406;
            public const uint FROSTBOLT_6 = 8407;
            public const uint FROSTBOLT_7 = 8408;
            public const uint FROSTBOLT_8 = 10179;
            public const uint FROSTBOLT_9 = 10180;
            public const uint FROSTBOLT_10 = 10181;
            public const uint FROSTBOLT_11 = 25304;
            public const uint ICE_ARMOR_1 = 7302;
            public const uint ICE_ARMOR_2 = 7320;
            public const uint ICE_ARMOR_3 = 10219;
            public const uint ICE_ARMOR_4 = 10220;
            public const uint ICE_BARRIER_1 = 11426;
            public const uint ICE_BARRIER_2 = 13031;
            public const uint ICE_BARRIER_3 = 13032;
            public const uint ICE_BARRIER_4 = 13033;
            public const uint ICE_BLOCK_1 = 27619;
            public const uint ICE_SHARDS_1 = 11207;
            public const uint ICE_SHARDS_2 = 12672;
            public const uint IMPROVED_BLIZZARD_1 = 11185;
            public const uint IMPROVED_BLIZZARD_2 = 12487;
            public const uint IMPROVED_BLIZZARD_3 = 12488;
            public const uint IMPROVED_CONE_OF_COLD_1 = 11190;
            public const uint IMPROVED_CONE_OF_COLD_2 = 12489;
            public const uint IMPROVED_CONE_OF_COLD_3 = 12490;
            public const uint IMPROVED_FIRE_BLAST_1 = 11078;
            public const uint IMPROVED_FIRE_BLAST_2 = 11080;
            public const uint IMPROVED_FIREBALL_1 = 11069;
            public const uint IMPROVED_FIREBALL_2 = 12338;
            public const uint IMPROVED_FIREBALL_3 = 12339;
            public const uint IMPROVED_FIREBALL_4 = 12340;
            public const uint IMPROVED_FIREBALL_5 = 12341;
            public const uint IMPROVED_FLAMESTRIKE_1 = 11108;
            public const uint IMPROVED_FLAMESTRIKE_2 = 12349;
            public const uint IMPROVED_FLAMESTRIKE_3 = 12350;
            public const uint IMPROVED_FROSTBOLT_1 = 11070;
            public const uint IMPROVED_FROSTBOLT_2 = 12473;
            public const uint INVISIBILITY_1 = 66;
            public const uint MAGE_ARMOR_1 = 6117;
            public const uint MAGE_ARMOR_2 = 22782;
            public const uint MAGE_ARMOR_3 = 22783;
            public const uint MAGIC_ABSORPTION_1 = 29441;
            public const uint MAGIC_ABSORPTION_2 = 29444;
            public const uint MAGIC_ABSORPTION_5 = 29447;
            public const uint MANA_SHIELD_1 = 1463;
            public const uint MANA_SHIELD_2 = 8494;
            public const uint MANA_SHIELD_3 = 8495;
            public const uint MANA_SHIELD_4 = 10191;
            public const uint MANA_SHIELD_5 = 10192;
            public const uint MANA_SHIELD_6 = 10193;
            public const uint MASTER_OF_ELEMENTS_1 = 29074;
            public const uint MASTER_OF_ELEMENTS_2 = 29075;
            public const uint MASTER_OF_ELEMENTS_3 = 29076;
            public const uint PERMAFROST_1 = 11175;
            public const uint PERMAFROST_2 = 12569;
            public const uint PERMAFROST_3 = 12571;
            public const uint PIERCING_ICE_1 = 11151;
            public const uint PIERCING_ICE_2 = 12952;
            public const uint PIERCING_ICE_3 = 12953;
            public const uint POLYMORPH_1 = 118;
            public const uint POLYMORPH_2 = 12824;
            public const uint POLYMORPH_3 = 12825;
            public const uint POLYMORPH_4 = 12826;
            public const uint POLYMORPH_PIG_1 = 28272;
            public const uint POLYMORPH_TURTLE_1 = 28271;
            public const uint PORTAL_DARNASSUS_1 = 11419;
            public const uint PORTAL_IRONFORGE_1 = 11416;
            public const uint PORTAL_ORGRIMMAR_1 = 11417;
            public const uint PORTAL_STORMWIND_1 = 10059;
            public const uint PORTAL_THUNDER_BLUFF_1 = 11420;
            public const uint PORTAL_UNDERCITY_1 = 11418;
            public const uint PRESENCE_OF_MIND_1 = 12043;
            public const uint PYROBLAST_1 = 11366;
            public const uint PYROBLAST_2 = 12505;
            public const uint PYROBLAST_3 = 12522;
            public const uint PYROBLAST_4 = 12523;
            public const uint PYROBLAST_5 = 12524;
            public const uint PYROBLAST_6 = 12525;
            public const uint PYROBLAST_7 = 12526;
            public const uint PYROBLAST_8 = 18809;
            public const uint REMOVE_CURSE_MAGE_1 = 475;
            public const uint RIDING_1 = 33391;
            public const uint SCORCH_1 = 2948;
            public const uint SCORCH_2 = 8444;
            public const uint SCORCH_3 = 8445;
            public const uint SCORCH_4 = 8446;
            public const uint SCORCH_5 = 10205;
            public const uint SCORCH_6 = 10206;
            public const uint SCORCH_7 = 10207;
            public const uint SHATTER_1 = 11170;
            public const uint SHATTER_2 = 12982;
            public const uint SHATTER_3 = 12983;
            public const uint SHOOT_1 = 5019;
            public const uint SLOW_FALL_1 = 130;
            public const uint SLOW_1 = 246;
            public const uint TELEPORT_DARNASSUS_1 = 3565;
            public const uint TELEPORT_IRONFORGE_1 = 3562;
            public const uint TELEPORT_ORGRIMMAR_1 = 3567;
            public const uint TELEPORT_STORMWIND_1 = 3561;
            public const uint TELEPORT_THUNDER_BLUFF_1 = 3566;
            public const uint TELEPORT_UNDERCITY_1 = 3563;
            public const uint WINTERS_CHILL_1 = 11180;
            public const uint WINTERS_CHILL_2 = 28592;
            public const uint WINTERS_CHILL_3 = 28593;
            public const uint CONJURE_FOOD_1 = 587;
            public const uint CONJURE_FOOD_2 = 597;
            public const uint CONJURE_FOOD_3 = 990;
            public const uint CONJURE_FOOD_4 = 6129;
            public const uint CONJURE_FOOD_5 = 10144;
            public const uint CONJURE_FOOD_6 = 10145;
            public const uint CONJURE_FOOD_7 = 28612;
            public const uint CONJURE_MANA_GEM_1 = 759;
            public const uint CONJURE_MANA_GEM_3 = 10053;
            public const uint CONJURE_MANA_GEM_2 = 3552;
            public const uint CONJURE_MANA_GEM_4 = 10054;
            public const uint CONJURE_WATER_1 = 5504;
            public const uint CONJURE_WATER_2 = 5505;
            public const uint CONJURE_WATER_3 = 5506;
            public const uint CONJURE_WATER_4 = 6127;
            public const uint CONJURE_WATER_5 = 10138;
            public const uint CONJURE_WATER_6 = 10139;
            public const uint CONJURE_WATER_7 = 10140;
        }

        #endregion
    }
}
