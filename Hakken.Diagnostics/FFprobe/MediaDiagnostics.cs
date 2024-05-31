using Hakken.Diagnostics.Model;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;

namespace Hakken.Diagnostics.FFprobe
{
    /// <summary>
    /// Provides methods for performing media diagnostics using FFprobe.
    /// </summary>
    public static class MediaDiagnostics
    {
        private const string FFPROBE_COMMAND = "-i {0} -loglevel warning -show_streams -print_format json -show_format -find_stream_info -sexagesimal -print_format json";

        /// <summary>
        /// Attempts to extract <see cref="MediaProperties"/> from the specified file path.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="ffprobePath"></param>
        /// <returns></returns>
        public static MediaProperties ExtractMediaProperties(string filePath, int durationSeconds, string ffprobePath) => ExtractMediaProperties(new Uri(filePath), durationSeconds, ffprobePath);

        /// <summary>
        /// Attempts to extract <see cref="MediaProperties"/> using the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="mediaUri"></param>
        /// <param name="ffprobePath"></param>
        /// <returns></returns>
        public static MediaProperties ExtractMediaProperties(Uri mediaUri, int durationSeconds, string ffprobePath) => ExtractMediaPropertiesAsync(mediaUri, durationSeconds, ffprobePath).Result;

        /// <summary>
        /// Attempts to extract <see cref="MediaProperties"/> from the specified file path asynchronously.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="ffprobePath"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<MediaProperties> ExtractMediaPropertiesAsync(string filePath, int durationSeconds, string ffprobePath) => await ExtractMediaPropertiesAsync(new Uri(filePath), durationSeconds, ffprobePath);

        /// <summary>
        /// Attempts to extract <see cref="MediaProperties"/> using the specified <see cref="Uri"/> asynchronously.
        /// </summary>
        /// <param name="mediaUri"></param>
        /// <param name="ffprobePath"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<MediaProperties> ExtractMediaPropertiesAsync(Uri mediaUri, int durationSeconds, string ffprobePath)
        {
            string filePath = mediaUri.Scheme.StartsWith("http", StringComparison.CurrentCulture) ?
                              mediaUri.AbsoluteUri :
                              mediaUri.AbsolutePath;

            MediaProperties? retVal = new(filePath);

            try
            {
                using (Process process = new()
                {
                    StartInfo = new ProcessStartInfo(ffprobePath, string.Format(FFPROBE_COMMAND, filePath))
                    {
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                    }
                })
                {
                    process.Start();

                    var exitTask = process.WaitForExitAsync();
                    var delayTask = Task.Delay(TimeSpan.FromSeconds(durationSeconds));
                    var completedTask = await Task.WhenAny(exitTask, delayTask);

                    string stdOut = await process.StandardOutput.ReadToEndAsync();

                    string stdErr = await process.StandardError.ReadToEndAsync();

                    // If there's any error output, we'll pass this into be deserialized instead.
                    if (!string.IsNullOrEmpty(stdErr)
                    && (stdErr.Contains("Failed to resolve hostname")
                    || stdErr.Contains("400 Bad Request")
                    || stdErr.Contains("404 Not Found")
                    || stdErr.Contains("No such file or directory")
                    || stdErr.Contains("5XX Server Error")))
                    {
                        stdOut = stdErr;
                    }

                    try
                    {
                        retVal = JsonConvert.DeserializeObject<MediaProperties>(stdOut);
                    }
                    catch (Exception ex)
                    {
                        if (retVal is not null)
                        {
                            if (stdOut.Contains("Failed to resolve hostname", StringComparison.CurrentCultureIgnoreCase))
                            {
                                // Could be that the stream server is down or unreachable for some reason.
                                retVal.Exception = new HttpRequestException(stdOut, null, HttpStatusCode.NotFound);
                            }
                            else if (stdOut.Contains("400 Bad Request", StringComparison.CurrentCultureIgnoreCase))
                            {
                                // Could be that this station isn't available from the stream server, or malformed request.
                                retVal.Exception = new HttpRequestException(stdOut, null, HttpStatusCode.BadRequest);
                            }
                            else if (stdOut.Contains("No such file or directory", StringComparison.CurrentCultureIgnoreCase))
                            {
                                // File on disk doesn't exist.
                                retVal.Exception = new FileNotFoundException(stdOut, filePath);
                            }
                            else if (stdOut.Contains("404 Not Found", StringComparison.CurrentCultureIgnoreCase))
                            {
                                // Perhaps the media server value was configured incorrectly?
                                retVal.Exception = new HttpRequestException(stdOut, null, HttpStatusCode.NotFound);
                            }
                            else if (stdOut.Contains("5XX Server Error", StringComparison.CurrentCultureIgnoreCase))
                            {
                                // Some internal server error IE 5XX reply.
                                // This could mean we're not running StreamStack...
                                retVal.Exception = new HttpRequestException(stdOut, null, HttpStatusCode.InternalServerError);
                            }
                            else
                            {
                                retVal.Exception = ex;
                            }
                        }
                    }
                };
            }
            catch (OperationCanceledException oex)
            {
                return new MediaProperties(filePath)
                {
                    Exception = oex
                };
            }
            catch (Exception ex)
            {
                return new MediaProperties(filePath)
                {
                    Exception = ex
                };
            }

            return retVal ?? new MediaProperties(filePath);
        }
    }
}
