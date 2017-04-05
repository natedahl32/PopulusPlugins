using Populus.Core.Plugins;

namespace Populus.CombatManager
{
    public class CombatManager : PluginBase
    {
        #region Constructors

        public CombatManager()
        {
            // Set priority to highest state because this plugin gathers information
            Priority = 0;
        }

        #endregion

        #region Properties

        public override string Name => "Combat Manager";

        public override string Author => "Kazadoom";

        public override string Website => "";

        #endregion

        #region Public Methods

        public override void Initialize()
        {
            
        }

        #endregion
    }
}
