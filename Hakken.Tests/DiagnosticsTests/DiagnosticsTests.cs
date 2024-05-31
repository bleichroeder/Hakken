using Hakken.Channel.Model;
using Hakken.Device.Model;
using Hakken.Diagnostics.Extensions;
using Hakken.Diagnostics.Model;
using Hakken.Discovery;
using Hakken.Discovery.Model;
using Hakken.Tuner.Model;
using System.Net;
using System.Runtime.Versioning;

namespace Hakken.Tests.RemoteDeviceTests
{
    public class DiagnosticsTests
    {
        /// <summary>
        /// Discover the device at the specified IP address and capture the first available channel on the first available tuner.
        /// </summary>
        [SupportedOSPlatform("Windows")]
        [Fact]
        public async Task Test_ASTATS()
        {
            IPAddress deviceIP = IPAddress.Parse("REMOTEIP");
            DiscoveryResponse discoveryResponse = await DiscoveryHelper.FromIPAddress(deviceIP, DeviceLocation.Remote);
            IDeviceInfo? remoteDevice = discoveryResponse.Devices.Values.FirstOrDefault() as RemoteDeviceInfoBase;
            ChannelInfo? testChannel = remoteDevice?.ChannelLineup.FirstOrDefault();

            Assert.NotNull(testChannel);

            Assert.True(remoteDevice?.Tuners.GetFirstAvailableTuner() is RemoteTunerInfo availableTuner && availableTuner is not null);

            AudioStats? audioStats = await testChannel.GetAudioStats();

            Assert.True(audioStats?.SuccessfullyParsed);
        }

        /// <summary>
        /// Discover the device at the specified IP address and get media properties from a channel on an available tuner.
        /// </summary>
        [SupportedOSPlatform("Windows")]
        [Fact]
        public async Task Test_MediaProperties()
        {
            IPAddress deviceIP = IPAddress.Parse("REMOTEIP");
            DiscoveryResponse discoveryResponse = await DiscoveryHelper.FromIPAddress(deviceIP, DeviceLocation.Remote);
            IDeviceInfo? remoteDevice = discoveryResponse.Devices.Values.FirstOrDefault() as RemoteDeviceInfoBase;
            ChannelInfo? testChannel = remoteDevice?.ChannelLineup.FirstOrDefault();

            Assert.NotNull(testChannel);

            Assert.True(remoteDevice?.Tuners.GetFirstAvailableTuner() is RemoteTunerInfo availableTuner && availableTuner is not null);

            MediaProperties? mediaProperties = await testChannel.ExtractMediaProperties();

            Assert.True(mediaProperties?.SuccessfullyParsed);
        }

        /// <summary>
        /// Discover the device at the specified IP address and extract a thumbnail from a channel on an available tuner.
        /// </summary>
        [SupportedOSPlatform("Windows")]
        [Fact]
        public async Task Test_ExtractThumbnail()
        {
            IPAddress deviceIP = IPAddress.Parse("REMOTEIP");
            DiscoveryResponse discoveryResponse = await DiscoveryHelper.FromIPAddress(deviceIP, DeviceLocation.Remote);
            IDeviceInfo? remoteDevice = discoveryResponse.Devices.Values.FirstOrDefault() as RemoteDeviceInfoBase;
            ChannelInfo? testChannel = remoteDevice?.ChannelLineup.FirstOrDefault();

            Assert.NotNull(testChannel);

            Assert.True(remoteDevice?.Tuners.GetFirstAvailableTuner() is RemoteTunerInfo availableTuner && availableTuner is not null);

            Stream? thumbnail = await testChannel.ExtractImage();

            Assert.NotNull(thumbnail);

            string outputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string thumbnailPath = Path.Combine(outputDirectory, "test_thumbnail.jpg");

            using (FileStream fileStream = new(thumbnailPath, FileMode.Create, FileAccess.Write))
            {
                await thumbnail.CopyToAsync(fileStream);
            }

            Assert.True(File.Exists(thumbnailPath));
        }
    }
}