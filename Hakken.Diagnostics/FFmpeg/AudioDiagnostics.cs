using Hakken.Diagnostics.Model;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MediaFileQualityAnalyzer.Utilities.FFMpeg.Audio
{
    public static partial class AudioDiagnostics
    {
        private const string AUDIO_STATS_COMMAND = "-v info -i {0} -t {1} -af astats -f null -";

        private const string DEFAULT_FFMPEG_PATH = "ffmpeg";

        private const string PARSED_ASTATS = "Parsed_astats";

        /// <summary>
        /// Attempts to extract <see cref="AudioStats"/> using the FFMpeg ASTATS filter.
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="durationSeconds"></param>
        /// <param name="ffmpegPath"></param>
        /// <returns></returns>
        public static async Task<AudioStats?> ExtractAudioStatsUsingASTATS(Uri inputStream, int durationSeconds = 1, string ffmpegPath = DEFAULT_FFMPEG_PATH)
            => await ExtractAudioStatsUsingASTATS(inputStream.AbsoluteUri, durationSeconds, ffmpegPath);

        /// <summary>
        /// Attempts to extract <see cref="AudioStats"/> using the FFMpeg ASTATS filter.
        /// </summary>
        /// <param name="mediaFile"></param>
        /// <param name="ffmpegPath"></param>
        /// <returns></returns>
        public static async Task<AudioStats?> ExtractAudioStatsUsingASTATS(string mediaFile, int durationSeconds = 1, string ffmpegPath = DEFAULT_FFMPEG_PATH)
        {
            AudioStats? retVal = null;

            try
            {
                using (Process process = new()
                {
                    StartInfo = new ProcessStartInfo(ffmpegPath, string.Format(AUDIO_STATS_COMMAND, mediaFile, durationSeconds))
                    {
                        UseShellExecute = false,
                        RedirectStandardError = true
                    }
                })
                {
                    process.Start();

                    string output = process.StandardError.ReadToEnd();

                    var exitTask = process.WaitForExitAsync();
                    var delayTask = Task.Delay(TimeSpan.FromSeconds(10));
                    var completedTask = await Task.WhenAny(exitTask, delayTask);

                    if (completedTask == exitTask)
                    {
                        retVal = new();

                        if (!string.IsNullOrEmpty(output))
                        {
                            retVal = new AudioStats();

                            AudioStats.Channel curChan = new();

                            // Get our astat output lines from ffmpeg output.
                            List<string> astatsOutput = output.Split(new string[] { Environment.NewLine, "\r" }, StringSplitOptions.None)
                                .Where(x => x.Contains(PARSED_ASTATS, StringComparison.CurrentCultureIgnoreCase))
                                .ToList();

                            if (astatsOutput.Any())
                            {
                                foreach (string line in astatsOutput)
                                {
                                    switch (line)
                                    {
                                        case string x when x.Contains("Overall"):
                                            {
                                                if (curChan is not null)
                                                {
                                                    retVal.Channels.Add(curChan);
                                                }

                                                curChan = new AudioStats.Channel();
                                                retVal.Overall = curChan;
                                            }
                                            break;
                                        case string x when x.Contains("Channel:"):
                                            {
                                                if (curChan.ChannelID > 0)
                                                {
                                                    retVal.Channels.Add(curChan);
                                                }

                                                curChan = new AudioStats.Channel()
                                                {
                                                    ChannelID = retVal.Channels.Count + 1
                                                };
                                            }
                                            break;
                                        case string x when x.Contains("DC offset:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = DCOffsetRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    curChan.DCOffset = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("Min level:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = MinLevelRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    curChan.MinLevel = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("Max level:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = MaxLevelRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    curChan.MaxLevel = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("Min difference:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = MinDifferenceRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    curChan.MinDifference = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("Max difference:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = MaxDifferenceRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    curChan.MaximumDifference = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("Mean difference:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = MeanDifferenceRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    curChan.MeanDifference = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("RMS difference:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = RMSDifferenceRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    curChan.RMSDifference = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("Peak level dB:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = PeakLevelDbRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    if (matches[0].Groups[2].Value.Trim() != "-inf")
                                                        curChan.PeakLeveldB = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("RMS level dB:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = RMSLevelDbRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    if (matches[0].Groups[2].Value.Trim() != "-inf")
                                                        curChan.RMSLeveldB = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("RMS peak dB:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = RMSPeakDbRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    if (matches[0].Groups[2].Value.Trim() != "-inf")
                                                        curChan.RMSPeakdB = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("RMS trough dB:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = RMSTroughDbRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    if (matches[0].Groups[2].Value.Trim() != "-inf")
                                                        curChan.RMSTroughdB = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("Crest factor:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = CrestFactorRegex();

                                                    MatchCollection matches = startRegex.Matches(x);


                                                    curChan.CrestFactor = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("Flat factor:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = FlatFactorRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    if (matches[0].Groups[2].Value.Trim() != "-inf")
                                                        curChan.FlatFactor = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("Peak count:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = PeakCountRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    curChan.PeakCount = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("Noise floor dB:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = NoiseFloorDbRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    if (matches[0].Groups[2].Value.Trim() != "-inf")
                                                        curChan.NoiseFloordB = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("Noise floor count:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = NoiseFloorCountRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    curChan.NoiseFloorCount = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("Entropy:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = EntropyRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    curChan.Entropy = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("Bit depth:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = BitDepthRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    curChan.Bitdepth = matches[0].Groups[2].Value.Trim();
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("Dynamic range:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = DynamicRangeRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    if (matches[0].Groups[2].Value.Trim() != "-inf")
                                                        curChan.DynamicRange = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("Zero crossings:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = ZeroCrossingsRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    curChan.ZeroCrossings = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("Zero crossings rate:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = ZeroCrossingsRateRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    curChan.ZeroCrossingsRate = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("Number of NaNs:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = NumberOfNaNsRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    curChan.NumberOfNaNs = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("Number of Infs:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = NumberOfInfsRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    curChan.NumberOfInfs = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                        case string x when x.Contains("Number of denormals:"):
                                            {
                                                try
                                                {
                                                    Regex startRegex = NumberOfDenormalsRegex();

                                                    MatchCollection matches = startRegex.Matches(x);

                                                    curChan.NumberOfDenormals = double.Parse(matches[0].Groups[2].Value.Trim());
                                                }
                                                catch { }
                                            }
                                            break;
                                    }
                                }

                                if (curChan is not null)
                                {
                                    retVal.Overall = curChan;
                                }

                                retVal.SuccessfullyParsed = true;
                            }
                        }
                    }
                };
            }
            catch (Exception) { }

            return retVal;
        }

        [GeneratedRegex("(DC offset: )(.*)")]
        private static partial Regex DCOffsetRegex();
        [GeneratedRegex("(Min level: )(.*)")]
        private static partial Regex MinLevelRegex();
        [GeneratedRegex("(Max level: )(.*)")]
        private static partial Regex MaxLevelRegex();
        [GeneratedRegex("(Min difference: )(.*)")]
        private static partial Regex MinDifferenceRegex();
        [GeneratedRegex("(Max difference: )(.*)")]
        private static partial Regex MaxDifferenceRegex();
        [GeneratedRegex("(Mean difference: )(.*)")]
        private static partial Regex MeanDifferenceRegex();
        [GeneratedRegex("(RMS difference: )(.*)")]
        private static partial Regex RMSDifferenceRegex();
        [GeneratedRegex("(Peak level dB: )(.*)")]
        private static partial Regex PeakLevelDbRegex();
        [GeneratedRegex("(RMS level dB: )(.*)")]
        private static partial Regex RMSLevelDbRegex();
        [GeneratedRegex("(RMS peak dB: )(.*)")]
        private static partial Regex RMSPeakDbRegex();
        [GeneratedRegex("(RMS trough dB: )(.*)")]
        private static partial Regex RMSTroughDbRegex();
        [GeneratedRegex("(Crest factor: )(.*)")]
        private static partial Regex CrestFactorRegex();
        [GeneratedRegex("(Flat factor: )(.*)")]
        private static partial Regex FlatFactorRegex();
        [GeneratedRegex("(Peak count: )(.*)")]
        private static partial Regex PeakCountRegex();
        [GeneratedRegex("(Noise floor dB: )(.*)")]
        private static partial Regex NoiseFloorDbRegex();
        [GeneratedRegex("(Noise floor count: )(.*)")]
        private static partial Regex NoiseFloorCountRegex();
        [GeneratedRegex("(Entropy: )(.*)")]
        private static partial Regex EntropyRegex();
        [GeneratedRegex("(Bit depth: )(.*)")]
        private static partial Regex BitDepthRegex();
        [GeneratedRegex("(Dynamic range: )(.*)")]
        private static partial Regex DynamicRangeRegex();
        [GeneratedRegex("(Zero crossings: )(.*)")]
        private static partial Regex ZeroCrossingsRegex();
        [GeneratedRegex("(Zero crossings rate: )(.*)")]
        private static partial Regex ZeroCrossingsRateRegex();
        [GeneratedRegex("(Number of NaNs: )(.*)")]
        private static partial Regex NumberOfNaNsRegex();
        [GeneratedRegex("(Number of Infs: )(.*)")]
        private static partial Regex NumberOfInfsRegex();
        [GeneratedRegex("(Number of denormals: )(.*)")]
        private static partial Regex NumberOfDenormalsRegex();
    }
}