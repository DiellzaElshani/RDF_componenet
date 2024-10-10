using System;
using System.Diagnostics;
using System.IO;

namespace GraphWebsite.Server.old
{
    public class CommandLine
    {
        public static string RunCommand(string commandToRun, string workingDirectory = null)
        {
            if (string.IsNullOrEmpty(workingDirectory))
            {
                workingDirectory = Directory.GetCurrentDirectory(); // Better default to the current directory
            }

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe", // Use cmd.exe explicitly
                Arguments = $"/c {commandToRun}", // Use /c to execute the command and terminate cmd.exe
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory
            };

            using (var process = Process.Start(processStartInfo))
            {
                if (process == null)
                {
                    throw new Exception("Process could not be started.");
                }

                // Read the output and error streams
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                // Optionally handle error output
                if (process.ExitCode != 0)
                {
                    throw new Exception($"Command failed with exit code {process.ExitCode}: {error}");
                }

                return output;
            }
        }
    }
}
