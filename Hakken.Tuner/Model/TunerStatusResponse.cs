namespace Hakken.Tuner.Model
{
    /// <summary>
    /// The response from the status.json api.
    /// </summary>
    public class TunerStatusResponse
    {
        /// <summary>
        /// The resource value.
        /// </summary>
        public string Resource { get; set; } = string.Empty;

        /// <summary>
        /// The VctNumber value.
        /// </summary>
        public int VctNumber { get; set; }

        /// <summary>
        /// The VctName value.
        /// </summary>
        public string VctName { get; set; } = string.Empty;

        /// <summary>
        /// The Frequency value.
        /// </summary>
        public double Frequency { get; set; }

        /// <summary>
        /// The SignalStrength value.
        /// </summary>
        public int SignalStrengthPercent { get; set; }

        /// <summary>
        /// The SignalQuality value.
        /// </summary>
        public int SignalQualityPercent { get; set; }

        /// <summary>
        /// The SymbolQuality value.
        /// </summary>
        public int SymbolQualityPercent { get; set; }

        /// <summary>
        /// The TargetIP value.
        /// </summary>
        public string TargetIP { get; set; } = string.Empty;
    }
}
