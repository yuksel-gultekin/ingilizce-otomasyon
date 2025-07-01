using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EnglishAutomationApp.Data;
using EnglishAutomationApp.Models;
using EnglishAutomationApp.Services;
using Microsoft.EntityFrameworkCore;

namespace EnglishAutomationApp.Views.Pages
{
    public partial class DashboardUserControl : UserControl
    {
        private Label welcomeLabel = null!;
        private Label totalCoursesLabel = null!;
        private Label completedCoursesLabel = null!;
        private Label studyTimeLabel = null!;
        private Label vocabularyCountLabel = null!;
        private Button browseCoursesButton = null!;
        private Button studyVocabularyButton = null!;
        private Button viewProgressButton = null!;
        private ListBox recentActivityListBox = null!;

        public DashboardUserControl()
        {
            InitializeComponent();
            LoadDashboardData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // UserControl properties
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.Dock = DockStyle.Fill;

            // Welcome Section
            var welcomePanel = new Panel();
            welcomePanel.BackColor = Color.FromArgb(102, 126, 234);
            welcomePanel.Location = new Point(20, 20);
            welcomePanel.Size = new Size(this.Width - 40, 120);
            welcomePanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            welcomeLabel = new Label();
            welcomeLabel.Text = "Welcome to English Automation! 🌟";
            welcomeLabel.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            welcomeLabel.ForeColor = Color.White;
            welcomeLabel.Location = new Point(30, 20);
            welcomeLabel.Size = new Size(700, 40);

            var motivationLabel = new Label();
            motivationLabel.Text = "Continue your English automation learning journey and make progress today!";
            motivationLabel.Font = new Font("Segoe UI", 14);
            motivationLabel.ForeColor = Color.White;
            motivationLabel.Location = new Point(30, 65);
            motivationLabel.Size = new Size(700, 30);

            welcomePanel.Controls.Add(welcomeLabel);
            welcomePanel.Controls.Add(motivationLabel);

            // Statistics Panel
            var statsPanel = new Panel();
            statsPanel.Location = new Point(20, 160);
            statsPanel.Size = new Size(this.Width - 40, 120);
            statsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Total Courses Card
            var totalCoursesPanel = CreateStatCard("📚", "0", "Total Courses", new Point(0, 0));
            totalCoursesLabel = (Label)totalCoursesPanel.Controls[1];

            // Completed Courses Card
            var completedCoursesPanel = CreateStatCard("✅", "0", "Completed", new Point(190, 0));
            completedCoursesLabel = (Label)completedCoursesPanel.Controls[1];

            // Study Time Card
            var studyTimePanel = CreateStatCard("⏱️", "0h", "Study Time", new Point(380, 0));
            studyTimeLabel = (Label)studyTimePanel.Controls[1];

            // Vocabulary Card
            var vocabularyPanel = CreateStatCard("📖", "0", "Words Learned", new Point(570, 0));
            vocabularyCountLabel = (Label)vocabularyPanel.Controls[1];

            statsPanel.Controls.Add(totalCoursesPanel);
            statsPanel.Controls.Add(completedCoursesPanel);
            statsPanel.Controls.Add(studyTimePanel);
            statsPanel.Controls.Add(vocabularyPanel);

            // Quick Actions Panel
            var actionsPanel = new Panel();
            actionsPanel.BackColor = Color.White;
            actionsPanel.Location = new Point(20, 300);
            actionsPanel.Size = new Size(this.Width - 40, 100);
            actionsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            var actionsLabel = new Label();
            actionsLabel.Text = "Quick Actions";
            actionsLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            actionsLabel.Location = new Point(20, 15);
            actionsLabel.Size = new Size(200, 30);

            browseCoursesButton = new Button();
            browseCoursesButton.Text = "📚 Browse Courses";
            browseCoursesButton.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            browseCoursesButton.BackColor = Color.FromArgb(102, 126, 234);
            browseCoursesButton.ForeColor = Color.White;
            browseCoursesButton.FlatStyle = FlatStyle.Flat;
            browseCoursesButton.Location = new Point(20, 50);
            browseCoursesButton.Size = new Size(150, 35);
            browseCoursesButton.Click += BrowseCoursesButton_Click;

            studyVocabularyButton = new Button();
            studyVocabularyButton.Text = "📖 Study Vocabulary";
            studyVocabularyButton.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            studyVocabularyButton.BackColor = Color.FromArgb(76, 175, 80);
            studyVocabularyButton.ForeColor = Color.White;
            studyVocabularyButton.FlatStyle = FlatStyle.Flat;
            studyVocabularyButton.Location = new Point(190, 50);
            studyVocabularyButton.Size = new Size(150, 35);
            studyVocabularyButton.Click += StudyVocabularyButton_Click;

            viewProgressButton = new Button();
            viewProgressButton.Text = "📊 View Progress";
            viewProgressButton.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            viewProgressButton.BackColor = Color.FromArgb(255, 152, 0);
            viewProgressButton.ForeColor = Color.White;
            viewProgressButton.FlatStyle = FlatStyle.Flat;
            viewProgressButton.Location = new Point(360, 50);
            viewProgressButton.Size = new Size(150, 35);
            viewProgressButton.Click += ViewProgressButton_Click;

            actionsPanel.Controls.Add(actionsLabel);
            actionsPanel.Controls.Add(browseCoursesButton);
            actionsPanel.Controls.Add(studyVocabularyButton);
            actionsPanel.Controls.Add(viewProgressButton);

            // Recent Activity Panel
            var recentActivityPanel = new Panel();
            recentActivityPanel.BackColor = Color.White;
            recentActivityPanel.Location = new Point(20, 420);
            recentActivityPanel.Size = new Size(this.Width - 40, 200);
            recentActivityPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            var recentActivityLabel = new Label();
            recentActivityLabel.Text = "Recent Activity";
            recentActivityLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            recentActivityLabel.Location = new Point(20, 15);
            recentActivityLabel.Size = new Size(200, 30);

            recentActivityListBox = new ListBox();
            recentActivityListBox.Font = new Font("Segoe UI", 10);
            recentActivityListBox.Location = new Point(20, 50);
            recentActivityListBox.Size = new Size(720, 130);

            recentActivityPanel.Controls.Add(recentActivityLabel);
            recentActivityPanel.Controls.Add(recentActivityListBox);

            // Add all panels to UserControl
            this.Controls.Add(welcomePanel);
            this.Controls.Add(statsPanel);
            this.Controls.Add(actionsPanel);
            this.Controls.Add(recentActivityPanel);

            this.ResumeLayout(false);
        }

