namespace Hakken.Capture.Model
{
    public enum TranscodeProfile
    {
        /// <summary>
        /// No transcode profile will be used.
        /// </summary>
        None,

        /// <summary>
        /// Transcode to AVC with the same resolution, frame-rate, and interlacing as the original stream. For example 1080i60 → AVC 1080i60, 720p60 → AVC 720p60.
        /// </summary>
        Heavy,

        /// <summary>
        /// Trancode to AVC progressive not exceeding 1280×720 30fps.
        /// </summary>
        Mobile,

        /// <summary>
        /// Transcode to low bitrate AVC progressive not exceeding 960×540 30fps.
        /// </summary>
        Internet540,

        /// <summary>
        /// Transcode to low bitrate AVC progressive not exceeding 848×480 30fps for 16:9 content, not exceeding 640×480 30fps for 4:3 content.
        /// </summary>
        Internet480,

        /// <summary>
        /// Transcode to low bitrate AVC progressive not exceeding 640×360 30fps for 16:9 content, not exceeding 480×360 30fps for 4:3 content.
        /// </summary>
        Internet360,

        /// <summary>
        /// Transcode to low bitrate AVC progressive not exceeding 432×240 30fps for 16:9 content, not exceeding 320×240 30fps for 4:3 content.
        /// </summary>
        Internet240
    }
}
