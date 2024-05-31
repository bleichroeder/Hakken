namespace Hakken.Device.Hardware.Hauppauge
{
    internal class DCR3250 : HauppaugeHardwareBase
    {
        public override HardwareModel HardwareModel => HardwareModel.WINTV;

        public override string ParentModelName => "WinTV-DCR-3250";

        public override string ModelNumber => "WinTV-DCR-3250";

        public override List<SignalType> SignalTypes => new()
        {
            SignalType.USCableTV,
            SignalType.QAM64_256
        };

        public override int TunerCount => 3;

        public override bool WorksWithDVR => true;

        public override List<string> Ports => new()
        {
            "Coaxial-in",
            "USB (for Tuning Adapter)",
            "Ethernet (1000BASE-T)",
            "Power",
            "M-card (CableCARD) slot"
        };

        public override string PowerSupply => @"
5Volt 2.5Amp regulated power adapter (higher Amp rating ok)
2.1mm ID / 5.5mm OD plug, 9-10mm depth, center positive
Max 5.25V @ no-load
Min 4.75V @ 2.5A
Noise ⇐ 100mVp-p

Normal operating current is approximately 1.25A with a CableCARD present and 3 tuners streaming.
";
    }
}
