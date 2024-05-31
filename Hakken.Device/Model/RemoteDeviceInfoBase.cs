using Hakken.Channel.Model;
using Hakken.Tuner.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Net;

namespace Hakken.Device.Model
{
    /// <summary>
    /// Local device info.
    /// </summary>
    public abstract class RemoteDeviceInfoBase : IDeviceInfo
    {
        /// <summary>
        /// The device location.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public DeviceLocation Location => DeviceLocation.Remote;

        /// <summary>
        /// Gets the device access.
        /// </summary>
        public abstract string DeviceAccess { get; }

        /// <summary>
        /// Device ID.
        /// </summary>
        public abstract string? DeviceID { get; }

        /// <summary>
        /// Device IP Address
        /// </summary>
        public abstract IPAddress DeviceIP { get; }

        /// <summary>
        /// Gets the time this device was discovered.
        /// </summary>
        public abstract DateTime DiscoveredAt { get; }

        /// <summary>
        /// Gets the time this device was discovered in UTC.
        /// </summary>
        public abstract DateTime DiscoveredAtUTC { get; }

        /// <summary>
        /// The device's available tuners.
        /// </summary>
        public abstract DeviceTuners Tuners { get; set; }

        /// <summary>
        /// The channel lineup.
        /// </summary>
        public abstract Lineup ChannelLineup { get; set; }

        /// <summary>
        /// The system's available info.
        /// </summary>
        public abstract SystemInfo System { get; set; }

        /// <summary>
        /// The device's current status.
        /// </summary>
        public abstract DeviceStatus Status { get; set; }

        /// <summary>
        /// Optional path to the HDHomeRun_Config exe.
        /// </summary>
        public abstract string? HDHomeRunConfigPath { get; set; }
    }
}
