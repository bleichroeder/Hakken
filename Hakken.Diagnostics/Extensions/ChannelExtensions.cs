using Hakken.Channel.Model;
using Hakken.Diagnostics.FFprobe;
using Hakken.Diagnostics.Model;
using MediaFileQualityAnalyzer.Utilities.FFMpeg.Audio;

namespace Hakken.Diagnostics.Extensions
{
    /// <summary>
    /// Provides extensions for performing diagnostics on channels.
    /// </summary>
    public static class ChannelExtensions
    {
        private const string DEFAULT_FFMPEG_PATH = "ffmpeg";

        private const string DEFAULT_FFPROBE_PATH = "ffprobe";

        /// <summary>
        /// Attempts to extract a thumbnail and returns it as a <see cref="Stream"/>.
        /// </summary>
        /// <param name="channelInfo"></param>
        /// <returns></returns>
        public static async Task<Stream?> ExtractImage(this ChannelInfo channelInfo)
            => await ExtractImage(channelInfo, 0, 0, DEFAULT_FFMPEG_PATH);

        /// <summary>
        /// Attempts to extract a thumbnail and returns it as a <see cref="Stream"/>.
        /// </summary>
        /// <param name="channelInfo"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static async Task<Stream?> ExtractImage(this ChannelInfo channelInfo, int width, int height)
            => await ExtractImage(channelInfo, width, height, DEFAULT_FFMPEG_PATH);

        /// <summary>
        /// Attempts to extract a thumbnail and returns it as a <see cref="Stream"/>.
        /// </summary>
        /// <param name="channelInfo"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="ffmpegPath"></param>
        /// <returns></returns>
        public static async Task<Stream?> ExtractImage(this ChannelInfo channelInfo, int width, int height, string ffmpegPath = DEFAULT_FFMPEG_PATH)
            => await VideoDiagnostics.ExtractImageAsStream(channelInfo.StreamingUri, width, height, ffmpegPath);

        /// <summary>
        /// Attempts to extract <see cref="MediaProperties"/> from the specified channel.
        /// </summary>
        /// <param name="channelInfo"></param>
        /// <returns></returns>
        public static async Task<MediaProperties> ExtractMediaProperties(this ChannelInfo channelInfo)
            => await ExtractMediaProperties(channelInfo, 1, DEFAULT_FFPROBE_PATH);

        /// <summary>
        /// Attempts to extract <see cref="MediaProperties"/> from the specified channel.
        /// </summary>
        /// <param name="channelInfo"></param>
        /// <param name="ffprobePath"></param>
        /// <returns></returns>
        public static async Task<MediaProperties> ExtractMediaProperties(this ChannelInfo channelInfo, int durationSeconds = 1, string ffprobePath = DEFAULT_FFPROBE_PATH)
            => await MediaDiagnostics.ExtractMediaPropertiesAsync(channelInfo.StreamingUri, durationSeconds, ffprobePath);

        /// <summary>
        /// Attempts to detect frozen frames in the specified channel.
        /// </summary>
        /// <param name="channelInfo"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<FrozenFrames>> DetectFrozenFrames(this ChannelInfo channelInfo)
            => await DetectFrozenFrames(channelInfo, 30, 0.003, DEFAULT_FFMPEG_PATH);

        /// <summary>
        /// Attempts to detect frozen frames in the specified channel.
        /// </summary>
        /// <param name="channelInfo"></param>
        /// <param name="durationSeconds"></param>
        /// <param name="noiseTolerance"></param>
        /// <param name="ffmpegPath"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<FrozenFrames>> DetectFrozenFrames(this ChannelInfo channelInfo, int durationSeconds, double noiseTolerance, string ffmpegPath)
            => await VideoDiagnostics.ExtractFrozenFrameData(channelInfo.StreamingUri, durationSeconds, noiseTolerance, ffmpegPath);

        /// <summary>
        /// Attempts to extract audio stats from the specified channel.
        /// </summary>
        /// <param name="channelInfo"></param>
        /// <returns></returns>
        public static async Task<AudioStats?> GetAudioStats(this ChannelInfo channelInfo)
            => await GetAudioStats(channelInfo, 1, DEFAULT_FFMPEG_PATH);

        /// <summary>
        /// Attemtps to extract audio stats from the specified channel.
        /// </summary>
        /// <param name="channelInfo"></param>
        /// <param name="durationSeconds"></param>
        /// <param name="ffmpegPath"></param>
        /// <returns></returns>
        public static async Task<AudioStats?> GetAudioStats(this ChannelInfo channelInfo, int durationSeconds, string ffmpegPath)
            => await AudioDiagnostics.ExtractAudioStatsUsingASTATS(channelInfo.StreamingUri, durationSeconds, ffmpegPath);
    }
}
