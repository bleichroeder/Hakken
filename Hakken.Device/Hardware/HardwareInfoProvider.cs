using Hakken.Device.Hardware.Hauppauge;
using Hakken.Device.Hardware.HDHomeRun;
using Hakken.Device.Hardware.Unknown;
using System.Collections.Concurrent;
using System.Reflection;

namespace Hakken.Device.Hardware
{
    /// <summary>
    /// Provides a lookup for hardware info.
    /// </summary>
    public static class HardwareInfoProvider
    {
        /// <summary>
        /// Static constructor which dynamically loads all implementations of IHardwareInfo.
        /// </summary>
        static HardwareInfoProvider()
            => LoadImplementationsOfIHardwareInfo().ToList().ForEach(RegisterHardwareInfo);

        /// <summary>
        /// The hardware info dictionary.
        /// </summary>
        private static readonly ConcurrentDictionary<HardwareModel, List<IHardwareInfo>> _hardwareInfos = new();

        /// <summary>
        /// Loads all implementations and derivatives of IHardwareInfo.
        /// </summary>
        /// <returns></returns>
        private static List<IHardwareInfo> LoadImplementationsOfIHardwareInfo()
        {
            var result = new List<IHardwareInfo>();

            // Get the currently executing assembly
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Define a list of base classes that implement IHardwareInfo
            Type[] baseHardwareTypes = { typeof(HDHomeRunHardwareBase), typeof(HauppaugeHardwareBase) };

            // Get all types that derive from any of the base classes that implement IHardwareInfo and are not abstract
            var types = assembly.GetTypes()
                                .Where(t => t.IsClass && !t.IsAbstract && baseHardwareTypes.Any(baseType => baseType.IsAssignableFrom(t)))
                                .ToList();

            // Instantiate each type and add to the result list
            foreach (var type in types)
            {
                IHardwareInfo? instance = (IHardwareInfo?)Activator.CreateInstance(type);

                if (instance == null)
                    continue;

                result.Add(instance);
            }

            return result;
        }


        /// <summary>
        /// Registers a new hardware info with the hardware info provider.
        /// </summary>
        /// <param name="hardwareInfo"></param>
        public static void RegisterHardwareInfo(IHardwareInfo hardwareInfo)
        {
            if (_hardwareInfos.TryGetValue(hardwareInfo.HardwareModel, out List<IHardwareInfo>? models))
            {
                models.Add(hardwareInfo);
                return;
            }
            _hardwareInfos.TryAdd(hardwareInfo.HardwareModel, new List<IHardwareInfo>() { hardwareInfo });
        }

        /// <summary>
        /// Attempts to look up hardware info by a sub model number.
        /// </summary>
        /// <param name="modelNumber"></param>
        /// <returns></returns>
        public static IHardwareInfo GetHardwareInfoByModelNumber(string? modelNumber)
            => _hardwareInfos.Values.SelectMany(hwc => hwc).FirstOrDefault(hw => hw.ModelNumber.Equals(modelNumber, StringComparison.InvariantCultureIgnoreCase), new UknownHardware());

        /// <summary>
        /// Gets all hardware infos under a parent model name.
        /// </summary>
        /// <param name="parentModelName"></param>
        /// <returns></returns>
        public static List<IHardwareInfo> GetHardwareInfosByParentModel(HardwareModel hardwareModel)
            => _hardwareInfos[hardwareModel];
    }
}
