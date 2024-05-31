using Hakken.Device.Hardware;
using Hakken.Device.Model;
using System.Collections;
using System.Net;

namespace Hakken.Discovery.Model
{
    /// <summary>
    /// DeviceDiscoveryResponse object.
    /// Returned by any of the DeviceDiscoverer methods.
    /// </summary>
    public class DiscoveryResponse : IEnumerable<KeyValuePair<string, IDeviceInfo>>
    {
        /// <summary>
        /// The device lookup.
        /// </summary>
        private readonly Dictionary<string, IDeviceInfo> _devices = new();

        /// <summary>
        /// Discovered device lookup.
        /// </summary>
        public IDictionary<string, IDeviceInfo> Devices => _devices;

        /// <summary>
        /// Returns true if any devices were discovered.
        /// </summary>
        public bool Success => _devices.Any(device => device.Value?.DeviceID is not null);

        /// <summary>
        /// A message describing the result of the discovery.
        /// </summary>
        public string? Message { get; set; } = string.Empty;

        /// <summary>
        /// The amount of time it took to discover devices.
        /// </summary>
        public TimeSpan DiscoveryElapsedTime { get; set; }

        /// <summary>
        /// Get a device by its device ID.
        /// </summary>
        /// <param name="tunerNumber"></param>
        /// <returns></returns>
        public IDeviceInfo? this[string deviceID]
            => _devices[deviceID];

        /// <summary>
        /// Gets a device by its IP address.
        /// </summary>
        /// <param name="deviceIP"></param>
        /// <returns></returns>
        public IDeviceInfo? this[IPAddress deviceIP]
            => _devices.Values.FirstOrDefault(device => device.DeviceIP == deviceIP);

        /// <summary>
        /// Gets any devices that match the specified device model.
        /// </summary>
        /// <param name="deviceModel"></param>
        /// <returns></returns>
        public IEnumerable<IDeviceInfo> this[DeviceModel deviceModel]
            => _devices.Values.Where(device => device.System?.HardwareInfo.DeviceModel == deviceModel);

        /// <summary>
        /// Gets any devices that match the specified hardware model.
        /// </summary>
        /// <param name="hardwareModel"></param>
        /// <returns></returns>
        public IEnumerable<IDeviceInfo> this[HardwareModel hardwareModel]
            => _devices.Values.Where(device => device.System?.HardwareInfo.HardwareModel == hardwareModel);

        /// <summary>
        /// Gets a device that is capturing the specified channel by program number.
        /// </summary>
        /// <param name="programNumber"></param>
        /// <returns></returns>
        public IDeviceInfo? this[double programNumber]
            => _devices.Values.FirstOrDefault(device => device.Tuners.Any(tuner => tuner.ProgramNumber == programNumber));

        /// <summary>
        /// Gets the value associated with the specified device ID.
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public bool TryGetValue(string deviceID, out IDeviceInfo? deviceInfo)
            => _devices.TryGetValue(deviceID, out deviceInfo);

        /// <summary>
        /// True of the device ID is in the device lookup.
        /// </summary>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        public bool ContainsKey(string deviceID)
            => _devices.ContainsKey(deviceID);

        /// <summary>
        /// Returns an enumerator that iterates through the device lookup.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, IDeviceInfo>> GetEnumerator() => _devices.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the device lookup.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => _devices.GetEnumerator();
    }
}
