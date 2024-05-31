using Newtonsoft.Json;

namespace Hakken.Channel.Model
{
    public class ChannelScanStatusResponse
    {
        /// <summary>
        /// Scan state.
        /// </summary>
        [JsonProperty("ScanInProgress")]
        public ChannelScanStatus Status { get; set; }

        /// <summary>
        /// Scan progress in percent.
        /// </summary>
        public int Progress { get; set; }

        /// <summary>
        /// Number of channels found.
        /// </summary>
        public int Found { get; set; }
    }
}
