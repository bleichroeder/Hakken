using Hakken.Capture.Model;
using Hakken.Capture.Model.Request;
using Hakken.Capture.Model.Result;
using Hakken.Channel.Model;
using Hakken.Tuner.Model;

namespace Hakken.Capture.Extensions
{
    /// <summary>
    /// Provides capture extension methods for <see cref="LocalTunerInfo"/>.
    /// </summary>
    public static class TunerExtensions
    {
        /// <summary>
        /// Captures the specified channel using the specified tuner.
        /// </summary>
        /// <param name="tunerInfo"></param>
        /// <param name="channel"></param>
        /// <param name="captureRequest"></param>
        /// <returns></returns>
        public static async Task<HttpCaptureResult> Capture(this ITunerInfo tunerInfo,
                                                                 ChannelInfo channel,
                                                                 HttpCaptureRequest captureRequest)
        {
            captureRequest.TunerNumber = tunerInfo.TunerNumber;
            captureRequest.InputStreamUri = channel.StreamingUri;

            return await CaptureUtility.Capture(captureRequest);
        }

        /// <summary>
        /// Captures the specified channel on the first available tuner.
        /// If no duration or transcode profile is specified, the default values will be used.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="outputFileName"></param>
        /// <returns></returns>
        public static async Task<HttpCaptureResult> Capture(this ITunerInfo tunerInfo,
                                                                 ChannelInfo channel,
                                                                 string outputFileName,
                                                                 TranscodeProfile transcodeProfile = TranscodeProfile.None,
                                                                 TimeSpan? duration = null)
        {
            string fileName = Path.GetFileName(outputFileName);
            string filePath = outputFileName.Replace(fileName, string.Empty);

            HttpCaptureRequest request = new(channel, filePath, fileName)
            {
                TranscodeProfile = transcodeProfile,
                Duration = duration ?? TimeSpan.MaxValue,
                TunerNumber = tunerInfo.TunerNumber
            };

            return await CaptureUtility.Capture(request);
        }
    }
}
