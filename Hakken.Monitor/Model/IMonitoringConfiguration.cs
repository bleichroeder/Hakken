namespace Hakken.Monitor.Model
{
    /// <summary>
    /// Monitoring configuration interface.
    /// </summary>
    public interface IMonitoringConfiguration
    {
        /// <summary>
        /// The duration of the monitoring session.
        /// </summary>
        TimeSpan Duration { get; set; }

        /// <summary>
        /// The interval between monitoring requests.
        /// </summary>
        TimeSpan Interval { get; set; }

        /// <summary>
        /// The maximum length of time to store values.
        /// </summary>
        TimeSpan MaximumStoredDuration { get; set; }
    }
}
