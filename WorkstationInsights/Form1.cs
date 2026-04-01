using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using WorkstationInsights.Services;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System;

namespace WorkstationInsights
{
    public partial class Form1 : Form
    {
        private string currentLogFilePath;
        private ChatHistory chatHistory = new ChatHistory(Program.SystemPrompt);
        private Dictionary<string, string> threadFileMap = new Dictionary<string, string>();
        private bool safeModeEnabled = true;
        
        private ToolStripButton safeModeToggle;
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private System.Windows.Forms.Timer dashboardTimer;
        private Label cpuLabel, ramLabel;
        private ProgressBar cpuBar, ramBar;
        private BorderedPanel dashboardPanel;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        
        private SystemMonitorService monitorService = new SystemMonitorService();

        public Form1()
        {
            InitializeComponent();
            InitializeChatLogging();
            LoadChatThreads();
            
            InitializeSafeModeToggle();
            InitializeStatusStrip();
            InitializeTrayIcon();
            InitializeDashboard();

            Program.KernelInstance.FunctionInvoking += KernelInstance_FunctionInvoking;
            Program.KernelInstance.FunctionInvoked += KernelInstance_FunctionInvoked;
        }

        private void InitializeStatusStrip()
        {
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel("Ready");
            statusStrip.Items.Add(statusLabel);
            this.Controls.Add(statusStrip);
        }

        private void InitializeTrayIcon()
        {
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Open", null, (s, e) => { this.Show(); this.WindowState = FormWindowState.Normal; });
            trayMenu.Items.Add("Exit", null, (s, e) => Application.Exit());
            trayIcon = new NotifyIcon { Text = "Workstation Insights", Visible = true, ContextMenuStrip = trayMenu };
            trayIcon.DoubleClick += (s, e) => { this.Show(); this.WindowState = FormWindowState.Normal; };
        }

        private void InitializeDashboard()
        {
            dashboardPanel = new BorderedPanel { Dock = DockStyle.Right, Width = 180, BorderWidth = 1, BorderColor = Color.FromArgb(225, 225, 230), BackColor = Color.FromArgb(248, 249, 250), Padding = new Padding(15) };
            this.Controls.Add(dashboardPanel);
            dashboardPanel.BringToFront();

            dashboardPanel.Controls.Add(new Label { Text = "System Health", Font = new Font("Segoe UI", 10F, FontStyle.Bold), Dock = DockStyle.Top, Height = 30 });
            cpuLabel = new Label { Text = "CPU: 0%", Dock = DockStyle.Top, Height = 25 };
            cpuBar = new ProgressBar { Dock = DockStyle.Top, Height = 15, Style = ProgressBarStyle.Continuous };
            dashboardPanel.Controls.Add(cpuBar);
            dashboardPanel.Controls.Add(cpuLabel);
            dashboardPanel.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 15 });
            ramLabel = new Label { Text = "RAM: 0%", Dock = DockStyle.Top, Height = 25 };
            ramBar = new ProgressBar { Dock = DockStyle.Top, Height = 15, Style = ProgressBarStyle.Continuous };
            dashboardPanel.Controls.Add(ramBar);
            dashboardPanel.Controls.Add(ramLabel);

