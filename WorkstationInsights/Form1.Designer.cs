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
        private System.Windows.Forms.ListBox threadsListBox;
        private System.Windows.Forms.Panel inputPanel;
        private System.Windows.Forms.Panel rightPanel;

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
            this.inputPanel = new System.Windows.Forms.Panel();
            this.rightPanel = new System.Windows.Forms.Panel();

            this.SuspendLayout();

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
            this.threadsListBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.threadsListBox.Width = 180;
            this.threadsListBox.FormattingEnabled = true;
            this.threadsListBox.TabIndex = 4;

            // chatHistoryTextBox
            this.chatHistoryTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chatHistoryTextBox.ReadOnly = true;
            this.chatHistoryTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.chatHistoryTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chatHistoryTextBox.TabIndex = 1;

            // inputTextBox
            this.inputTextBox.Location = new System.Drawing.Point(10, 10);
            this.inputTextBox.Width = 180;
            this.inputTextBox.TabIndex = 0;
            this.inputTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.inputTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.inputTextBox_KeyDown);

            // sendButton
            this.sendButton.Text = "Send";
            this.sendButton.Location = new System.Drawing.Point(10, 40);
            this.sendButton.Width = 180;
            this.sendButton.TabIndex = 1;
            this.sendButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.sendButton.Click += new System.EventHandler(this.SendButton_Click);

            // inputPanel
            this.inputPanel.Dock = DockStyle.Top;
            this.inputPanel.Height = 80;
            this.inputPanel.Controls.Add(this.inputTextBox);
            this.inputPanel.Controls.Add(this.sendButton);

            // rightPanel
            this.rightPanel.Dock = DockStyle.Right;
            this.rightPanel.Width = 200;
            this.rightPanel.Controls.Add(this.inputPanel);

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.chatHistoryTextBox);
            this.Controls.Add(this.rightPanel);
            this.Controls.Add(this.threadsListBox);
            this.Controls.Add(this.toolStrip1);
            this.Text = "Workstation Insights";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}