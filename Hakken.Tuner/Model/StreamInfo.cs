using System.Text.RegularExpressions;

namespace Hakken.Tuner.Model
{
    /// <summary>
    /// Stream information for a tuner.
    /// </summary>
    public partial class StreamInfo
    {
        /// <summary>
        /// A list of detected and parsed ChannelDetail.
        /// </summary>
        public List<ChannelDetail> DetectedChannels { get; set; } = new();

        /// <summary>
        /// The parsed frequency.
        /// </summary>
        public double Frequency { get; set; }

        /// <summary>
        /// The parsed TSID.
        /// </summary>
        public int TSID { get; set; }

        /// <summary>
        /// The parsed ONID.
        /// </summary>
        public int ONID { get; set; }

        /// <summary>
        /// Attempts to parse 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static StreamInfo FromHDHomeRunConfigOutput(string content)
        {
            StreamInfo info = new();
            string[] lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (line.StartsWith("TSID"))
                {
                    info.TSID = Convert.ToInt32(line.Split('=')[1], 16);
                }
                else if (line.StartsWith("ONID"))
                {
                    info.ONID = Convert.ToInt32(line.Split('=')[1], 16);
                }
                else if (line.Contains(':'))
                {
                    string prefix = line.Split(':')[0].Trim();
                    if (int.TryParse(prefix, out _))
                    {
                        ParseChannel(line, info);
                    }
                    else
                    {
                        ParseFrequency(line, info);
                    }
                }
            }

            return info;
        }

        /// <summary>
        /// Parses the frequency from the line.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="info"></param>
        private static void ParseFrequency(string line, StreamInfo info)
        {
            Regex regex = FrequencyRegex();
            Match match = regex.Match(line);

            if (match.Success && double.TryParse(match.Groups[1].Value, out double frequency))
            {
                info.Frequency = frequency / 1000000;
            }
        }

        /// <summary>
        /// Parses the channel from the line.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="info"></param>
        private static void ParseChannel(string line, StreamInfo info)
        {
            Regex regex = ChannelRegex();
            Match match = regex.Match(line);

            if (match.Success)
            {
                ChannelDetail channelDetail = new()
                {
                    GuideNumber = double.Parse(match.Groups[2].Value),
                    GuideName = match.Groups[3].Value,
                    Encrypted = !string.IsNullOrEmpty(match.Groups[4].Value)
                };

                string[] guideSplit = match.Groups[3].Value.Split(' ');
                if (guideSplit.Length >= 2)
                {
                    channelDetail.IsHD = guideSplit[1].Equals("HD", StringComparison.InvariantCultureIgnoreCase);
                    channelDetail.GuideName = channelDetail.GuideName.Replace(" HD", string.Empty);
                }

                info.DetectedChannels.Add(channelDetail);
            }
        }

        [GeneratedRegex("\\w+:(\\d+)")]
        private static partial Regex FrequencyRegex();
        [GeneratedRegex("(\\d+):\\s+(\\d+)\\s+(.+?)(?:\\s+\\((ENCRYPTED)\\))?$")]
        private static partial Regex ChannelRegex();
    }
}
