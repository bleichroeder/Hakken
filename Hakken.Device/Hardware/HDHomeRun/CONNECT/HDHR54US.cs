namespace Hakken.Device.Hardware.HDHomeRun.CONNECT
{
    public class HDHR54US : HDHomeRunHardwareBase
    {
        public override HardwareModel HardwareModel => HardwareModel.CONNECT;

        public override string ParentModelName => "HDHomeRun CONNECT QUATRO";

        public override string ModelNumber => "HDHR5-4US";

        public override List<SignalType> SignalTypes => new()
        {
            SignalType.Eight_VSB_ATSC,
            SignalType.QAM64_256
        };

        public override int TunerCount => 4;

        public override bool WorksWithDVR => true;

        public override List<string> Ports => new()
        {
            "Antenna",
            "Ehernet (100baseTX)",
            "Power"
        };

        public override string PowerSupply => @"5Volt 1.5Amp regulated power adapter (higher Amp rating ok)
1.7mm ID / 4.0mm OD plug, 9.5mm depth, center positive
Max 5.25V @ no-load
Min 4.75V @ 1.5A
Noise ⇐ 150mVp-p";
    }
}
