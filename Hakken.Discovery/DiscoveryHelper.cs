using Hakken.Device.Generic;
using Hakken.Device.Hardware;
using Hakken.Device.Hauppauge;
using Hakken.Device.HDHomeRun;
using Hakken.Device.Model;
using Hakken.Discovery.Extensions;
using Hakken.Discovery.Model;
using System.Net;

namespace Hakken.Discovery
{
    public static class DiscoveryHelper
    {
        /// <summary>
        /// Retrieves device information from a single IP address.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static async Task<DiscoveryResponse> FromIPAddress(IPAddress ipAddress, DeviceLocation location)
            => await FromIPAddresses(new List<IPAddress> { ipAddress }, location, CancellationToken.None);

        /// <summary>
        /// Retrieves device information from a single IP address.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static async Task<DiscoveryResponse> FromIPAddress(IPAddress ipAddress, DeviceLocation location, CancellationToken cancellationToken)
            => await FromIPAddresses(new List<IPAddress> { ipAddress }, location, cancellationToken);

        /// <summary>
        /// Retrieves device information from a list of IP addresses.
        /// </summary>
        /// <param name="iPAddresses"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static async Task<DiscoveryResponse> FromIPAddresses(List<IPAddress> iPAddresses, DeviceLocation location)
            => await FromIPAddresses(iPAddresses, location, CancellationToken.None);

        /// <summary>
        /// Retrieves device information from a list of IP addresses.
        /// </summary>
        /// <param name="iPAddresses"></param>
        /// <returns></returns>
        public static async Task<DiscoveryResponse> FromIPAddresses(List<IPAddress> iPAddresses, DeviceLocation location, CancellationToken cancellationToken)
        {
            DiscoveryResponse retVal = new();

            foreach (IPAddress ipAddress in iPAddresses.DistinctBy(ip => ip.ToString()))
            {
                try
                {
                    IDeviceInfo? device = location == DeviceLocation.Local ?
                        new LocalGenericDeviceInfo(ipAddress)
                      : new RemoteGenericDeviceInfo(ipAddress);

                    await device.RefreshDeviceInfoAsync(cancellationToken);

                    device = device.System.HardwareInfo.DeviceModel switch
                    {
                        DeviceModel.HDHomeRun => device.Location == DeviceLocation.Local ? new LocalHDHomeRunDeviceInfo(device)
                                                                                         : new RemoteHDHomeRunDeviceInfo(device),

                        DeviceModel.Hauppauge => device.Location == DeviceLocation.Local ? new LocalHauppaugeDeviceInfo(device)
                                                                                         : new RemoteHauppaugeDeviceInfo(device),
                        DeviceModel.Unknown => device,

                        _ => throw new NotImplementedException()
                    };

                    if (string.IsNullOrEmpty(device?.DeviceID) == false)
                    {
                        retVal.Devices.Add(device.DeviceID.ToUpperInvariant(), device);
                        await device.RefreshAllTunerDataAsync(cancellationToken);
                    }
                }
                catch (Exception ex) { retVal.Message = ex.Message; }
            }

            return retVal;
        }
    }
}
