using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace Hakken.Tuner.Model
{
    /// <summary>
    /// Represents a tuner issue.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TunerIssue
    {
        LowStreamingRate,
        LowSignalStrength,
        LowSignalQuality,
        LowSymbolQuality,
    }
}
