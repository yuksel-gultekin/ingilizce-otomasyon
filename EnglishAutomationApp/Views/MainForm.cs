using System;
using System.Drawing;
using System.Windows.Forms;
using EnglishAutomationApp.Services;
using EnglishAutomationApp.Views.Pages;
using EnglishAutomationApp.Helpers;


namespace EnglishAutomationApp.Views
{
    public partial class MainForm : Form
    {
        private MenuStrip menuStrip = null!;
        private StatusStrip statusStrip = null!;
        private ToolStripStatusLabel statusLabel = null!;
        private Panel contentPanel = null!;
        private Label welcomeLabel = null!;
        private ComboBox languageComboBox = null!;
        private bool isEnglish = true;

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

            var fileMenu = new ToolStripMenuItem("File");
            fileMenu.DropDownItems.Add("Logout", null, LogoutMenuItem_Click);
            fileMenu.DropDownItems.Add("Exit", null, ExitMenuItem_Click);

            var learningMenu = new ToolStripMenuItem("Learning");
            learningMenu.DropDownItems.Add("📊 Dashboard", null, DashboardMenuItem_Click);
            learningMenu.DropDownItems.Add("📚 Courses", null, CoursesMenuItem_Click);
            learningMenu.DropDownItems.Add("📖 Vocabulary", null, VocabularyMenuItem_Click);

            var adminMenu = new ToolStripMenuItem("Admin");
            adminMenu.DropDownItems.Add("⚙️ Admin Panel", null, AdminMenuItem_Click);
            adminMenu.Visible = AuthenticationService.IsAdmin;

            var helpMenu = new ToolStripMenuItem("Help");
            helpMenu.DropDownItems.Add("About", null, AboutMenuItem_Click);

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(learningMenu);
            menuStrip.Items.Add(adminMenu);
            menuStrip.Items.Add(helpMenu);

            // Language ComboBox
            languageComboBox = new ComboBox();
            languageComboBox.Items.AddRange(new[] { "🇺🇸 English", "🇹🇷 Türkçe" });
            languageComboBox.SelectedIndex = 0;
            languageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            languageComboBox.Font = new Font("Segoe UI", 9);
            languageComboBox.BackColor = Color.FromArgb(67, 56, 202);
            languageComboBox.ForeColor = Color.White;
            languageComboBox.Width = 120;
            languageComboBox.SelectedIndexChanged += LanguageComboBox_SelectedIndexChanged;

            var languageItem = new ToolStripControlHost(languageComboBox);
            languageItem.Alignment = ToolStripItemAlignment.Right;
            menuStrip.Items.Add(languageItem);

            // Welcome Label
            welcomeLabel = new Label();
            welcomeLabel.Font = ModernUIHelper.Fonts.BodyBold;
            welcomeLabel.ForeColor = Color.White;
            welcomeLabel.BackColor = ModernUIHelper.Colors.Primary;
            welcomeLabel.TextAlign = ContentAlignment.MiddleRight;
            welcomeLabel.Dock = DockStyle.Right;
            welcomeLabel.Width = 250;
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

            // Set language for supported controls
            if (userControl is VocabularyUserControl vocabControl)
            {
                vocabControl.SetLanguage(isEnglish);
            }
            else if (userControl is CoursesUserControl coursesControl)
            {
                //coursesControl.SetLanguage(isEnglish);
            }

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



        private void LanguageComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            isEnglish = languageComboBox.SelectedIndex == 0;
            UpdateLanguage();

            // Update current user control language
            if (contentPanel.Controls.Count > 0)
            {
                var currentControl = contentPanel.Controls[0];
                if (currentControl is VocabularyUserControl vocabControl)
                {
                    vocabControl.SetLanguage(isEnglish);
                }
                else if (currentControl is CoursesUserControl coursesControl)
                {
                   // coursesControl.SetLanguage(isEnglish);
                }
            }
        }

        private void UpdateLanguage()
        {
            if (isEnglish)
            {
                // English
                this.Text = "English Automation Platform";
                welcomeLabel.Text = $"Welcome, {AuthenticationService.CurrentUser?.FullName ?? "User"}!";
                statusLabel.Text = "Ready";

                // Update menu items
                UpdateMenuLanguage("File", "Learning", "Admin", "Help");
                UpdateMenuItemsLanguage(
                    new[] { "Logout", "Exit" },
                    new[] { "📊 Dashboard", "📚 Courses", "📖 Vocabulary" },
                    new[] { "⚙️ Admin Panel" },
                    new[] { "About" }
                );
            }
            else
            {
                // Turkish
                this.Text = "İngilizce Otomasyon Platformu";
                welcomeLabel.Text = $"Hoş geldiniz, {AuthenticationService.CurrentUser?.FullName ?? "Kullanıcı"}!";
                statusLabel.Text = "Hazır";

                // Update menu items
                UpdateMenuLanguage("Dosya", "Öğrenme", "Yönetici", "Yardım");
                UpdateMenuItemsLanguage(
                    new[] { "Çıkış Yap", "Kapat" },
                    new[] { "📊 Kontrol Paneli", "📚 Kurslar", "📖 Kelimeler" },
                    new[] { "⚙️ Yönetici Paneli" },
                    new[] { "Hakkında" }
                );
            }
        }

        private void UpdateMenuLanguage(string file, string learning, string admin, string help)
        {
            if (menuStrip.Items.Count >= 4)
            {
                menuStrip.Items[0].Text = file;
                menuStrip.Items[1].Text = learning;
                menuStrip.Items[2].Text = admin;
                menuStrip.Items[3].Text = help;
            }
        }

        private void UpdateMenuItemsLanguage(string[] fileItems, string[] learningItems, string[] adminItems, string[] helpItems)
        {
            // Update File menu items
            if (menuStrip.Items[0] is ToolStripMenuItem fileMenu && fileMenu.DropDownItems.Count >= 2)
            {
                fileMenu.DropDownItems[0].Text = fileItems[0]; // Logout
                fileMenu.DropDownItems[1].Text = fileItems[1]; // Exit
            }

            // Update Learning menu items
            if (menuStrip.Items[1] is ToolStripMenuItem learningMenu && learningMenu.DropDownItems.Count >= 3)
            {
                learningMenu.DropDownItems[0].Text = learningItems[0]; // Dashboard
                learningMenu.DropDownItems[1].Text = learningItems[1]; // Courses
                learningMenu.DropDownItems[2].Text = learningItems[2]; // Vocabulary
            }

            // Update Admin menu items
            if (menuStrip.Items[2] is ToolStripMenuItem adminMenu && adminMenu.DropDownItems.Count >= 1)
            {
                adminMenu.DropDownItems[0].Text = adminItems[0]; // Admin Panel
            }

            // Update Help menu items
            if (menuStrip.Items[3] is ToolStripMenuItem helpMenu && helpMenu.DropDownItems.Count >= 1)
            {
                helpMenu.DropDownItems[0].Text = helpItems[0]; // About
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            UpdateLanguage();
        }
    }
}
