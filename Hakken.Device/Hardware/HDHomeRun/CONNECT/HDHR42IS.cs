namespace Hakken.Device.Hardware.HDHomeRun.CONNECT
{
    public class HDHR42IS : HDHomeRunHardwareBase
    {
        public override HardwareModel HardwareModel => HardwareModel.CONNECT;

        public override string ParentModelName => "HDHomeRun CONNECT";

        public override string ModelNumber => "HDHR4-2IS";

        public override List<SignalType> SignalTypes => new()
        {
            SignalType.ISDB_T
        };

        public override int TunerCount => 2;

        public override bool WorksWithDVR => true;

        public override List<string> Ports => new()
        {
            "Antenna",
            "Ehernet (100baseTX)",
            "Power"
        };

        public override string PowerSupply => @"5Volt 1Amp regulated power adapter (higher Amp rating ok)
2.1mm ID / 5.5mm OD plug, 9-10mm depth, center positive
Max 5.25V @ no-load
Min 4.75V @ 1A
Noise ⇐ 100mVp-p";
    }
}
