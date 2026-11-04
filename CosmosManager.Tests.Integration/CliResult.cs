using System.Text.RegularExpressions;

namespace CosmosManager.Tests.Integration
{
    /// <summary>
    /// Result of running a .csql script through the CosmosManager2019.exe command line.
    /// </summary>
    public class CliResult
    {
        public int ExitCode { get; set; }
        public string Output { get; set; } = "";

        /// <summary>True when the exe exited cleanly and no query reported a translation/execution error.</summary>
        public bool Succeeded => ExitCode == 0 && !Output.Contains("Unable to run");

        /// <summary>The "Total Records Returned: N" value from the .CResult, or -1 if absent.</summary>
        public int RecordCount
        {
            get
            {
                var matches = Regex.Matches(Output, @"Total Records Returned:\s*(\d+)");
                if (matches.Count == 0)
                {
                    return -1;
                }
                return int.Parse(matches[matches.Count - 1].Groups[1].Value);
            }
        }
    }
}
