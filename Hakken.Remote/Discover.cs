using Hakken.Device.Model;
using Hakken.Discovery;
using Hakken.Discovery.Model;
using Hakken.Remote.Model;
using Hakken.Remote.Utility;
using System.Diagnostics;
using System.Net;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace Hakken.Remote
{
    /// <summary>
    /// Provides methods for discovering HDHomeRun devices on a remote host.
    /// </summary>
    public static partial class Discover
    {
        private const string REMOTEDISCOVERY_HDHRCONFIGCOMMAND = "CMD.EXE /S /C CALL \"{0}\" discover";
        private const string REMOTEDISCOVERY_ARPCOMMAND = "CMD.EXE /S /C arp -a";
        private const string REMOTEDISCOVERY_TEMPPATH = "\\\\{0}\\c$\\Users\\{1}\\AppData\\Local\\Temp";
        private const string REMOTEDISCOVERY_FILEPATH = "%TEMP%";
        private const string REMOTEDISCOVERY_FILENAME = "HDHR_DISCOVERY";

        /// <summary>
        /// Attempts to discover devices on a remote host.
        /// </summary>
        /// <param name="remoteRequest"></param>
        /// <returns></returns>
        [SupportedOSPlatform("Windows")]
        public static async Task<DiscoveryResponse> RemoteDevices(RemoteDiscoveryRequest remoteRequest)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            List<IPAddress> ipAddressList;

            string userName = remoteRequest.User.Contains('\\') ?
                              remoteRequest.User.Split('\\')[1]
                            : remoteRequest.User;

            string remoteTempPath = string.Format(REMOTEDISCOVERY_TEMPPATH,
                                                  remoteRequest.RemoteHost,
                                                  userName);

            string remoteCommand = remoteRequest.UseHDHRConfig ?
                $"{string.Format(REMOTEDISCOVERY_HDHRCONFIGCOMMAND, remoteRequest.HDHomerunConfigPath)} > {REMOTEDISCOVERY_FILEPATH}\\{REMOTEDISCOVERY_FILENAME}"
              : $"{REMOTEDISCOVERY_ARPCOMMAND} > \"{REMOTEDISCOVERY_FILEPATH}\\{REMOTEDISCOVERY_FILENAME}\"";

            try
            {
                await ManagementHelper.InvokeAsync(remoteCommand,
                                                   remoteRequest.RemoteHost,
                                                   remoteRequest.User,
                                                   remoteRequest.Password);

                string remoteFilePath = Path.Combine(remoteTempPath, REMOTEDISCOVERY_FILENAME);

                RemoteReaderResponse remoteReaderResponse = await RemoteReader.GetRemoteContentAsync(remoteFilePath,
                                                                                                     userName,
                                                                                                     remoteRequest.Password);
                if (remoteReaderResponse.Success == false)
                {
                    return new DiscoveryResponse()
                    {
                        Message = remoteReaderResponse.ErrorMessage
                    };
                }

                ipAddressList = remoteRequest.UseHDHRConfig ?
                        ParseIPAddressesFromHDHRConfigOutput(remoteReaderResponse.Content).ToList()
                      : ParseIPAddressesFromARPCommandOutput(remoteReaderResponse.Content, remoteRequest.TargetMACs.Distinct()).ToList();

                DiscoveryResponse discoveryResponse = await DiscoveryHelper.FromIPAddresses(ipAddressList, DeviceLocation.Remote);
                discoveryResponse.DiscoveryElapsedTime = stopwatch.Elapsed;

                return discoveryResponse;
            }
            catch (Exception ex)
            {
                return new DiscoveryResponse()
                {
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Attempts to parse a list of IPAddresses from remotely executed ARP output.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="targetMacs"></param>
        /// <returns></returns>
        private static HashSet<IPAddress> ParseIPAddressesFromARPCommandOutput(string output, IEnumerable<string> targetMacs)
        {
            HashSet<IPAddress> ipAddresses = new();

            string[] lines = output.Split(Environment.NewLine);

            foreach (string mac in targetMacs)
            {
                var foundMACAddressesMatchingPattern = lines.Where(m => m.Contains(mac, StringComparison.InvariantCultureIgnoreCase))
                                                            .Select(m => IPAddressRegex().Match(m).Value.Trim())
                                                            .Where(ip => IPAddress.TryParse(ip, out var _));

                foreach (var ipString in foundMACAddressesMatchingPattern)
                {
                    if (IPAddress.TryParse(ipString, out IPAddress? parsedIPAddress))
                    {
                        ipAddresses.Add(parsedIPAddress);
                    }
                }
            }

            return ipAddresses;
        }


        /// <summary>
        /// Attempts to parse a list of IPAddresses from remotely executed HDHRConfig output.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        private static HashSet<IPAddress> ParseIPAddressesFromHDHRConfigOutput(string output)
        {
            HashSet<IPAddress> ipAddresses = new();

            MatchCollection matchedIPAddresses = IPAddressRegex().Matches(output);

            foreach (Match matchedIPAddress in matchedIPAddresses.Cast<Match>())
            {
                if (IPAddress.TryParse(matchedIPAddress.Value.Trim(), out IPAddress? parsedIPAddress))
                {
                    ipAddresses.Add(parsedIPAddress);
                }
            }

            return ipAddresses;
        }

        /// <summary>
        /// The IPAddress Regex expression.
        /// </summary>
        /// <returns></returns>
        [GeneratedRegex("\\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\b")]
        private static partial Regex IPAddressRegex();
    }
}