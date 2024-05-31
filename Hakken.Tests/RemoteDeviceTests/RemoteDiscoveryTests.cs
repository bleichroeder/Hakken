using Hakken.Channel.Model;
using Hakken.Device.HDHomeRun;
using Hakken.Device.Model;
using Hakken.Discovery;
using Hakken.Discovery.Extensions;
using Hakken.Discovery.Model;
using Hakken.Local.Extensions;
using Hakken.Remote;
using Hakken.Remote.Extensions;
using Hakken.Remote.Utility;
using Hakken.Tuner.Model;
using System.Net;
using System.Runtime.Versioning;
using System.Security;

namespace Hakken.Tests.RemoteDeviceTests
{
    public class RemoteDiscoveryTests
    {
        private static readonly string _user = @"some\user";
        private static readonly string _password = "password";
        private static readonly string _host = "host.domain.com";
        private static readonly string _targetChannelGuid = "GUID";

        /// <summary>
        /// Attempts to discover devices on a remote network using the ARP method executed via a remote host.
        /// Asserts that the discovery was successful.
        /// </summary>
        [SupportedOSPlatform("Windows")]
        [Fact]
        public async Task TestRemoteDeviceDiscovery_ARP()
        {
            RemoteDiscoveryRequest request = new(_user, _password, _host);

            DiscoveryResponse discoveryResponse = await Discover.RemoteDevices(request);

            Assert.True(discoveryResponse.Success);
        }

        /// <summary>
        /// Attempts to discover devices on a remote network using the HDHomeRunConfig method executed via a remote host.
        /// Asserts that the discovery was successful.
        /// </summary>
        [SupportedOSPlatform("Windows")]
        [Fact]
        public async Task TestRemoteDeviceDiscovery_HDHomeRunConfig()
        {
            RemoteDiscoveryRequest request = new(_user, _password, _host, true);

            DiscoveryResponse discoveryResponse = await Discover.RemoteDevices(request);

            Assert.True(discoveryResponse.Success);
        }

        /// <summary>
        /// Attempts to discover devices on a remote network using the HDHRConfig method executed via a remote host.
        /// Then attempts to get the stream info for the specified tuner.
        /// Validates the stream info against the tuner info and the target station UID.
        /// </summary>
        [SupportedOSPlatform("Windows")]
        [Fact]
        public async Task TestRemoteDeviceDiscovery_HDHomeRunConfig_StreamInfo()
        {
            RemoteDiscoveryRequest request = new(_user, _password, _host, true);

            DiscoveryResponse discoveryResponse = await Discover.RemoteDevices(request);
            RemoteTunerInfo? tunerInfo = discoveryResponse["DEVICEID"]?.Tuners[0] as RemoteTunerInfo;

            Assert.NotNull(tunerInfo);

            SecureString securePassword = ManagementHelper.ConvertToSecureString(_password);
            StreamInfo? streamInfo = await tunerInfo.GetStreamInfoAsync(_user, securePassword, _host);

            Assert.NotNull(streamInfo);

            bool frequencyMatches = tunerInfo.Frequency == streamInfo.Frequency;

            ChannelDetail? matchingChannel = streamInfo.DetectedChannels.Find(channel => channel.Matches(_targetChannelGuid));

            Assert.NotNull(matchingChannel);

            Assert.True(frequencyMatches);
        }

        [SupportedOSPlatform("Windows")]
        [Fact]
        public async Task TestFindTunerByGUID_HDHomeRunConfig_StreamInfo()
        {
            RemoteDiscoveryRequest request = new(_user, _password, _host, true);

            DiscoveryResponse discoveryResponse = await Discover.RemoteDevices(request);
            IDeviceInfo? remoteDevice = discoveryResponse["DEVICEID"] as RemoteDeviceInfoBase;

            if (remoteDevice is null)
                Assert.True(false);

            ChannelDetail? matchingChannelDetail = null;
            ChannelInfo? matchingChannelInfo = null;
            foreach (RemoteTunerInfo tuner in remoteDevice.Tuners.Cast<RemoteTunerInfo>())
            {
                StreamInfo? streamInfo = await tuner.GetStreamInfoAsync(_user, ManagementHelper.ConvertToSecureString(_password), _host);

                Assert.NotNull(streamInfo);

                matchingChannelDetail = streamInfo.DetectedChannels.Find(channel => channel.Matches(_targetChannelGuid));

                if (matchingChannelDetail is not null)
                {
                    matchingChannelInfo = remoteDevice.ChannelLineup.GetChannelByGuideNumber(matchingChannelDetail.GuideNumber);
                    break;
                }
            }

            Assert.NotNull(matchingChannelInfo);
        }

