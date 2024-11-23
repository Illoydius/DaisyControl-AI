using DaisyControl_AI.Common.Diagnostics;
using Newtonsoft.Json;

namespace DaisyControl_AI.Common.Serialization
{
    public static class JsonCommonSerializer
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static T DeserializeFromString<T>(string aSerializedObjectToDeserialize) // where T : ISerializable
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(aSerializedObjectToDeserialize, Settings);
            } catch (Exception _Ex)
            {
                LoggingManager.LogToFile("7738DC29-7E5D-4559-B6E3-C07D8B2A5717", $"Couldn't deserialize object [{typeof(T)}]", _Ex);
                throw;
            }
        }

        public static string SerializeToString<T>(T aObjectToSerialize) // where T : ISerializable
        {
            try
            {
                return JsonConvert.SerializeObject(aObjectToSerialize, Settings);
            } catch (Exception _Ex)
            {
                LoggingManager.LogToFile("00967675-8EA2-48C8-9AEF-066156DD9D23", $"Couldn't serialize object [{typeof(T)}]", _Ex);
                throw;
            }
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public static JsonSerializerSettings Settings => new()
        {
            TypeNameHandling = TypeNameHandling.Objects
        };
    }
}
