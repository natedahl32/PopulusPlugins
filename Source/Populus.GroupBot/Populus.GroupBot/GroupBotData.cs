using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;

namespace Populus.GroupBot
{
    /// <summary>
    /// Data that gets serialized to file and saved for a bot
    /// </summary>
    internal class GroupBotData
    {
        #region Declarations

        private ulong mGuid;
        private string mSpecName;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the spec the bot is currently using. If null or empty, no spec is currently set.
        /// </summary>
        public string SpecName
        {
            get { return mSpecName; }
            set
            {
                mSpecName = value;
                Serialize();
            }
        }

        /// <summary>
        /// Gets the serializer used to write bot data
        /// </summary>
        private JsonSerializer Serializer
        {
            get
            {
                var serializer = new JsonSerializer();
                serializer.Converters.Add(new JavaScriptDateTimeConverter());
                serializer.NullValueHandling = NullValueHandling.Ignore;
                return serializer;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves bot data out to a file
        /// </summary>
        internal void Serialize()
        {
            var filePath = $"bot\\{mGuid}.bot";
            var serializer = this.Serializer;
            using (StreamWriter sw = new StreamWriter(filePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
                serializer.Serialize(writer, this);
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Loads data for a bot from file
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        internal static GroupBotData LoadData(ulong guid)
        {
            var filePath = $"bot\\{guid}.bot";
            if (!Directory.Exists("bot")) Directory.CreateDirectory("bot");
            if (!File.Exists(filePath)) return new GroupBotData() { mGuid = guid };

            var data = File.ReadAllText(filePath);
            var botData = new GroupBotData();
            if (!string.IsNullOrEmpty(data) && !string.IsNullOrEmpty(data.Trim()))
                botData = JsonConvert.DeserializeObject<GroupBotData>(data);
            botData.mGuid = guid;
            return botData;
        }

        #endregion
    }
}
