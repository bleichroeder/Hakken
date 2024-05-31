namespace Hakken.Monitor.Model
{
    public interface IMonitoringResult<T> where T : class
    {
        /// <summary>
        /// Gets the monitoring configuration.
        /// </summary>
        public IMonitoringConfiguration Configuration { get; }

        /// <summary>
        /// The stored monitoring data.
        /// </summary>
        public IReadOnlyCollection<(DateTime Timestamp, T StoredData)> StoredData { get; }

        /// <summary>
        /// Gets the last stored tuner info.
        /// </summary>
        public (DateTime Timestamp, T StoredData) MostRecentData { get; }

        /// <summary>
        /// Gets the oldest stored tuner info.
        /// </summary>
        public (DateTime Timestamp, T StoredData) OldestData { get; }

        /// <summary>
        /// Gets the DateTime the monitoring started.
        /// This is not necessarily the earliest value in the stored data.
        /// </summary>
        public DateTime MonitoringStartActual { get; }

        /// <summary>
        /// Gets the earliest stored data DateTime.
        /// </summary>
        public DateTime OldestStoredDataDateTime { get; }

        /// <summary>
        /// Gets the monitoring end date time.
        /// </summary>
        public DateTime MostRecentStoredDataDateTime { get; }

        /// <summary>
        /// Gets the total duration of the monitoring session.
        /// </summary>
        public TimeSpan TotalMonitoredDuration { get; }

        /// <summary>
        /// Gets the duration of the stored data.
        /// </summary>
        public TimeSpan TotalStoredDataDuration { get; }

        /// <summary>
        /// Gets the tuner info data between the specified start and end date times.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public IEnumerable<T> GetDataBetween(DateTime start, DateTime end);

        /// <summary>
        /// Adds the specified tuner info to the monitoring result.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="tunerInfo"></param>
        public void Add(DateTime dateTime, T storedData);
    }
}
