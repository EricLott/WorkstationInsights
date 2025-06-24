namespace WorkstationInsights
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton newChatButton;
        private System.Windows.Forms.ToolStripButton apiKeyButton;
        private System.Windows.Forms.ToolStripButton settingsButton;
        private System.Windows.Forms.RichTextBox chatHistoryTextBox;
        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.Button sendButton;
        private WorkstationInsights.ThreadListBox threadsListBox;
        private WorkstationInsights.BorderedPanel inputPanel;
        private WorkstationInsights.BorderedPanel mainContentPanel;
        private System.Windows.Forms.ContextMenuStrip threadsContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem renameThreadMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteThreadMenuItem;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.newChatButton = new System.Windows.Forms.ToolStripButton();
            this.apiKeyButton = new System.Windows.Forms.ToolStripButton();
            this.settingsButton = new System.Windows.Forms.ToolStripButton();
            this.chatHistoryTextBox = new System.Windows.Forms.RichTextBox();
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.threadsListBox = new WorkstationInsights.ThreadListBox();
            this.inputPanel = new WorkstationInsights.BorderedPanel();
            this.mainContentPanel = new WorkstationInsights.BorderedPanel();
            this.threadsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renameThreadMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteThreadMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            this.SuspendLayout();

            //
            // threadsContextMenuStrip
            //
            this.threadsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameThreadMenuItem,
            this.deleteThreadMenuItem});
            this.threadsContextMenuStrip.Name = "threadsContextMenuStrip";
            this.threadsContextMenuStrip.Size = new System.Drawing.Size(118, 48);
            //
            // renameThreadMenuItem
            //
            this.renameThreadMenuItem.Name = "renameThreadMenuItem";
            this.renameThreadMenuItem.Size = new System.Drawing.Size(117, 22);
            this.renameThreadMenuItem.Text = "Rename";
            this.renameThreadMenuItem.Click += new System.EventHandler(this.RenameThreadMenuItem_Click); // Added event handler
            //
            // deleteThreadMenuItem
            //
            this.deleteThreadMenuItem.Name = "deleteThreadMenuItem";
            this.deleteThreadMenuItem.Size = new System.Drawing.Size(117, 22);
            this.deleteThreadMenuItem.Text = "Delete";
            this.deleteThreadMenuItem.Click += new System.EventHandler(this.DeleteThreadMenuItem_Click); // Added event handler

            // toolStrip1
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.newChatButton,
                this.apiKeyButton,
                this.settingsButton});
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 0;

            this.newChatButton.Text = "New Chat";
            this.newChatButton.Click += new System.EventHandler(this.NewChatButton_Click);

            this.apiKeyButton.Text = "Manage API Key";
            this.apiKeyButton.Click += new System.EventHandler(this.ApiKeyButton_Click);

            this.settingsButton.Text = "Settings";
            this.settingsButton.Click += new System.EventHandler(this.SettingsButton_Click);

            // threadsListBox
            this.threadsListBox.BackColor = System.Drawing.Color.White;
            this.threadsListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.threadsListBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.threadsListBox.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.threadsListBox.FormattingEnabled = true;
            this.threadsListBox.ItemHeight = 36;
            this.threadsListBox.Location = new System.Drawing.Point(0, 25);
            this.threadsListBox.Name = "threadsListBox";
            this.threadsListBox.Size = new System.Drawing.Size(220, 525);
            this.threadsListBox.TabIndex = 4;
            this.threadsListBox.ContextMenuStrip = this.threadsContextMenuStrip;
            this.threadsListBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ThreadsListBox_MouseDown);

            // chatHistoryTextBox
            this.chatHistoryTextBox.BackColor = System.Drawing.Color.White;
            this.chatHistoryTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chatHistoryTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chatHistoryTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chatHistoryTextBox.Location = new System.Drawing.Point(10, 10);
            this.chatHistoryTextBox.Name = "chatHistoryTextBox";
            this.chatHistoryTextBox.ReadOnly = true;
            this.chatHistoryTextBox.Size = new System.Drawing.Size(560, 405);
            this.chatHistoryTextBox.TabIndex = 0;
            this.chatHistoryTextBox.Text = "";
            this.chatHistoryTextBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.ChatHistoryTextBox_LinkClicked);

            // inputTextBox
            this.inputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.inputTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.inputTextBox.Location = new System.Drawing.Point(10, 10);
            this.inputTextBox.Margin = new System.Windows.Forms.Padding(10);
            this.inputTextBox.Multiline = true;
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.inputTextBox.Size = new System.Drawing.Size(460, 60);
            this.inputTextBox.TabIndex = 0;
            this.inputTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.inputTextBox_KeyDown);
            this.inputTextBox.Enter += new System.EventHandler(this.InputTextBox_Enter);
            this.inputTextBox.Leave += new System.EventHandler(this.InputTextBox_Leave);

            // sendButton
            this.sendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sendButton.BackColor = System.Drawing.Color.FromArgb(0, 120, 212);
            this.sendButton.FlatAppearance.BorderSize = 0;
            this.sendButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sendButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.sendButton.ForeColor = System.Drawing.Color.White;
            this.sendButton.Location = new System.Drawing.Point(480, 10);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(90, 60);
            this.sendButton.TabIndex = 1;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = false;
            this.sendButton.Click += new System.EventHandler(this.SendButton_Click);
            this.sendButton.MouseEnter += new System.EventHandler(this.SendButton_MouseEnter);
            this.sendButton.MouseLeave += new System.EventHandler(this.SendButton_MouseLeave);

            // inputPanel
            this.inputPanel.BackColor = System.Drawing.Color.FromArgb(250, 250, 252);
            this.inputPanel.BorderColor = System.Drawing.Color.FromArgb(225, 225, 230);
            this.inputPanel.BorderWidth = 1;
            this.inputPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.inputPanel.Location = new System.Drawing.Point(220, 450);
            this.inputPanel.Name = "inputPanel";
            this.inputPanel.Padding = new System.Windows.Forms.Padding(10);
            this.inputPanel.Size = new System.Drawing.Size(580, 100);
            this.inputPanel.TabIndex = 2;
            this.inputPanel.Controls.Add(this.inputTextBox);
            this.inputPanel.Controls.Add(this.sendButton);

            // mainContentPanel
            this.mainContentPanel.BackColor = System.Drawing.Color.White;
            this.mainContentPanel.BorderColor = System.Drawing.Color.FromArgb(225, 225, 230);
            this.mainContentPanel.BorderWidth = 1;
            this.mainContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainContentPanel.Location = new System.Drawing.Point(220, 25);
            this.mainContentPanel.Name = "mainContentPanel";
            this.mainContentPanel.Padding = new System.Windows.Forms.Padding(10);
            this.mainContentPanel.Size = new System.Drawing.Size(580, 425);
            this.mainContentPanel.TabIndex = 1;
            this.mainContentPanel.Controls.Add(this.chatHistoryTextBox);

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(800, 550);
            this.Controls.Add(this.mainContentPanel);
            this.Controls.Add(this.inputPanel);
            this.Controls.Add(this.threadsListBox);
            this.Controls.Add(this.toolStrip1);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Workstation Insights";
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private void Form1_Resize(object sender, EventArgs e)
        {
            // Adjust the chat history text box size when the form is resized
            if (chatHistoryTextBox != null)
            {
                var padding = 20; // Adjust padding as needed
                chatHistoryTextBox.Width = mainContentPanel.Width - (padding * 2);
                chatHistoryTextBox.Left = padding;
            }
        }

        private void ChatHistoryTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = e.LinkText,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to open link: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InputTextBox_Enter(object sender, EventArgs e)
        {
            inputPanel.BorderColor = Color.FromArgb(0, 120, 212);
        }

        private void InputTextBox_Leave(object sender, EventArgs e)
        {
            inputPanel.BorderColor = Color.FromArgb(225, 225, 230);
        }

        private void SendButton_MouseEnter(object sender, EventArgs e)
        {
            sendButton.BackColor = Color.FromArgb(0, 100, 180);
        }

        private void SendButton_MouseLeave(object sender, EventArgs e)
        {
            sendButton.BackColor = Color.FromArgb(0, 120, 212);
        }
    }
}