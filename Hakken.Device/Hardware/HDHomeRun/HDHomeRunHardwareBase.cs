namespace Hakken.Device.Hardware.HDHomeRun
{
    /// <summary>
    /// HDHomeRun hardware base class.
    /// </summary>
    public abstract class HDHomeRunHardwareBase : IHardwareInfo
    {
        public DeviceModel DeviceModel => DeviceModel.HDHomeRun;

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
