using Populus.Core.Constants;
using Populus.Core.Shared;
using System;
using System.Collections.Generic;
using System.IO;

namespace Populus.GroupBot.Talents
{
    /// <summary>
    /// Singleton class that manages all talents available for bots
    /// </summary>
    public class TalentManager
    {
        #region Singleton

        static readonly TalentManager instance = new TalentManager();

        static TalentManager() { }

        TalentManager()
        { }

        public static TalentManager Instance { get { return instance; } }

        #endregion

        #region Declarations

        internal const int MAX_TALENT_POINTS = 51;

        private readonly Dictionary<ClassType, List<TalentSpec>> mTalentSpecs = new Dictionary<ClassType, List<TalentSpec>>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets all talent specs for a particular class
        /// </summary>
        /// <param name="classSpec"></param>
        /// <returns></returns>
        public IEnumerable<TalentSpec> TalentSpecsByClass(ClassType classSpec)
        {
            if (!mTalentSpecs.ContainsKey(classSpec))
                return new List<TalentSpec>();
            return mTalentSpecs[classSpec];
        }

        /// <summary>
        /// Loads all talents
        /// </summary>
        internal void LoadTalents()
        {
            // Read all files from the talents folder
            var folderLocation = $"bot\\talents";
            foreach (string file in Directory.EnumerateFiles(folderLocation, "*.*"))
            {
                var lines = File.ReadAllLines(file);
                var classSpec = (ClassType)Convert.ToByte(lines[0]);
                var specName = lines[1];
                uint[] talents = new uint[MAX_TALENT_POINTS];
                for (int i = 2; (i < (MAX_TALENT_POINTS + 2)) && (i < lines.Length); i++)
                    talents[i - 2] = Convert.ToUInt32(lines[i]);

                var talentSpec = new TalentSpec(classSpec, specName, talents);
                if (!mTalentSpecs.ContainsKey(classSpec))
                    mTalentSpecs.Add(classSpec, new List<TalentSpec>());
                mTalentSpecs[classSpec].Add(talentSpec);
            }
        }

        #endregion
    }
}
