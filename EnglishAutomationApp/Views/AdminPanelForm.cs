using System;
using System.Drawing;
using System.Windows.Forms;
using EnglishAutomationApp.Views.Pages;

namespace EnglishAutomationApp.Views
{
    public partial class AdminPanelForm : Form
    {
        private Panel sidebarPanel = null!;
        private Panel contentPanel = null!;
        private Button usersButton = null!;
        private Button coursesButton = null!;
        private Button vocabularyButton = null!;
        private Button settingsButton = null!;
        private Button logoutButton = null!;
        private Label titleLabel = null!;

        private AdminUserControl? currentAdminControl;

        public AdminPanelForm()
        {
            InitializeComponent();
            LoadUsersPanel(); // Default panel
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Admin Panel - English Automation Platform";
            this.Size = new Size(1200, 800);
            this.MinimumSize = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(15, 23, 42); // Slate-900

            CreateSidebar();
            CreateContentPanel();

            // Add panels to form
            this.Controls.Add(contentPanel);
            this.Controls.Add(sidebarPanel);

            this.ResumeLayout(false);
        }

        private void CreateSidebar()
        {
            sidebarPanel = new Panel();
            sidebarPanel.BackColor = Color.FromArgb(30, 41, 59); // Slate-800
            sidebarPanel.Dock = DockStyle.Left;
            sidebarPanel.Width = 250;

            // Title
            titleLabel = new Label();
            titleLabel.Text = "🔧 Admin Panel";
            titleLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Location = new Point(0, 30);
            titleLabel.Size = new Size(250, 40);

            // Navigation buttons
            usersButton = CreateNavButton("👥 User Management", 100);
            usersButton.Click += (s, e) => LoadUsersPanel();

            coursesButton = CreateNavButton("📚 Course Management", 150);
            coursesButton.Click += (s, e) => LoadCoursesPanel();

            vocabularyButton = CreateNavButton("📖 Vocabulary Management", 200);
            vocabularyButton.Click += (s, e) => LoadVocabularyPanel();

            settingsButton = CreateNavButton("⚙️ Settings", 250);
            settingsButton.Click += (s, e) => LoadSettingsPanel();

            // Logout button at bottom
            logoutButton = new Button();
            logoutButton.Text = "🚪 Logout";
            logoutButton.Font = new Font("Segoe UI", 11);
            logoutButton.ForeColor = Color.FromArgb(248, 113, 113); // Red-400
            logoutButton.BackColor = Color.Transparent;
            logoutButton.FlatStyle = FlatStyle.Flat;
            logoutButton.FlatAppearance.BorderSize = 1;
            logoutButton.FlatAppearance.BorderColor = Color.FromArgb(248, 113, 113);
            logoutButton.Location = new Point(25, 700);
            logoutButton.Size = new Size(200, 40);
            logoutButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            logoutButton.Cursor = Cursors.Hand;
            logoutButton.Click += LogoutButton_Click;

            sidebarPanel.Controls.Add(titleLabel);
            sidebarPanel.Controls.Add(usersButton);
            sidebarPanel.Controls.Add(coursesButton);
            sidebarPanel.Controls.Add(vocabularyButton);
            sidebarPanel.Controls.Add(settingsButton);
            sidebarPanel.Controls.Add(logoutButton);
        }

        private Button CreateNavButton(string text, int y)
        {
            var button = new Button();
            button.Text = text;
            button.Font = new Font("Segoe UI", 11);
            button.ForeColor = Color.White;
            button.BackColor = Color.FromArgb(51, 65, 85); // Slate-700
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Location = new Point(25, y);
            button.Size = new Size(200, 40);
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.Cursor = Cursors.Hand;

            // Hover effects
            button.MouseEnter += (s, e) => button.BackColor = Color.FromArgb(71, 85, 105); // Slate-600
            button.MouseLeave += (s, e) => button.BackColor = Color.FromArgb(51, 65, 85); // Slate-700

            return button;
        }

        private void CreateContentPanel()
        {
            contentPanel = new Panel();
            contentPanel.BackColor = Color.FromArgb(15, 23, 42); // Slate-900
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Padding = new Padding(20);
        }

        private void LoadUsersPanel()
        {
            SetActiveButton(usersButton);
            LoadAdminControl(new AdminUserControl());
        }

        private void LoadCoursesPanel()
        {
            SetActiveButton(coursesButton);
            // Create a simple courses management panel
            var coursesControl = new UserControl();
            coursesControl.BackColor = Color.FromArgb(15, 23, 42);
            coursesControl.Dock = DockStyle.Fill;

            var label = new Label();
            label.Text = "📚 Course Management\n\nComing soon...";
            label.Font = new Font("Segoe UI", 16);
            label.ForeColor = Color.White;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Dock = DockStyle.Fill;

            coursesControl.Controls.Add(label);
            LoadAdminControl(coursesControl);
        }

        private void LoadVocabularyPanel()
        {
            SetActiveButton(vocabularyButton);
            // Create a simple vocabulary management panel
            var vocabControl = new UserControl();
            vocabControl.BackColor = Color.FromArgb(15, 23, 42);
            vocabControl.Dock = DockStyle.Fill;

            var label = new Label();
            label.Text = "📖 Vocabulary Management\n\nComing soon...";
            label.Font = new Font("Segoe UI", 16);
            label.ForeColor = Color.White;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Dock = DockStyle.Fill;

            vocabControl.Controls.Add(label);
            LoadAdminControl(vocabControl);
        }

        private void LoadSettingsPanel()
        {
            SetActiveButton(settingsButton);
            // Create a simple settings panel
            var settingsControl = new UserControl();
            settingsControl.BackColor = Color.FromArgb(15, 23, 42);
            settingsControl.Dock = DockStyle.Fill;

            var label = new Label();
            label.Text = "⚙️ System Settings\n\nComing soon...";
            label.Font = new Font("Segoe UI", 16);
            label.ForeColor = Color.White;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Dock = DockStyle.Fill;

            settingsControl.Controls.Add(label);
            LoadAdminControl(settingsControl);
        }

        private void LoadAdminControl(UserControl control)
        {
            contentPanel.Controls.Clear();
            control.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(control);
            currentAdminControl = control as AdminUserControl;
        }

        private void SetActiveButton(Button activeButton)
        {
            // Reset all buttons
            foreach (Control control in sidebarPanel.Controls)
            {
                if (control is Button btn && btn != logoutButton)
                {
                    btn.BackColor = Color.FromArgb(51, 65, 85); // Slate-700
                }
            }

            // Set active button
            activeButton.BackColor = Color.FromArgb(99, 102, 241); // Indigo-500
        }

        private void LogoutButton_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout from admin panel?", 
                "Logout Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}
