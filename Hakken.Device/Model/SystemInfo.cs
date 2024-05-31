using Hakken.Device.Hardware;
using System.Net;
using System.Net.NetworkInformation;

namespace Hakken.Device.Model
{

    /// <summary>
    /// The system's info.
    /// </summary>
    public class SystemInfo
    {
        /// <summary>
        /// Gets the device ID.
        /// </summary>
        public string? DeviceID { get; set; }

        /// <summary>
        /// Device name.
        /// </summary>
        public string? DeviceName { get; set; }

        /// <summary>
        /// Gets the system hardware model.
        /// </summary>
        public string? HardwareModel { get; set; }

        public IHardwareInfo HardwareInfo
            => HardwareInfoProvider.GetHardwareInfoByModelNumber(HardwareModel);

        /// <summary>
        /// Gets the system's subnetmask.
        /// </summary>
        public IPAddress? SubnetMask { get; set; }

        /// <summary>
        /// Gets the system memory report.
        /// </summary>
        public MemoryReport? MemoryReport { get; set; }

        /// <summary>
        /// Gets the system physical address.
        /// </summary>
        public PhysicalAddress? MACAddress { get; set; }

        /// <summary>
        /// Gets the firmware version.
        /// </summary>
        public string? FirmwareVersion { get; set; }

        /// <summary>
        /// True if an update is available for the device.
        /// </summary>
        public bool UpdateAvailable { get; set; }

        /// <summary>
        /// The version of the available firmware update.
        /// </summary>
        public string? AvailableRelease { get; set; }
    }
}
