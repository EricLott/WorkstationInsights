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
        private System.Windows.Forms.ListBox threadsListBox; // Added
        private System.Windows.Forms.Panel inputPanel; // Added

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
            this.threadsListBox = new System.Windows.Forms.ListBox();
            this.inputPanel = new System.Windows.Forms.Panel(); // Added instantiation

            this.SuspendLayout();
            //
            // inputPanel
            //
            this.inputPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.inputPanel.Location = new System.Drawing.Point(0, 375); // Will be adjusted by Dock
            this.inputPanel.Name = "inputPanel";
            this.inputPanel.Size = new System.Drawing.Size(800, 45); // Height 45
            this.inputPanel.TabIndex = 5; // After threadsListBox
            // No Text property for Panel by default

            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.newChatButton,
                this.apiKeyButton,
                this.settingsButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25); // Original client size
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";

            // 
            // newChatButton
            // 
            this.newChatButton.Text = "New Chat";
            this.newChatButton.Click += new System.EventHandler(this.NewChatButton_Click);

            // 
            // apiKeyButton
            // 
            this.apiKeyButton.Text = "Manage API Key";
            this.apiKeyButton.Click += new System.EventHandler(this.ApiKeyButton_Click);

            // 
            // settingsButton
            // 
            this.settingsButton.Text = "Settings";
            this.settingsButton.Click += new System.EventHandler(this.SettingsButton_Click);

            //
            // threadsListBox
            //
            this.threadsListBox.FormattingEnabled = true;
            this.threadsListBox.ItemHeight = 15; // Default item height for Segoe UI 9pt
            this.threadsListBox.Location = new System.Drawing.Point(0, 25); // Below toolStrip1
            this.threadsListBox.Name = "threadsListBox";
            this.threadsListBox.Size = new System.Drawing.Size(180, 395); // Width 180, height fills below toolstrip to bottom (420-25)
            this.threadsListBox.TabIndex = 4; // After sendButton
            this.threadsListBox.Dock = System.Windows.Forms.DockStyle.Left; // Dock to left

            // 
            // chatHistoryTextBox
            // 
            // Adjusted Location and Size, and Dock
            this.chatHistoryTextBox.Location = new System.Drawing.Point(180, 28); // X = threadsListBox.Width, Y = toolStrip1.Height + small_margin
            this.chatHistoryTextBox.Size = new System.Drawing.Size(608, 342); // Width = ClientSize.Width - threadsListBox.Width - margins, Height = ClientSize.Height - toolstrip - inputArea - margins
            this.chatHistoryTextBox.ReadOnly = true;
            this.chatHistoryTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.chatHistoryTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chatHistoryTextBox.Dock = System.Windows.Forms.DockStyle.Fill; // Fill remaining space
            this.chatHistoryTextBox.TabIndex = 1;


            // 
            // inputTextBox
            // 
            // Now relative to inputPanel
            this.inputTextBox.Location = new System.Drawing.Point(6, 10); // Small margin from left/top of panel
            this.inputTextBox.Size = new System.Drawing.Size(590, 23); // Adjusted Width
            this.inputTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.inputTextBox_KeyDown);
            this.inputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom))); // Anchor to all sides of panel for resizing
            this.inputTextBox.TabIndex = 0; // First control in panel


            // 
            // sendButton
            // 
            // Now relative to inputPanel
            this.sendButton.Location = new System.Drawing.Point(602, 9); // To the right of inputTextBox
            this.sendButton.Size = new System.Drawing.Size(113, 25); // Adjusted height to match textbox better
            this.sendButton.Text = "Send";
            this.sendButton.Click += new System.EventHandler(this.SendButton_Click);
            this.sendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom))); // Anchor to keep it on right
            this.sendButton.TabIndex = 1; // Second control in panel

            // Add inputTextBox and sendButton to inputPanel's controls
            this.inputPanel.Controls.Add(this.inputTextBox);
            this.inputPanel.Controls.Add(this.sendButton);

            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 420);
            // Finalized Controls.Add order for proper docking
            this.Controls.Add(this.toolStrip1);       // Top (implicit)
            this.Controls.Add(this.threadsListBox);   // Left
            this.Controls.Add(this.inputPanel);       // Bottom
            this.Controls.Add(this.chatHistoryTextBox); // Fill (should be last or after specific docks)
            this.Text = "Workstation Insights";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
