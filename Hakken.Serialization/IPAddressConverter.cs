using Newtonsoft.Json;
using System.Net;

namespace Hakken.Serialization
{
    /// <summary>
    /// Provides a converter for <see cref="IPAddress"/> objects to and from JSON.
    /// </summary>
    public class IPAddressConverter : JsonConverter
    {
        /// <summary>
        /// Gets the singleton instance of the <see cref="IPAddressConverter"/> class.
        /// </summary>
        public static IPAddressConverter Instance { get; } = new IPAddressConverter();

        /// <summary>
        /// Gets the <see cref="JsonSerializerSettings"/> that can be used to serialize and deserialize <see cref="IPAddress"/> objects.
        /// </summary>
        public static JsonSerializerSettings JsonSerializerSettings => new()
        {
            Converters = new JsonConverter[] { Instance },
            Formatting = Formatting.Indented
        };

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IPAddress);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var address = value as IPAddress;
            writer.WriteValue(address?.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            return reader.Value is not null ? IPAddress.Parse((string)reader.Value) : string.Empty;
        }

        public override bool CanRead => true;
    }
}