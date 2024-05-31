using Hakken.Channel.Model;
using Hakken.Device.HDHomeRun;
using Hakken.Device.Model;
using Hakken.Tuner.Model;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Hakken.Discovery.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IDeviceInfo"/> objects.
    /// </summary>
    public static partial class DeviceExtensions
    {
        /// <summary>
        /// HTTP client used for device info refresh.
        /// </summary>
        private static readonly HttpClient _httpClient = new();

        /// <summary>
        /// The html xpath for the logs on the device.
        /// </summary>
        private const string LOGS_XPATH = @"/html/body/div/pre";

        /// <summary>
        /// Attempts to load device logs via HTTP.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<string>> GetLogsAsync(this IDeviceInfo deviceInfo)
            => await deviceInfo.GetLogsAsync(CancellationToken.None);

        /// <summary>
        /// Attempts to load device logs via HTTP.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<string>> GetLogsAsync(this IDeviceInfo deviceInfo, CancellationToken cancellationToken)
        {
            IEnumerable<string> logs = new List<string>();

            try
            {
                string responseString = await _httpClient.GetStringAsync($"{deviceInfo.DeviceAccess}/log.html", cancellationToken);
                HtmlDocument logPage = new();
                logPage.LoadHtml(responseString);
                HtmlNode logsNode = logPage.DocumentNode.SelectSingleNode(LOGS_XPATH);
                logs = logsNode.InnerText.Split(Environment.NewLine);
            }
            catch { }

            return logs;
        }

        /// <summary>
        /// Finds any tuners with issues.
        /// </summary>
        public static IEnumerable<ITunerInfo> ReturnTunersWithIssues(this IDeviceInfo deviceInfo)
            => deviceInfo.Tuners.Where(t => t.IssueDetected);

        /// <summary>
        /// Finds any tuners with issues and returns a list of <see cref="ITunerInfo"/> and <see cref="TunerIssue"/> objects.
        /// </summary>
        public static IEnumerable<Tuple<ITunerInfo, IEnumerable<TunerIssue>>> ReturnTunersAndIssues(this IDeviceInfo deviceInfo)
            => deviceInfo.Tuners.Select(t => new Tuple<ITunerInfo, IEnumerable<TunerIssue>>(t, t.GetIssues()));

        /// <summary>
        /// Returns all channels that are not DRM protected.
        /// </summary>
        /// <returns>List of Channels</returns>
        public static IEnumerable<ChannelInfo>? ReturnNonDRMChannels(this IDeviceInfo deviceInfo)
            => deviceInfo.ChannelLineup.Channels?.Where(c => c.DRM is false);

        /// <summary>
        /// Location agnostic tuner data refresh.
        /// If the device is local and <see cref="IDeviceInfo.HDHomeRunConfigPath"/> has been set
        /// and exists in the specified location, hdhomerun_config.exe will be used for tuner data retrieval.
        /// Otherwise, HTTP will be used.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<IDeviceInfo> RefreshAllTunerDataAsync(this IDeviceInfo deviceInfo, CancellationToken cancellationToken)
            => await deviceInfo.RefreshAllTunerDataAsync(true, cancellationToken);

        /// <summary>
        /// Refreshes all tuners on the device.
        /// useHDHRConfig will be ignored if the device is a <see cref="RemoteHauppaugeDeviceInfo"/>.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="useHDHRConfig"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static async Task<IDeviceInfo> RefreshAllTunerDataAsync(this IDeviceInfo deviceInfo, bool useHDHRConfig, CancellationToken cancellationToken)
        {
            if (deviceInfo is LocalDeviceInfoBase localDeviceInfo)
            {
                bool useHDHomeRun = useHDHRConfig && !string.IsNullOrEmpty(localDeviceInfo.HDHomeRunConfigPath) && File.Exists(localDeviceInfo.HDHomeRunConfigPath);
                return await localDeviceInfo.RefreshAllTunerDataAsync(useHDHomeRun, cancellationToken);
            }
            else if (deviceInfo is RemoteDeviceInfoBase remoteDeviceInfo)
            {
                return await remoteDeviceInfo.RefreshAllTunerDataAsync(cancellationToken);
            }

            throw new NotSupportedException($"Device type {deviceInfo.GetType()} is not supported.");
        }

        /// <summary>
        /// Refreshes all tuners on the device.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public static async Task<IDeviceInfo> RefreshAllTunerDataAsync(this RemoteDeviceInfoBase deviceInfo)
            => await deviceInfo.RefreshAllTunerDataAsync(CancellationToken.None);

        /// <summary>
        /// Refreshes all tuners on the device.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<IDeviceInfo> RefreshAllTunerDataAsync(this RemoteDeviceInfoBase deviceInfo, CancellationToken cancellationToken)
        {
            await Task.WhenAll(deviceInfo.Tuners.Select(tuner => tuner.RefreshTunerInfoUsingHTTP(cancellationToken)));
            return deviceInfo;
        }

        /// <summary>
        /// Refreshes all tuners on the device.
        /// Local tuner refresh provides the option to use hdhomerun_config.exe for tuner data retrieval.
        /// If <see cref="IDeviceInfo.HDHomeRunConfigPath"/> has not been set in the parent device,
        /// the default path will be used.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="useHDHRConfig"></param>
        /// <returns></returns>
        public static async Task<IDeviceInfo> RefreshAllTunerDataAsync(this LocalDeviceInfoBase deviceInfo, bool useHDHRConfig)
            => await deviceInfo.RefreshAllTunerDataAsync(useHDHRConfig, CancellationToken.None);

        /// <summary>
        /// Refreshes all tuners on the device.
        /// Local tuner refresh provides the option to use hdhomerun_config.exe for tuner data retrieval.
        /// If <see cref="IDeviceInfo.HDHomeRunConfigPath"/> has not been set in the parent device,
        /// the default path will be used.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="useHDHRConfig"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<IDeviceInfo> RefreshAllTunerDataAsync(this LocalDeviceInfoBase deviceInfo, bool useHDHRConfig, CancellationToken cancellationToken)
        {
            var tasks = deviceInfo.Tuners.Select(async tuner =>
            {
                if (useHDHRConfig)
                {
                    await tuner.RefreshTunerInfoUsingHTTP(cancellationToken);
                }
                else
                {
                    LocalTunerInfo? localTunerInfo = tuner as LocalTunerInfo;
                    if (localTunerInfo is not null)
                    {
                        await localTunerInfo.RefreshTunerInfoUsingConfig(deviceInfo.HDHomeRunConfigPath);
                    }
                }
            });

            await Task.WhenAll(tasks);
            return deviceInfo;
        }

        /// <summary>
        /// Allows for a custom tuner refresh delegate to be used.
        /// The action is performed on each tuner.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task<IDeviceInfo> RefreshAllTunerDataAsync(this IDeviceInfo deviceInfo, Func<ITunerInfo, CancellationToken, Task<ITunerInfo>> action)
            => await deviceInfo.RefreshAllTunerDataAsync(action, CancellationToken.None);

        /// <summary>
        /// Allows for a custom tuner refresh delegate to be used.
        /// The action is performed on each tuner.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="tunerRefreshDelegate"></param>
        /// <returns></returns>
        public static async Task<IDeviceInfo> RefreshAllTunerDataAsync(this IDeviceInfo deviceInfo, Func<ITunerInfo, CancellationToken, Task<ITunerInfo>> action, CancellationToken cancellationToken)
        {
            await Task.WhenAll(deviceInfo.Tuners.Select(async tuner => await tuner.RefreshTunerDataAsync(action, cancellationToken)));
            return deviceInfo;
        }

        /// <summary>
        /// Allows for a custom device refresh delegate to be used.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="deviceRefreshDelegate"></param>
        /// <returns></returns>
        public static async Task<IDeviceInfo> RefreshDeviceInfoAsync(this RemoteDeviceInfoBase deviceInfo,
                                                                     Func<IDeviceInfo, CancellationToken, Task<IDeviceInfo>> deviceRefreshDelegate)
            => await deviceInfo.RefreshDeviceInfoAsync(deviceRefreshDelegate, CancellationToken.None);

        /// <summary>
        /// Allows for a custom device refresh delegate to be used.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="deviceRefreshDelegate"></param>
        /// <returns></returns>
        public static async Task<IDeviceInfo> RefreshDeviceInfoAsync(this RemoteDeviceInfoBase deviceInfo,
                                                                     Func<IDeviceInfo, CancellationToken, Task<IDeviceInfo>> deviceRefreshDelegate,
                                                                     CancellationToken cancellationToken)
            => await deviceRefreshDelegate(deviceInfo, cancellationToken);


        /// <summary>
        /// Refreshes all tuner signal stats using the status api.
        /// For whatever reason this method of refreshing tuner stats does not update StreamingRate.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public static async Task<LocalHDHomeRunDeviceInfo> RefreshTunerSignalStatsAsync(this LocalHDHomeRunDeviceInfo deviceInfo)
            => (LocalHDHomeRunDeviceInfo)await RefreshTunerSignalStatsAsync(deviceInfo, CancellationToken.None);

        /// <summary>
        /// Refreshes all tuner signal stats using the status api.
        /// For whatever reason this method of refreshing tuner stats does not update StreamingRate.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<LocalHDHomeRunDeviceInfo> RefreshTunerSignalStatsAsync(this LocalHDHomeRunDeviceInfo deviceInfo, CancellationToken cancellationToken)
            => await RefreshTunerSignalStatsAsync(deviceInfo, cancellationToken);

        /// <summary>
        /// Refreshes all tuner signal stats using the status api.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public static async Task<RemoteHDHomeRunDeviceInfo> RefreshTunerSignalStatsAsync(this RemoteHDHomeRunDeviceInfo deviceInfo)
            => (RemoteHDHomeRunDeviceInfo)await RefreshTunerSignalStatsAsync(deviceInfo, CancellationToken.None);

        /// <summary>
        /// Refreshes all tuner signal stats using the status api.
        /// For whatever reason this method of refreshing tuner stats does not update StreamingRate.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<RemoteHDHomeRunDeviceInfo> RefreshTunerSignalStatsAsync(this RemoteHDHomeRunDeviceInfo deviceInfo, CancellationToken cancellationToken)
            => await RefreshTunerSignalStatsAsync(deviceInfo, cancellationToken);

        /// <summary>
        /// Refreshes all tuner signal stats using the status api.
        /// For whatever reason this method of refreshing tuner stats does not update StreamingRate.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        private static async Task<IDeviceInfo> RefreshTunerSignalStatsAsync(this IDeviceInfo deviceInfo, CancellationToken cancellationToken)
        {
            List<TunerStatusResponse>? tunerStatusResponse = await GetTunerStatusResponseAsync(deviceInfo, cancellationToken);

            if (tunerStatusResponse is not null)
            {
                foreach (TunerStatusResponse status in tunerStatusResponse)
                {
                    int tunerNumber = int.Parse(status.Resource.Replace("tuner", string.Empty, StringComparison.InvariantCultureIgnoreCase));
                    ITunerInfo? tunerInfo = deviceInfo.Tuners[tunerNumber];
                    if (tunerInfo is not null)
                    {
                        tunerInfo.Frequency = status.Frequency;
                        tunerInfo.SignalStrength = status.SignalStrengthPercent;
                        tunerInfo.SignalQuality = status.SignalQualityPercent;
                        tunerInfo.SymbolQuality = status.SymbolQualityPercent;
                    }
                }
            }

            return deviceInfo;
        }

        /// <summary>
        /// Gets a TunerStatusResponse from the device status API.
        /// Only supported by HDHomeRun devices currently.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        private static async Task<List<TunerStatusResponse>?> GetTunerStatusResponseAsync(this IDeviceInfo deviceInfo)
            => await GetTunerStatusResponseAsync(deviceInfo, CancellationToken.None);

        /// <summary>
        /// Gets a TunerStatusResponse from the device status API.
        /// Only supported by HDHomeRun devices currently.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        private static async Task<List<TunerStatusResponse>?> GetTunerStatusResponseAsync(this IDeviceInfo deviceInfo, CancellationToken cancellationToken)
        {
            string? statusResponse = await _httpClient.GetStringAsync($"{deviceInfo.DeviceAccess}/status.json", cancellationToken);
            return JsonConvert.DeserializeObject<List<TunerStatusResponse>>(statusResponse);
        }

        /// <summary>
        /// Refreshes device info.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public static async Task<IDeviceInfo> RefreshDeviceInfoAsync(this IDeviceInfo deviceInfo)
            => await deviceInfo.RefreshDeviceInfoAsync(CancellationToken.None);

        /// <summary>
        /// Refreshes device info.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public static async Task<IDeviceInfo> RefreshDeviceInfoAsync(this IDeviceInfo deviceInfo, CancellationToken cancellationToken)
        {
            string responseString = await _httpClient.GetStringAsync($"http://{deviceInfo.DeviceIP}/discover.json", cancellationToken);

            DiscoverModel? discoverResponse = JsonConvert.DeserializeObject<DiscoverModel>(responseString);

            if (discoverResponse?.DeviceID is not null)
            {
                string devicePage = await _httpClient.GetStringAsync($"http://{deviceInfo.DeviceIP}/", cancellationToken);
                string systemPage = await _httpClient.GetStringAsync($"http://{deviceInfo.DeviceIP}/system.html", cancellationToken);
                string lineupPage = await _httpClient.GetStringAsync($"http://{deviceInfo.DeviceIP}/lineup.json?show=found", cancellationToken);

                deviceInfo.System = new()
                {
                    MemoryReport = new(MatchDevicePage(MemoryReportRegex(), systemPage)),
                    MACAddress = PhysicalAddress.Parse(MatchDevicePage(MACAddressRegex(), systemPage)),
                    SubnetMask = IPAddress.Parse(MatchDevicePage(SubmetMaskRegex(), systemPage)),
                    HardwareModel = discoverResponse.ModelNumber,
                    DeviceName = discoverResponse.FriendlyName,
                    FirmwareVersion = discoverResponse.FirmwareVersion,
                    DeviceID = discoverResponse.DeviceID,
                    UpdateAvailable = discoverResponse.UpgradeAvailable,
                    AvailableRelease = discoverResponse.UpgradeVersion
                };

                deviceInfo.Status = new()
                {
                    Validated = MatchDevicePageSuccess(CardValidationRegex(), devicePage),
                    Authenticated = MatchDevicePageSuccess(CardAuthenticationRegex(), devicePage),
                    OOBLock = DeviceStatus.ParseOOBLock(MatchDevicePage(OOBLockRegex(), devicePage))
                };

                deviceInfo.Tuners.TunerCount = deviceInfo.Tuners.TunerCount == 0 ?
                    ReturnTunerCount(await _httpClient.GetStringAsync($"http://{deviceInfo.DeviceIP}/tuners.html", cancellationToken))
                  : deviceInfo.Tuners.TunerCount;

                if (deviceInfo.Tuners.Any() == false)
                {
                    deviceInfo.Tuners.CreateTuners(deviceInfo.Tuners.TunerCount,
                                                   deviceInfo.DeviceIP,
                                                   discoverResponse.DeviceID,
                                                   (TunerLocation)deviceInfo.Location);
                }

                List<ChannelInfo>? channels = JsonConvert.DeserializeObject<List<ChannelInfo>>(lineupPage);
                if (channels is not null && channels.Any())
                    deviceInfo.ChannelLineup.Channels = channels;
            }

            return deviceInfo;
        }

        /// <returns>
        /// Device tuner count.
        /// </returns>
        private static int ReturnTunerCount(string tunerPage)
        {
            int tunerNumber = 0;
            string tunerPageUpper = tunerPage.ToUpper();

            while (tunerPageUpper.Contains($"TUNER{tunerNumber}"))
            {
                tunerNumber++;
            }

            return tunerNumber;
        }

        /// <summary>
        /// Matches a regular expression on the device page content.
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="devicePage"></param>
        /// <returns></returns>
        private static string MatchDevicePage(Regex reg, string devicePage)
        {
            var match = reg.Match(devicePage);
            if (match.Groups.Count > 1)
                if (match.Groups[2].Value != null)
                    return match.Groups[2].Value;
            return string.Empty;
        }

        /// <summary>
        /// Checks for success using a regular expression on the device page content.
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="devicePage"></param>
        /// <returns></returns>
        private static bool MatchDevicePageSuccess(Regex reg, string devicePage)
        {
            var match = reg.Match(devicePage);
            if (match.Groups.Count > 1)
                if (match.Groups[2].Value.ToString().ToUpper() == "SUCCESS")
                    return true;
            return false;
        }

        [GeneratedRegex("(Card Authentication<\\/td><td>)(.*)(<\\/td><\\/tr>)", RegexOptions.IgnoreCase)]
        private static partial Regex CardAuthenticationRegex();

        [GeneratedRegex("(Memory Report</td><td>)(.*)(</td></tr>)", RegexOptions.IgnoreCase)]
        private static partial Regex MemoryReportRegex();

        [GeneratedRegex("(Subnet Mask</td><td>)(.*)(</td></tr>)", RegexOptions.IgnoreCase)]
        private static partial Regex SubmetMaskRegex();

        [GeneratedRegex("(MAC Address</td><td>)(.*)(</td></tr>)", RegexOptions.IgnoreCase)]
        private static partial Regex MACAddressRegex();

        [GeneratedRegex("(Card OOB Lock<\\/td><td>)(.*)(<\\/td><\\/tr>)", RegexOptions.IgnoreCase)]
        private static partial Regex OOBLockRegex();

        [GeneratedRegex("(Card Validation<\\/td><td>)(.*)(<\\/td><\\/tr>)", RegexOptions.IgnoreCase)]
        private static partial Regex CardValidationRegex();
    }
}
