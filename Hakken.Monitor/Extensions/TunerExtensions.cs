using Hakken.Discovery.Extensions;
using Hakken.Monitor.Model;
using Hakken.Tuner.Model;

namespace Hakken.Monitor.Extensions
{
    /// <summary>
    /// Provides monitoring extensions for <see cref="ITuner"/>.
    /// </summary>
    public static class TunerExtensions
    {
        /// <summary>
        /// Starts a monitoring session for the specified tuner using default configuration options.
        /// Monitoring will continue until the cancellation token is cancelled.
        /// </summary>
        /// <param name="tuner"></param>
        /// <param name="postQueryAction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<TunerMonitoringResult> MonitorUntilCancelled(this ITunerInfo tuner,
                                                                              int intervalSeconds,
                                                                              Func<ITunerInfo, TunerMonitoringResult, CancellationToken, Task> postQueryAction,
                                                                              CancellationToken cancellationToken)
            => await tuner.Monitor(new TunerMonitoringConfiguration()
            {
                Interval = TimeSpan.FromSeconds(intervalSeconds),
            }, postQueryAction, cancellationToken);

        /// <summary>
        /// Starts a monitoring session for the specified tuner.
        /// </summary>
        /// <param name="tuner"></param>
        /// <param name="configuration"></param>
        /// <param name="postQueryAction"></param>
        /// <returns></returns>
        public static async Task<TunerMonitoringResult> Monitor(this ITunerInfo tuner,
                                                                IMonitoringConfiguration configuration,
                                                                Func<ITunerInfo, TunerMonitoringResult, CancellationToken, Task> postQueryAction)
            => await tuner.Monitor(configuration, postQueryAction, CancellationToken.None);

        /// <summary>
        /// Starts a monitoring session for the specified tuner.
        /// </summary>
        /// <param name="tunerInfo"></param>
        /// <param name="configuration"></param>
        /// <param name="postQueryAction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<TunerMonitoringResult> Monitor(this ITunerInfo tunerInfo,
                                                                IMonitoringConfiguration configuration,
                                                                Func<ITunerInfo, TunerMonitoringResult, CancellationToken, Task> postQueryAction,
                                                                CancellationToken cancellationToken)
        {
            TunerMonitoringResult monitoringResult = new(configuration);

            DateTime monitorStartTime = DateTime.UtcNow;
            DateTime monitorEndTime = monitorStartTime.Add(configuration.Duration);

            while (DateTime.UtcNow < monitorEndTime
                   && cancellationToken.IsCancellationRequested == false)
            {
                // Refresh the tuner info using HTTP.
                await tunerInfo.RefreshTunerInfoUsingHTTP(cancellationToken);

                // Store the recently updated tuner info.
                monitoringResult.Add(DateTime.UtcNow, tunerInfo);

                // Perform the post query action.
                await postQueryAction(tunerInfo, monitoringResult, cancellationToken);

                // Wait for the polling interval.
                await Task.Delay(configuration.Interval, cancellationToken);
            }

            return monitoringResult;
        }

        /// <summary>
        /// Starts a monitoring session for the specified tuner.
        /// </summary>
        /// <param name="tunerInfo"></param>
        /// <param name="configuration"></param>
        /// <param name="tunerRefreshAction"></param>
        /// <param name="postQueryAction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<TunerMonitoringResult> Monitor(this ITunerInfo tunerInfo,
                                                                IMonitoringConfiguration configuration,
                                                                Func<ITunerInfo, CancellationToken, Task<ITunerInfo>> tunerRefreshAction,
                                                                Func<ITunerInfo, TunerMonitoringResult, CancellationToken, Task> postQueryAction,
                                                                CancellationToken cancellationToken)
        {
            TunerMonitoringResult monitoringResult = new(configuration);

            DateTime monitorStartTime = DateTime.UtcNow;
            DateTime monitorEndTime = monitorStartTime.Add(configuration.Duration);

            while (DateTime.UtcNow < monitorEndTime
                   && cancellationToken.IsCancellationRequested == false)
            {
                // Refresh the tuner info using specified action.
                await tunerInfo.RefreshTunerDataAsync(tunerRefreshAction, cancellationToken);

                // Store the recently updated tuner info.
                monitoringResult.Add(DateTime.UtcNow, tunerInfo);

                // Perform the post query action.
                await postQueryAction(tunerInfo, monitoringResult, cancellationToken);

                // Wait for the polling interval.
                await Task.Delay(configuration.Interval, cancellationToken);
            }

            return monitoringResult;
        }
    }
}
