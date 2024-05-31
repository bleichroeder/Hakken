using Hakken.Channel.Model;
using Hakken.Control.Extensions;
using Hakken.Device.Model;
using Hakken.Discovery;
using Hakken.Discovery.Model;
using System.Net;
using System.Runtime.Versioning;

namespace Hakken.Tests.RemoteDeviceTests
{
    public class GeneralControlTests
    {
        /// <summary>
        /// Attempts to discover devices on a remote network using the ARP method executed via a remote host.
        /// Asserts that the discovery was successful.
        /// </summary>
        [SupportedOSPlatform("Windows")]
        [Fact]
        public async Task TestRemoteDeviceControl_Scan()
        {
            IPAddress deviceIP = IPAddress.Parse("REMOTEIP");

            DiscoveryResponse discoveryResponse = await DiscoveryHelper.FromIPAddress(deviceIP, DeviceLocation.Remote);

            IDeviceInfo? testDevice = discoveryResponse.Devices.Values.FirstOrDefault() as RemoteDeviceInfoBase;

            Assert.NotNull(testDevice);

            await testDevice.StartChannelScanAsync();

            await Task.Delay(3000);

            ChannelScanStatusResponse? scanStatus = await testDevice.GetChannelScanStatusAsync();

            if (scanStatus?.Status is ChannelScanStatus.Running)
            {
                await testDevice.StopChannelScanAsync();
            }

            Assert.True(discoveryResponse.Success);
        }

        /// <summary>
        /// Attempts to discover devices on a remote network using the ARP method executed via a remote host.
        /// Asserts that the discovery was successful.
        /// </summary>
        [SupportedOSPlatform("Windows")]
        [Fact]
        public async Task TestRemoteDeviceControl_Scan_Wait()
        {
            IPAddress deviceIP = IPAddress.Parse("REMOTEIP");

            DiscoveryResponse discoveryResponse = await DiscoveryHelper.FromIPAddress(deviceIP, DeviceLocation.Remote);

            IDeviceInfo? testDevice = discoveryResponse.Devices.Values.FirstOrDefault() as RemoteDeviceInfoBase;

            Assert.NotNull(testDevice);

            bool scanCompleted = await (await testDevice.StartChannelScanAsync()).WaitForScanCompletionAsync();

            Assert.True(scanCompleted);
        }
    }
}