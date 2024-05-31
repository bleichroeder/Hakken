using System.Text.RegularExpressions;

namespace Hakken.Channel.Model
{
    /// <summary>
    /// Channel object
    /// </summary>
    public class ChannelInfo
    {
        /// <summary>
        /// Channel guide name.
        /// </summary>
        public string GuideName { get; set; } = string.Empty;

        /// <summary>
        /// Channel guide number.
        /// </summary>
        public double GuideNumber { get; set; }

        /// <summary>
        /// Channel video codec.
        /// </summary>
        public string VideoCodec { get; set; } = string.Empty;

        /// <summary>
        /// Channel audio codec.
        /// </summary>
        public string AudioCodec { get; set; } = string.Empty;

        /// <summary>
        /// Channel is HD.
        /// </summary>
        public bool HD { get; set; }

        /// <summary>
        /// Channel is DRM protected.
        /// </summary>
        public bool DRM { get; set; }

        /// <summary>
        /// Channel streaming URL
        /// </summary>
        public string URL { get; set; } = string.Empty;

        /// <summary>
        /// Gets the URL as a Uri object.
        /// </summary>
        public Uri StreamingUri => new(URL);

        /// <summary>
        /// Gets the channel's network affiliation if it was able to be determined.
        /// </summary>
        public NetworkAffiliation NetworkAffiliation => ParseNetworkAffiliation(GuideName);

        /// <summary>
        /// Returns true if there is a match between the guide name and the call sign.
        /// </summary>
        /// <param name="callSign"></param>
        /// <returns></returns>
        public bool Matches(string callSign)
        {
            return Regex.IsMatch(GuideName, Regex.Escape(callSign), RegexOptions.IgnoreCase)
                || Regex.IsMatch(callSign, Regex.Escape(GuideName), RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Returns true if there is a match between the guide name and the regular expression.
        /// </summary>
        /// <param name="regularExpression"></param>
        /// <param name="regexOptions"></param>
        /// <returns></returns>
        public bool Matches(string regularExpression, RegexOptions regexOptions)
            => Regex.IsMatch(GuideName, regularExpression, regexOptions)
            || Regex.IsMatch(regularExpression, GuideName, regexOptions);

        /// <summary>
        /// Attempts to parse the network affiliation from the input string.
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static NetworkAffiliation ParseNetworkAffiliation(string? inputString)
        {
            NetworkAffiliation result = NetworkAffiliation.UNKNOWN;

            if (string.IsNullOrEmpty(inputString) == false)
            {
                string[] inputStringParts = inputString.Split(new char[] { '-', ' ' });

                if (inputStringParts.Any())
                {
                    for (int i = 0; i < inputStringParts.Length; i++)
                    {
                        string sanitized = inputStringParts[i].Trim()
                                                              .ToUpperInvariant()
                                                              .Replace("(", string.Empty)
                                                              .Replace(")", string.Empty)
                                                              .Replace("HD", string.Empty);

                        if (Enum.TryParse(sanitized, out NetworkAffiliation parsed) &&
                            Enum.IsDefined(typeof(NetworkAffiliation), parsed))
                        {
                            result = parsed;
                            break;
                        }
                    }
                }
            }

            return result;
        }
    }
}
