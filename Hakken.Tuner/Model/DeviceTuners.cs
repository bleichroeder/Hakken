using System.Collections;
using System.Net;

namespace Hakken.Tuner.Model
{
    /// <summary>
    /// The device's available tuners.
    /// </summary>
    public class DeviceTuners : IEnumerable<ITunerInfo>
    {
        private IReadOnlyCollection<ITunerInfo> _tunerInfos = new List<ITunerInfo>();

        /// <summary>
        /// Gets the device's available tuners.
        /// </summary>
        public IReadOnlyCollection<ITunerInfo> TunerInfos => _tunerInfos;

        /// <summary>
        /// Indexer to get a Tuner by its tuner number.
        /// </summary>
        /// <param name="tunerNumber"></param>
        /// <returns></returns>
        public ITunerInfo? this[int tunerNumber]
            => _tunerInfos.FirstOrDefault(tuner => tuner.TunerNumber == tunerNumber);

        /// <summary>
        /// Indexer to get a Tuner by it's currently used virtual channel.
        /// </summary>
        /// <param name="tunerNumber"></param>
        /// <returns></returns>
        public ITunerInfo? this[string virtualChannel]
            => _tunerInfos.FirstOrDefault(tuner => tuner.VirtualChannel == virtualChannel);

        /// <summary>
        /// Returns the first available tuner.
        /// </summary>
        /// <returns></returns>
        public ITunerInfo? GetFirstAvailableTuner()
            => _tunerInfos.FirstOrDefault(tuner => tuner.IsStreaming == false);

        /// <summary>
        /// Gets the number of tuners on the device.
        /// </summary>
        public int TunerCount { get; set; }

        /// <summary>
        /// Creates the device's available tuners.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="deviceIP"></param>
        /// <param name="deviceID"></param>
        public void CreateTuners(int count, IPAddress deviceIP, string deviceID, TunerLocation tunerLocation)
        {
            List<ITunerInfo> tuners = new();

            for (int tunerNumber = 0; tunerNumber < count; tunerNumber++)
            {
                ITunerInfo tunerInfo = tunerLocation == TunerLocation.Local ?
                                       new LocalTunerInfo(deviceIP, tunerNumber, deviceID)
                                     : new RemoteTunerInfo(deviceIP, tunerNumber, deviceID);

                tuners.Add(tunerInfo);
            }

            _tunerInfos = tuners;
        }

        public IEnumerator<ITunerInfo> GetEnumerator() => _tunerInfos.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _tunerInfos.GetEnumerator();
    }

}
