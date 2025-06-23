namespace WorkstationInsights
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton newChatButton;
        private System.Windows.Forms.ToolStripButton apiKeyButton;
        private System.Windows.Forms.ToolStripButton settingsButton;
        private System.Windows.Forms.TextBox chatHistoryTextBox;
        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.Button sendButton;

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
            this.chatHistoryTextBox = new System.Windows.Forms.TextBox();
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();

            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.newChatButton,
                this.apiKeyButton,
                this.settingsButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
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
            // chatHistoryTextBox
            // 
            this.chatHistoryTextBox.Multiline = true;
            this.chatHistoryTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.chatHistoryTextBox.Location = new System.Drawing.Point(12, 40);
            this.chatHistoryTextBox.Size = new System.Drawing.Size(776, 330);
            this.chatHistoryTextBox.ReadOnly = true;

            // 
            // inputTextBox
            // 
            this.inputTextBox.Location = new System.Drawing.Point(12, 380);
            this.inputTextBox.Size = new System.Drawing.Size(650, 23);

            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(675, 380);
            this.sendButton.Size = new System.Drawing.Size(113, 23);
            this.sendButton.Text = "Send";
            this.sendButton.Click += new System.EventHandler(this.SendButton_Click);

            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 420);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.chatHistoryTextBox);
            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.sendButton);
            this.Text = "Workstation Insights";
        }

        #endregion
    }
}
