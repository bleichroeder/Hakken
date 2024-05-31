using Hakken.Tuner.Model;

namespace Hakken.Monitor.Model
{
    public class TunerMonitoringResult : IMonitoringResult<ITunerInfo>
    {
        /// <summary>
        /// Gets the monitoring configuration.
        /// </summary>
        public IMonitoringConfiguration Configuration => _monitoringConfiguration;

        /// <summary>
        /// The stored monitoring data.
        /// </summary>
        public IReadOnlyCollection<(DateTime Timestamp, ITunerInfo StoredData)> StoredData => _storedData;

        /// <summary>
        /// Gets the last stored tuner info.
        /// </summary>
        public (DateTime Timestamp, ITunerInfo StoredData) MostRecentData => _storedData.Last();

        /// <summary>
        /// Gets the oldest stored tuner info.
        /// </summary>
        public (DateTime Timestamp, ITunerInfo StoredData) OldestData => _storedData.Last();

        /// <summary>
        /// Gets the DateTime the monitoring started.
        /// This is not necessarily the earliest value in the stored data.
        /// </summary>
        public DateTime MonitoringStartActual => _startDateTime;

        /// <summary>
        /// Gets the earliest stored data DateTime.
        /// </summary>
        public DateTime OldestStoredDataDateTime => _storedData.Min(d => d.Timestamp);

        /// <summary>
        /// Gets the monitoring end date time.
        /// </summary>
        public DateTime MostRecentStoredDataDateTime => _storedData.Max(d => d.Timestamp);

        /// <summary>
        /// Gets the total duration of the monitoring session.
        /// </summary>
        public TimeSpan TotalMonitoredDuration => MostRecentStoredDataDateTime - MonitoringStartActual;

        /// <summary>
        /// Gets the duration of the stored data.
        /// </summary>
        public TimeSpan TotalStoredDataDuration => MostRecentStoredDataDateTime - OldestStoredDataDateTime;

        /// <summary>
        /// Gets the tuner info data between the specified start and end date times.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public IEnumerable<ITunerInfo> GetDataBetween(DateTime start, DateTime end)
            => _storedData.Where(d => d.Timestamp >= start && d.Timestamp <= end).Select(d => d.TunerInfo);

        /// <summary>
        /// Gets the average signal strength between the specified start and end date times.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public double GetAverageSignalStrengthBetween(DateTime start, DateTime end)
            => GetDataBetween(start, end).Average(d => d.SignalStrength);

        /// <summary>
        /// Gets the average signal quality between the specified start and end date times.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public double GetAverageSignalQualityBetween(DateTime start, DateTime end)
            => GetDataBetween(start, end).Average(d => d.SignalQuality);

        /// <summary>
        /// Gets the average symbol quality between the specified start and end date times.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public double GetAverageSymbolQualityBetween(DateTime start, DateTime end)
            => GetDataBetween(start, end).Average(d => d.SymbolQuality);

        /// <summary>
        /// Gets the average streaming rate between the specified start and end date times.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public decimal GetAverageStreamingRateBetween(DateTime start, DateTime end)
            => GetDataBetween(start, end).Average(d => d.StreamingRate);

        private readonly IMonitoringConfiguration _monitoringConfiguration;
        private readonly List<(DateTime Timestamp, ITunerInfo TunerInfo)> _storedData;
        private readonly DateTime _startDateTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="TunerMonitoringResult"/> class.
        /// </summary>
        /// <param name="configuration"></param>
        public TunerMonitoringResult(IMonitoringConfiguration configuration)
        {
            _monitoringConfiguration = configuration;
            _storedData = new List<(DateTime Timestamp, ITunerInfo TunerInfo)>();
            _startDateTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Adds the specified tuner info to the monitoring result.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="tunerInfo"></param>
        public void Add(DateTime dateTime, ITunerInfo tunerInfo)
        {
            _storedData.Add((Timestamp: dateTime, TunerInfo: tunerInfo));

            if (TotalStoredDataDuration > _monitoringConfiguration.MaximumStoredDuration)
                _storedData.RemoveAt(0);
        }
    }
}
