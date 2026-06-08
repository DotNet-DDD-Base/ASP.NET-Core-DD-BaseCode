using System;
using System.Diagnostics;

namespace UserApp.Infrastructure.Services.CodeGeneration.Shared;

public class CommandRunner
{
    private readonly PathProvider _paths;

    public CommandRunner(PathProvider paths)
    {
        _paths = paths;
    }

    public string Run(string fileName, string arguments)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = _paths.SolutionRoot
            }
        };

        process.Start();

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"Command failed: {fileName} {arguments}\n{error}");
        }

        return output;
    }
}
