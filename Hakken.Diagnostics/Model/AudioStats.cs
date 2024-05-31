using Newtonsoft.Json;

namespace Hakken.Diagnostics.Model
{
    /// <summary>
    /// FFMpeg VolumeDetect output.
    /// </summary>
    public class AudioStats
    {
        /// <summary>
        /// Represents whether the <see cref="AudioStats"/> were parsed successfully.
        /// </summary>
        [JsonIgnore]
        public bool SuccessfullyParsed { get; set; }

        /// <summary>
        /// Gets or sets the Overall value.
        /// </summary>
        public Channel? Overall { get; set; }

        /// <summary>
        /// Gets or sets the Channels value.
        /// </summary>
        [JsonIgnore]
        public List<Channel> Channels { get; set; } = new List<Channel>();

        /// <summary>
        /// Audio channel values.
        /// </summary>
        public class Channel
        {
            /// <summary>
            /// Gets or sets the ChannelID value.
            /// </summary>
            [JsonIgnore]
            public int ChannelID { get; set; }

            /// <summary>
            /// Gets or sets the DCOffset value.
            /// </summary>
            [JsonIgnore]
            public double DCOffset { get; set; }

            /// <summary>
            /// Gets or sets the MinLevel value.
            /// </summary>
            public double MinLevel { get; set; }

            /// <summary>
            /// Gets or sets the MaxLevel value.
            /// </summary>
            public double MaxLevel { get; set; }

            /// <summary>
            /// Gets or sets the MinDifference value.
            /// </summary>
            public double MinDifference { get; set; }

            /// <summary>
            /// Gets or sets the MaximumDifference value.
            /// </summary>
            public double MaximumDifference { get; set; }

            /// <summary>
            /// Gets or sets the MeanDifference value.
            /// </summary>
            public double MeanDifference { get; set; }

            /// <summary>
            /// Gets or sets the RMSDifference value.
            /// </summary>
            [JsonIgnore]
            public double RMSDifference { get; set; }

            /// <summary>
            /// Gets or sets the PeakLeveldB value.
            /// </summary>
            public double PeakLeveldB { get; set; }

            /// <summary>
            /// Gets or sets the RMSLeveldB value.
            /// </summary>
            [JsonIgnore]
            public double RMSLeveldB { get; set; }

            /// <summary>
            /// Gets or sets the RMSPeakdB value.
            /// </summary>
            [JsonIgnore]
            public double RMSPeakdB { get; set; }

            /// <summary>
            /// Gets or sets the RMSTroughdB value.
            /// </summary>
            [JsonIgnore]
            public double RMSTroughdB { get; set; }

            /// <summary>
            /// Gets or sets the CrestFactor value.
            /// </summary>
            [JsonIgnore]
            public double CrestFactor { get; set; }

            /// <summary>
            /// Gets or sets the FlatFactor value.
            /// </summary>
            [JsonIgnore]
            public double FlatFactor { get; set; }

            /// <summary>
            /// Gets or sets the PeakCount value.
            /// </summary>
            [JsonIgnore]
            public double PeakCount { get; set; }

            /// <summary>
            /// Gets or sets the NoiseFloordB value.
            /// </summary>
            [JsonIgnore]
            public double NoiseFloordB { get; set; }

            /// <summary>
            /// Gets or sets the NoiseFloorCount value.
            /// </summary>
            [JsonIgnore]
            public double NoiseFloorCount { get; set; }

            /// <summary>
            /// Gets or sets the Entropy value.
            /// </summary>
            [JsonIgnore]
            public double Entropy { get; set; }

            /// <summary>
            /// Gets or sets the Bitdepth value.
            /// </summary>
            [JsonIgnore]
            public string? Bitdepth { get; set; }

            /// <summary>
            /// Gets or sets the DynamicRange value.
            /// </summary>
            [JsonIgnore]
            public double DynamicRange { get; set; }

            /// <summary>
            /// Gets or sets the ZeroCrossings value.
            /// </summary>
            [JsonIgnore]
            public double ZeroCrossings { get; set; }

            /// <summary>
            /// Gets or sets the ZeroCrossingsRate value.
            /// </summary>
            [JsonIgnore]
            public double ZeroCrossingsRate { get; set; }

            /// <summary>
            /// Gets or sets the NumberOfNaNs value.
            /// </summary>
            [JsonIgnore]
            public double NumberOfNaNs { get; set; }

            /// <summary>
            /// Gets or sets the NumberOfInfs value.
            /// </summary>
            [JsonIgnore]
            public double NumberOfInfs { get; set; }

            /// <summary>
            /// Gets or sets the NumberOfDenormals value.
            /// </summary>
            [JsonIgnore]
            public double NumberOfDenormals { get; set; }
        }

    }
}