using Hakken.Channel.Model;
using Hakken.Device.Model;
using Hakken.Remote.Utility;
using Hakken.Tuner.Model;
using System.Runtime.Versioning;
using System.Security;

namespace Hakken.Remote.Extensions
{
    public static class DeviceExtensions
    {
        private static readonly HttpClient _httpClient = new();

        /// <summary>
        /// Generates a channel reference table for corellating channel numbers to frequencies.
        /// Requires at least one tuner to be available.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="host"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [SupportedOSPlatform("Windows")]
        public static async Task<ChannelReferenceTable> GenerateChannelReferenceTable(this RemoteDeviceInfoBase deviceInfo, string host, string user, string password)
            => await GenerateChannelReferenceTable(deviceInfo, host, user, ManagementHelper.ConvertToSecureString(password), CancellationToken.None);

        /// <summary>
        /// Generates a channel reference table for corellating channel numbers to frequencies.
        /// Requires at least one tuner to be available.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        [SupportedOSPlatform("Windows")]
        public static async Task<ChannelReferenceTable> GenerateChannelReferenceTable(this RemoteDeviceInfoBase deviceInfo, string host, string user, SecureString password, CancellationToken cancellationToken)
        {
            ChannelReferenceTable channelReferenceTable = new();

            RemoteTunerInfo? availableTuner = deviceInfo.Tuners.GetFirstAvailableTuner() as RemoteTunerInfo
                ?? throw new InvalidOperationException("No available tuners found.");

            foreach (ChannelInfo channel in deviceInfo.ChannelLineup)
            {
                using HttpRequestMessage request = new(HttpMethod.Get, channel.StreamingUri);
                using HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);

                await Task.Run(async () =>
                {
                    // Error handling example
                    try
                    {
                        StreamInfo? streamInfo = await availableTuner.GetStreamInfoAsync(user, password, host, cancellationToken);

                        if (streamInfo is not null)
                        {
                            foreach (ChannelDetail channelDetail in streamInfo.DetectedChannels)
                            {
                                if (channelReferenceTable.TryGetValue(streamInfo.Frequency, out List<ChannelDetail>? channelDetails))
                                {
                                    channelDetails?.Add(channelDetail);
                                    continue;
                                }

                                channelReferenceTable.TryAdd(streamInfo.Frequency, new List<ChannelDetail>() { channelDetail });
                            }
                        }
                    }
                    catch (Exception) { }
                }, cancellationToken);
            }

            return channelReferenceTable;
        }
    }
}
