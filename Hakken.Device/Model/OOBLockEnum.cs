using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hakken.Device.Model
{
    /// <summary>
    /// Represents the OOBLock state.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OOBLock
    {
        None,
        Weak,
        Success,
        Unknown
    }
}
