using Hakken.Device.Model;
using Newtonsoft.Json;

namespace Hakken.Serialization
{
    /// <summary>
    /// Device serialization extensions.
    /// </summary>
    public static class DeviceExtensions
    {
        /// <summary>
        /// Serializes the device info to JSON.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public static string? Serialize(this IDeviceInfo deviceInfo)
            => JsonConvert.SerializeObject(deviceInfo, SerializationHelper.DefaultSerializer);

        /// <summary>
        /// Serializes the device info to JSON with the specified formatting.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="formatting"></param>
        /// <returns></returns>
        public static string? Serialize(this IDeviceInfo device, Formatting formatting)
            => JsonConvert.SerializeObject(device, SerializationHelper.GetSerializer(formatting));
    }
}
