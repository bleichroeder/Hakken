using Newtonsoft.Json;

namespace Hakken.Diagnostics.Model
{
    /// <summary>
    /// FFProbe MediaProperties.
    /// </summary>
    public class MediaProperties
    {
        /// <summary>
        /// Gets the Exception value.
        /// </summary>
        public Exception? Exception
        {
            get
            {
                return _exception;
            }
            set
            {
                _exception = value;
            }
        }

        public Error? Error { get; set; }

        /// <summary>
        /// Gets the media file path.
        /// </summary>
        public string MediaFilePath
        {
            get
            {
                if (!string.IsNullOrEmpty(_mediaFilePath))
                    return _mediaFilePath;

                if (Format is not null)
                    if (!string.IsNullOrEmpty(Format.FileName))
                        return Format.FileName;

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the SuccessfullyParsed value.
        /// </summary>
        public bool SuccessfullyParsed => Streams is not null && Streams.Any();

        /// <summary>
        /// Returns true if there is an audio stream.
        /// </summary>
        public bool ContainsAudioStream => AudioStreams?.Count > 0;

        /// <summary>
        /// Returns true if there is an video stream.
        /// </summary>
        public bool ContainsVideoStream => VideoStreams?.Count > 0;

        /// <summary>
        /// Returns true if there is more than one video stream.
        /// </summary>
        public bool ContainsMultipleVideoStreams => VideoStreams?.Count > 1;

        /// <summary>
        /// Returns true if there is more than one audio stream.
        /// </summary>
        public bool ContainsMultipleAudioStreams => AudioStreams?.Count > 1;

        /// <summary>
        /// Returns true if FrozenFrames were detected.
        /// </summary>
        public bool ContainsFrozenFrames => FrozenFrames is not null;

        /// <summary>
        /// Gets the VideoStream.
        /// </summary>
        public List<MediaStream>? VideoStreams => Streams?.Where(x => x.Type.Equals(StreamType.Video)).ToList();

        /// <summary>
        /// Gets the AudioStream.
        /// </summary>
        public List<MediaStream>? AudioStreams => Streams?.Where(x => x.Type.Equals(StreamType.Audio)).ToList();

        /// <summary>
        /// Gets or sets the Format value.
        /// </summary>
        public Format? Format { get; set; }

        /// <summary>
        /// Gets or sets the Streams value.
        /// </summary>
        public List<MediaStream>? Streams { internal get; set; }

        /// <summary>
        /// Gets or sets the FrozenFrames value.
        /// </summary>
        public List<FrozenFrames>? FrozenFrames { get; set; }

        /// <summary>
        /// Gets or sets the VolumeStats value.
        /// </summary>
        public AudioStats? AudioStats { get; set; }

        private Exception? _exception;
        private readonly string? _mediaFilePath;

        /// <summary>
        /// MediaFile MediaProperties.
        /// </summary>
        public MediaProperties(string? mediaFilePath)
        {
            _mediaFilePath = mediaFilePath;
        }
    }

    /// <summary>
    /// Error object.
    /// </summary>
    public class Error
    {
        [JsonProperty("code")]
        public long Code { get; set; }

        [JsonProperty("string")]
        public string? Message { get; set; }
    }
}