using Hakken.Device.Model;
using Hakken.Discovery.Model;
using Hakken.Remote;
using Hakken.Remote.Control.Extensions;
using System.Runtime.Versioning;

namespace Hakken.Tests.RemoteDeviceTests
{
    public class RemoteControlTests
    {
        private static readonly string _user = @"some\user";
        private static readonly string _password = "password";
        private static readonly string _host = "some.host.com";

        /// <summary>
        /// Attempts to discover devices on a remote network using the ARP method executed via a remote host.
        /// Asserts that the discovery was successful.
        /// </summary>
        [SupportedOSPlatform("Windows")]
        [Fact]
        public async Task TestRemoteDeviceControl_Restart()
        {
            RemoteDiscoveryRequest request = new(_user, _password, _host, true);

            DiscoveryResponse discoveryResponse = await Discover.RemoteDevices(request);

            RemoteDeviceInfoBase? testDevice = discoveryResponse.Devices.Values.FirstOrDefault() as RemoteDeviceInfoBase;

            Assert.NotNull(testDevice);

            await testDevice.RestartAsync(request.User, request.Password, request.RemoteHost);

            Assert.True(discoveryResponse.Success);
        }
    }
}