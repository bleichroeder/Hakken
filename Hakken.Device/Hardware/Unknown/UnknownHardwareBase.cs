namespace Hakken.Device.Hardware.Unknown
{
    /// <summary>
    /// HDHomeRun hardware base class.
    /// </summary>
    public abstract class UnknownHardwareBase : IHardwareInfo
    {
        public DeviceModel DeviceModel => DeviceModel.Unknown;

        public abstract HardwareModel HardwareModel { get; }

        public abstract string ParentModelName { get; }

        public abstract string ModelNumber { get; }

        public abstract List<SignalType> SignalTypes { get; }

        public abstract int TunerCount { get; }

        public abstract bool WorksWithDVR { get; }

        public abstract List<string> Ports { get; }

        public abstract string PowerSupply { get; }
    }
}
