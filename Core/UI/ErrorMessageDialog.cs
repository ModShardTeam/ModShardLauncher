using System;
using System.Drawing;
using System.Windows.Forms;
using Serilog;

namespace ModShardLauncher.Core.UI
{
    public class ErrorMessageDialog : Form
    {
        private readonly RichTextBox messageTextBox;
        public DialogResult Result { get; private set; }
        private readonly TableLayoutPanel panel;
        private readonly Button okButton;
        private readonly Button cpyButton;
        private readonly Button logButton;
        public ErrorMessageDialog(string title, string message, string? logPath = null)
        {
            panel = new();  
            messageTextBox = new RichTextBox();
            okButton = new Button();
            cpyButton = new Button();
            logButton = new Button();

            InitializeComponent();
            SetupDialog(title, message, logPath);
        }
        private void InitializeComponent()
        {
            SuspendLayout();

            Text = string.Empty;
            Size = new Size(450, 200);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            Dock = DockStyle.Fill;

            // panel
            panel.Anchor = AnchorStyles.Top;
            panel.Size = new Size(300, 150);
            panel.BorderStyle = BorderStyle.None;
            panel.ColumnCount = 3;
            panel.RowCount = 2;
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            panel.Dock = DockStyle.Fill;

            // Message TextBox
            messageTextBox.ReadOnly = true;
            messageTextBox.BorderStyle = BorderStyle.None;
            messageTextBox.BackColor = BackColor;
            messageTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            messageTextBox.TabStop = false;

            // ok button
            okButton.Text = "OK";
            okButton.Size = new Size(75, 23);
            okButton.DialogResult = DialogResult.OK;
            okButton.Click += (s, e) => { Result = DialogResult.OK; Close(); };

            // copy button
            cpyButton.Text = "Copy error";
            cpyButton.Size = new Size(100, 23);
            cpyButton.Click += CopyButton_Click;

            // log button
            logButton.Text = "Open Log Folder";
            logButton.Size = new Size(120, 23);
            logButton.Click += LogButton_Click;
 
            panel.Controls.Add(messageTextBox, 0, 0);
            panel.SetColumnSpan(messageTextBox, 3);
            panel.Controls.Add(okButton, 0, 1);
            panel.Controls.Add(cpyButton, 1, 1);
            panel.Controls.Add(logButton, 2, 1);

            Controls.Add(panel);

            // Set default button and cancel button
            AcceptButton = okButton;
            CancelButton = okButton;

            ResumeLayout();
        }
        private void SetupDialog(string title, string message, string? logPath = null)
        {
            Text = title;
            messageTextBox.Rtf = message;
            messageTextBox.AutoSize = true;

            Size size = messageTextBox.GetPreferredSize(new Size(800, 0)) + new Size(0, 50);
            int newHeight = size.Height + 100;
            int newWidth = Math.Max(450, size.Width + 50);

            messageTextBox.Size = new Size(size.Width, size.Height);
            Size = new Size(newWidth, newHeight);

            // Hide log button if no path provided
            if (string.IsNullOrEmpty(logPath))
            {
                logButton.Visible = false;
                okButton.Location = new Point(340, 125); // Center the OK button
            }
            else
            {
                logButton.Tag = logPath; // Store the log path
            }
        }
        private void LogButton_Click(object? sender, EventArgs e)
        {
            try
            {
                string? logPath = logButton.Tag?.ToString();
                if (!string.IsNullOrEmpty(logPath))
                {
                    System.Diagnostics.Process.Start("explorer.exe", logPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open log folder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // nifty trick: use Retry to indicate log button was clicked
            Result = DialogResult.Retry;
            Close();
        }
        private void CopyButton_Click(object? sender, EventArgs e)
        {
            Clipboard.SetText(messageTextBox.Text);
        }
        public static DialogResult Show(string title, string message, string? logPath = null)
        {
            using var dialog = new ErrorMessageDialog(title, message, logPath);
            dialog.ShowDialog();
            return dialog.Result;
        }
    }
}