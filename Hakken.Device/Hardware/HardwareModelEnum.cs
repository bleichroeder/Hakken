using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hakken.Device.Hardware
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum HardwareModel
    {
        FLEX,
        QUATRO,
        PRIME,
        CONNECT,
        SERVIO,
        EXPAND,
        SCRIBE,
        DUAL,
        WINTV,
        UNKNOWN
    }
}
