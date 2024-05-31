using Hakken.Remote.Model;
using System.Net;
using System.Security;

namespace Hakken.Remote.Utility
{
    /// <summary>
    /// Provides methods for accessing content on a remote host.
    /// </summary>
    public static class RemoteReader
    {
        /// <summary>
        /// Attempts to read content on a remote host.
        /// </summary>
        /// <param name="remoteContentPath"></param>
        /// <returns></returns>
        public static async Task<RemoteReaderResponse> GetRemoteContentAsync(string remoteContentPath)
            => await GetRemoteContentAsync(remoteContentPath, false, string.Empty, new SecureString(), CancellationToken.None);

        /// <summary>
        /// Attempts to read content on a remote host.
        /// </summary>
        /// <param name="remoteContentPath"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static async Task<RemoteReaderResponse> GetRemoteContentAsync(string remoteContentPath, string user, SecureString password)
            => await GetRemoteContentAsync(remoteContentPath, false, user, password, CancellationToken.None);

        /// <summary>
        /// Attempts to read content on a remote host.
        /// </summary>
        /// <param name="remoteContentPath"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<RemoteReaderResponse> GetRemoteContentAsync(string remoteContentPath, CancellationToken cancellationToken)
            => await GetRemoteContentAsync(remoteContentPath, false, string.Empty, new SecureString(), cancellationToken);

        /// <summary>
        /// Attempts to read content on a remote host.
        /// </summary>
        /// <param name="remoteContentPath"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static async Task<RemoteReaderResponse> GetRemoteContentAsync(string remoteContentPath, bool deleteFile, string user, SecureString password, CancellationToken cancellationToken)
        {
            RemoteReaderResponse response = new()
            {
                FilePath = remoteContentPath
            };

            string? directoryName = Path.GetDirectoryName(remoteContentPath);

            if (string.IsNullOrEmpty(directoryName))
            {
                response.ErrorMessage = $"Unable to parse the directory name from {remoteContentPath}.";
                return response;
            }

            NetworkCredential credentials = new(user, password);

            if (NetworkConnection.TryCreateNetworkConnection(credentials, directoryName, out NetworkConnectionResult connectionStatus) == false)
            {
                response.ErrorMessage = connectionStatus.ErrorMessage;
            }

            using (connectionStatus.Connection)
            {
                string remoteFilePath = remoteContentPath;

                string? locateError = null;
                int fileExistsTries = 0;
                bool fileExists = File.Exists(remoteFilePath);
                while (!fileExists && fileExistsTries < 20)
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                        fileExists = File.Exists(remoteFilePath);
                        locateError = null;
                    }
                    catch (Exception ex) { locateError = ex.Message; }

                    fileExistsTries++;
                }

                if (!fileExists)
                {
                    response.ErrorMessage = locateError;
                    return response;
                }

                string? readError = null;
                int fileReadTries = 0;
                bool fileRead = false;
                while (!fileRead && fileReadTries < 10)
                {
                    try
                    {
                        response.Content = (await File.ReadAllTextAsync(remoteFilePath, cancellationToken)).ToUpper();
                        response.Success = true;
                        fileRead = true;
                        readError = null;

                        if (deleteFile) { File.Delete(remoteFilePath); }
                    }
                    catch (Exception ex) { readError = ex.Message; }

                    fileReadTries++;
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }

                if (fileRead == false)
                    response.ErrorMessage = readError;
            }

            if (string.IsNullOrEmpty(response.Content))
            {
                response.ErrorMessage ??= "Unable to read the remote discovery file content.";
            }

            return response;
        }
    }
}
