using System.Text.Json;
using System.Text.Json.Serialization;
using DaisyControl_AI.Storage.Dtos.Date;

namespace DaisyControl_AI.Storage.Dtos.JsonConverters
{
    /// <summary>
    /// Converter for Json serialization.
    /// </summary>
    public class DateTimeUnixJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            long value = reader.GetInt64();
            return value.FromUnixTime(DateTimeUtils.Unit.Milliseconds);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.ToUnixTime());
        }
    }
}
