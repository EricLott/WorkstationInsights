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
            this.threadsListBox = new System.Windows.Forms.ListBox(); // Added

            this.SuspendLayout(); // Added to prevent layout issues during programmatic changes
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
            // Adjusted Location and Size, and Dock
            this.inputTextBox.Location = new System.Drawing.Point(180, 380); // X = threadsListBox.Width
            this.inputTextBox.Size = new System.Drawing.Size(488, 23); // Width = ClientSize.Width - threadsListBox.Width - sendButton.Width - margins
            this.inputTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.inputTextBox_KeyDown);
            this.inputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputTextBox.TabIndex = 2;


            // 
            // sendButton
            // 
            // Adjusted Location and Size, and Dock
            this.sendButton.Location = new System.Drawing.Point(675, 380); // X = ClientSize.Width - sendButton.Width - margins
            this.sendButton.Size = new System.Drawing.Size(113, 23);
            this.sendButton.Text = "Send";
            this.sendButton.Click += new System.EventHandler(this.SendButton_Click);
            this.sendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sendButton.TabIndex = 3;

            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 420); // Original client size
            // Add threadsListBox first so other controls can dock relative to it if needed.
            this.Controls.Add(this.chatHistoryTextBox); // Dock = Fill, should be after fixed size/docked controls
            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.threadsListBox); // Dock = Left
            this.Controls.Add(this.toolStrip1); // toolStrip is often added first or last
            this.Text = "Workstation Insights";
            this.ResumeLayout(false); // Added
            this.PerformLayout();
        }

        #endregion
    }
}
