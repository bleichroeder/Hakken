namespace Hakken.Diagnostics.Model
{
    /// <summary>
    /// FFMpeg FrozenFrames detection output.
    /// </summary>
    public class FrozenFrames
    {
        /// <summary>
        /// Gets or sets the NoiseTolerance value.
        /// </summary>
        public double? NoiseTolerance { get; set; }

        /// <summary>
        /// Gets or sets Start value.
        /// </summary>
        public TimeSpan Start { get; set; }

        /// <summary>
        /// Gets or sets the End value.
        /// </summary>
        public TimeSpan End { get; set; }

        /// <summary>
        /// Gets or sets the Duration value.
        /// </summary>
        public TimeSpan Duration { get; set; }
    }
}
