using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace SemanticRefactor
{
    public class PowershellPlugin 
    {
        private readonly ILogger<PowershellPlugin> _logger;

        public PowershellPlugin(ILogger<PowershellPlugin> logger)
        {
            _logger = logger;
        }

        [KernelFunction, Description("Execute a command at powershell terminal. Returns standard out")]
        public async Task<string> ExecuteAsync([Description("command")] string command)
        {
            _logger.LogInformation("Executing command {0}", command);

            using (var process = new System.Diagnostics.Process())
            {
                process.StartInfo.FileName = "powershell.exe";
                process.StartInfo.Arguments = command;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                process.WaitForExit();

                return output;
            }
        }
    }
}
