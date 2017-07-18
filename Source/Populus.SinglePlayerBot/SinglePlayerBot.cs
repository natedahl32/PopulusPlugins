using Populus.Core.Plugins;
using Populus.Core.World.Objects;

namespace Populus.SinglePlayerBot
{
    public class SinglePlayerBot : PluginBase
    {
        #region Declarations

        // static instance of our bot handlers collection
        private static WoWGuidCollection<SpBotHandler> mBotHandlerCollection = new WoWGuidCollection<SpBotHandler>();

        #endregion

        #region Properties

        public override string Name => "Single Player Bot";

        public override string Author => "Kazadoom";

        public override string Website => "";

        #endregion

        #region Public Methods

        public override void Unload()
        {
            // Save bot data
            foreach (var handler in mBotHandlerCollection.GetAll())
                handler.SaveBotData();
        }

        public override void OnTick(Bot bot, float deltaTime)
        {
            var handler = mBotHandlerCollection.Get(bot.Guid);
            if (handler == null)
            {
                handler = new SpBotHandler(bot);
                mBotHandlerCollection.AddOrUpdate(bot.Guid, handler);
            }

            // Update the handler
            handler.Update(deltaTime);

            base.OnTick(bot, deltaTime);
        }

        #endregion
    }
}