        /// <summary>
        /// Discovers a device by its IPAddress.
        /// </summary>
        [SupportedOSPlatform("Windows")]
        [Fact]
        public async Task TestRemoteDeviceDiscovery_IPAddress()
        {
            string ipString = "REMOTEIP";
            IPAddress iPAddress = IPAddress.Parse(ipString);

            DiscoveryResponse discoveryResponse = await DiscoveryHelper.FromIPAddress(iPAddress, DeviceLocation.Remote);

            Assert.True(discoveryResponse.Success);
        }

        /// <summary>
        /// Discover a remote device and refresh tuner stats
        /// using a custom delegate function.
        /// </summary>
        [Fact]
        public async Task TestRemoteDeviceCustomTunerRefresh_Delegate()
        {
            string ipString = "REMOTEIP";
            IPAddress iPAddress = IPAddress.Parse(ipString);

            DiscoveryResponse discoveryResponse = await DiscoveryHelper.FromIPAddress(iPAddress, DeviceLocation.Remote);

            RemoteHDHomeRunDeviceInfo? hdhr = discoveryResponse.FirstOrDefault().Value as RemoteHDHomeRunDeviceInfo;

            Assert.NotNull(hdhr);

            CancellationToken token = CancellationToken.None;
            await hdhr.RefreshAllTunerDataAsync(async (deviceInfo, token) =>
            {
                await Task.Delay(100, token);
                Assert.True(true);
                return deviceInfo;
            }, token);
        }

        /// <summary>
        /// Discover a remote device and refresh device info
        /// using a custom delegate function.
        /// </summary>
        [Fact]
        public async Task TestRemoteDeviceCustomDeviceRefresh_Delegate()
        {
            string ipString = "REMOTEIP";
            IPAddress iPAddress = IPAddress.Parse(ipString);

            DiscoveryResponse discoveryResponse = await DiscoveryHelper.FromIPAddress(iPAddress, DeviceLocation.Remote);

            RemoteHDHomeRunDeviceInfo? hdhr = discoveryResponse.FirstOrDefault().Value as RemoteHDHomeRunDeviceInfo;

            Assert.NotNull(hdhr);

            CancellationToken token = CancellationToken.None;
            await hdhr.RefreshDeviceInfoAsync(async (deviceInfo, token) =>
            {
                await Task.Delay(100, token);
                Assert.True(true);
                return deviceInfo;
            }, token);
        }

        /// <summary>
        /// Generates a channel reference table for corellating channel numbers to frequencies.
        /// This test will take some time to complete.
        /// Local table generation is much faster.
        /// </summary>
        [SupportedOSPlatform("Windows")]
        [Fact]
        public async Task TestRemoteDeviceDiscovery_ReferenceTableGeneration()
        {
            string ipString = "REMOTEIP";
            IPAddress iPAddress = IPAddress.Parse(ipString);

            DiscoveryResponse discoveryResponse = await DiscoveryHelper.FromIPAddress(iPAddress, DeviceLocation.Remote);

            RemoteDeviceInfoBase? remoteDevice = discoveryResponse["DEVICEID"] as RemoteDeviceInfoBase;

            Assert.NotNull(remoteDevice);

            ChannelReferenceTable referenceTable = await remoteDevice.GenerateChannelReferenceTable(_host, _user, _password);

            Assert.True(referenceTable.IsEmpty is false);
        }
    }
}