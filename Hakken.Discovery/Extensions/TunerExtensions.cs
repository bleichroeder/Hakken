using Hakken.Tuner.Model;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace Hakken.Discovery.Extensions
{
    /// <summary>
    /// Provides extensions for <see cref="LocalTunerInfo"/>.
    /// </summary>
    public static partial class TunerExtensions
    {
        /// <summary>
        /// The default path for the hdhomerun_config.exe file.
        /// </summary>
        private const string HDHOMERUNCONFIG_PATH = "C:\\Program Files\\Silicondust\\HDHomeRun\\hdhomerun_config.exe";

        /// <summary>
        /// HttpClient used for performing tuner refresh HTTP requests.
        /// </summary>
        private static readonly HttpClient _httpClient = new();

        /// <summary>
        /// Allows for the tuner info to be refreshed using a custom action.
        /// </summary>
        /// <param name="tunerInfo"></param>
        /// <param name="tunerRefreshAction"></param>
        /// <returns></returns>
        public static async Task<ITunerInfo> RefreshTunerDataAsync(this ITunerInfo tunerInfo,
                                                                   Func<ITunerInfo, CancellationToken, Task<ITunerInfo>> tunerRefreshAction)
            => await tunerRefreshAction(tunerInfo, CancellationToken.None);

        /// <summary>
        /// Allows for the tuner info to be refreshed using a custom action.
        /// </summary>
        /// <param name="tunerInfo"></param>
        /// <param name="tunerRefreshAction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<ITunerInfo> RefreshTunerDataAsync(this ITunerInfo tunerInfo,
                                                                   Func<ITunerInfo, CancellationToken, Task<ITunerInfo>> tunerRefreshAction,
                                                                   CancellationToken cancellationToken)
        {
            return await tunerRefreshAction(tunerInfo, cancellationToken);
        }

        /// <summary>
        /// Refreshes the tuner info using hdhomerun_config.exe.
        /// </summary>
        /// <param name="tunerInfo"></param>
        /// <param name="hdhomerunConfigPath"></param>
        /// <returns></returns>
        public static async Task<LocalTunerInfo> RefreshTunerInfoUsingConfig(this LocalTunerInfo tunerInfo, string? hdhomerunConfigPath)
        {
            hdhomerunConfigPath ??= HDHOMERUNCONFIG_PATH;

            ProcessStartInfo processStartInfo = new()
            {
                FileName = hdhomerunConfigPath,
                Arguments = string.Format("{0} get /tuner{1}/status", tunerInfo.DeviceID, tunerInfo.TunerNumber),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };

            Process process = new()
            {
                StartInfo = processStartInfo
            };

            process.Start();

            var standardOut = await process.StandardOutput.ReadToEndAsync();

            Regex reg = TunerInfoRegex();
            var match = reg.Match(standardOut);
            if (match.Groups.Count > 1)
            {
                tunerInfo.ModulationLock = match.Groups[5].Value;
                tunerInfo.SignalStrength = int.Parse(match.Groups[8].Value);
                tunerInfo.SignalQuality = int.Parse(match.Groups[11].Value);
                tunerInfo.SymbolQuality = int.Parse(match.Groups[14].Value);
                tunerInfo.StreamingRate = Math.Truncate(decimal.Parse(match.Groups[17].Value) / 1000000 * 100) / 100;
            }

            return tunerInfo;
        }

        /// <summary>
        /// Refreshes the tuner info using HTTP.
        /// </summary>
        /// <param name="tunerInfo"></param>
        /// <returns></returns>
        public static async Task<ITunerInfo> RefreshTunerInfoUsingHTTP(this ITunerInfo tunerInfo)
            => await RefreshTunerInfoUsingHTTP(tunerInfo, CancellationToken.None);

        /// <summary>
        /// Refreshes the tuner info using HTTP.
        /// </summary>
        /// <param name="tunerInfo"></param>
        /// <returns></returns>
        public static async Task<ITunerInfo> RefreshTunerInfoUsingHTTP(this ITunerInfo tunerInfo, CancellationToken cancellationToken)
        {
            HtmlDocument tunerDocument = new()
            {
                OptionFixNestedTags = true,
                OptionOutputAsXml = true
            };

            HttpResponseMessage response = await _httpClient.GetAsync(tunerInfo.TunerURI, cancellationToken);
            response.EnsureSuccessStatusCode(); // Throw an exception if the request was not successful

            string content = await response.Content.ReadAsStringAsync(cancellationToken);
            tunerDocument.LoadHtml(content);

            HtmlNode tableDataNode = tunerDocument.DocumentNode.SelectSingleNode("/html/body/div[2]/table");

            string? authorization = ParseFor<string?>(tableDataNode, "Authorization");
            if (!string.IsNullOrEmpty(authorization))
                tunerInfo.Authorization = authorization;

            string? cciProtection = ParseFor<string?>(tableDataNode, "CCI Protection");
            if (!string.IsNullOrEmpty(cciProtection))
                tunerInfo.CCIProtection = cciProtection;

            string? modulationLock = ParseFor<string?>(tableDataNode, "Modulation Lock");
            if (!string.IsNullOrEmpty(modulationLock))
                tunerInfo.ModulationLock = modulationLock;

            int programNumber = ParseFor<int>(tableDataNode, "Program Number");
            tunerInfo.ProgramNumber = programNumber;

            string? virtualChannel = ParseFor<string?>(tableDataNode, "Virtual Channel");
            if (!string.IsNullOrEmpty(virtualChannel))
                tunerInfo.VirtualChannel = virtualChannel;

            string? frequency = ParseFor<string?>(tableDataNode, "Frequency");
            if (!string.IsNullOrEmpty(frequency))
            {
                string parsedVal = frequency.Split(' ')[0];
                tunerInfo.Frequency = Convert.ToDouble(parsedVal);
            }

            string? pcrLock = ParseFor<string?>(tableDataNode, "PCR Lock");
            if (!string.IsNullOrEmpty(pcrLock))
                tunerInfo.PCRLock = pcrLock;

            string? signalQuality = ParseFor<string?>(tableDataNode, "Signal Quality");
            if (!string.IsNullOrEmpty(signalQuality))
            {
                string parsedVal = signalQuality.Split(' ')[0].Replace("%", "");
                tunerInfo.SignalQuality = Convert.ToInt32(parsedVal);
            }

            string? signalStrength = ParseFor<string?>(tableDataNode, "Signal Strength");
            if (!string.IsNullOrEmpty(signalStrength))
            {
                string parsedVal = signalStrength.Split(' ')[0].Replace("%", "");
                tunerInfo.SignalStrength = Convert.ToInt32(parsedVal);
            }

            string? symbolQuality = ParseFor<string?>(tableDataNode, "Symbol Quality");
            if (!string.IsNullOrEmpty(symbolQuality))
            {
                string parsedVal = symbolQuality.Split(' ')[0].Replace("%", "");
                tunerInfo.SymbolQuality = Convert.ToInt32(parsedVal);
            }

            string? streamingRate = ParseFor<string?>(tableDataNode, "Streaming Rate");
            if (!string.IsNullOrEmpty(streamingRate))
            {
                string parsedVal = streamingRate.Split(' ')[0];
                tunerInfo.StreamingRate = Convert.ToDecimal(parsedVal);
            }

            string? resourceLock = ParseFor<string?>(tableDataNode, "Resource Lock");
            if (!string.IsNullOrEmpty(resourceLock))
                tunerInfo.ResourceLock = IPAddress.Parse(resourceLock);

            return tunerInfo;
        }

        /// <summary>
        /// Parses HTML node for specified string and returns value.
        /// </summary>
        private static T? ParseFor<T>(HtmlNode targetNode, string request)
        {
            foreach (var tableRow in targetNode.ChildNodes)
            {
                if (tableRow.Name != "#text")
                {
                    var att = tableRow.FirstChild.InnerText;
                    var val = tableRow.LastChild.InnerText;

                    if (att == request && val.ToUpper() != "NONE")
                    {
                        return (T)Convert.ChangeType(val, typeof(T));
                    }
                }
            }
            return default;
        }

        [GeneratedRegex("(ch=)(.*)( )(lock=)(.*)( )(ss=)(.*)( )(snq=)(.*)( )(seq=)(.*)( )(bps=)(.*)( )(pps=)(.*)")]
        private static partial Regex TunerInfoRegex();
    }
}