            dashboardTimer = new System.Windows.Forms.Timer { Interval = 2000 };
            dashboardTimer.Tick += (s, e) => {
                var (cpu, ram) = monitorService.GetCurrentMetrics();
                cpuLabel.Text = $"CPU: {cpu:F1}%";
                cpuBar.Value = (int)Math.Clamp(cpu, 0, 100);
                ramLabel.Text = $"RAM: {ram:F1}%";
                ramBar.Value = (int)Math.Clamp(ram, 0, 100);
                cpuBar.ForeColor = cpu > 80 ? Color.Red : Color.FromArgb(0, 120, 212);
                ramBar.ForeColor = ram > 85 ? Color.Red : Color.FromArgb(0, 120, 212);
            };
            dashboardTimer.Start();
        }

        private void InitializeSafeModeToggle()
        {
            safeModeToggle = new ToolStripButton { Text = "Safe Mode: ON", BackColor = Color.FromArgb(235, 245, 235), ForeColor = Color.Green, Alignment = ToolStripItemAlignment.Right };
            safeModeToggle.Click += (s, e) => {
                safeModeEnabled = !safeModeEnabled;
                safeModeToggle.Text = safeModeEnabled ? "Safe Mode: ON" : "Safe Mode: OFF";
                safeModeToggle.BackColor = safeModeEnabled ? Color.FromArgb(235, 245, 235) : Color.FromArgb(255, 235, 235);
                safeModeToggle.ForeColor = safeModeEnabled ? Color.Green : Color.Red;
            };
            toolStrip1.Items.Add(safeModeToggle);
        }

        private void InitializeChatLogging()
        {
            var existing = ChatLogger.ListThreads();
            if (existing.Count == 0) {
                currentLogFilePath = ChatLogger.GetLogPath("Chat_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                ChatLogger.SaveChatLog(currentLogFilePath, chatHistory);
            }
        }

        private void LoadChatThreads()
        {
            threadsListBox.Items.Clear();
            threadFileMap = ChatLogger.ListThreads();
            foreach (var thread in threadFileMap.Keys) threadsListBox.Items.Add(thread);
        }

        // Helper to sanitize string for use as a filename
        private string SanitizeFileName(string name)
        {
            string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            string sanitizedName = name;
            foreach (char c in invalidChars)
            {
                sanitizedName = sanitizedName.Replace(c.ToString(), "_");
            }
            return sanitizedName.Length > 50 ? sanitizedName.Substring(0, 50) : sanitizedName; // Keep it reasonably short
        }


        private void DeleteThreadMenuItem_Click(object sender, EventArgs e)
        {
            if (threadsListBox.SelectedItem == null) return;

            string selectedThreadName = threadsListBox.SelectedItem.ToString();
            if (!threadFileMap.TryGetValue(selectedThreadName, out string filePathToDelete))
            {
                MessageBox.Show("Could not find the file for the selected thread. Please reload.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadChatThreads(); // Attempt to refresh
                return;
            }

            var confirmResult = MessageBox.Show($"Are you sure you want to delete the thread '{selectedThreadName}'?\nThis action cannot be undone.",
                                               "Confirm Delete",
                                               MessageBoxButtons.YesNo,
                                               MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    File.Delete(filePathToDelete);

                    bool wasCurrentChat = (currentLogFilePath == filePathToDelete);

                    threadFileMap.Remove(selectedThreadName);
                    threadsListBox.Items.Remove(selectedThreadName);

                    if (wasCurrentChat)
                    {
                        chatHistoryTextBox.Clear();
                        chatHistory.Clear();
                        currentLogFilePath = null;
                        // Optional: Start a new log file immediately or wait for user to click "New Chat" or send a message.
                        // For now, just clear the view. User can click "New Chat"
                        // Or, select the next available thread, or clear selection.
                        if (threadsListBox.Items.Count > 0)
                        {
                            threadsListBox.SelectedIndex = 0; // Select the first available thread
                            // This will trigger ThreadsListBox_SelectedIndexChanged and load that chat
                        }
                        else
                        {
                            // No threads left, behave like New Chat was clicked
                            NewChatButton_Click(null, EventArgs.Empty); // Call NewChat to reset state
                        }
                    }

                    MessageBox.Show("Thread deleted successfully.", "Delete Thread", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (IOException ioEx)
                {
                    MessageBox.Show($"Error deleting thread (file operation): {ioEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LoadChatThreads(); // Refresh list as file state might be inconsistent
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LoadChatThreads(); // Refresh list
                }
            }
        }

        private void NewChatButton_Click(object sender, EventArgs e)
        {
            chatHistory.Clear();
            chatHistory.AddSystemMessage(Program.SystemPrompt);
            chatHistoryTextBox.Clear();
            currentLogFilePath = ChatLogger.GetLogPath("Chat_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            ChatLogger.SaveChatLog(currentLogFilePath, chatHistory);
            LoadChatThreads();
        }

        private async void SendButton_Click(object sender, EventArgs e)
        {
            var input = inputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            chatHistoryTextBox.AppendText($"> {input}{Environment.NewLine}");
            chatHistory.AddUserMessage(input);
            LogMessage(AuthorRole.User, input);

            try {
                var result = await Program.ChatService.GetChatMessageContentAsync(chatHistory, new Microsoft.SemanticKernel.Connectors.OpenAI.OpenAIPromptExecutionSettings { ToolCallBehavior = Microsoft.SemanticKernel.Connectors.OpenAI.ToolCallBehavior.AutoInvokeKernelFunctions }, Program.KernelInstance);
                if (result.Role == AuthorRole.Assistant) {
                    AppendMarkdownMessage("AI", result.Content ?? "");
                    chatHistory.AddAssistantMessage(result.Content ?? "");
                    LogMessage(AuthorRole.Assistant, result.Content ?? "");
                }
            } catch (Exception ex) {
                chatHistoryTextBox.AppendText($"[Error]: {ex.Message}{Environment.NewLine}");
            }
            inputTextBox.Clear();
        }

        private void ThreadsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (threadsListBox.SelectedItem == null) return;
            if (threadFileMap.TryGetValue(threadsListBox.SelectedItem.ToString(), out string path)) {
                currentLogFilePath = path;
                chatHistoryTextBox.Clear();
                var loaded = ChatLogger.LoadChatFromLog(path);
                if (loaded != null) {
                    chatHistory = loaded;
                    foreach (var msg in chatHistory) {
                        if (msg.Role == AuthorRole.User) chatHistoryTextBox.AppendText($"> {msg.Content}{Environment.NewLine}");
                        else if (msg.Role == AuthorRole.Assistant) AppendMarkdownMessage("AI", msg.Content);
                    }
                }
            }
        }

        private void ApiKeyButton_Click(object sender, EventArgs e)
        {
            var input = Microsoft.VisualBasic.Interaction.InputBox("Enter OpenAI API Key:", "Settings", "");
            if (!string.IsNullOrWhiteSpace(input)) {
                ConfigService.UpdateOpenAiSetting("ApiKey", input);
                MessageBox.Show("API Key updated.");
            }
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            var menu = new ContextMenuStrip();
            menu.Items.Add("GPT-4o", null, (s, e) => ConfigService.UpdateOpenAiSetting("ModelId", "gpt-4o"));
            menu.Items.Add("GPT-3.5-Turbo", null, (s, e) => ConfigService.UpdateOpenAiSetting("ModelId", "gpt-3.5-turbo"));
            menu.Show(Cursor.Position);
        }

        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift) {
                e.SuppressKeyPress = true;
                SendButton_Click(sender, e);
            }
        }
    }

    public class BorderedPanel : Panel
    {
        public int BorderWidth { get; set; } = 1;
        public Color BorderColor { get; set; } = Color.Black;
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            using var pen = new Pen(BorderColor, BorderWidth);
            e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        }
    }
}
