using System;
using System.Drawing;
using System.Windows.Forms;
using EnglishAutomationApp.Helpers;

namespace EnglishAutomationApp.Views.Pages
{
    public partial class AdminUserControl : UserControl
    {
        public AdminUserControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // UserControl properties
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.Dock = DockStyle.Fill;

            // Title
            var titleLabel = new Label();
            titleLabel.Text = "⚙️ Admin Panel";
            titleLabel.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(102, 126, 234);
            titleLabel.Location = new Point(30, 20);
            titleLabel.Size = new Size(500, 40);

            // Content Panel
            var contentPanel = new Panel();
            contentPanel.BackColor = Color.White;
            contentPanel.Location = new Point(30, 80);
            contentPanel.Size = new Size(740, 500);

            // Database Management Section
            var dbManagementLabel = new Label();
            dbManagementLabel.Text = "Database Management";
            dbManagementLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            dbManagementLabel.Location = new Point(30, 30);
            dbManagementLabel.Size = new Size(300, 30);

            // Database Info
            var dbInfoLabel = new Label();
            dbInfoLabel.Text = AccessDatabaseHelper.GetDatabaseInfo();
            dbInfoLabel.Font = new Font("Segoe UI", 10);
            dbInfoLabel.Location = new Point(30, 70);
            dbInfoLabel.Size = new Size(680, 100);

            // Database Management Buttons
            var backupButton = new Button();
            backupButton.Text = "💾 Backup Database";
            backupButton.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            backupButton.BackColor = Color.FromArgb(76, 175, 80);
            backupButton.ForeColor = Color.White;
            backupButton.FlatStyle = FlatStyle.Flat;
            backupButton.Location = new Point(30, 180);
            backupButton.Size = new Size(150, 35);
            backupButton.Click += BackupButton_Click;

            var compactButton = new Button();
            compactButton.Text = "🗜️ Compact Database";
            compactButton.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            compactButton.BackColor = Color.FromArgb(255, 152, 0);
            compactButton.ForeColor = Color.White;
            compactButton.FlatStyle = FlatStyle.Flat;
            compactButton.Location = new Point(190, 180);
            compactButton.Size = new Size(150, 35);
            compactButton.Click += CompactButton_Click;

            var restoreButton = new Button();
            restoreButton.Text = "📁 Restore Database";
            restoreButton.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            restoreButton.BackColor = Color.FromArgb(102, 126, 234);
            restoreButton.ForeColor = Color.White;
            restoreButton.FlatStyle = FlatStyle.Flat;
            restoreButton.Location = new Point(350, 180);
            restoreButton.Size = new Size(150, 35);
            restoreButton.Click += RestoreButton_Click;

            // System Info Section
            var systemInfoLabel = new Label();
            systemInfoLabel.Text = "System Information";
            systemInfoLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            systemInfoLabel.Location = new Point(30, 240);
            systemInfoLabel.Size = new Size(300, 30);

            var systemInfo = new Label();
            systemInfo.Text = $"Application: English Automation Platform v1.0\n" +
                             $"Framework: .NET 9.0\n" +
                             $"UI: Windows Forms\n" +
                             $"Database: SQLite (Access-like)\n" +
                             $"Database Size: {AccessDatabaseHelper.GetDatabaseSizeFormatted()}";
            systemInfo.Font = new Font("Segoe UI", 10);
            systemInfo.Location = new Point(30, 280);
            systemInfo.Size = new Size(680, 100);

            contentPanel.Controls.Add(dbManagementLabel);
            contentPanel.Controls.Add(dbInfoLabel);
            contentPanel.Controls.Add(backupButton);
            contentPanel.Controls.Add(compactButton);
            contentPanel.Controls.Add(restoreButton);
            contentPanel.Controls.Add(systemInfoLabel);
            contentPanel.Controls.Add(systemInfo);

            // Add controls to UserControl
            this.Controls.Add(titleLabel);
            this.Controls.Add(contentPanel);

            this.ResumeLayout(false);
        }

        private void BackupButton_Click(object sender, EventArgs e)
        {
            AccessDatabaseHelper.BackupDatabase();
        }

        private void CompactButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("This will compact the database to reduce its size. Continue?",
                "Compact Database", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                AccessDatabaseHelper.CompactDatabase();
            }
        }

        private void RestoreButton_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select Database Backup File";
            openFileDialog.Filter = "Database files (*.db)|*.db|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var result = MessageBox.Show("This will replace the current database with the backup. Continue?",
                    "Restore Database", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    AccessDatabaseHelper.RestoreDatabase(openFileDialog.FileName);
                }
            }
        }
    }
}
