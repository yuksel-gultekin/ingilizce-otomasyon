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
        private Button vocabularyButton = null!;
        private Button logoutButton = null!;
        private Label titleLabel = null!;
        private ComboBox languageComboBox = null!;

        private UserControl? currentAdminControl;
        private bool isEnglish = true;

        public AdminPanelForm()
        {
            InitializeComponent();
            LoadUsersPanel();
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

            // Language ComboBox
            languageComboBox = new ComboBox();
            languageComboBox.Items.AddRange(new[] { "🇺🇸 English", "🇹🇷 Türkçe" });
            languageComboBox.SelectedIndex = 0;
            languageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            languageComboBox.Font = new Font("Segoe UI", 9);
            languageComboBox.BackColor = Color.FromArgb(51, 65, 85);
            languageComboBox.ForeColor = Color.White;
            languageComboBox.Location = new Point(10, 10);
            languageComboBox.Size = new Size(120, 25);
            languageComboBox.SelectedIndexChanged += LanguageComboBox_SelectedIndexChanged;

            // Title
            titleLabel = new Label();
            titleLabel.Text = "🔧 Admin Panel";
            titleLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Location = new Point(0, 45);
            titleLabel.Size = new Size(250, 40);

            // Navigation buttons
            usersButton = CreateNavButton("👥 User Management", 115);
            usersButton.Click += (s, e) => LoadUsersPanel();

            vocabularyButton = CreateNavButton("📖 Vocabulary Management", 165);
            vocabularyButton.Click += (s, e) => LoadVocabularyPanel();

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

            sidebarPanel.Controls.Add(languageComboBox);
            sidebarPanel.Controls.Add(titleLabel);
            sidebarPanel.Controls.Add(usersButton);
            sidebarPanel.Controls.Add(vocabularyButton);
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
            var adminUserControl = new AdminUserControl();
            adminUserControl.SetLanguage(isEnglish);
            LoadAdminControl(adminUserControl);
        }

        private void LoadVocabularyPanel()
        {
            SetActiveButton(vocabularyButton);
            var adminVocabularyControl = new AdminVocabularyControl();
            adminVocabularyControl.SetLanguage(isEnglish);
            LoadAdminControl(adminVocabularyControl);
        }



        private void LoadAdminControl(UserControl control)
        {
            contentPanel.Controls.Clear();
            control.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(control);
            currentAdminControl = control;
        }

        private void SetActiveButton(Button activeButton)
        {
            // Reset all buttons
            usersButton.BackColor = Color.FromArgb(51, 65, 85); // Slate-700
            vocabularyButton.BackColor = Color.FromArgb(51, 65, 85); // Slate-700

            // Set active button
            activeButton.BackColor = Color.FromArgb(99, 102, 241); // Indigo-500
        }

        private void LogoutButton_Click(object? sender, EventArgs e)
        {
            var message = isEnglish ? "Are you sure you want to logout from admin panel?" : "Admin panelinden çıkmak istediğinizden emin misiniz?";
            var title = isEnglish ? "Logout Confirmation" : "Çıkış Onayı";

            var result = MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void LanguageComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            isEnglish = languageComboBox.SelectedIndex == 0;
            UpdateLanguage();

            // Update current admin control language
            if (currentAdminControl is AdminUserControl adminUserControl)
            {
                adminUserControl.SetLanguage(isEnglish);
            }
            else if (currentAdminControl is AdminVocabularyControl adminVocabularyControl)
            {
                adminVocabularyControl.SetLanguage(isEnglish);
            }
        }

        private void UpdateLanguage()
        {
            if (isEnglish)
            {
                // English
                this.Text = "Admin Panel - English Automation Platform";
                titleLabel.Text = "🔧 Admin Panel";
                usersButton.Text = "👥 User Management";
                vocabularyButton.Text = "📖 Vocabulary Management";
                logoutButton.Text = "🚪 Logout";
            }
            else
            {
                // Turkish
                this.Text = "Yönetici Paneli - İngilizce Otomasyon Platformu";
                titleLabel.Text = "🔧 Yönetici Paneli";
                usersButton.Text = "👥 Kullanıcı Yönetimi";
                vocabularyButton.Text = "📖 Kelime Yönetimi";
                logoutButton.Text = "🚪 Çıkış";
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            UpdateLanguage();
        }
    }
}
