using System.Security;

namespace Hakken.Discovery.Model
{
    /// <summary>
    /// RemoteDeviceDiscoveryRequest.
    /// Required for performing remote device discovery.
    /// </summary>
    public class RemoteDiscoveryRequest
    {
        /// <summary>
        /// The default path to the HDHomeRun Config executable.
        /// </summary>
        private static readonly string DEFAULT_HDHOMERUN_CONFIG_PATH = @"C:\Program Files\SiliconDust\HDHomeRun\hdhomerun_config.exe";

        /// <summary>
        /// Default MAC addresses to be used for remote discovery.
        /// </summary>
        private readonly List<string> DEFAULT_TARGET_MAC_ADDRESSES = new()
        {
            "00-18-DD",
            "0018DD",
            "00-0D-FE",
            "000DFE"
        };

        /// <summary>
        /// Remote host username.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Remote host password.
        /// </summary>
        public SecureString Password { get; set; }

        /// <summary>
        /// Remote host.
        /// </summary>
        public string RemoteHost { get; set; }

        /// <summary>
        /// Use hdhomerun_config.exe on remote host for device discovery.
        /// If set to false, will use ARP parsing method.
        /// </summary>
        public bool UseHDHRConfig { get; set; }

        /// <summary>
        /// Array of full or partial MAC addresses to be used for remote discovery.
        /// Defaults to an array of known MAC addresses.
        /// </summary>
        public List<string> TargetMACs => DEFAULT_TARGET_MAC_ADDRESSES;

        /// <summary>
        /// The local path on the remote host to the HDHomeRun Config executable.
        /// </summary>
        public string? HDHomerunConfigPath { get; set; } = DEFAULT_HDHOMERUN_CONFIG_PATH;

        /// <summary>
        /// Builds a request for remote device discovery using ARP parsing.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="remoteHost"></param>
        public RemoteDiscoveryRequest(string user,
                                      SecureString password,
                                      string remoteHost)
        {
            User = user;
            Password = password;
            RemoteHost = remoteHost;
        }

        /// <summary>
        /// Builds a request for remote device discovery using ARP parsing.
        /// Remote discovery using the HDHomeRunConfig is preferred.
        /// </summary>
        public RemoteDiscoveryRequest(string user,
                                      string password,
                                      string remoteHost)
            : this(user, ConvertToSecureString(password), remoteHost) { }

        /// <summary>
        /// Builds a request for remote device discovery using HDHomeRun Config.
        /// Uses the default installation path for HDHomeRun Config.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="remoteHost"></param>
        /// <param name="useHDHomeRunConfig"></param>
        public RemoteDiscoveryRequest(string user,
                                      string password,
                                      string remoteHost,
                                      bool useHDHomeRunConfig)
        {
            User = user;
            Password = ConvertToSecureString(password);
            RemoteHost = remoteHost;
            UseHDHRConfig = useHDHomeRunConfig;
        }

        /// <summary>
        /// Builds a request for remote device discovery using HDHomeRun Config.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="remoteHost"></param>
        /// <param name="useHDHRConfig"></param>
        /// <param name="hDHomerunConfigPath"></param>
        public RemoteDiscoveryRequest(string user,
                                      SecureString password,
                                      string remoteHost,
                                      bool useHDHRConfig,
                                      string? hDHomerunConfigPath) : this(user, password, remoteHost)
        {
            UseHDHRConfig = useHDHRConfig;
            HDHomerunConfigPath = hDHomerunConfigPath;
        }



        /// <summary>
        /// Builds a request for remote device discovery using HDHomeRun Config.
        /// Uses the specified path for HDHomeRun Config.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="remoteHost"></param>
        /// <param name="hdhomerunPath"></param>
        public RemoteDiscoveryRequest(string user,
                                      string password,
                                      string remoteHost,
                                      string hdhomerunPath)
        {
            User = user;
            Password = ConvertToSecureString(password);
            RemoteHost = remoteHost;
            HDHomerunConfigPath = hdhomerunPath;
            UseHDHRConfig = true;
        }

        /// <summary>
        /// Builds a request for remote device discovery using HDHomeRun Config.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="remoteHost"></param>
        /// <param name="hdhomerunPath"></param>
        public RemoteDiscoveryRequest(string user,
                                      SecureString password,
                                      string remoteHost,
                                      string hdhomerunPath)
        {
            User = user;
            Password = password;
            RemoteHost = remoteHost;
            HDHomerunConfigPath = hdhomerunPath;
            UseHDHRConfig = true;
        }

        /// <summary>
        /// Converts a string to a SecureString.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static SecureString ConvertToSecureString(string password)
        {
            var securePassword = new SecureString();

            foreach (char c in password)
                securePassword.AppendChar(c);

            securePassword.MakeReadOnly();
            return securePassword;
        }
    }
}
