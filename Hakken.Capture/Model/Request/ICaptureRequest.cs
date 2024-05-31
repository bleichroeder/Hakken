namespace Hakken.Capture.Model.Request
{
    public interface ICaptureRequest
    {
        /// <summary>
        /// The tuner number used when building the parameterized input stream URI.
        /// </summary>
        public int TunerNumber { get; set; }

        /// <summary>
        /// The input stream URI.
        /// </summary>
        public Uri InputStreamUri { get; set; }

        /// <summary>
        /// The parameterized input stream URI.
        /// </summary>
        public Uri? ParameterizedInputStreamUri { get; }

        /// <summary>
        /// The transcode profile used when building the parameterized input stream URI.
        /// </summary>
        public TranscodeProfile TranscodeProfile { get; set; }

        /// <summary>
        /// The duration used when building the parameterized input stream URI.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// The output path used when building the output file full path.
        /// This should be a directory.
        /// </summary>
        public string OutputPath { get; }

        /// <summary>
        /// The output file name used when building the output file full path.
        /// This should include the file name and extension.
        /// The extension is extracted and stored in <see cref="OutputFileExtension"/>.
        /// </summary>
        public string OutputFileName { get; }

        /// <summary>
        /// The output file extension used when building the output file full path.
        /// </summary>
        public string OutputFileExtension { get; }

        /// <summary>
        /// The full path of the output file.
        /// </summary>
        public string OutputFileFullPath { get; }
    }
}
