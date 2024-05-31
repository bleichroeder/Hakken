using Hakken.Remote.Utility;

namespace Hakken.Remote.Model
{
    /// <summary>
    /// The result and status of a network connection.
    /// </summary>
    public class NetworkConnectionResult
    {
        /// <summary>
        /// True if the network connection was created.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// True if the connection is active.
        /// </summary>
        public bool IsActive => Connection is not null && Connection.IsActive;

        /// <summary>
        /// The exception thrown while attempting to create the network connection.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// The network connection.
        /// </summary>
        public NetworkConnection? Connection { get; set; }
    }
}
