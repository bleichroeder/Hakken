using Hakken.Capture.Model;
using Hakken.Capture.Model.Result;
using System.Net;

namespace Hakken.Capture.FFmpeg.Model
{
    /// <summary>
    /// The result of a capture operation.
    /// </summary>
    public class FFmpegCaptureResult : ICaptureResult
    {
        /// <summary>
        /// The capture response status code.
        /// </summary>
        public HttpStatusCode ResponseStatusCode { get; set; }

        /// <summary>
        /// The capture response definition.
        /// </summary>
        public ResponseDefinition ResponseDefinition
            => ErrorHeader != 0 ? (ResponseDefinition)(int)ErrorHeader : ResponseDefinition.Success;

        /// <summary>
        /// The path to the resulting capture output file.
        /// </summary>
        public string? OutputFullFilePath { get; set; }

        /// <summary>
        /// The error header, if any.
        /// </summary>
        public int ErrorHeader { get; set; }

        /// <summary>
        /// True if the capture was successful.
        /// </summary>
        public bool Success => ResponseDefinition == ResponseDefinition.Success &&
                                                     ProcessResult.Success &&
                                                     File.Exists(OutputFullFilePath);

        /// <summary>
        /// The process result.
        /// </summary>
        public FFmpegProcessResult ProcessResult { get; set; } = new FFmpegProcessResult();
    }
}
