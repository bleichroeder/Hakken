using Hakken.Discovery.Model;
using Newtonsoft.Json;

namespace Hakken.Serialization
{
    /// <summary>
    /// Discovery serialization extensions.
    /// </summary>
    public static class DiscoveryExtensions
    {
        /// <summary>
        /// Serializes the <see cref="DiscoveryResponse"/> to a string.
        /// </summary>
        /// <param name="discoveryResponse"></param>
        /// <returns></returns>
        public static string Serialize(this DiscoveryResponse discoveryResponse)
            => JsonConvert.SerializeObject(discoveryResponse, SerializationHelper.DefaultSerializer);

        /// <summary>
        /// Serializes the <see cref="DiscoveryResponse"/> to a string with the specified formatting.
        /// </summary>
        /// <param name="discoveryResponse"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static string Serialize(this DiscoveryResponse discoveryResponse, Formatting formatting)
            => JsonConvert.SerializeObject(discoveryResponse, SerializationHelper.GetSerializer(formatting));

        /// <summary>
        /// Serializes the <see cref="DiscoveryResponse"/> to a string with as a DTO.
        /// </summary>
        /// <param name="discoveryResponse"></param>
        /// <returns></returns>
        public static string SerializeAsDTO(this DiscoveryResponse discoveryResponse)
        {
            var dataTransferObject = new
            {
                discoveryResponse.Devices,
                discoveryResponse.Success,
                discoveryResponse.Message,
                discoveryResponse.DiscoveryElapsedTime
            };

            return JsonConvert.SerializeObject(dataTransferObject, SerializationHelper.DefaultSerializer);
        }

        /// <summary>
        /// Serializes the <see cref="DiscoveryResponse"/> to a string with the specified formatting and settings as a DTO.
        /// </summary>
        /// <param name="discoveryResponse"></param>
        /// <param name="formatting"></param>
        /// <returns></returns>
        public static string SerializeAsDTO(this DiscoveryResponse discoveryResponse, Formatting formatting)
        {
            var dataTransferObject = new
            {
                discoveryResponse.Devices,
                discoveryResponse.Success,
                discoveryResponse.Message,
                discoveryResponse.DiscoveryElapsedTime
            };
            return JsonConvert.SerializeObject(dataTransferObject, SerializationHelper.GetSerializer(formatting));
        }
    }
}
