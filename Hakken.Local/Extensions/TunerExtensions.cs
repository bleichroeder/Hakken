using Hakken.Tuner.Model;
using System.Diagnostics;

namespace Hakken.Local.Extensions
{
    /// <summary>
    /// Provides methods for performing actions on HDHomeRun devices.
    /// </summary>
    public static class TunerExtensions
    {
        private const string HDHRCONFIG_PATH = @"C:\Program Files\Silicondust\HDHomeRun\hdhomerun_config.exe";
        private static readonly string LOCAL_STREAMINFO_COMMAND = "\"{0}\" {1} get /tuner{2}/streaminfo && " +
                                                                  "\"{0}\" {1} get /tuner{2}/channel streaminfo";

        /// <summary>
        /// Attempts to restart a device on a remote host.
        /// </summary>
        /// <param name="tunerInfo"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        public static async Task<StreamInfo?> GetStreamInfoAsync(this LocalTunerInfo tunerInfo)
            => await GetStreamInfoAsync(tunerInfo, HDHRCONFIG_PATH);

        /// <summary>
        /// Attempts to restart a device on a remote host.
        /// </summary>
        /// <param name="tunerInfo"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="host"></param>
        /// <param name="hdhrconfigPath"></param>
        /// <returns></returns>
        public static async Task<StreamInfo?> GetStreamInfoAsync(this LocalTunerInfo tunerInfo,
                                                                      string hdhrconfigPath)
        {
            string localCommand = string.Format(LOCAL_STREAMINFO_COMMAND,
                                                hdhrconfigPath,
                                                tunerInfo.DeviceID,
                                                tunerInfo.TunerNumber);

            ProcessStartInfo startInfo = new()
            {
                FileName = hdhrconfigPath,
                Arguments = localCommand,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            using Process process = new() { StartInfo = startInfo };

            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();

            await process.WaitForExitAsync();

            return StreamInfo.FromHDHomeRunConfigOutput(output);
        }
    }
}
