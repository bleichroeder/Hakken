using Hakken.Discovery.Model;
using Hakken.Local;

namespace Hakken.Tests.LocalDeviceTests
{
    public class LocalDiscoveryTests
    {
        /// <summary>
        /// Attempts to discover devices on the local network using the UDP broadcast method.
        /// Asserts that the discovery was successful.
        /// </summary>
        [Fact]
        public async Task TestLocalDeviceDiscovery_UDP()
        {
            DiscoveryResponse discoveryResponse = await Discover.DevicesByUDPBroadcast();

            Assert.True(discoveryResponse.Success);
        }
    }
}