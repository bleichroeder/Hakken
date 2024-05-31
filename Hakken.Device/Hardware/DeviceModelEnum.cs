using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hakken.Device.Hardware
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DeviceModel
    {
        Hauppauge,
        HDHomeRun,
        Unknown
    }
}
