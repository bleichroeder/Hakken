using Hakken.Capture.FFmpeg.Model;
using Hakken.Capture.Model;
using Hakken.Capture.Model.Request;
using System.Diagnostics;

namespace Hakken.Capture.FFmpeg
{
    /// <summary>
    /// Provides methods for capturing streams using FFmpeg.
    /// </summary>
    public class FFmpegCaptureUtility
    {
        private string _ffmpegPath = "ffmpeg"; // Default path. Assumes ffmpeg is in system's PATH

        // Event hooks for standard output and error
        public event Action<string?> OnStandardOutput = _ => { };
        public event Action<string?> OnStandardError = _ => { };

        /// <summary>
        /// Creates a new instance of FFmpegCaptureUtility with default settings.
        /// </summary>
        /// <returns></returns>
        public static FFmpegCaptureUtility CreateWithDefaults() => new();

        /// <summary>
        /// Creates a new instance of FFmpegCaptureUtility with the specified path to the FFmpeg executable.
        /// </summary>
        /// <param name="ffmpegPath"></param>
        /// <returns></returns>
        public static FFmpegCaptureUtility Create(string ffmpegPath)
            => new(ffmpegPath, _ => { });

        /// <summary>
        /// Creates a new instance of FFmpegCaptureUtility with the specified event hooks.
        /// </summary>
        /// <param name="onStandardErrorEvent"></param>
        /// <param name="onStandardOutputEvent"></param>
        /// <returns></returns>
        public static FFmpegCaptureUtility Create(Action<string?> onStandardErrorEvent,
                                                  Action<string?>? onStandardOutputEvent = null)
            => new(null, onStandardErrorEvent, onStandardOutputEvent);

        /// <summary>
        /// Creates a new instance of FFmpegCaptureUtility with the specified path to the FFmpeg executable and event hooks.
        /// </summary>
        /// <param name="ffmpegPath"></param>
        /// <param name="onStandardErrorEvent"></param>
        /// <param name="onStandardOutputEvent"></param>
        /// <returns></returns>
        public static FFmpegCaptureUtility Create(string ffmpegPath, Action<string?> onStandardErrorEvent,
                                                                     Action<string?>? onStandardOutputEvent = null)
        {
            FFmpegCaptureUtility instance = new(ffmpegPath, onStandardErrorEvent, onStandardOutputEvent);
            instance.SetFFmpegPath(ffmpegPath);
            return instance;
        }

        private FFmpegCaptureUtility() { }

        /// <summary>
        /// Creates a new instance of FFmpegCaptureUtility with the specified path to the FFmpeg executable and event hooks.
        /// </summary>
        /// <param name="ffmpegPath"></param>
        /// <param name="onStandardErrorEvent"></param>
        /// <param name="onStandardOutputEvent"></param>
        private FFmpegCaptureUtility(string? ffmpegPath,
                                     Action<string?>? onStandardErrorEvent = null,
                                     Action<string?>? onStandardOutputEvent = null)
        {
            if (ffmpegPath is not null)
            {
                SetFFmpegPath(ffmpegPath);
            }
            if (onStandardErrorEvent is not null)
            {
                OnStandardError = onStandardErrorEvent;
            }
            if (onStandardOutputEvent is not null)
            {
                OnStandardOutput = onStandardOutputEvent;
            }
        }

        /// <summary>
        /// Sets the path to the FFmpeg executable.
        /// If the path is not set, the default path is "ffmpeg" and assumes ffmpeg is in the system's PATH.
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public void SetFFmpegPath(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("Path cannot be null or empty.");
            if (!File.Exists(path)) throw new FileNotFoundException("FFmpeg executable not found.", path);
            _ffmpegPath = path;
        }

        /// <summary>
        /// Attempts to capture a stream using FFmpeg.
        /// </summary>
        /// <param name="captureRequest"></param>
        /// <returns></returns>
        public async Task<FFmpegCaptureResult> Capture(FFmpegCaptureRequest captureRequest)
            => await Capture(captureRequest.ParameterizedInputStreamUri,
                             captureRequest.OutputFileFullPath,
                             captureRequest.FFmpegCommand);

        /// <summary>
        /// Attempts to capture a stream using FFmpeg.
        /// </summary>
        /// <param name="streamUri"></param>
        /// <param name="outputPath"></param>
        /// <param name="fileName"></param>
        /// <param name="outputExtension"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<FFmpegCaptureResult> Capture(Uri streamUri, string outputFileFullPath, string ffmpegCommand)
        {
            FFmpegCaptureResult result = new()
            {
                OutputFullFilePath = outputFileFullPath,
                ErrorHeader = await CaptureUtility.GetHDHomeRunErrorHeaderValueAsync(streamUri.ToString())
            };

            if (result.ResponseDefinition != ResponseDefinition.Success)
            {
                return result;
            }

            result.ProcessResult = await ExecuteFFmpegCommand(ffmpegCommand);

            return result;
        }

        /// <summary>
        /// Executes the FFmpeg command and returns the result.
        /// </summary>
        /// <param name="ffmpegCommand"></param>
        /// <returns></returns>
        private async Task<FFmpegProcessResult> ExecuteFFmpegCommand(string ffmpegCommand)
        {
            FFmpegProcessResult result = new()
            {
                Command = ffmpegCommand
            };

            ProcessStartInfo startInfo = new()
            {
                FileName = _ffmpegPath,
                Arguments = ffmpegCommand,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            using Process process = new() { StartInfo = startInfo };

            process.OutputDataReceived += (s, e) => OnStandardOutput(e.Data);
            process.ErrorDataReceived += (s, e) => OnStandardError(e.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();

            result.ExitCode = process.ExitCode;

            return result;
        }
    }
}
