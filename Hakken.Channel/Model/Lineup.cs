using System.Collections;
using System.Text.RegularExpressions;

namespace Hakken.Channel.Model
{
    /// <summary>
    /// Provides access to the channel lineup.
    /// </summary>
    public class Lineup : IEnumerable<ChannelInfo>
    {
        private IEnumerable<ChannelInfo> _channels = Enumerable.Empty<ChannelInfo>();

        /// <summary>
        /// The channel lineup.
        /// </summary>
        public IEnumerable<ChannelInfo> Channels
        {
            get => _channels;
            set => _channels = value;
        }

        /// <summary>
        /// Indexer to get a channel by its guide number.
        /// </summary>
        /// <param name="index">Guide number.</param>
        /// <returns></returns>
        public ChannelInfo? this[int guideNumber]
            => GetChannelByGuideNumber(guideNumber);

        /// <summary>
        /// Indexer to get a channel by its guide number.
        /// </summary>
        /// <param name="index">Guide number.</param>
        /// <returns></returns>
        public ChannelInfo? this[string guideName]
            => GetChannelByGuideName(guideName);

        /// <summary>
        /// Attempts to return a channel by its guide name.
        /// </summary>
        /// <param name="guideName"></param>
        /// <returns></returns>
        public ChannelInfo? GetChannelByGuideName(string guideName, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
            => _channels.FirstOrDefault(c => c.GuideName.Equals(guideName, stringComparison));

        /// <summary>
        /// Attempts to return a channel by its guide number.
        /// </summary>
        /// <param name="channelNumber"></param>
        /// <returns></returns>
        public ChannelInfo? GetChannelByGuideNumber(double channelNumber)
            => _channels.FirstOrDefault(c => c.GuideNumber == channelNumber);

        /// <summary>
        /// Gets all channels matching the given pattern.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public IEnumerable<ChannelInfo> GetChannels(string pattern)
            => GetChannels(new Regex(pattern, RegexOptions.IgnoreCase));

        /// <summary>
        /// Gets all channels matching the given network affiliation.
        /// </summary>
        /// <param name="networkAffiliation"></param>
        /// <returns></returns>
        public IEnumerable<ChannelInfo> GetChannelsByNetworkAffiliation(NetworkAffiliation networkAffiliation)
            => _channels.Where(c => c.NetworkAffiliation == networkAffiliation);

        /// <summary>
        /// Returns all channels matching the given pattern.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public IEnumerable<ChannelInfo> GetChannels(Regex pattern)
            => _channels.Where(c => pattern.IsMatch(c.GuideName));

        public IEnumerator<ChannelInfo> GetEnumerator() => _channels.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _channels.GetEnumerator();
    }
}
