using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hakken.Device.Model
{
    /// <summary>
    /// Represents the location of a device.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DeviceLocation
    {
        Local,
        Remote,
        Unknown
    }
}
