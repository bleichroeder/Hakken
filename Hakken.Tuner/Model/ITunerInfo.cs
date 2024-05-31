using System.Net;

namespace Hakken.Tuner.Model
{
    /// <summary>
    /// TunerInfo object.
    /// </summary>
    public interface ITunerInfo
    {
        /// <summary>
        /// Gets the tuner location, local or remote.
        /// </summary>
        public TunerLocation Location { get; }

        /// <summary>
        /// Gets the device access.
        /// </summary>
        public string? DeviceAccess => TunerURI;

        /// <summary>
        /// hdhomerun_config.exe path. Set by DeviceDiscoverer, can be changed in TunerInfo object as well.
        /// Used when 'useConfig' is set to True when calling the RefreshTunerInfo method.
        /// </summary>
        public string? HDHomerunConfigPath { get; set; }

        /// <summary>
        /// Tuner's parent device ID.
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// Tuner's parent device IP.
        /// </summary>
        public IPAddress DeviceIP { get; set; }

        /// <summary>
        /// Tuner URI.
        /// </summary>
        public string TunerURI { get; set; }

        /// <summary>
        /// Tuner number.
        /// </summary>
        public int TunerNumber { get; set; }

        /// <summary>
        /// Tuner's current channel name.
        /// </summary>
        public string? VirtualChannel { get; set; }

        /// <summary>
        /// Tuner's currently tuned frequency.
        /// </summary>
        public double Frequency { get; set; }

        /// <summary>
        /// Tuner's currently tuned program number.
        /// </summary>
        public double ProgramNumber { get; set; }

        /// <summary>
        /// Tuner authorization.
        /// </summary>
        public string? Authorization { get; set; }

        /// <summary>
        /// Tuner CCI Protection.
        /// </summary>
        public string? CCIProtection { get; set; }

        /// <summary>
        /// Tuner modulation lock.
        /// </summary>
        public string? ModulationLock { get; set; }

        /// <summary>
        /// Tuner PCR lock.
        /// </summary>
        public string? PCRLock { get; set; }

        /// <summary>
        /// Current signal strength.
        /// </summary>
        public int SignalStrength { get; set; }

        /// <summary>
        /// Current signal quality.
        /// </summary>
        public int SignalQuality { get; set; }

        /// <summary>
        /// Current symbol quality.
        /// </summary>
        public int SymbolQuality { get; set; }

        /// <summary>
        /// Current streaming/network rate.
        /// </summary>
        public decimal StreamingRate { get; set; }

        /// <summary>
        /// Tuner's resource lock. (IPAddress)
        /// </summary>
        public IPAddress? ResourceLock { get; set; }

        /// <summary>
        /// Tuner's network target.
        /// </summary>
        public string? NetworkTarget { get; set; }

        /// <summary>
        /// Returns true if this tuner is streaming.
        /// </summary>
        public bool IsStreaming => ResourceLock is not null;

        /// <summary>
        /// Signal quality as a percentage.
        /// </summary>
        public float SignalQualityPercent => SignalQuality / 100.0f;

        /// <summary>
        /// Symbol quality as a percentage.
        /// </summary>
        public float SymbolQualityPercent => SymbolQuality / 100.0f;

        /// <summary>
        /// Signal strength as a percentage.
        /// </summary>
        public float SignalStrengthPercent => SignalStrength / 100.0f;

        /// <summary>
        /// The minimum signal quality threshold. If the tuner's signal quality is below this threshold, an issue is detected.
        /// </summary>
        public decimal SignalQualityLowerThreshold { get; set; }

        /// <summary>
        /// The minimum symbol quality threshold. If the tuner's symbol quality is below this threshold, an issue is detected.
        /// </summary>
        public decimal SymbolQualityLowerThreshold { get; set; }

        /// <summary>
        /// The minimum signal strength threshold. If the tuner's signal strength is below this threshold, an issue is detected.
        /// </summary>
        public decimal SignalStrengthLowerThreshold { get; set; }

        /// <summary>
        /// The minimum streaming rate threshold. If the tuner's streaming rate is below this threshold, an issue is detected.
        /// </summary>
        public decimal StreamingRateLowerThreshold { get; set; }

        /// <summary>
        /// Returns True if tuner's streaming rate is below a specified threshold.
        /// </summary>
        public bool StreamingRateLowerThan(decimal threshold);

        /// <summary>
        /// Returns True if tuner's signal quality is below a specified threshold.
        /// </summary>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public bool SignalQualityLowerThan(decimal threshold);

        /// <summary>
        /// Returns True if tuner's symbol quality is below a specified threshold.
        /// </summary>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public bool SymbolQualityLowerThan(decimal threshold);

        /// <summary>
        /// Returns True if tuner's signal strength is below a specified threshold.
        /// </summary>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public bool SignalStrengthLowerThan(decimal threshold);

        /// <summary>
        /// Returns True if any issues are detected with this tuner.
        /// </summary>
        public bool IssueDetected { get; }

        /// <summary>
        /// Returns any issues detected with this tuner.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TunerIssue> GetIssues();
    }
}
