using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hakken.Device.Model
{
    [JsonConverter(typeof(StringEnumConverter))]
    internal enum DeviceType
    {
        FLEX_DUO,
        FLEX_QUATRO,
        FLEX_4K,
        CONNECT_4K,
        PRIME,
        SERVIO,
        SCRIBE_DUO,
        SCRIBE_QUATRO,
        EXTEND,
        EXPAND,
        DUAL,
        UNKNOWN
    }
}
