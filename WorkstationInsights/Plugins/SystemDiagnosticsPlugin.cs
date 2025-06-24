using Microsoft.SemanticKernel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.ServiceProcess;

public class SystemDiagnosticsPlugin
{
    [KernelFunction]
    public string RunPowerShell(string command)
    {
        try
        {
            var psi = new ProcessStartInfo("powershell", $"-Command \"{command}\"")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = Process.Start(psi);
            var output = proc?.StandardOutput.ReadToEnd();
            var error = proc?.StandardError.ReadToEnd();

            if (!string.IsNullOrWhiteSpace(error))
                return $"Error: {error.Trim()}";

            return output?.Trim() ?? "No output";
        }
        catch (Exception ex)
        {
            return $"PowerShell execution failed: {ex.Message}";
        }
    }
}
