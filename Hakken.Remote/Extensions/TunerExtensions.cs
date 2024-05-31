using Hakken.Remote.Model;
using Hakken.Remote.Utility;
using Hakken.Tuner.Model;
using System.Runtime.Versioning;
using System.Security;

namespace Hakken.Remote.Extensions
{
    /// <summary>
    /// Provides methods for performing actions on HDHomeRun devices.
    /// </summary>
    public static class TunerExtensions
    {
        private static readonly string HDHRCONFIG_PATH = @"C:\Program Files\Silicondust\HDHomeRun\hdhomerun_config.exe";
        private static readonly string REMOTE_STREAMINFO_COMMAND = "CMD.EXE /S /C CALL \"{0}\" {1} get /tuner{2}/streaminfo > {3} && " +
                                                                   "CMD.EXE /S /C CALL \"{0}\" {1} get /tuner{2}/channel streaminfo >> {3}";

        private static readonly string REMOTEPROCESS_TEMPPATH = "\\\\{0}\\c$\\Users\\{1}\\AppData\\Local\\Temp";
        private static readonly string REMOTEPROCESS_FILEPATH = "%TEMP%";
        private static readonly string REMOTEPROCESS_FILENAME = "STREAMINFO";

        #region RemoteExtensions

        /// <summary>
        /// Attempts to retrieve the stream info for a tuner on a remote host.
        /// </summary>
        /// <param name="tunerInfo"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        [SupportedOSPlatform("Windows")]
        public static async Task<StreamInfo?> GetStreamInfoAsync(this RemoteTunerInfo tunerInfo,
                                                                      string user,
                                                                      SecureString password,
                                                                      string host)
            => await GetStreamInfoAsync(tunerInfo, user, password, host, HDHRCONFIG_PATH, CancellationToken.None);

        /// <summary>
        /// Attempts to restart a device on a remote host.
        /// </summary>
        /// <param name="tunerInfo"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        [SupportedOSPlatform("Windows")]
        public static async Task<StreamInfo?> GetStreamInfoAsync(this RemoteTunerInfo tunerInfo,
                                                                      string user,
                                                                      SecureString password,
                                                                      string host,
                                                                      CancellationToken cancellationToken)
            => await GetStreamInfoAsync(tunerInfo, user, password, host, HDHRCONFIG_PATH, cancellationToken);

        /// <summary>
        /// Attempts to restart a device on a remote host.
        /// </summary>
        /// <param name="tunerInfo"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="host"></param>
        /// <param name="hdhrconfigPath"></param>
        /// <returns></returns>
        [SupportedOSPlatform("Windows")]
        public static async Task<StreamInfo?> GetStreamInfoAsync(this RemoteTunerInfo tunerInfo,
                                                                      string user,
                                                                      SecureString password,
                                                                      string host,
                                                                      string hdhrconfigPath,
                                                                      CancellationToken cancellationToken)
        {
            string userName = user.Contains('\\') ?
                              user.Split('\\')[1]
                            : user;

            string remoteTempPath = string.Format(REMOTEPROCESS_TEMPPATH, host, userName);
            string processOutputFileName = Path.Combine(REMOTEPROCESS_FILEPATH, REMOTEPROCESS_FILENAME);
            string retrieveOutputFileName = Path.Combine(remoteTempPath, REMOTEPROCESS_FILENAME);

            string remoteCommand = string.Format(REMOTE_STREAMINFO_COMMAND,
                                                 hdhrconfigPath,
                                                 tunerInfo.DeviceID,
                                                 tunerInfo.TunerNumber,
                                                 processOutputFileName);

            await ManagementHelper.InvokeAsync(remoteCommand, host, user, password);

            RemoteReaderResponse remoteReaderResponse = await RemoteReader.GetRemoteContentAsync(retrieveOutputFileName,
                                                                                                 true,
                                                                                                 userName,
                                                                                                 password,
                                                                                                 cancellationToken);

            return remoteReaderResponse.Success ? StreamInfo.FromHDHomeRunConfigOutput(remoteReaderResponse.Content) : null;
        }

        #endregion
    }
}
