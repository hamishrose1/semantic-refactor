using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Diagnostics;

namespace SemanticRefactor
{
    public class DirectoryIOPlugin
    {
        private readonly ILogger _logger;

        public DirectoryIOPlugin(ILogger<DirectoryIOPlugin> logger)
        {
            _logger = logger;    
        }

        /// <summary>
        /// Read a file
        /// </summary>
        /// <example>
        /// {{file.readAsync $path }} => "hello world"
        /// </example>
        /// <param name="path"> Source file </param>
        /// <returns> File content </returns>
        [KernelFunction, Description("Get directory contents")]
        public async Task<string> List([Description("directory path")] string path)
        {
            _logger.LogInformation("Executing directory list: {0}", path);
            try
            {
                using (var process = new Process())
                {
                    // Set up process start info
                    process.StartInfo.FileName = IsWindows() ? "powershell.exe" : "bash";
                    process.StartInfo.Arguments = IsWindows() ? $"ls \"{path}\"" : $"-c \"ls {path}\"";
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;

                    // Start the process
                    process.Start();
                    string output = await process.StandardOutput.ReadToEndAsync();
                    process.WaitForExit();

                    return output;
                }
            }
            catch (Exception ex)
            {
                return $"Error executing command: {ex.Message}";
            }
        }

        [KernelFunction, Description("Create a directory")]
        public async Task<string> CreateDirectory([Description("directory path")] string path)
        {
            try
            {
                // Check if the directory already exists
                if (Directory.Exists(path))
                {
                    return $"Directory '{path}' already exists.";
                }

                // Create the directory
                Directory.CreateDirectory(path);
                return $"Directory '{path}' created successfully.";
            }
            catch (Exception ex)
            {
                return $"Error creating directory: {ex.Message}";
            }
        }



        private bool IsWindows()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT;
        }
    }

}