        private Panel CreateStatCard(string icon, string value, string label, Point location)
        {
            var panel = new Panel();
            panel.BackColor = Color.White;
            panel.Location = location;
            panel.Size = new Size(180, 120);

            var iconLabel = new Label();
            iconLabel.Text = icon;
            iconLabel.Font = new Font("Segoe UI", 24);
            iconLabel.TextAlign = ContentAlignment.MiddleCenter;
            iconLabel.Location = new Point(0, 15);
            iconLabel.Size = new Size(180, 35);

            var valueLabel = new Label();
            valueLabel.Text = value;
            valueLabel.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            valueLabel.ForeColor = Color.FromArgb(102, 126, 234);
            valueLabel.TextAlign = ContentAlignment.MiddleCenter;
            valueLabel.Location = new Point(0, 50);
            valueLabel.Size = new Size(180, 30);

            var descLabel = new Label();
            descLabel.Text = label;
            descLabel.Font = new Font("Segoe UI", 10);
            descLabel.ForeColor = Color.Gray;
            descLabel.TextAlign = ContentAlignment.MiddleCenter;
            descLabel.Location = new Point(0, 85);
            descLabel.Size = new Size(180, 20);

            panel.Controls.Add(iconLabel);
            panel.Controls.Add(valueLabel);
            panel.Controls.Add(descLabel);

            return panel;
        }

        private async void LoadDashboardData()
        {
            try
            {
                if (AuthenticationService.CurrentUser == null) return;

                // Load statistics from Access DB
                var allCourses = await Data.AccessDatabaseHelper.GetAllCoursesAsync();
                var allWords = await Data.AccessDatabaseHelper.GetAllVocabularyWordsAsync();

                var totalCourses = allCourses.Count;
                var completedCourses = 0; // User progress functionality needs to be implemented
                var totalTimeMinutes = 0; // User progress functionality needs to be implemented
                var vocabularyCount = allWords.Count;

                // Update UI
                totalCoursesLabel.Text = totalCourses.ToString();
                completedCoursesLabel.Text = completedCourses.ToString();
                studyTimeLabel.Text = $"{totalTimeMinutes / 60}h";
                vocabularyCountLabel.Text = vocabularyCount.ToString();

                // Update welcome message
                if (AuthenticationService.CurrentUser != null)
                {
                    welcomeLabel.Text = $"Welcome back, {AuthenticationService.CurrentUser.FirstName}! 🌟";
                }

                // Load recent activities
                LoadRecentActivities();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadRecentActivities()
        {
            recentActivityListBox.Items.Clear();
            recentActivityListBox.Items.Add("📚 Started English Automation Course - 2 hours ago");
            recentActivityListBox.Items.Add("📖 Learned 5 new vocabulary words - 1 day ago");
            recentActivityListBox.Items.Add("🏆 Achieved 'First Steps' badge - 2 days ago");
            recentActivityListBox.Items.Add("📈 Completed progress assessment - 3 days ago");
        }

        // Event Handlers
        private void BrowseCoursesButton_Click(object? sender, EventArgs e)
        {
            var mainForm = this.FindForm() as MainForm;
            if (mainForm != null)
            {
                mainForm.NavigateToCoursesPage();
            }
        }

        private void StudyVocabularyButton_Click(object? sender, EventArgs e)
        {
            var mainForm = this.FindForm() as MainForm;
            if (mainForm != null)
            {
                mainForm.ShowUserControl(new VocabularyUserControl());
            }
        }

        private void ViewProgressButton_Click(object? sender, EventArgs e)
        {
            var mainForm = this.FindForm() as MainForm;
            if (mainForm != null)
            {
                mainForm.NavigateToProgressPage();
            }
        }
    }
}
