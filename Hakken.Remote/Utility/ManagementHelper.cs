using System.Management;
using System.Runtime.Versioning;
using System.Security;

namespace Hakken.Remote.Utility
{
    /// <summary>
    /// Provides helper methods for interacting with Windows management classes.
    /// </summary>
    [SupportedOSPlatform("Windows")]
    public static class ManagementHelper
    {
        private const string MANAGEMENT_SCOPE = "\\\\{0}\\root\\cimv2";
        private const string MANAGEMENT_PATH = "Win32_Process";
        private const string MANAGEMENT_METHOD = "Create";

        [SupportedOSPlatform("Windows")]
        public static async Task InvokeAsync(string command, string host, string user, SecureString password)
        {
            await Task.Run(() =>
            {
                ConnectionOptions connectionOptions = new()
                {
                    Username = user,
                    SecurePassword = password
                };

                ManagementScope managementScope = new(string.Format(MANAGEMENT_SCOPE, host), connectionOptions);
                managementScope.Connect();

                ObjectGetOptions managementOptions = new();
                ManagementPath path = new(MANAGEMENT_PATH);

                using ManagementClass managementClass = new(managementScope, path, managementOptions);
                managementClass.InvokeMethod(MANAGEMENT_METHOD, new object[1] { command });
            });
        }

        /// <summary>
        /// Converts a string to a SecureString.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static SecureString ConvertToSecureString(string password)
        {
            var securePassword = new SecureString();

            foreach (char c in password)
                securePassword.AppendChar(c);

            securePassword.MakeReadOnly();
            return securePassword;
        }
    }
}
