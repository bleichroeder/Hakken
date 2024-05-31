namespace Hakken.Device.Hardware.HDHomeRun.FLEX
{
    public class HDFX2US : HDHomeRunHardwareBase
    {
        public override HardwareModel HardwareModel => HardwareModel.FLEX;

        public override string ParentModelName => "HDHomeRun FLEX DUO";

        public override string ModelNumber => "HDFX-2US";

        public override List<SignalType> SignalTypes => new()
        {
            SignalType.ATSC1,
            SignalType.QAM64_256
        };

        public override int TunerCount => 2;

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
