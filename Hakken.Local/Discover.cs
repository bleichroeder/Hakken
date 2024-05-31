using Hakken.Device.Model;
using Hakken.Discovery;
using Hakken.Discovery.Model;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Hakken.Local
{
    /// <summary>
    /// Provides methods for discovering HDHR devices on the local or remote networks.
    /// </summary>
    public static class Discover
    {
        /// <summary>
        /// The discovery bytes payload.
        /// </summary>
        private static readonly byte[] DISCOVERY_BYTES = { 0, 2, 0, 12, 1, 4, 255, 255, 255, 255, 2, 4, 255, 255, 255, 255, 115, 204, 125, 143 };

        /// <summary>
        /// Attempts to discover HDHR devices on the local network by UDP broadcast.
        /// </summary>
        /// <returns></returns>
        public static async Task<DiscoveryResponse> DevicesByUDPBroadcast(int receiveTimeout = 500, int sendTimeout = 500)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            List<IPAddress> ipAddresses = new();

            using (var udpClient = new UdpClient())
            {

                udpClient.Client.ReceiveTimeout = receiveTimeout;
                udpClient.Client.SendTimeout = sendTimeout;
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);

                try
                {
                    var localBroadcastAddress = GetLocalBroadcastAddress();
                    udpClient.Send(DISCOVERY_BYTES, DISCOVERY_BYTES.Length, new IPEndPoint(IPAddress.Parse(localBroadcastAddress), 65001));

                    var endpoint = new IPEndPoint(0, 0);

                    byte[]? response = null;

                    while (true)
                    {
                        response = udpClient.Receive(ref endpoint);

                        if (response is not null)
                        {
                            lock (ipAddresses)
                                ipAddresses.Add(endpoint.Address);
                        }
                    }
                }
                catch { }
            }

            DiscoveryResponse retVal = await DiscoveryHelper.FromIPAddresses(ipAddresses, DeviceLocation.Local);
            retVal.DiscoveryElapsedTime = stopwatch.Elapsed;

            return retVal;
        }

        /// <summary>
        /// Gets the local broadcast address.
        /// </summary>
        /// <returns></returns>
        private static string GetLocalBroadcastAddress()
        {
            string? hostname = Dns.GetHostName();
            IPAddress[] addressList = Dns.GetHostEntry(hostname).AddressList;

            string? ipAddressString = addressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork)?.ToString();

            if (string.IsNullOrEmpty(ipAddressString))
                throw new Exception("Unable to determine local IP address.");

            IPAddress ipAddress = IPAddress.Parse(ipAddressString);
            var ipBytes = ipAddress.GetAddressBytes();
            string subnetMask = GetSubnetMask(ipAddress);

            var maskBytes = IPAddress.Parse(subnetMask).GetAddressBytes();
            for (int i = 0; i < ipBytes.Length; i++)
            {
                ipBytes[i] = (byte)(ipBytes[i] | maskBytes[i] ^ 255);
            }

            return new IPAddress(ipBytes).ToString();
        }

        /// <summary>
        /// Gets the subnet mask for the specified IP address.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        private static string GetSubnetMask(IPAddress ipAddress)
        {
            var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
                .FirstOrDefault(nic => nic.GetIPProperties().UnicastAddresses
                .Any(unicast => unicast.Address.Equals(ipAddress)));

            if (networkInterface is not null)
            {
                var unicast = networkInterface.GetIPProperties().UnicastAddresses
                    .First(unicast => unicast.Address.Equals(ipAddress));

                return unicast.IPv4Mask.ToString();
            }

            return string.Empty;
        }
    }
}