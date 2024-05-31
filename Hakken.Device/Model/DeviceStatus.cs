namespace Hakken.Device.Model
{
    /// <summary>
    /// The device status.
    /// </summary>
    public class DeviceStatus
    {
        /// <summary>
        /// True if authenticated.
        /// </summary>
        public bool Authenticated { get; set; }

        /// <summary>
        /// True if validated.
        /// </summary>
        public bool Validated { get; set; }

        /// <summary>
        /// Gets the OOBLock.
        /// </summary>
        public OOBLock? OOBLock { get; set; }

        /// <summary>
        /// Parses the OOBLock string value.
        /// </summary>
        /// <param name="input"></param>
        public static OOBLock ParseOOBLock(string input)
            => string.IsNullOrEmpty(input) ?
            Model.OOBLock.Unknown
          : (OOBLock)Enum.Parse(typeof(OOBLock), input, true);
    }
}
