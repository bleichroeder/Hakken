using Hakken.Channel.Model;
using Hakken.Device.Model;
using Hakken.Tuner.Model;

namespace Hakken.Local.Extensions
{
    /// <summary>
    /// Provides local device extensions.
    /// </summary>
    public static class DeviceExtensions
    {
        private static readonly HttpClient _httpClient = new();

        /// <summary>
        /// Generates a channel reference table for corellating channel numbers to frequencies.
        /// Requires at least one tuner to be available.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public static async Task<ChannelReferenceTable> GenerateChannelReferenceTableAsync(this LocalDeviceInfoBase deviceInfo)
        {
            ChannelReferenceTable channelReferenceTable = new();

            LocalTunerInfo? availableTuner = deviceInfo.Tuners.GetFirstAvailableTuner() as LocalTunerInfo
                ?? throw new InvalidOperationException("No available tuners found.");

            foreach (ChannelInfo channel in deviceInfo.ChannelLineup)
            {
                HttpRequestMessage request = new(HttpMethod.Get, channel.StreamingUri);
                HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                using Stream stream = await response.Content.ReadAsStreamAsync();

                await Task.Run(async () =>
                {
                    StreamInfo? streamInfo = await availableTuner.GetStreamInfoAsync();

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
                });

                stream.Close();
            }

            return channelReferenceTable;
        }
    }
}
