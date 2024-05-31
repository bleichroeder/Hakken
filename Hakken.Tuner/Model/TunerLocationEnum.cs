using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hakken.Tuner.Model
{
    /// <summary>
    /// Represents the location of a tuner.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TunerLocation
    {
        Local,
        Remote
    }
}
