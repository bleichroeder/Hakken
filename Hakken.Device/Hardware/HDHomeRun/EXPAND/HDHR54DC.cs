namespace Hakken.Device.Hardware.HDHomeRun.PRIME
{
    public class HDHR54DC : HDHomeRunHardwareBase
    {
        public override HardwareModel HardwareModel => HardwareModel.EXPAND;

        public override string ParentModelName => "HDHomeRun EXPAND";

        public override string ModelNumber => "HDHR3-4DC";

        public override List<SignalType> SignalTypes => new()
        {
            SignalType.DVB_C
        };

        public override int TunerCount => 4;

        public override bool WorksWithDVR => true;

        public override List<string> Ports => new()
        {
            "Power",
            "Ethernet(100baseTX)",
            "Coaxial-in"
        };

        public override string PowerSupply => @"
5Volt 1.5Amp regulated power adapter (higher Amp rating ok)
1.7mm ID / 4.0mm OD plug, 9.5mm depth, center positive
Max 5.25V @ no-load
Min 4.75V @ 1.5A
Noise ⇐ 150mVp-p
";
    }
}
