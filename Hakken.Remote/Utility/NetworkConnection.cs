using Hakken.Remote.Model;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.InteropServices;

namespace Hakken.Remote.Utility
{
    /// <summary>
    /// Provides the ability to create a network connection
    /// for remote file access and remote process execution.
    /// </summary>
    public class NetworkConnection : IDisposable
    {
        private readonly string _networkName;

        /// <summary>
        /// True if the network connection is active.
        /// </summary>
        public bool IsActive
        {
            get
            {
                bool active = false;

                try
                {
                    active = Directory.Exists(_networkName);
                }
                catch { }

                return active;
            }
        }

        /// <summary>
        /// Attempts to create a network connection.
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="networkName"></param>
        /// <param name="connectionStatus"></param>
        /// <returns></returns>
        public static bool TryCreateNetworkConnection(NetworkCredential credentials,
                                                      string networkName,
                                                      out NetworkConnectionResult connectionStatus)
        {
            try
            {
                NetworkConnection connection = new(networkName, credentials);
                connectionStatus = new()
                {
                    Connection = connection,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                connectionStatus = new()
                {
                    ErrorMessage = new Win32Exception(ex.HResult).Message
                };
            }
            return connectionStatus.Success;
        }

        public NetworkConnection(string networkName, NetworkCredential credentials)
        {
            _networkName = networkName;
            int num = WNetAddConnection2(new NetResource
            {
                Scope = ResourceScope.GlobalNetwork,
                ResourceType = ResourceType.Disk,
                DisplayType = ResourceDisplaytype.Share,
                RemoteName = networkName
            }, username: string.IsNullOrEmpty(credentials.Domain) ? credentials.UserName : $"{credentials.Domain}\\{credentials.UserName}", password: credentials.Password, flags: 0);

            if (num is not 0 && num is not 1219)
            {
                throw new Win32Exception(num);
            }
        }

        ~NetworkConnection()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        [SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "<Pending>")]
        protected virtual void Dispose(bool disposing)
        {
            WNetCancelConnection2(_networkName, 0, force: true);
        }

        [DllImport("mpr.dll")]
        [SuppressMessage("Globalization", "CA2101:Specify marshaling for P/Invoke string arguments", Justification = "<Pending>")]
        [SuppressMessage("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time", Justification = "<Pending>")]
        private static extern int WNetAddConnection2(NetResource netResource, string password, string username, int flags);

        [DllImport("mpr.dll")]
        [SuppressMessage("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time", Justification = "<Pending>")]
        [SuppressMessage("Globalization", "CA2101:Specify marshaling for P/Invoke string arguments", Justification = "<Pending>")]
        private static extern int WNetCancelConnection2(string name, int flags, bool force);
    }

    [StructLayout(LayoutKind.Sequential)]
    public class NetResource
    {
        public ResourceScope Scope;

        public ResourceType ResourceType;

        public ResourceDisplaytype DisplayType;

        public int Usage;

        public string? LocalName;

        public string? RemoteName;

        public string? Comment;

        public string? Provider;
    }

    public enum ResourceScope
    {
        Connected = 1,
        GlobalNetwork,
        Remembered,
        Recent,
        Context
    }

    public enum ResourceType
    {
        Any = 0,
        Disk = 1,
        Print = 2,
        Reserved = 8
    }

    public enum ResourceDisplaytype
    {
        Generic,
        Domain,
        Server,
        Share,
        File,
        Group,
        Network,
        Root,
        Shareadmin,
        Directory,
        Tree,
        Ndscontainer
    }
}
