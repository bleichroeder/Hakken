using Hakken.Channel.Model;
using System.Collections.Specialized;
using System.Web;

namespace Hakken.Capture.Model.Request
{
    /// <summary>
    /// Represents a request to capture a video.
    /// </summary>
    public class HttpCaptureRequest
    {
        private static readonly string DURATION_PARAM = "duration";
        private static readonly string TRANSCODE_PROFILE_PARAM = "transcode";
        private static readonly string TUNER_NUMBER_PARAM = "tuner";
        private static readonly string TUNER_AUTO_PARAM = "auto";

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
            set => _inputStreamUri = value ?? throw new ArgumentNullException(nameof(InputStreamUri));
        }

        /// <summary>
        /// Gets the parameterized input stream URI.
        /// </summary>
        public Uri? ParameterizedInputStreamUri => GetInputStreamUriWithParameters();

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
        /// Gets the full output file path.
        /// </summary>
        public string OutputFileFullPath => Path.Combine(_outputPath, _outputFileName + _outputFileExtension);

        private TimeSpan _duration = TimeSpan.MaxValue;
        private Uri _inputStreamUri;
        private TranscodeProfile _transcodeProfile = TranscodeProfile.None;
        private int _tunerNumber = -1;

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
        public HttpCaptureRequest(int tunerNumber, Uri inputStreamUri, string outputPath, string fileName)
        {
            _inputStreamUri = inputStreamUri;
            _outputPath = outputPath;
            _outputFileExtension = Path.GetExtension(fileName);
            _outputFileName = fileName.Replace(_outputFileExtension, string.Empty);
            _tunerNumber = tunerNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpCaptureRequest"/> class.
        /// </summary>
        /// <param name="inputStreamUri"></param>
        /// <param name="outputPath"></param>
        /// <param name="fileName"></param>
        public HttpCaptureRequest(Uri inputStreamUri, string outputPath, string fileName)
        {
            _inputStreamUri = inputStreamUri;
            _outputPath = outputPath;
            _outputFileExtension = Path.GetExtension(fileName);
            _outputFileName = fileName.Replace(_outputFileExtension, string.Empty);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpCaptureRequest"/> class.
        /// </summary>
        /// <param name="inputStreamUri"></param>
        /// <param name="outputPath"></param>
        /// <param name="fileName"></param>
        public HttpCaptureRequest(ChannelInfo channelInfo, string outputPath, string fileName)
        {
            _inputStreamUri = channelInfo.StreamingUri;
            _outputPath = outputPath;
            _outputFileExtension = Path.GetExtension(fileName);
            _outputFileName = fileName.Replace(_outputFileExtension, string.Empty);
        }

        /// <summary>
        /// Gets the input stream URI with duration and transcode parameters.
        /// </summary>
        private Uri? GetInputStreamUriWithParameters()
        {
            if (InputStreamUri is null)
            {
                return null;
            }

            UriBuilder uriBuilder = new(InputStreamUri);

            NameValueCollection? query = HttpUtility.ParseQueryString(uriBuilder.Query);

            if (Duration != TimeSpan.MaxValue)
            {
                query[DURATION_PARAM] = Duration.TotalSeconds.ToString();
            }

            if (TranscodeProfile != TranscodeProfile.None)
            {
                query[TRANSCODE_PROFILE_PARAM] = TranscodeProfile.ToString().ToLower();
            }

            uriBuilder.Query = query.ToString();

            string uri = uriBuilder.Uri.ToString();

            if (_tunerNumber >= 0)
            {
                uri = uri.Replace(TUNER_AUTO_PARAM, $"{TUNER_NUMBER_PARAM}{_tunerNumber}");
            }

            return new Uri(uri);
        }

    }
}
