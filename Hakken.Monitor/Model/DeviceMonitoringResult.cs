using Hakken.Device.Model;

namespace Hakken.Monitor.Model
{
    public class DeviceMonitoringResult : IMonitoringResult<IDeviceInfo>
    {
        /// <summary>
        /// Gets the monitoring configuration.
        /// </summary>
        public IMonitoringConfiguration Configuration => _monitoringConfiguration;

        /// <summary>
        /// The stored monitoring data.
        /// </summary>
        public IReadOnlyCollection<(DateTime Timestamp, IDeviceInfo StoredData)> StoredData => _storedData;

        /// <summary>
        /// Gets the last stored tuner info.
        /// </summary>
        public (DateTime Timestamp, IDeviceInfo StoredData) MostRecentData => _storedData.Last();

        /// <summary>
        /// Gets the oldest stored tuner info.
        /// </summary>
        public (DateTime Timestamp, IDeviceInfo StoredData) OldestData => _storedData.Last();

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
        public IEnumerable<IDeviceInfo> GetDataBetween(DateTime start, DateTime end)
            => _storedData.Where(d => d.Timestamp >= start && d.Timestamp <= end).Select(d => d.StoredData);

        private readonly IMonitoringConfiguration _monitoringConfiguration;
        private readonly List<(DateTime Timestamp, IDeviceInfo StoredData)> _storedData;
        private readonly DateTime _startDateTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="TunerMonitoringResult"/> class.
        /// </summary>
        /// <param name="configuration"></param>
        public DeviceMonitoringResult(IMonitoringConfiguration configuration)
        {
            _monitoringConfiguration = configuration;
            _storedData = new List<(DateTime Timestamp, IDeviceInfo StoredData)>();
            _startDateTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Adds the specified tuner info to the monitoring result.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="storedData"></param>
        public void Add(DateTime dateTime, IDeviceInfo storedData)
        {
            _storedData.Add((Timestamp: dateTime, StoredData: storedData));

            if (TotalStoredDataDuration > _monitoringConfiguration.MaximumStoredDuration)
                _storedData.RemoveAt(0);
        }
    }
}
