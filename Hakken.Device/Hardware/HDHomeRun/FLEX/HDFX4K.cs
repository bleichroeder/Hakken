namespace Hakken.Device.Hardware.HDHomeRun.FLEX
{
    public class HDFX4K : HDHomeRunHardwareBase
    {
        public override HardwareModel HardwareModel => HardwareModel.FLEX;

        public override string ParentModelName => "HDHomeRun FLEX 4K";

        public override string ModelNumber => "HDFX-4K";

        public override List<SignalType> SignalTypes => new()
        {
            SignalType.ATSC3,
            SignalType.ATSC1
        };

        public override int TunerCount => 4;

        public override bool WorksWithDVR => true;

        public override List<string> Ports => new()
        {
            "Antenna",
            "Ethernet (100baseTX)",
            "Power",
            "USB (for DVR)"
        };

        public override string PowerSupply => @"
12Volt 1.0Amp regulated power adapter (higher Amp rating ok)
1.3mm ID / 3.4mm OD plug, 9.5mm depth, center positive
Noise ⇐ 150mVp-p
";
    }
}
