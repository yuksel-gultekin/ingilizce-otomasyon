using System;
using System.Drawing;
using System.Windows.Forms;
using EnglishAutomationApp.Services;
using EnglishAutomationApp.Views.Pages;
using EnglishAutomationApp.Helpers;
using EnglishAutomationApp.Forms;

namespace EnglishAutomationApp.Views
{
    public partial class MainForm : Form
    {
        private MenuStrip menuStrip = null!;
        private StatusStrip statusStrip = null!;
        private ToolStripStatusLabel statusLabel = null!;
        private Panel contentPanel = null!;
        private Label welcomeLabel = null!;

        public MainForm()
        {
            InitializeComponent();
            InitializeWindow();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "English Learning Platform";
            this.Size = new Size(1200, 700);
            this.MinimumSize = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            ModernUIHelper.ApplyModernFormStyle(this);

            // Menu Strip
            menuStrip = new MenuStrip();
            menuStrip.BackColor = ModernUIHelper.Colors.Primary;
            menuStrip.ForeColor = Color.White;
            menuStrip.Font = ModernUIHelper.Fonts.Body;

            // File Menu
            var fileMenu = new ToolStripMenuItem("File");
            fileMenu.DropDownItems.Add("Logout", null, LogoutMenuItem_Click);
            fileMenu.DropDownItems.Add("Exit", null, ExitMenuItem_Click);

            // Learning Menu
            var learningMenu = new ToolStripMenuItem("Learning");
            learningMenu.DropDownItems.Add("📊 Dashboard", null, DashboardMenuItem_Click);
            learningMenu.DropDownItems.Add("📚 Courses", null, CoursesMenuItem_Click);
            learningMenu.DropDownItems.Add("📖 Vocabulary", null, VocabularyMenuItem_Click);
            learningMenu.DropDownItems.Add("📈 Progress", null, ProgressMenuItem_Click);

            // Tools Menu
            var toolsMenu = new ToolStripMenuItem("Tools");
            toolsMenu.DropDownItems.Add("🏆 Achievements", null, AchievementsMenuItem_Click);

            // Admin Menu (will be shown only for admin users)
            var adminMenu = new ToolStripMenuItem("Admin");
            adminMenu.DropDownItems.Add("⚙️ Admin Panel", null, AdminMenuItem_Click);
            adminMenu.Visible = AuthenticationService.IsAdmin;

            // Help Menu
            var helpMenu = new ToolStripMenuItem("Help");
            helpMenu.DropDownItems.Add("About", null, AboutMenuItem_Click);

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(learningMenu);
            menuStrip.Items.Add(toolsMenu);
            menuStrip.Items.Add(adminMenu);
            menuStrip.Items.Add(helpMenu);

            // Welcome Label
            welcomeLabel = new Label();
            welcomeLabel.Font = ModernUIHelper.Fonts.BodyBold;
            welcomeLabel.ForeColor = Color.White;
            welcomeLabel.BackColor = ModernUIHelper.Colors.Primary;
            welcomeLabel.TextAlign = ContentAlignment.MiddleRight;
            welcomeLabel.Dock = DockStyle.Right;
            welcomeLabel.Width = 350;
            welcomeLabel.Padding = new Padding(ModernUIHelper.Spacing.Large);

            // Add welcome label to menu strip
            var welcomeItem = new ToolStripControlHost(welcomeLabel);
            welcomeItem.Alignment = ToolStripItemAlignment.Right;
            menuStrip.Items.Add(welcomeItem);

            // Content Panel
            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.BackColor = ModernUIHelper.Colors.Background;
            contentPanel.Padding = new Padding(ModernUIHelper.Spacing.Large);

            // Status Strip
            statusStrip = new StatusStrip();
            statusStrip.BackColor = ModernUIHelper.Colors.SurfaceVariant;
            statusStrip.ForeColor = ModernUIHelper.Colors.TextSecondary;
            statusLabel = new ToolStripStatusLabel();
            statusLabel.Text = "Ready";
            statusLabel.Font = ModernUIHelper.Fonts.Small;
            statusStrip.Items.Add(statusLabel);

            // Add controls to form
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(contentPanel);
            this.Controls.Add(statusStrip);
            this.Controls.Add(menuStrip);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void InitializeWindow()
        {
            // Show user information
            if (AuthenticationService.CurrentUser != null)
            {
                UpdateWelcomeMessage();
            }

            // Show Dashboard by default
            ShowDashboard();
            UpdateStatus("Dashboard");
        }

        private void UpdateWelcomeMessage()
        {
            if (AuthenticationService.CurrentUser != null)
            {
                welcomeLabel.Text = $"Welcome, {AuthenticationService.CurrentUser.FirstName}! 👋";
            }
        }

        // Menu Event Handlers
        private void DashboardMenuItem_Click(object? sender, EventArgs e)
        {
            ShowDashboard();
            UpdateStatus("Dashboard");
        }

        private void CoursesMenuItem_Click(object? sender, EventArgs e)
        {
            ShowUserControl(new CoursesUserControl());
            UpdateStatus("Courses");
        }

        private void VocabularyMenuItem_Click(object? sender, EventArgs e)
        {
            ShowUserControl(new VocabularyUserControl());
            UpdateStatus("Vocabulary");
        }

        private void ProgressMenuItem_Click(object? sender, EventArgs e)
        {
            ShowUserControl(new ProgressUserControl());
            UpdateStatus("Progress");
        }

        private void AchievementsMenuItem_Click(object? sender, EventArgs e)
        {
            ShowUserControl(new AchievementsUserControl());
            UpdateStatus("Achievements");
        }



        private void AdminMenuItem_Click(object? sender, EventArgs e)
        {
            if (AuthenticationService.IsAdmin)
            {
                var adminForm = new AdminPanelForm();
                adminForm.ShowDialog();
                UpdateStatus("Admin Panel");
            }
            else
            {
                MessageBox.Show("Bu sayfaya erişim yetkiniz yok.", "Erişim Reddedildi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AboutMenuItem_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("English Automation Platform v1.0\n\nA comprehensive English learning platform with automation features.", 
                "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LogoutMenuItem_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                AuthenticationService.Logout();
                
                var loginForm = new LoginForm();
                loginForm.Show();
                this.Close();
            }
        }

        private void ExitMenuItem_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        // Helper Methods
        private void ShowDashboard()
        {
            ShowUserControl(new DashboardUserControl());
        }

        public void ShowUserControl(UserControl userControl)
        {
            contentPanel.Controls.Clear();
            userControl.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(userControl);
        }

        private void UpdateStatus(string status)
        {
            statusLabel.Text = status;
        }

        // Public navigation methods for other controls to use
        public void NavigateToCoursesPage()
        {
            CoursesMenuItem_Click(null, EventArgs.Empty);
        }

        public void NavigateToProgressPage()
        {
            ProgressMenuItem_Click(null, EventArgs.Empty);
        }


    }
}
