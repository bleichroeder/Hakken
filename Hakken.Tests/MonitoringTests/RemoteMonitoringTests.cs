using Hakken.Device.Model;
using Hakken.Discovery;
using Hakken.Discovery.Model;
using Hakken.Monitor.Extensions;
using Hakken.Monitor.Model;
using Hakken.Tuner.Model;
using System.Net;
using System.Runtime.Versioning;
using Xunit.Abstractions;

namespace Hakken.Tests.RemoteDeviceTests
{
    public class RemoteMonitoringTests
    {
        private readonly ITestOutputHelper _output;

        public RemoteMonitoringTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private static readonly string _user = @"some\user";
        private static readonly string _password = "password";
        private static readonly string _host = "some.host.com";

        [SupportedOSPlatform("Windows")]
        [Fact]
        public async Task MonitorRemoteTuner()
        {
            RemoteDiscoveryRequest request = new(_user, _password, _host, true);

            IPAddress deviceIP = IPAddress.Parse("REMOTEIP");
            DiscoveryResponse discoveryResponse = await DiscoveryHelper.FromIPAddress(deviceIP, DeviceLocation.Remote);
            IDeviceInfo? remoteDevice = discoveryResponse.Devices.FirstOrDefault().Value as RemoteDeviceInfoBase;

            Assert.NotNull(remoteDevice);

            ITunerInfo? remoteTuner = remoteDevice.Tuners[0] as RemoteTunerInfo;

            Assert.NotNull(remoteTuner);

            int secondsToMonitor = 30;
            int secondsBetweenPolling = 1;
            int maximumSecondsToStore = 10;

            CancellationTokenSource cancellationTokenSource = new();
            CancellationToken token = cancellationTokenSource.Token;

            TunerMonitoringConfiguration configuration = new()
            {
                Duration = TimeSpan.FromSeconds(secondsToMonitor),
                Interval = TimeSpan.FromSeconds(secondsBetweenPolling),
                MaximumStoredDuration = TimeSpan.FromSeconds(maximumSecondsToStore)
            };

            IMonitoringResult<ITunerInfo> monitoringResult = await remoteTuner.Monitor(configuration, async (tuner, monitor, token) =>
            {
                token.ThrowIfCancellationRequested();
                await Task.Run(() => _output.WriteLine($"SignalStrength: {monitor.MostRecentData.StoredData.SignalStrength}%"), token);
            });

            Assert.True(monitoringResult.TotalStoredDataDuration.TotalSeconds >= maximumSecondsToStore - 1
                     || monitoringResult.TotalStoredDataDuration.TotalSeconds >= maximumSecondsToStore + 1);
        }
    }
}