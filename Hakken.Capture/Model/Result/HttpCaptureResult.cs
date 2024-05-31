using System.Net;

namespace Hakken.Capture.Model.Result
{
    /// <summary>
    /// The result of a capture operation.
    /// </summary>
    public class HttpCaptureResult : ICaptureResult
    {
        /// <summary>
        /// The capture response status code.
        /// </summary>
        public HttpStatusCode ResponseStatusCode { get; set; }

        /// <summary>
        /// The capture response definition.
        /// </summary>
        public ResponseDefinition ResponseDefinition
            => ErrorHeader != 0 ? (ResponseDefinition)ErrorHeader : ResponseDefinition.Success;

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
        public bool Success => ResponseStatusCode == HttpStatusCode.OK && File.Exists(OutputFullFilePath);
    }
}
