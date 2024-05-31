using Newtonsoft.Json;

namespace Hakken.Serialization
{
    /// <summary>
    /// Provides access to default serialization settings.
    /// </summary>
    public static class SerializationHelper
    {
        /// <summary>
        /// Gets a default serializer with <see cref="Formatting.Indented"/> and the <see cref="IPAddressConverter"/> registered.
        /// </summary>
        public static JsonSerializerSettings DefaultSerializer => new()
        {
            Converters = new List<JsonConverter> { new IPAddressConverter() },
            Formatting = Formatting.Indented
        };

        /// <summary>
        /// Gets a default serializer with the specified <see cref="Formatting"/>.
        /// </summary>
        /// <param name="formatting"></param>
        /// <returns></returns>
        public static JsonSerializerSettings GetSerializer(Formatting formatting) => new()
        {
            Converters = new List<JsonConverter> { new IPAddressConverter() },
            Formatting = formatting
        };
    }
}
