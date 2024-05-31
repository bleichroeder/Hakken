using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hakken.Diagnostics.Model
{
    /// <summary>
    /// The FFProbe Stream.
    /// </summary>
    public class MediaStream
    {
        private const string VIDEO_CODEC_IDENTIFIER = "video";

        /// <summary>
        /// Gets the Type value.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public StreamType Type
        {
            get
            {
                return CodecType is not null &&
                    CodecType.Equals(VIDEO_CODEC_IDENTIFIER, StringComparison.CurrentCultureIgnoreCase) ?
                    StreamType.Video :
                    StreamType.Audio;
            }
        }

        /// <summary>
        /// Gets the Index value.
        /// </summary>
        public int Index { get; set; }

        public double FPS
        {
            get
            {
                try
                {
                    string[]? split = AverageFrameRate?.Split('/');
                    return Math.Round(Convert.ToDouble(split?[0]) / Convert.ToDouble(split?[1]), 2);
                }
                catch (Exception) { }
                return 0;
            }
        }

        /// <summary>
        /// Gets the IsHD value.
        /// Represents whether this file is considered HD. (Greater than or equal to 1280x720 resolution).
        /// </summary>
        public bool IsHD
        {
            get
            {
                return Height >= 720 && Width >= 1280;
            }
        }

        /// <summary>
        /// Gets or sets the CodecName value.
        /// </summary>
        [JsonProperty("codec_name")]
        public string? CodecName { get; set; }

        /// <summary>
        /// Gets or sets the CodecLongName value.
        /// </summary>
        [JsonProperty("codec_long_name")]
        public string? CodecLongName { get; set; }

        /// <summary>
        /// Gets or sets the Profile value.
        /// </summary>
        [JsonIgnore]
        public string? Profile { internal get; set; }

        /// <summary>
        /// Gets or sets the CodecType value.
        /// </summary>
        [JsonProperty("codec_type")]
        public string? CodecType { get; set; }

        /// <summary>
        /// Gets or sets the CodecTagString value.
        /// </summary>
        [JsonIgnore]
        [JsonProperty("codec_tag_string")]
        public string? CodecTagString { internal get; set; }

        /// <summary>
        /// Gets or sets the CodecTag value.
        /// </summary>
        [JsonIgnore]
        [JsonProperty("codec_tag")]
        public string? CodecTag { internal get; set; }

        /// <summary>
        /// Gets or sets the Width value.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the Height value.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the CodedWidth value.
        /// </summary>
        [JsonIgnore]
        [JsonProperty("coded_width")]
        public int CodedWidth { internal get; set; }

        /// <summary>
        /// Gets or sets the CodedHeight value.
        /// </summary>
        [JsonIgnore]
        [JsonProperty("coded_height")]
        public int CodedHeight { internal get; set; }

        /// <summary>
        /// Gets or sets the ClosedCaptions value.
        /// </summary>
        [JsonProperty("closed_captions")]
        public bool ClosedCaptions { get; set; }

        /// <summary>
        /// Gets or sets the FilmGrain value.
        /// </summary>
        [JsonIgnore]
        [JsonProperty("film_grain")]
        public bool FilmGrain { internal get; set; }

        /// <summary>
        /// Gets or sets the HasBFrames value.
        /// </summary>
        [JsonIgnore]
        [JsonProperty("has_b_frames")]
        public bool HasBFrames { internal get; set; }

        /// <summary>
        /// Gets or sets the SampleAspectRatio value.
        /// </summary>
        [JsonIgnore]
        [JsonProperty("sample_aspect_ratio")]
        public string? SampleAspectRatio { internal get; set; }

        /// <summary>
        /// Gets or sets the AspectRatio value.
        /// </summary>
        [JsonProperty("display_aspect_ratio")]
        public string? AspectRatio { get; set; }

        /// <summary>
        /// Gets or sets the PixelFormat value.
        /// </summary>
        [JsonIgnore]
        [JsonProperty("pix_fmt")]
        public string? PixelFormat { internal get; set; }

        /// <summary>
        /// Gets or sets the Level value.
        /// </summary>
        [JsonIgnore]
        public int Level { internal get; set; }

        /// <summary>
        /// Gets or sets the ChromaLocation value.
        /// </summary>
        [JsonIgnore]
        [JsonProperty("chroma_location")]
        public string? ChromaLocation { internal get; set; }

        /// <summary>
        /// Gets or sets the FieldOrder value.
        /// </summary>
        [JsonIgnore]
        [JsonProperty("field_order")]
        public string? FieldOrder { internal get; set; }

        /// <summary>
        /// Gets or sets the Refs value.
        /// </summary>
        [JsonIgnore]
        public int Refs { internal get; set; }

        /// <summary>
        /// Gets or sets the IsAVC value.
        /// </summary>
        [JsonIgnore]
        [JsonProperty("is_avc")]
        public string? IsAVC { internal get; set; }

        /// <summary>
        /// Gets or sets the NalLengthSize value.
        /// </summary>
        [JsonIgnore]
        [JsonProperty("nal_length_size")]
        public string? NalLengthSize { internal get; set; }

        /// <summary>
        /// Gets or sets the Id value.
        /// </summary>
        [JsonIgnore]
        public string? Id { internal get; set; }

        /// <summary>
        /// Gets or sets the RFrameRate value.
        /// </summary>
        [JsonIgnore]
        [JsonProperty("r_frame_rate")]
        public string? RFrameRate { internal get; set; }

        /// <summary>
        /// Gets or sets the AverageFrameRate value.
        /// </summary>
        [JsonProperty("avg_frame_rate")]
        public string? AverageFrameRate { get; set; }

        /// <summary>
        /// Gets or sets the TimeBase value.
        /// </summary>
        [JsonIgnore]
        [JsonProperty("time_base")]
        public string? TimeBase { internal get; set; }

        /// <summary>
        /// Gets or sets the StartPTS value.
        /// </summary>
        [JsonIgnore]
        [JsonProperty("start_pts")]
        public long StartPTS { internal get; set; }

        /// <summary>
        /// Gets or sets the DurationTS value.
        /// </summary>
        [JsonIgnore]
        [JsonProperty("duration_ts")]
        public int DurationTS { internal get; set; }

        /// <summary>
        /// Gets or sets the Duration value.
        /// </summary>
        public TimeSpan Duration { internal get; set; }

        /// <summary>
        /// Gets or sets the BitsPerRawSample value.
        /// </summary>
        [JsonIgnore]
        [JsonProperty("bits_per_raw_sample")]
        public string? BitsPerRawSample { internal get; set; }

        /// <summary>
        /// Gets or sets the ExtraDataSize value.
        /// </summary>
        [JsonIgnore]
        [JsonProperty("extradata_size")]
        public int ExtraDataSize { internal get; set; }

        /// <summary>
        /// Gets or sets the Disposition value.
        /// </summary>

        [JsonIgnore]
        public Disposition? Disposition { internal get; set; }
    }

    public class Disposition
    {
        /// <summary>
        /// Gets or sets the Default value.
        /// </summary>
        [JsonProperty("_default")]
        public int Default { get; set; }

        /// <summary>
        /// Gets or sets the Dub value.
        /// </summary>
        public int Dub { get; set; }

        /// <summary>
        /// Gets or sets the Original value.
        /// </summary>
        public int Original { get; set; }

        /// <summary>
        /// Gets or sets the Comment value.
        /// </summary>
        public int Comment { get; set; }

        /// <summary>
        /// Gets or sets the Lyrics value.
        /// </summary>
        public int Lyrics { get; set; }

        /// <summary>
        /// Gets or sets the Karakoe value.
        /// </summary>
        public int Karaoke { get; set; }

        /// <summary>
        /// Gets or sets the Forced value.
        /// </summary>
        public int Forced { get; set; }

        /// <summary>
        /// Gets or sets the HearingImpared value.
        /// </summary>
        [JsonProperty("hearing_impaired")]
        public int HearingImpared { get; set; }

        /// <summary>
        /// Gets or sets the Visualimpaired value.
        /// </summary>
        [JsonProperty("visual_impaired")]
        public int VisualImpaired { get; set; }

        /// <summary>
        /// Gets or sets the CleanEffects.
        /// </summary>
        [JsonProperty("Clean_effects")]
        public int CleanEffects { get; set; }

        /// <summary>
        /// Gets or sets the AttachedPicValue.
        /// </summary>
        [JsonProperty("attached_pic")]
        public int AttachedPic { get; set; }

        /// <summary>
        /// Gets or sets the TimedThumbnails value.
        /// </summary>
        [JsonProperty("timed_thumbnails")]
        public int TimedThumbnails { get; set; }

        /// <summary>
        /// Gets or sets the Captions value.
        /// </summary>
        public int Captions { get; set; }

        /// <summary>
        /// Gets or sets the Descriptions value.
        /// </summary>
        public int Descriptions { get; set; }

        /// <summary>
        /// Gets or sets the Metadata value.
        /// </summary>
        public int Metadata { get; set; }

        /// <summary>
        /// Gets or sets the Dependent value.
        /// </summary>
        public int Dependent { get; set; }

        /// <summary>
        /// Gets or sets the StillImage value.
        /// </summary>
        [JsonProperty("still_image")]
        public int StillImage { get; set; }
    }

    /// <summary>
    /// The FFProbe Stream type.
    /// </summary>
    public enum StreamType
    {
        Audio,
        Video
    }
}
