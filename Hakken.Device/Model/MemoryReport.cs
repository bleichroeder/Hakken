using System.Text.RegularExpressions;

namespace Hakken.Device.Model
{
    /// <summary>
    /// The device's memory report.
    /// </summary>
    public class MemoryReport
    {
        /// <summary>
        /// Gets the SDRAM in bytes.
        /// </summary>
        public int SDRAMBytes { get; set; }

        /// <summary>
        /// Gets the SDRAM in megabytes.
        /// </summary>
        public int SDRAMMb { get; set; }

        /// <summary>
        /// Gets the Flash in bytes.
        /// </summary>
        public int FlashBytes { get; set; }

        /// <summary>
        /// Gets the Flash in megabytes.
        /// </summary>
        public int FlashMb { get; set; }

        /// <summary>
        /// Creates a new MemoryReport.
        /// </summary>
        /// <param name="memoryReportString"></param>
        public MemoryReport(string memoryReportString)
        {
            SDRAMMb = ParseMemoryValue(memoryReportString, "MB SDRAM");
            FlashMb = ParseMemoryValue(memoryReportString, "MB Flash");

            SDRAMBytes = SDRAMMb * 1024 * 1024;
            FlashBytes = FlashMb * 1024 * 1024;
        }

        /// <summary>
        /// Parses the memory value from the memory report string.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="memoryType"></param>
        /// <returns></returns>
        static int ParseMemoryValue(string input, string memoryType)
        {
            try
            {
                string pattern = $@"(\d+)\s+{Regex.Escape(memoryType)}";
                Match match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);

                if (match.Success && int.TryParse(match.Groups[1].Value, out int value))
                {
                    return value;
                }
            }
            catch { }

            return -1;
        }
    }
}
