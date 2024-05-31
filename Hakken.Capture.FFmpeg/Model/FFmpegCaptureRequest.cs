using Hakken.Channel.Model;
using System.Text;
using System.Web;

namespace Hakken.Capture.Model.Request
{
    /// <summary>
    /// Represents a request to capture a video.
    /// </summary>
    public class FFmpegCaptureRequest : ICaptureRequest
    {
        private static readonly string DURATION_PARAM = "duration";
        private static readonly string TRANSCODE_PROFILE_PARAM = "transcode";
        private static readonly string TUNER_NUMBER_PARAM = "tuner";
        private static readonly string TUNER_AUTO_PARAM = "auto";

        /// <summary>
        /// Gets or sets the FFmpeg flag pairs.
        /// If setting, the dictionary will be validated.
        /// It's important to not that the order of the flags is important
        /// and is the order in which they will be applied to the FFmpeg command.
        /// </summary>
        public Dictionary<string, string> FFmpegFlagPairs
        {
            get => _ffmpegFlagPairs;
            set
            {
                BuildFFmpegFlagPairs(value);
            }
        }

        /// <summary>
        /// The tuner number to use for capture
        /// </summary>
        public int TunerNumber
        {
            get => _tunerNumber;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(TunerNumber), "Tuner number must be greater than or equal to zero.");
                _tunerNumber = value;
            }
        }

        /// <summary>
        /// Gets the input stream URI.
        /// </summary>
        public Uri InputStreamUri
        {
            get => _inputStreamUri;
            set => _inputStreamUri = value;
        }

        /// <summary>
        /// Gets the parameterized input stream URI.
        /// </summary>
        public Uri ParameterizedInputStreamUri => GetInputStreamUriWithParameters();

        /// <summary>
        /// The transcode profile for capture.
        /// Only supported on HDHomeRun EXTEND devices.
        /// </summary>
        public TranscodeProfile TranscodeProfile
        {
            get => _transcodeProfile;
            set => _transcodeProfile = value;
        }

        /// <summary>
        /// Gets the timespan duration of capture.
        /// </summary>
        public TimeSpan Duration
        {
            get => _duration;
            set => _duration = value;
        }

        /// <summary>
        /// Gets the output path.
        /// </summary>
        public string OutputPath => _outputPath;

        /// <summary>
        /// Gets the output file name.
        /// </summary>
        public string OutputFileName => _outputFileName;

        /// <summary>
        /// Gets the output file extension.
        /// </summary>
        public string OutputFileExtension => _outputFileExtension;

        /// <summary>
        /// Gets the FFmpeg command.
        /// </summary>
        public string FFmpegCommand => BuildFFmpegCommand();

        /// <summary>
        /// Gets the full output file path.
        /// </summary>
        public string OutputFileFullPath => Path.Combine(_outputPath, _outputFileName + _outputFileExtension);

        private TimeSpan _duration = TimeSpan.MaxValue;
        private Uri _inputStreamUri;
        private TranscodeProfile _transcodeProfile = TranscodeProfile.None;
        private int _tunerNumber = -1;
        private Dictionary<string, string> _ffmpegFlagPairs = new();

        private readonly string _outputFileName;
        private readonly string _outputPath;
        private readonly string _outputFileExtension;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpCaptureRequest"/> class.
        /// </summary>
        /// <param name="tunerNumber"></param>
        /// <param name="inputStreamUri"></param>
        /// <param name="outputPath"></param>
        /// <param name="fileName"></param>
        public FFmpegCaptureRequest(int tunerNumber,
                                    Uri inputStreamUri,
                                    string outputPath,
                                    string fileName,
                                    Dictionary<string, string>? ffmpegParameters = null)
        {
            _inputStreamUri = inputStreamUri;
            _outputPath = outputPath;
            _outputFileExtension = Path.GetExtension(fileName);
            _outputFileName = fileName.Replace(_outputFileExtension, string.Empty);
            _tunerNumber = tunerNumber;

            BuildFFmpegFlagPairs(ffmpegParameters);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpCaptureRequest"/> class.
        /// </summary>
        /// <param name="inputStreamUri"></param>
        /// <param name="outputPath"></param>
        /// <param name="fileName"></param>
        public FFmpegCaptureRequest(Uri inputStreamUri,
                                    string outputPath,
                                    string fileName,
                                    Dictionary<string, string>? ffmpegParameters = null)
        {
            _inputStreamUri = inputStreamUri;
            _outputPath = outputPath;
            _outputFileExtension = Path.GetExtension(fileName);
            _outputFileName = fileName.Replace(_outputFileExtension, string.Empty);

            BuildFFmpegFlagPairs(ffmpegParameters);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpCaptureRequest"/> class.
        /// </summary>
        /// <param name="inputStreamUri"></param>
        /// <param name="outputPath"></param>
        /// <param name="fileName"></param>
        public FFmpegCaptureRequest(ChannelInfo channelInfo,
                                    string outputPath,
                                    string fileName,
                                    Dictionary<string, string>? ffmpegParameters = null)
        {
            _inputStreamUri = channelInfo.StreamingUri;
            _outputPath = outputPath;
            _outputFileExtension = Path.GetExtension(fileName);
            _outputFileName = fileName.Replace(_outputFileExtension, string.Empty);

            BuildFFmpegFlagPairs(ffmpegParameters);
        }

        /// <summary>
        /// Builds the dictionary of ffmpeg flag pairs.
        /// The input stream always goes first, and output file goes last.
        /// Adds the additional parameters in the order they're provided.
        /// </summary>
        /// <param name="flagPairs"></param>
        private void BuildFFmpegFlagPairs(Dictionary<string, string>? flagPairs = null)
        {
            _ffmpegFlagPairs.Clear();
            _ffmpegFlagPairs.Add("-y", string.Empty);
            _ffmpegFlagPairs.Add("-i", $"\"{ParameterizedInputStreamUri}\"");
            if (flagPairs is not null)
            {
                foreach (var pair in flagPairs)
                {
                    _ffmpegFlagPairs.Add(pair.Key, pair.Value);
                }
            }
            _ffmpegFlagPairs.Add(string.Empty, $"\"{OutputFileFullPath}\"");
        }

        /// <summary>
        /// Builds the ffmpeg command using the specified flag pairs.
        /// </summary>
        /// <returns></returns>
        private string BuildFFmpegCommand()
        {
            var stringBuilder = new StringBuilder();

            foreach (var kvp in _ffmpegFlagPairs)
            {
                stringBuilder.Append(kvp.Key);
                if (!string.IsNullOrEmpty(kvp.Value))
                {
                    stringBuilder.Append(' ').Append(kvp.Value);
                }
                stringBuilder.Append(' ');
            }

            return stringBuilder.ToString().Trim();
        }

        /// <summary>
        /// Gets the input stream URI with duration and transcode parameters.
        /// </summary>
        private Uri GetInputStreamUriWithParameters()
        {
            var builder = new StringBuilder(InputStreamUri.ToString());
            var queryParams = new List<string>();

            if (Duration != TimeSpan.MaxValue)
            {
                queryParams.Add($"{DURATION_PARAM}={HttpUtility.UrlEncode(Duration.TotalSeconds.ToString())}");
            }

            if (TranscodeProfile != TranscodeProfile.None)
            {
                queryParams.Add($"{TRANSCODE_PROFILE_PARAM}={HttpUtility.UrlEncode(TranscodeProfile.ToString().ToLower())}");
            }

            if (queryParams.Any())
            {
                builder.Append(builder.ToString().Contains('?') ? "&" : "?");
                builder.Append(string.Join("&", queryParams));
            }

            if (_tunerNumber >= 0)
            {
                builder.Replace(TUNER_AUTO_PARAM, $"{TUNER_NUMBER_PARAM}{_tunerNumber}");
            }

            return new Uri(builder.ToString());
        }

    }
}
