using Hakken.Tuner.Model;
using System.Collections;

namespace Hakken.Device.Model
{
    /// <summary>
    /// The device's channel reference table.
    /// </summary>
    public class ChannelReferenceTable : IEnumerable<KeyValuePair<double, List<ChannelDetail>>>
    {
        /// <summary>
        /// The underlying reference table.
        /// </summary>
        private readonly Dictionary<double, List<ChannelDetail>> _referenceTable = new();

        /// <summary>
        /// True if the table is empty.
        /// </summary>
        public bool IsEmpty => _referenceTable.Count == 0;

        /// <summary>
        /// Attempts to get the channel details for the given frequency.
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="channelDetails"></param>
        /// <returns></returns>
        public bool TryGetValue(double frequency, out List<ChannelDetail>? channelDetails)
        {
            return _referenceTable.TryGetValue(frequency, out channelDetails);
        }

        /// <summary>
        /// Attempts to add channel details for the given frequency.
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="channelDetails"></param>
        /// <returns></returns>
        public bool TryAdd(double frequency, List<ChannelDetail> channelDetails)
            => _referenceTable.TryAdd(frequency, channelDetails);

        /// <summary>
        /// Gets the number of unique channels in the table.
        /// </summary>
        public int UniqueChannelCount => _referenceTable.Values
                                                        .SelectMany(ch => ch)
                                                        .Select(i => i.GuideName)
                                                        .Distinct()
                                                        .Count();

        /// <summary>
        /// The minimum and maximum frequencies in the table.
        /// </summary>
        public (double MinFrequency, double MaxFrequency) FrequencySpan
        {
            get
            {
                if (_referenceTable.Count == 0)
                    return (0, 0);

                double minFrequency = _referenceTable.Keys.Min();
                double maxFrequency = _referenceTable.Keys.Max();
                return (minFrequency, maxFrequency);
            }
        }

        public IEnumerator<KeyValuePair<double, List<ChannelDetail>>> GetEnumerator() => _referenceTable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _referenceTable.GetEnumerator();
    }

}
