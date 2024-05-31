using Hakken.Channel.Model;
using Hakken.Device.Model;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;

namespace Hakken.Control.Extensions
{
    /// <summary>
    /// Provides methods for performing actions on HDHomeRun devices.
    /// </summary>
    public static class DeviceExtensions
    {
        private static readonly HttpClient _httpClient = new();

        private const string HDHRCONFIG_PATH = @"C:\Program Files\Silicondust\HDHomeRun\hdhomerun_config.exe";

        private const string HTTP_SCAN_START = "{0}/lineup.post?scan=start";
        private const string HTTP_SCAN_STOP = "{0}/lineup.post?scan=abort";
        private const string HTTP_SCAN_STATUS = "{0}/lineup_status.json";
        private const string HTTP_UPDATE_START = "{0}/system.post?upgrade=install";
        private const string RESTART_COMMAND = "{0} set /sys/restart self";

        /// <summary>
        /// Attempts to restart a local device.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public static async Task<bool> RestartAsync(this LocalDeviceInfoBase deviceInfo)
            => await RestartAsync(deviceInfo, HDHRCONFIG_PATH);

        /// <summary>
        /// Attempts to restart a local device.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="hdhrconfigPath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<bool> RestartAsync(this LocalDeviceInfoBase deviceInfo, string hdhrconfigPath)
        {
            ProcessStartInfo processStartInfo = new()
            {
                FileName = hdhrconfigPath,
                Arguments = string.Format(RESTART_COMMAND, deviceInfo.DeviceID),
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = new()
            {
                StartInfo = processStartInfo
            };

            process.Start();

            await process.WaitForExitAsync();

            return process.ExitCode == 0;
        }

        /// <summary>
        /// Attempts to perform a channel scan on a remote device.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="host"></param>
        /// <param name="hdhrconfigPath"></param>
        /// <returns></returns>
        public static async Task<IDeviceInfo> StartChannelScanAsync(this IDeviceInfo deviceInfo)
        {
            string request = string.Format(HTTP_SCAN_START, deviceInfo.DeviceAccess);
            await _httpClient.PostAsync(request, null);
            return deviceInfo;
        }

        /// <summary>
        /// Attempts to stop a channel scan.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public static async Task<IDeviceInfo> StopChannelScanAsync(this IDeviceInfo deviceInfo)
        {
            string request = string.Format(HTTP_SCAN_STOP, deviceInfo.DeviceAccess);
            await _httpClient.PostAsync(request, null);
            return deviceInfo;
        }

        /// <summary>
        /// Attempts to get the status of a channel scan.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public static async Task<ChannelScanStatusResponse?> GetChannelScanStatusAsync(this IDeviceInfo deviceInfo)
        {
            string request = string.Format(HTTP_SCAN_STATUS, deviceInfo.DeviceAccess);
            HttpResponseMessage response = await _httpClient.GetAsync(request);
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ChannelScanStatusResponse>(responseBody);
        }

        /// <summary>
        /// Waits for the channel scan to complete.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public static async Task<bool> WaitForScanCompletionAsync(this IDeviceInfo deviceInfo)
            => await WaitForScanCompletionAsync(deviceInfo, TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(5));

        /// <summary>
        /// Waits for the channel scan to complete.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="pollingInterval"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<bool> WaitForScanCompletionAsync(this IDeviceInfo deviceInfo,
                                                                       TimeSpan pollingInterval,
                                                                       TimeSpan timeout)
        {
            DateTime startTime = DateTime.UtcNow;

            while (DateTime.UtcNow - startTime < timeout)
            {
                ChannelScanStatusResponse? status = await deviceInfo.GetChannelScanStatusAsync();

                if (status is null || status.Status is not ChannelScanStatus.Running)
                {
                    return true;
                }

                await Task.Delay(pollingInterval);
            }

            return false;
        }

        /// <summary>
        /// Attempts to start a firmware update.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public static async Task<HttpStatusCode> UpdateAsync(this IDeviceInfo deviceInfo)
        {
            string request = string.Format(HTTP_UPDATE_START, deviceInfo.DeviceAccess);
            HttpResponseMessage response = await _httpClient.PostAsync(request, null);
            return response.StatusCode;
        }
    }
}
