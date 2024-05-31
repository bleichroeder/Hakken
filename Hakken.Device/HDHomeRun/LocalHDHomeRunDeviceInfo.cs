using Hakken.Channel.Model;
using Hakken.Device.Model;
using Hakken.Tuner.Model;
using System.Net;

namespace Hakken.Device.HDHomeRun
{
    /// <summary>
    /// Remote device info.
    /// </summary>
    public class LocalHDHomeRunDeviceInfo : RemoteDeviceInfoBase
    {
        private readonly IPAddress _deviceIP;
        private readonly DateTime _discoveredAt;

        /// <summary>
        /// Gets the device access.
        /// </summary>
        public override string DeviceAccess => $"http://{DeviceIP}";

        /// <summary>
        /// Device ID.
        /// </summary>
        public override string? DeviceID => System.DeviceID;

        /// <summary>
        /// Device IP Address
        /// </summary>
        public override IPAddress DeviceIP => _deviceIP;

        /// <summary>
        /// Gets the time this device was discovered.
        /// </summary>
        public override DateTime DiscoveredAt => _discoveredAt;

        /// <summary>
        /// Gets the time this device was discovered in UTC.
        /// </summary>
        public override DateTime DiscoveredAtUTC => DiscoveredAt.ToUniversalTime();

        /// <summary>
        /// The device's available tuners.
        /// </summary>
        public override DeviceTuners Tuners { get; set; } = new();

        /// <summary>
        /// The channel lineup.
        /// </summary>
        public override Lineup ChannelLineup { get; set; } = new();

        /// <summary>
        /// The system's available info.
        /// </summary>
        public override SystemInfo System { get; set; } = new();

        /// <summary>
        /// The device's current status.
        /// </summary>
        public override DeviceStatus Status { get; set; } = new();

        /// <summary>
        /// Optional path to the HDHomeRun_Config exe.
        /// </summary>
        public override string? HDHomeRunConfigPath { get; set; }

        /// <summary>
        /// Creates a new DeviceInfo object.
        /// </summary>
        /// <param name="ipAddress"></param>
        public LocalHDHomeRunDeviceInfo(IPAddress ipAddress)
        {
            _deviceIP = ipAddress;
            _discoveredAt = DateTime.Now;
        }

        /// <summary>
        /// Converts a generic device info object to a LocalHDHomeRun device info object.
        /// </summary>
        /// <param name="genericDeviceInfo"></param>
        public LocalHDHomeRunDeviceInfo(IDeviceInfo genericDeviceInfo)
        {
            _deviceIP = genericDeviceInfo.DeviceIP;
            _discoveredAt = DateTime.Now;

            Tuners = genericDeviceInfo.Tuners;
            ChannelLineup = genericDeviceInfo.ChannelLineup;
            System = genericDeviceInfo.System;
            Status = genericDeviceInfo.Status;
            HDHomeRunConfigPath = genericDeviceInfo.HDHomeRunConfigPath;
        }
    }
}
