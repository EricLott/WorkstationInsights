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
    private const int DefaultTimeoutSeconds = 30;
    private const int MaxOutputLength = 4000; // Truncate output to prevent token overflow

    [KernelFunction]
    public string RunPowerShell(string command)
    {
        try
        {
            // Implementation of Phase 2: Command Blocklist (Regex)
            string[] blocklist = { "Remove-Item", "Format-Volume", "Set-ExecutionPolicy", "Remove-Partition", "-Force", "del ", "rmdir" };
            foreach (var blocked in blocklist)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(command, $@"\b{blocked}\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    return $"Error: High-risk command '{blocked}' detected and blocked for security.";
                }
            }

            // Implementation of Phase 1: Execution Timeouts & Better Error Handling
            var psi = new ProcessStartInfo("powershell", $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = new Process { StartInfo = psi };
            proc.Start();

            // Using WaitForExit with timeout to prevent hung processes
            bool exited = proc.WaitForExit(DefaultTimeoutSeconds * 1000);
            
            if (!exited)
            {
                proc.Kill();
                return $"Error: PowerShell command timed out after {DefaultTimeoutSeconds} seconds.";
            }

            var output = proc.StandardOutput.ReadToEnd();
            var error = proc.StandardError.ReadToEnd();

            if (!string.IsNullOrWhiteSpace(error))
            {
                // Better Error Parsing: focus on the primary error message
                return $"PowerShell Error: {error.Trim()}";
            }

            if (string.IsNullOrWhiteSpace(output))
                return "No output returned.";

            // Phase 1: Output Truncation
            if (output.Length > MaxOutputLength)
            {
                return output.Substring(0, MaxOutputLength) + "\n\n[Output truncated due to length...]";
            }

            return output.Trim();
        }
        catch (Exception ex)
        {
            return $"PowerShell execution failed: {ex.Message}";
        }
    }
}
