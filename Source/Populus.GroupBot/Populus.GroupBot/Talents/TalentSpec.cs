using Populus.Core.Constants;
using Populus.Core.DBC;
using Populus.GroupBot.Combat;
using System.Collections.Generic;
using System.Linq;

namespace Populus.GroupBot.Talents
{
    public class TalentSpec
    {
        #region Declarations

        private List<uint> mTalents;

        #endregion

        #region Constructors

        internal TalentSpec(ClassType classSpec, string name, uint[] talents)
        {
            this.ForClass = classSpec;
            this.Name = name;
            this.mTalents = talents.ToList();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the class this talent spec is for
        /// </summary>
        public ClassType ForClass { get; private set; }

        /// <summary>
        /// Gets the name of this talent spec
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets all talents in this spec
        /// </summary>
        public IEnumerable<uint> Talents
        {
            get { return mTalents; }
        }

        /// <summary>
        /// Returns the talent spec these talents refer to based on talents data
        /// </summary>
        public MainSpec Spec
        {
            get
            {
                // Get talent entry for each talent selected and tally up the total number of points in each talent tab. The tab with the highest points wins.
                var talentTabCounts = new Dictionary<uint, int>();
                foreach (var t in Talents)
                {
                    var talentEntry = TalentTable.Instance.getBySpell(t);
                    if (talentEntry != null)
                    {
                        var talentTab = TalentTabTable.Instance.getById(talentEntry.TalentTabId);
                        if (talentTabCounts.ContainsKey(talentTab.TabPage))
                            talentTabCounts[talentTab.TabPage]++;
                        else
                            talentTabCounts.Add(talentTab.TabPage, 1);
                    }
                }

                // get the KVP that has the highest value
                var maxValue = talentTabCounts.Select(kvp => kvp.Value).DefaultIfEmpty(0).Max();
                if (maxValue == 0)
                    return MainSpec.NONE;
                var value = talentTabCounts.Where(kvp => kvp.Value == maxValue).FirstOrDefault();
                var tab = value.Key;

                // Get the spec this tab relates to from the class logic
                return CombatLogicHandler.GetSpecFromTalentTab(ForClass, tab);
            }
        }

        #endregion
    }
}
