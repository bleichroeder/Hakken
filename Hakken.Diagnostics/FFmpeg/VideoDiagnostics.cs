using Hakken.Diagnostics.Model;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Hakken.Diagnostics
{
    public static partial class VideoDiagnostics
    {
        private const string FREEZE_DETECT_COMMAND = "-i {0} -t {1} -vf \"freezedetect=n={2}\" -map 0:v:0 -f null -";
        private const string EXTRACT_THUMBNAIL_COMMAND = "-i {0} -v quiet -vframes 1 {1} -vframes 1 -f mjpeg pipe:1";
        private const string DEFAULT_FFMPEG_PATH = "ffmpeg";

        /// <summary>
        /// Attempts to extract frozen frame data from the specified channel.
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        public static async Task<Stream?> ExtractImageAsStream(Uri inputStream)
            => await ExtractImageAsStream(inputStream, 0, 0);

        /// <summary>
        /// Attempts to extract a thumbnail and returns it as a <see cref="Stream"/>.
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static async Task<Stream?> ExtractImageAsStream(Uri inputStream, int width, int height)
            => await ExtractImageAsStream(inputStream, width, height, "ffmpeg");

        /// <summary>
        /// Attempts to extract a thumbnail and returns it as a <see cref="Stream"/>.
        /// </summary>
        /// <param name="ffmpegPath"></param>
        /// <param name="mediaFile"></param>
        /// <param name="seek"></param>
        /// <returns></returns>
        public static async Task<Stream?> ExtractImageAsStream(Uri inputStream, int width, int height, string ffmpegPath)
        {
            byte[]? thumbnailBytes = await ExtractImageAsByteArray(inputStream, width, height, ffmpegPath);
            return thumbnailBytes is not null ? new MemoryStream(thumbnailBytes) : null;
        }

        /// <summary>
        /// Attempts to extract a thumbnail and returns it as a byte array.
        /// </summary>
        /// <param name="ffmpegPath"></param>
        /// <param name="mediaFile"></param>
        /// <param name="seek"></param>
        /// <returns></returns>
        public static async Task<byte[]?> ExtractImageAsByteArray(Uri inputStream, int width = 0, int height = 0, string ffmpegPath = DEFAULT_FFMPEG_PATH)
        {
            string resFilter = height != 0 && width != 0 ?
                               $"-vf scale={width}:{height}" :
                               string.Empty;

            string arguments = string.Format(EXTRACT_THUMBNAIL_COMMAND, inputStream.AbsoluteUri, resFilter);

            try
            {
                using (Process process = new()
                {
                    StartInfo = new ProcessStartInfo(ffmpegPath, arguments)
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                    }
                })
                {
                    process.Start();

                    byte[] buffer = new byte[16 * 1024];
                    using MemoryStream ms = new();
                    int read;
                    Stream pipe = process.StandardOutput.BaseStream;
                    while ((read = process.StandardOutput.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                        ms.Write(buffer, 0, read);

                    var exitTask = process.WaitForExitAsync();
                    var delayTask = Task.Delay(TimeSpan.FromSeconds(10));
                    var completedTask = await Task.WhenAny(exitTask, delayTask);

                    return ms.ToArray();
                };
            }
            catch (Exception) { }

            return null;
        }

        /// <summary>
        /// Extracts frozen frame data from FFMpeg standard error.
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="durationSeconds"></param>
        /// <param name="noiseTolerance"></param>
        /// <param name="ffmpegPath"></param>
        /// <returns></returns>
        public static async Task<List<FrozenFrames>> ExtractFrozenFrameData(Uri inputStream, int durationSeconds = 30, double noiseTolerance = 0.003, string ffmpegPath = DEFAULT_FFMPEG_PATH)
            => await ExtractFrozenFrameData(inputStream.AbsoluteUri, durationSeconds, noiseTolerance, ffmpegPath);

        /// <summary>
        /// Extracts frozen frame data from FFMpeg standard error.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="noiseTolerance"></param>
        public static async Task<List<FrozenFrames>> ExtractFrozenFrameData(string mediaFile, int durationSeconds = 30, double noiseTolerance = 0.003, string ffmpegPath = DEFAULT_FFMPEG_PATH)
        {
            List<FrozenFrames> retVal = new();

            try
            {
                using (Process process = new()
                {
                    StartInfo = new ProcessStartInfo(ffmpegPath, string.Format(FREEZE_DETECT_COMMAND, mediaFile, durationSeconds, noiseTolerance))
                    {
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                    }
                })
                {
                    process.Start();

                    string output = process.StandardError.ReadToEnd();

                    var exitTask = process.WaitForExitAsync();
                    var delayTask = Task.Delay(TimeSpan.FromSeconds(30));
                    var completedTask = await Task.WhenAny(exitTask, delayTask);

                    if (completedTask == exitTask)
                    {
                        List<string> freezeLines = output
                            .Split(new string[] { Environment.NewLine, "\r" }, StringSplitOptions.None)
                            .Where(x => x.Contains("freezedetect"))
                            .ToList();

                        if (freezeLines.Any())
                        {
                            FrozenFrames curFrozenFrame = new()
                            {
                                NoiseTolerance = noiseTolerance
                            };

                            foreach (string freezeLine in freezeLines)
                            {
                                if (freezeLine.Contains("freeze_start"))
                                {
                                    Regex startRegex = FreezeStartRegex();

                                    MatchCollection matches = startRegex.Matches(freezeLine);

                                    if (curFrozenFrame.Start != TimeSpan.Zero)
                                    {
                                        retVal.Add(curFrozenFrame);

                                        curFrozenFrame = new FrozenFrames()
                                        {
                                            NoiseTolerance = noiseTolerance
                                        };
                                    }

                                    curFrozenFrame.Start = TimeSpan.FromSeconds(Convert.ToDouble(matches[0].Groups[2].Value));
                                }
                                else if (freezeLine.Contains("freeze_end"))
                                {
                                    Regex startRegex = FreezeEndRegex();

                                    MatchCollection matches = startRegex.Matches(freezeLine);

                                    curFrozenFrame.End = TimeSpan.FromSeconds(Convert.ToDouble(matches[0].Groups[2].Value));
                                }
                                else if (freezeLine.Contains("freeze_duration"))
                                {
                                    Regex startRegex = FreezeDurationRegex();

                                    MatchCollection matches = startRegex.Matches(freezeLine);

                                    curFrozenFrame.Duration = TimeSpan.FromSeconds(Convert.ToDouble(matches[0].Groups[2].Value));
                                }
                            }

                            if (curFrozenFrame is not null && !retVal.Contains(curFrozenFrame))
                            {
                                retVal.Add(curFrozenFrame);
                            }
                        }
                    }
                };
            }
            catch (Exception) { }

            return retVal;
        }

        [GeneratedRegex("(freeze_start: )([0-9]?[0-9]?[0-9]?\\.?[0-9]?[0-9]?[0-9][0-9]?[0-9]?[0-9]?)")]
        private static partial Regex FreezeStartRegex();
        [GeneratedRegex("(freeze_end: )([0-9]?[0-9]?[0-9]?\\.?[0-9]?[0-9]?[0-9][0-9]?[0-9]?[0-9]?)")]
        private static partial Regex FreezeEndRegex();
        [GeneratedRegex("(freeze_duration: )([0-9]?[0-9]?[0-9]?\\.?[0-9]?[0-9]?[0-9][0-9]?[0-9]?[0-9]?)")]
        private static partial Regex FreezeDurationRegex();
    }
}
