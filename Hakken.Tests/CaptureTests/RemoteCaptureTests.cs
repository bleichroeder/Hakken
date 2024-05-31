using Hakken.Capture.Extensions;
using Hakken.Capture.FFmpeg;
using Hakken.Capture.FFmpeg.Model;
using Hakken.Capture.Model;
using Hakken.Capture.Model.Request;
using Hakken.Capture.Model.Result;
using Hakken.Channel.Model;
using Hakken.Device.Model;
using Hakken.Discovery;
using Hakken.Discovery.Model;
using Hakken.Tuner.Model;
using System.Net;
using System.Runtime.Versioning;

namespace Hakken.Tests.RemoteDeviceTests
{
    public class RemoteCaptureTests
    {
        /// <summary>
        /// Discover the device at the specified IP address and capture the first available channel on the first available tuner.
        /// </summary>
        [SupportedOSPlatform("Windows")]
        [Fact]
        public async Task TestRemoteCapature_HTTP()
        {
            IPAddress deviceIP = IPAddress.Parse("REMOTEIP");
            DiscoveryResponse discoveryResponse = await DiscoveryHelper.FromIPAddress(deviceIP, DeviceLocation.Remote);
            IDeviceInfo? remoteDevice = discoveryResponse.Devices.Values.FirstOrDefault() as RemoteDeviceInfoBase;
            ChannelInfo? testChannel = remoteDevice?.ChannelLineup.FirstOrDefault();
            RemoteTunerInfo? availableTuner = remoteDevice?.Tuners.GetFirstAvailableTuner() as RemoteTunerInfo;

            Assert.NotNull(testChannel);
            Assert.NotNull(availableTuner);

            string outputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string outputFileName = $"{testChannel.GuideName}.ts";

            HttpCaptureRequest captureRequest = new(testChannel, outputDirectory, outputFileName)
            {
                Duration = TimeSpan.FromSeconds(10),
                TranscodeProfile = TranscodeProfile.Mobile,
                InputStreamUri = testChannel.StreamingUri
            };

            HttpCaptureResult? captureResult = await availableTuner.Capture(testChannel, captureRequest);

            Assert.True(captureResult.Success);
        }

        /// <summary>
        /// Discover the device at the specified IP address and capture the first available channel on the first available tuner using FFmpeg.
        /// </summary>
        [SupportedOSPlatform("Windows")]
        [Fact]
        public async Task TestRemoteCapature_FFmpeg()
        {
            IPAddress deviceIP = IPAddress.Parse("REMOTEIP");
            DiscoveryResponse discoveryResponse = await DiscoveryHelper.FromIPAddress(deviceIP, DeviceLocation.Remote);
            IDeviceInfo? remoteDevice = discoveryResponse.Devices.Values.FirstOrDefault() as RemoteDeviceInfoBase;
            ChannelInfo? testChannel = remoteDevice?.ChannelLineup[1606];
            RemoteTunerInfo? availableTuner = remoteDevice?.Tuners.GetFirstAvailableTuner() as RemoteTunerInfo;

            Assert.NotNull(testChannel);
            Assert.NotNull(availableTuner);

            string outputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string outputFileName = $"{testChannel.GuideName}.mp4";

            FFmpegCaptureRequest captureRequest = new(testChannel, outputDirectory, outputFileName)
            {
                Duration = TimeSpan.FromSeconds(30),
                TranscodeProfile = TranscodeProfile.None, // No transcoding because we're specifying res and framerate in FFmpegFlagPairs.
                InputStreamUri = testChannel.StreamingUri,
                FFmpegFlagPairs = new()
                {
                    { "-s", "200x200" },
                    { "-r", "30" }
                }
            };

            Assert.NotNull(captureRequest.ParameterizedInputStreamUri);

            FFmpegCaptureResult? captureResult = await FFmpegCaptureUtility.CreateWithDefaults().Capture(captureRequest);

            Assert.True(captureResult.Success);
        }
    }
}