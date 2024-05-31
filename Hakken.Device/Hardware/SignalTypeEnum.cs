using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hakken.Device.Hardware
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SignalType
    {
        USCableTV,
        QAM64_256,
        DVB_T_T2,
        ISDB_T,
        Eight_VSB_ATSC,
        DVB_C,
        ATSC3,
        ATSC1
    }
}
