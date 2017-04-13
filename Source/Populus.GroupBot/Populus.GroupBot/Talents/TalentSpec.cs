using Populus.Core.Constants;
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

        internal TalentSpec(ClassName classSpec, string name, uint[] talents)
        {
            this.ClassSpec = classSpec;
            this.Name = name;
            this.mTalents = talents.ToList();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the class this talent spec is for
        /// </summary>
        public ClassName ClassSpec { get; private set; }

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

        #endregion
    }
}
