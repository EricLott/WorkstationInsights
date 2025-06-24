using Microsoft.SemanticKernel;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Collections.Generic;
using Microsoft.Win32;

public class SystemDiagnosticsPlugin
{
    [KernelFunction]
    public string GetOSVersion() => Environment.OSVersion.ToString();

    [KernelFunction]
    public string GetSystemUptime()
    {
        var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
        return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m";
    }

    [KernelFunction]
    public Dictionary<string, string> GetEnvironmentVariables()
    {
        return Environment.GetEnvironmentVariables()
                         .Cast<System.Collections.DictionaryEntry>()
                         .ToDictionary(k => k.Key.ToString(), v => v.Value?.ToString());
    }

    [KernelFunction]
    public string GetSystemManufacturer()
    {
        using var searcher = new ManagementObjectSearcher("SELECT Manufacturer, Model FROM Win32_ComputerSystem");
        foreach (var obj in searcher.Get())
        {
            return $"{obj["Manufacturer"]} {obj["Model"]}";
        }
        return "Unknown";
    }

    [KernelFunction]
    public string GetCpuUsage()
    {
        using var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
        cpuCounter.NextValue();
        System.Threading.Thread.Sleep(1000);
        return $"{cpuCounter.NextValue():F1}%";
    }

    [KernelFunction]
    public string GetLogicalProcessorCount() => Environment.ProcessorCount.ToString();

    [KernelFunction]
    public string GetTotalPhysicalMemory()
    {
        using var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
        foreach (var obj in searcher.Get())
        {
            return FormatBytes(Convert.ToUInt64(obj["TotalPhysicalMemory"]));
        }
        return "Unknown";
    }

    [KernelFunction]
    public string GetAvailablePhysicalMemory()
    {
        using var searcher = new ManagementObjectSearcher("SELECT FreePhysicalMemory FROM Win32_OperatingSystem");
        foreach (var obj in searcher.Get())
        {
            var kbytes = Convert.ToUInt64(obj["FreePhysicalMemory"]);
            return FormatBytes(kbytes * 1024);
        }
        return "Unknown";
    }

    [KernelFunction]
    public string GetMemoryUsagePercent()
    {
        var total = Convert.ToDouble(GetTotalPhysicalMemory().Replace(" bytes", ""));
        var free = Convert.ToDouble(GetAvailablePhysicalMemory().Replace(" bytes", ""));
        var used = total - free;
        return $"{(used / total * 100):F1}%";
    }

    [KernelFunction]
    public List<string> GetDriveInfo()
    {
        return DriveInfo.GetDrives()
                        .Where(d => d.IsReady)
                        .Select(d => $"{d.Name} - {FormatBytes((ulong)d.AvailableFreeSpace)} free of {FormatBytes((ulong)d.TotalSize)}")
                        .ToList();
    }

    [KernelFunction]
    public List<string> GetActiveNetworkInterfaces()
    {
        return NetworkInterface.GetAllNetworkInterfaces()
            .Where(ni => ni.OperationalStatus == OperationalStatus.Up)
            .Select(ni => $"{ni.Name} - {ni.NetworkInterfaceType}")
            .ToList();
    }

    [KernelFunction]
    public string PingTest(string hostname)
    {
        try
        {
            using var ping = new Ping();
            var reply = ping.Send(hostname);
            return reply.Status == IPStatus.Success ? $"Ping: {reply.RoundtripTime}ms" : "Ping failed";
        }
        catch
        {
            return "Ping error";
        }
    }

    [KernelFunction]
    public List<string> ListTopCpuProcesses()
    {
        var results = new List<string>();
        foreach (var process in Process.GetProcesses())
        {
            try
            {
                results.Add($"{process.ProcessName} - CPU Time: {process.TotalProcessorTime.TotalSeconds:F0}s");
            }
            catch { }
        }
        return results.OrderByDescending(p =>
        {
            var split = p.Split(':');
            return double.TryParse(split.Last().Replace("s", "").Trim(), out var secs) ? secs : 0;
        }).Take(5).ToList();
    }

    [KernelFunction]
    public List<string> ListTopMemoryProcesses()
    {
        return Process.GetProcesses()
            .OrderByDescending(p => p.WorkingSet64)
            .Take(5)
            .Select(p => $"{p.ProcessName} - RAM: {FormatBytes((ulong)p.WorkingSet64)}")
            .ToList();
    }

    [KernelFunction]
    public bool IsProcessRunning(string processName)
    {
        return Process.GetProcessesByName(processName).Any();
    }

    [KernelFunction]
    public List<string> GetEventLogs(string logName = "System")
    {
        try
        {
            using var log = new EventLog(logName);
            return log.Entries.Cast<EventLogEntry>()
                .OrderByDescending(e => e.TimeGenerated)
                .Take(10)
                .Select(e => $"[{e.TimeGenerated}] {e.EntryType}: {e.Source} - {e.Message}")
                .ToList();
        }
        catch
        {
            return new List<string> { "Error reading event log." };
        }
    }

    [KernelFunction]
    public List<string> ListRunningServices()
    {
        return ServiceController.GetServices()
            .Where(s => s.Status == ServiceControllerStatus.Running)
            .Select(s => s.DisplayName)
            .ToList();
    }

