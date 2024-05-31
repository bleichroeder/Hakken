using Hakken.Channel.Model;
using System.Text.RegularExpressions;

namespace Hakken.Tuner.Model
{
    /// <summary>
    /// Stores the parsed channel information from the HDHomeRunConfig output
    /// when requesting the stream info for a tuner.
    /// </summary>
    public class ChannelDetail
    {
        /// <summary>
        /// The parsed guide number.
        /// Also sometimes referenced as virtual channel number.
        /// </summary>
        public double GuideNumber { get; set; }

        /// <summary>
        /// The parsed guide name.
        /// If the guide name indicates the channel is HD, IsHD will be set to true
        /// and the guide name will be stripped of the HD indicator.
        /// </summary>
        public string GuideName { get; set; } = string.Empty;

        /// <summary>
        /// The NetworkAffiliation if it was able to be determined.
        /// </summary>
        public NetworkAffiliation NetworkAffiliation => ChannelInfo.ParseNetworkAffiliation(GuideName);

        /// <summary>
        /// True if the channel is encrypted.
        /// </summary>
        public bool Encrypted { get; set; }

        /// <summary>
        /// True if the channel is HD.
        /// </summary>
        public bool IsHD { get; set; }

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
    }
}
