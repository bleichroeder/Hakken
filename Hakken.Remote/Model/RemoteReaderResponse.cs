namespace Hakken.Remote.Model
{
    public class RemoteReaderResponse
    {
        /// <summary>
        /// True if content was able to be read.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The file path to the content.
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// The remote content.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// The error message is present.
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
