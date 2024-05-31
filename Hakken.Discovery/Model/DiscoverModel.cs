using Newtonsoft.Json;

namespace Hakken.Discovery
{
    /// <summary>
    /// Device discover response object.
    /// Returned from the device from /discover.json.
    /// </summary>
    public class DiscoverModel
    {
        /// <summary>
        /// Device friendly name. (Model)
        /// </summary>
        public string FriendlyName { get; set; } = string.Empty;
        /// <summary>
        /// Device model number.
        /// </summary>
        public string ModelNumber { get; set; } = string.Empty;
        /// <summary>
        /// Device firmware version.
        /// </summary>
        public string FirmwareVersion { get; set; } = string.Empty;
        /// <summary>
        /// Device ID.
        /// </summary>
        public string DeviceID { get; set; } = string.Empty;
        /// <summary>
        /// Device tuner count.
        /// </summary>
        public int Tunercount { get; set; }

        /// <summary>
        /// True if a device firmware upgrade is available.
        /// </summary>
        [JsonIgnore]
        public bool UpgradeAvailable => !string.IsNullOrEmpty(UpgradeVersion);

        /// <summary>
        /// Device firmware upgrade version.
        /// </summary>
        [JsonProperty(nameof(UpgradeAvailable))]
        public string UpgradeVersion { get; set; } = string.Empty;

        /// <summary>
        /// Device lineup url. (Channel list)
        /// </summary>
        public string LineupURL { get; set; } = string.Empty;
    }
}
