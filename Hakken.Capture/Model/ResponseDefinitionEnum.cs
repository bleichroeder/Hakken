namespace Hakken.Capture.Model
{
    /// <summary>
    /// Response definition for error codes.
    /// </summary>
    public enum ResponseDefinition
    {
        /// <summary>
        /// Success
        /// </summary>
        Success = 200,

        /// <summary>
        /// Unknown Channel (801)
        /// </summary>
        UnknownChannel = 801,

        /// <summary>
        /// Unknown Transcode Profile (EXTEND only) (802)
        /// </summary>
        UnknownTranscodeProfile = 802,

        /// <summary>
        /// System Busy (normally means the device is in the middle of a channel scan) (803)
        /// </summary>
        SystemBusy = 803,

        /// <summary>
        /// Tuner In Use (when a specific tuner is used instead of auto) (804)
        /// </summary>
        TunerInUse = 804,

        /// <summary>
        /// All Tuners In Use (805)
        /// </summary>
        AllTunersInUse = 805,

        /// <summary>
        /// Tune Failed (TA reported an error (PRIME only), or hardware error) (806)
        /// </summary>
        TuneFailed = 806,

        /// <summary>
        /// No Video Data (bad reception/station off air/particular service is not being transmitted at that time) (807)
        /// </summary>
        NoVideoData = 807,

        /// <summary>
        /// DVR Failure (DVR can't write to the recording location) (808)
        /// </summary>
        DVRFailure = 808,

        /// <summary>
        /// Playback Connection Limit (DVR has hit the limit of playback streams) (809)
        /// </summary>
        PlaybackConnectionLimit = 809,

        /// <summary>
        /// DVR Digital Video Recorder Full (810)
        /// </summary>
        DVRFull = 810,

        /// <summary>
        /// Content Protection Required (PRIME only, channel is copy protected) (811)
        /// </summary>
        ContentProtectionRequired = 811
    }
}
