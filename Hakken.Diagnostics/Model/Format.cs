using Newtonsoft.Json;

namespace Hakken.Diagnostics.Model
{
    /// <summary>
    /// The FFProbe Stream.
    /// </summary>
    public class Format
    {
        /// <summary>
        /// Gets or sets the FileName value.
        /// </summary>
        [JsonProperty("filename")]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the StreamCount value.
        /// </summary>
        [JsonProperty("nb_streams")]
        public int StreamCount { get; set; }

        /// <summary>
        /// Gets or sets the FormatName value.
        /// </summary>
        [JsonProperty("format_name")]
        public string? FormatName { get; set; }

        /// <summary>
        /// Gets or sets the FormatLongName value.
        /// </summary>
        [JsonProperty("format_long_name")]
        public string? FormatLongName { get; set; }

        /// <summary>
        /// Gets or sets the DurationSeconds value.
        /// </summary>
        [JsonProperty("duration")]
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets the Size value.
        /// </summary>
        [JsonProperty("size")]
        public double Size { get; set; }

        /// <summary>
        /// Gets the size value in Mb.
        /// </summary>
        public double SizeMB => Size / 1000000;

        /// <summary>
        /// Gets or sets the BitRate value.
        /// </summary>
        [JsonProperty("bit_rate")]
        public long BitRate { get; set; }

        /// <summary>
        /// Gets or sets the Tags.
        /// </summary>
        public List<Tags> Tags { get; set; } = new();
    }

    public class Tags
    {
        /// <summary>
        /// Gets or sets the MajorBrand value.
        /// </summary>
        [JsonProperty("major_brand")]
        public string? MajorBrand { get; set; }

        /// <summary>
        /// Gets or sets the MinorBrand value.
        /// </summary>
        [JsonProperty("minor_version")]
        public string? MinorVersion { get; set; }

        /// <summary>
        /// Gets or sets the CompatibleBrands value.
        /// </summary>
        [JsonProperty("compatible_brands")]
        public string? CompatibleBrands { get; set; }

        /// <summary>
        /// gets or sets the CreationTime value.
        /// </summary>
        [JsonProperty("creation_time")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the Rotate value.
        /// </summary>
        [JsonProperty("rotate")]
        public int Rotate { get; set; }
    }
}
