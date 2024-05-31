using Hakken.Device.Model;
using Hakken.Remote.Utility;
using System.Runtime.Versioning;
using System.Security;

namespace Hakken.Remote.Control.Extensions
{
    /// <summary>
    /// Provides methods for performing actions on HDHomeRun devices.
    /// </summary>
    public static class DeviceExtensions
    {
        private static readonly string HDHRCONFIG_PATH = @"C:\Program Files\Silicondust\HDHomeRun\hdhomerun_config.exe";
        private static readonly string REMOTE_HDHRCONFIG_RESTART = "CMD.EXE /S /C CALL \"{0}\" {1} set /sys/restart self";

        /// <summary>
        /// Attempts to restart a device on a remote host.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        [SupportedOSPlatform("Windows")]
        public static async Task RestartAsync(this RemoteDeviceInfoBase deviceInfo,
                                                   string user,
                                                   string password,
                                                   string host)
            => await RestartAsync(deviceInfo, user, password, host, HDHRCONFIG_PATH);

        /// <summary>
        /// Attempts to restart a device on a remote host.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="host"></param>
        /// <param name="hdhrconfigPath"></param>
        /// <returns></returns>
        [SupportedOSPlatform("Windows")]
        public static async Task RestartAsync(this RemoteDeviceInfoBase deviceInfo,
                                                   string user,
                                                   string password,
                                                   string host,
                                                   string hdhrconfigPath)
        {
            string remoteCommand = string.Format(REMOTE_HDHRCONFIG_RESTART, hdhrconfigPath, deviceInfo.DeviceID);

            await ManagementHelper.InvokeAsync(remoteCommand, host, user, ManagementHelper.ConvertToSecureString(password));
        }

        /// <summary>
        /// Attempts to restart a device on a remote host.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        [SupportedOSPlatform("Windows")]
        public static async Task RestartAsync(this RemoteDeviceInfoBase deviceInfo,
                                                   string user,
                                                   SecureString password,
                                                   string host)
            => await RestartAsync(deviceInfo, user, password, host, HDHRCONFIG_PATH);

        /// <summary>
        /// Attempts to restart a device on a remote host.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="host"></param>
        /// <param name="hdhrconfigPath"></param>
        /// <returns></returns>
        [SupportedOSPlatform("Windows")]
        public static async Task RestartAsync(this RemoteDeviceInfoBase deviceInfo,
                                                   string user,
                                                   SecureString password,
                                                   string host,
                                                   string hdhrconfigPath)
        {
            string remoteCommand = string.Format(REMOTE_HDHRCONFIG_RESTART, hdhrconfigPath, deviceInfo.DeviceID);

            await ManagementHelper.InvokeAsync(remoteCommand, host, user, password);
        }
    }
}
