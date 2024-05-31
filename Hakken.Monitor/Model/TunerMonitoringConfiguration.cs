namespace Hakken.Monitor.Model
{
    public class TunerMonitoringConfiguration : IMonitoringConfiguration
    {
        /// <summary>
        /// The total duration to monitor the tuner for.
        /// Defaults to <see cref="TimeSpan.MaxValue"/>.
        /// </summary>
        public TimeSpan Duration { get; set; } = TimeSpan.MaxValue;

        /// <summary>
        /// The interval between polling the tuner for data.
        /// Defaults to <see cref="TimeSpan.FromSeconds(1)"/>.
        /// </summary>
        public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(1);

        /// <summary>
        /// The maximum duration of data to store.
        /// Defaults to <see cref="TimeSpan.MaxValue"/>.
        /// </summary>
        public TimeSpan MaximumStoredDuration { get; set; } = TimeSpan.MaxValue;
    }
}
