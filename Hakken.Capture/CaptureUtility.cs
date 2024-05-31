using Hakken.Capture.Model.Request;
using Hakken.Capture.Model.Result;

namespace Hakken.Capture
{
    /// <summary>
    /// Provides methods for capturing a stream.
    /// </summary>
    public static class CaptureUtility
    {
        /// <summary>
        /// HTTP client used to capture the stream.
        /// </summary>
        private static readonly HttpClient _httpClient = new();

        /// <summary>
        /// The header name for the X-HDHomeRun-Error header.
        /// </summary>
        private const string ERROR_HEADER = "X-HDHomeRun-Error";

        /// <summary>
        /// Captures a stream.
        /// </summary>
        /// <param name="captureRequest"></param>
        /// <returns></returns>
        public static async Task<HttpCaptureResult> Capture(HttpCaptureRequest captureRequest)
        {
            HttpCaptureResult result = new();

            using var response = await _httpClient.GetAsync(captureRequest.ParameterizedInputStreamUri,
                                                            HttpCompletionOption.ResponseHeadersRead);

            if (response.Headers.TryGetValues(ERROR_HEADER, out var headerValues))
            {
                string? xHDHomeRunErrorHeader = headerValues.FirstOrDefault();

                result.ErrorHeader = ExtractHDHomeRunErrorHeader(xHDHomeRunErrorHeader);
            }

            result.ResponseStatusCode = response.StatusCode;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                using var stream = await response.Content.ReadAsStreamAsync();
                using var fileStream = File.Create(captureRequest.OutputFileFullPath);

                var buffer = new byte[81920];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer)) > 0)
                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));

                result.OutputFullFilePath = captureRequest.OutputFileFullPath;
            }

            return result;
        }

        /// <summary>
        /// Performs a HEAD request to the specified URI and returns the X-HDHomeRun-Error header value.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static async Task<int> GetHDHomeRunErrorHeaderValueAsync(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Head, uri);

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                if (response.Headers.TryGetValues(ERROR_HEADER, out var headerValues))
                {
                    string? xHDHomeRunErrorHeader = headerValues.FirstOrDefault();

                    if (!string.IsNullOrEmpty(xHDHomeRunErrorHeader))
                    {
                        return ExtractHDHomeRunErrorHeader(xHDHomeRunErrorHeader);
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Parses the X-HDHomeRun-Error header value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int ExtractHDHomeRunErrorHeader(string? value)
        {
            if (int.TryParse(value?.Split(" ")[0], out int errorHeaderValue))
            {
                return errorHeaderValue;
            }

            return 0;
        }
    }
}
