﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using Injectio.Attributes;
using NoeticTools.Git2SemVer.Core.Exceptions;
using NoeticTools.Git2SemVer.Core.Logging;


namespace NoeticTools.Git2SemVer.Core.Tools;

[RegisterTransient]
public sealed class ProcessCli : IProcessCli
{
    public ProcessCli(ILogger logger)
    {
        WorkingDirectory = Environment.CurrentDirectory;
        Logger = logger;
    }

    public ILogger Logger { get; }

    public int TimeLimitMilliseconds { get; set; } = 60000;

    public string WorkingDirectory { get; set; }

    public (int returnCode, string stdOutput) Run(string application, string commandLineArguments)
    {
        var outWriter = new StringWriter();
        var errorWriter = new StringWriter();
        var returnCode = Run(application, commandLineArguments, outWriter, errorWriter);
        var output = outWriter.ToString();
        var errorOutput = errorWriter.ToString();
        if (!string.IsNullOrWhiteSpace(errorOutput) && returnCode == 0)
        {
            Logger.LogInfo(output);
            throw new Git2SemVerArgumentException($"ERROR: {errorOutput}\nOUTPUT:\n{output}");
        }

        return (returnCode, output);
    }

    /// <summary>
    ///     Run dotnet cli with provided command line arguments.
    /// </summary>
    public int Run(string application, string commandLineArguments,
                   TextWriter standardOut, TextWriter? errorOut = null)
    {
        Logger.LogTrace($"Running '{application} {commandLineArguments}'.");

        using var process = new Process();
        process.StartInfo.FileName = application;
        process.StartInfo.Arguments = commandLineArguments;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;

        if (WorkingDirectory.Length > 0)
        {
            process.StartInfo.WorkingDirectory = WorkingDirectory;
        }

        process.OutputDataReceived += (sender, data) => OnOutputDataReceived(data.Data, standardOut);

        if (errorOut != null)
        {
            process.ErrorDataReceived += (sender, data) => OnErrorDataReceived(data.Data, errorOut);
        }

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        var completed = process.WaitForExit(TimeLimitMilliseconds);
        if (completed)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                process.WaitForExit();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // hack! to allow time for standard outputs to be received
                Thread.Sleep(25);
            }
        }

        if (!completed)
        {
            var message =
                $"ProcessCli Run timed out after {TimeLimitMilliseconds} milliseconds. Command was 'dotnet {commandLineArguments}'.";
            OnError(errorOut, message);
            process.Kill();
            process.WaitForExit(30000);
        }

        var exitCode = process.ExitCode;
        if (exitCode != 0)
        {
            var message = $"ProcessCli Run returned non-zero exit code {exitCode}.";
            OnError(errorOut, message);
        }

        standardOut.Flush();

        return exitCode;
    }

    private void OnError(TextWriter? errorOut, string message)
    {
        errorOut?.WriteLine(message);
        if (errorOut == null)
        {
            Logger.LogError(message);
        }
    }

    private static void OnErrorDataReceived(string? data, TextWriter errorOut)
    {
        if (data == null)
        {
            return;
        }

        errorOut.WriteLine(data);
    }

    private static void OnOutputDataReceived(string? data, TextWriter standardOut)
    {
        if (data == null)
        {
            return;
        }

        standardOut.WriteLine(data);
    }
}