    [KernelFunction]
    public string CheckServiceStatus(string serviceName)
    {
        var svc = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == serviceName);
        return svc != null ? svc.Status.ToString() : "Not found";
    }

    [KernelFunction]
    public string GetWindowsDefenderStatus()
    {
        using var searcher = new ManagementObjectSearcher("root\\SecurityCenter2", "SELECT * FROM AntiVirusProduct");
        foreach (var obj in searcher.Get())
        {
            return $"{obj["displayName"]} - {obj["productState"]}";
        }
        return "No antivirus status found.";
    }

    [KernelFunction]
    public string GetFirewallStatus()
    {
        try
        {
            using var mgr = new ManagementClass("root\\StandardCimv2", "MSFT_NetFirewallProfile", null);
            foreach (ManagementObject profile in mgr.GetInstances())
            {
                return $"{profile["Name"]}: Enabled={profile["Enabled"]}";
            }
        }
        catch { }
        return "Could not determine firewall status.";
    }

    [KernelFunction]
    public string GetLastRebootTime()
    {
        using var searcher = new ManagementObjectSearcher("SELECT LastBootUpTime FROM Win32_OperatingSystem");
        foreach (ManagementObject obj in searcher.Get())
        {
            var bootTime = ManagementDateTimeConverter.ToDateTime(obj["LastBootUpTime"].ToString());
            return bootTime.ToString();
        }
        return "Unknown";
    }

    [KernelFunction]
    public string GetLoggedInUser() => Environment.UserName;

    [KernelFunction]
    public string GetIpAddress()
    {
        return NetworkInterface.GetAllNetworkInterfaces()
            .Where(ni => ni.OperationalStatus == OperationalStatus.Up)
            .SelectMany(ni => ni.GetIPProperties().UnicastAddresses)
            .Where(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            .Select(ip => ip.Address.ToString())
            .FirstOrDefault() ?? "Unknown";
    }

    [KernelFunction]
    public List<string> GetDnsServers()
    {
        return NetworkInterface.GetAllNetworkInterfaces()
            .Where(ni => ni.OperationalStatus == OperationalStatus.Up)
            .SelectMany(ni => ni.GetIPProperties().DnsAddresses)
            .Select(dns => dns.ToString())
            .Distinct()
            .ToList();
    }

    [KernelFunction]
    public bool IsUserAdmin()
    {
        using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
        var principal = new System.Security.Principal.WindowsPrincipal(identity);
        return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
    }

    [KernelFunction]
    public string GetBatteryStatus()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT EstimatedChargeRemaining, BatteryStatus FROM Win32_Battery");
            foreach (ManagementObject obj in searcher.Get())
            {
                return $"Charge: {obj["EstimatedChargeRemaining"]}% - Status: {obj["BatteryStatus"]}";
            }
        }
        catch { }
        return "Battery not available";
    }

    [KernelFunction]
    public string GetGpuInfo()
    {
        using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController");
        foreach (var obj in searcher.Get())
        {
            return obj["Name"].ToString();
        }
        return "Unknown";
    }

    [KernelFunction]
    public string GetBiosVersion()
    {
        using var searcher = new ManagementObjectSearcher("SELECT SMBIOSBIOSVersion FROM Win32_BIOS");
        foreach (var obj in searcher.Get())
        {
            return obj["SMBIOSBIOSVersion"].ToString();
        }
        return "Unknown";
    }

    [KernelFunction]
    public string GetMotherboardInfo()
    {
        using var searcher = new ManagementObjectSearcher("SELECT Product, Manufacturer FROM Win32_BaseBoard");
        foreach (var obj in searcher.Get())
        {
            return $"{obj["Manufacturer"]} {obj["Product"]}";
        }
        return "Unknown";
    }

    [KernelFunction]
    public string GetSystemBootMode()
    {
        using var searcher = new ManagementObjectSearcher("SELECT BootMode FROM Win32_ComputerSystem");
        foreach (var obj in searcher.Get())
        {
            return obj["BootMode"]?.ToString() ?? "Unknown";
        }
        return "Unknown";
    }

    [KernelFunction]
    public List<string> ListInstalledPrograms()
    {
        var results = new List<string>();
        using var regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall");
        foreach (var subKeyName in regKey.GetSubKeyNames())
        {
            using var subKey = regKey.OpenSubKey(subKeyName);
            var name = subKey.GetValue("DisplayName") as string;
            var version = subKey.GetValue("DisplayVersion") as string;
            if (!string.IsNullOrWhiteSpace(name))
                results.Add($"{name} {(version != null ? "v" + version : string.Empty)}");
        }
        return results;
    }

    [KernelFunction]
    public bool IsProgramInstalled(string name)
    {
        return ListInstalledPrograms().Any(p => p.Contains(name, StringComparison.OrdinalIgnoreCase));
    }

    [KernelFunction]
    public string GetWifiSsid()
    {
        var output = RunCmd("netsh wlan show interfaces");
        var line = output.Split('\n').FirstOrDefault(l => l.TrimStart().StartsWith("SSID"));
        return line?.Split(':').Last().Trim() ?? "Not connected";
    }

    [KernelFunction]
    public string FlushDns() => RunCmd("ipconfig /flushdns");

    [KernelFunction]
    public string RenewIp() => RunCmd("ipconfig /renew");

    [KernelFunction]
    public string RunDiskCheck() => RunCmd("chkdsk");

    private string RunCmd(string cmd)
    {
        var psi = new ProcessStartInfo("cmd", $"/c {cmd}")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        using var process = Process.Start(psi);
        return process.StandardOutput.ReadToEnd();
    }

    private string FormatBytes(ulong bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}
