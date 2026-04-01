using System;
using System.Drawing;
using System.Windows.Forms;

namespace WorkstationInsights.Services
{
    public static class MarkdownHelper
    {
        public static void AppendMarkdown(RichTextBox richTextBox, string sender, string markdown)
        {
            richTextBox.SelectionStart = richTextBox.TextLength;
            richTextBox.SelectionAlignment = sender == "AI" ? HorizontalAlignment.Left : HorizontalAlignment.Right;
            
            richTextBox.SelectionFont = new Font("Segoe UI", 10F, FontStyle.Bold);
            richTextBox.SelectionColor = sender == "AI" ? Color.FromArgb(0, 90, 158) : Color.Black;
            richTextBox.AppendText($"{sender}: ");
            
            richTextBox.SelectionFont = new Font("Segoe UI", 10F, FontStyle.Regular);
            richTextBox.SelectionColor = Color.Black;

            string[] lines = markdown.Split('\n');
            foreach (var line in lines)
            {
                string processedLine = line;
                
                if (processedLine.Contains("**"))
                {
                    var parts = processedLine.Split("**");
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (i % 2 == 1) // Bold part
                        {
                            richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Bold);
                            richTextBox.AppendText(parts[i]);
                            richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Regular);
                        }
                        else
                        {
                            richTextBox.AppendText(parts[i]);
                        }
                    }
                }
                else if (processedLine.Trim().StartsWith("`") && processedLine.Trim().EndsWith("`"))
                {
                    richTextBox.SelectionFont = new Font("Consolas", 9.5F);
                    richTextBox.SelectionBackColor = Color.FromArgb(240, 240, 240);
                    richTextBox.AppendText(processedLine.Replace("`", ""));
                    richTextBox.SelectionFont = new Font("Segoe UI", 10F);
                    richTextBox.SelectionBackColor = Color.White;
                }
                else
                {
                    richTextBox.AppendText(processedLine);
                }
                richTextBox.AppendText(Environment.NewLine);
            }
            richTextBox.ScrollToCaret();
        }
    }
}
