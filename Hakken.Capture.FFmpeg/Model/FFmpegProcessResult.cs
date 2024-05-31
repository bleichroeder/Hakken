namespace Hakken.Capture.FFmpeg.Model
{
    /// <summary>
    /// The result of a capture operation using FFmpeg.
    /// </summary>
    public class FFmpegProcessResult
    {
        /// <summary>
        /// The error code, if any.
        /// </summary>
        public int ExitCode { get; set; } = -1;

        /// <summary>
        /// True if the capture was successful.
        /// </summary>
        public bool Success => ExitCode == 0;

        /// <summary>
        /// The command that was executed.
        /// </summary>
        public string Command { get; set; } = string.Empty;
    }
}
