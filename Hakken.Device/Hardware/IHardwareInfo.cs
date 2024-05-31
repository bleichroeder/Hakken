namespace Hakken.Device.Hardware
{
    public interface IHardwareInfo
    {
        /// <summary>
        /// The device model.
        /// </summary>
        public DeviceModel DeviceModel { get; }

        /// <summary>
        /// The hardware model.
        /// </summary>
        public HardwareModel HardwareModel { get; }

        /// <summary>
        /// The parent model name.
        /// </summary>
        public string ParentModelName { get; }

        /// <summary>
        /// The child model names.
        /// </summary>
        public string ModelNumber { get; }

        /// <summary>
        /// The signal types this device supports.
        /// </summary>
        public List<SignalType> SignalTypes { get; }

        /// <summary>
        /// The device's tuner count.
        /// </summary>
        public int TunerCount { get; }

        /// <summary>
        /// True if this device works with a DVR.
        /// </summary>
        public bool WorksWithDVR { get; }

        /// <summary>
        /// The device's available ports.
        /// </summary>
        public List<string> Ports { get; }

        /// <summary>
        /// Details about the device's power supply.
        /// </summary>
        public string PowerSupply { get; }
    }
}
