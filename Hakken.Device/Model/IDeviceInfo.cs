using Hakken.Channel.Model;
using Hakken.Tuner.Model;
using System.Net;

namespace Hakken.Device.Model
{
    /// <summary>
    /// The device interface.
    /// </summary>
    public interface IDeviceInfo
    {
        /// <summary>
        /// Gets the device location. Local or Remote.
        /// </summary>
        public DeviceLocation Location { get; }

        /// <summary>
        /// Gets the device access address.
        /// </summary>
        public string DeviceAccess { get; }

        /// <summary>
        /// The device's IP.
        /// </summary>
        public IPAddress DeviceIP { get; }

        /// <summary>
        /// The device's ID.
        /// </summary>
        public string? DeviceID { get; }

        /// <summary>
        /// The time this device was discovered.
        /// </summary>
        public DateTime DiscoveredAt { get; }

        /// <summary>
        /// The time this device was discovered in UTC.
        /// </summary>
        public DateTime DiscoveredAtUTC { get; }

        /// <summary>
        /// The device's available channel lineup.
        /// </summary>
        public Lineup ChannelLineup { get; set; }

        /// <summary>
        /// The device's available tuners.
        /// </summary>
        public DeviceTuners Tuners { get; set; }

        /// <summary>
        /// The device's available system information.
        /// </summary>
        public SystemInfo System { get; set; }

        /// <summary>
        /// The device's current status.
        /// </summary>
        public DeviceStatus Status { get; set; }

        /// <summary>
        /// Optional path to the HDHomeRun_Config exe.
        /// </summary>
        public string? HDHomeRunConfigPath { get; set; }
    }
}
