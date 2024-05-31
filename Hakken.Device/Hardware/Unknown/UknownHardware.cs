namespace Hakken.Device.Hardware.Unknown
{
    public class UknownHardware : UnknownHardwareBase
    {
        public override HardwareModel HardwareModel => HardwareModel.UNKNOWN;

        public override string ParentModelName => "Unknown";

        public override string ModelNumber => "Unknown";

        public override List<SignalType> SignalTypes => new();

        public override int TunerCount => 0;

        public override bool WorksWithDVR => false;

        public override List<string> Ports => new();

        public override string PowerSupply => string.Empty;
    }
}
