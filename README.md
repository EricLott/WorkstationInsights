# WorkstationInsights

A Semantic Kernel plugin to diagnose and monitor system health on a Windows machine. This plugin exposes various system-level diagnostics as functions, allowing LLM agents to query the state of the machine using natural language.

---

## Download

Get the latest pre-compiled version from the **[Releases](https://github.com/EricLott/WorkstationInsights/releases)** page. 

1.  Download the latest `WorkstationInsights.zip`.
2.  Extract to a folder of your choice.
3.  Run `WorkstationInsights.exe`.

---

## Features

- OS and system info
- CPU, memory, and disk usage
- Running processes and services
- Event logs and uptime
- Network and ping tests
- Environment variables
---

## PowerShell Engine

The core of this plugin is an open-ended PowerShell execution engine that allows the AI to craft and execute any diagnostic script.

- **Execution Guardrails**: Timeouts and safety prompts prevent hung or dangerous operations.
- **Structured Data**: The AI favors `ConvertTo-Json` output for highly accurate diagnostics.
- **Auto-Truncation**: Intelligent handling of large command outputs to stay within LLM context limits.

---

## Use Cases

### Natural Language Troubleshooting
> "Why is my computer lagging?"
- Calls `GetCpuUsage`, `GetMemoryUsagePercent`, `ListTopCpuProcesses`, etc.
- LLM provides a diagnosis like: "Your CPU usage is at 85%, and Chrome is using the most resources."

### Automated Health Check
> Daily or hourly health scans
- Calls uptime, updates, disk info, and running services
- Summarized into a status report by the LLM

### Network Diagnostics
> "Is the internet down?"
- Calls `PingTest("8.8.8.8")`, `GetActiveNetworkInterfaces`
- LLM summarizes connection status and suggestions

### Debugging App or Service Problems
> "Why won’t SQL Server start?"
- Calls `CheckServiceStatus`, `GetEventLogs("Application")`
- LLM diagnoses from service state and logs

### System Inventory Bot
> "How much RAM and CPU does this machine have?"
- Calls `GetTotalPhysicalMemory`, `GetLogicalProcessorCount`
- LLM replies with a concise system spec

### Auto-Shutdown or Alert Triggers
> Alert when RAM > 90%
- Kernel auto-checks memory and CPU
- Triggers alert or flow when thresholds are exceeded

### User-Friendly System Clean-up Suggestions
> "How can I free up space?"
- Uses `GetDriveInfo`, `ListTopMemoryProcesses`
- GPT suggests uninstalling or clearing large files

---

## Getting Started

1. Add this plugin class to your Semantic Kernel project.
2. Register with the Kernel:

```csharp
builder.Plugins.AddFromType<WorkstationInsights>();
```

3. Now your LLM can invoke diagnostics like:

> "Show the top memory-using processes"

---

## Structure

This plugin uses:
- `System.Diagnostics`
- `System.Management`
- `System.IO`
- `System.ServiceProcess`
- `System.Net.NetworkInformation`

No external dependencies are required.

---

## License

MIT — use freely in personal or commercial Semantic Kernel applications.
