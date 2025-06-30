using System;
using System.Drawing;
using System.Windows.Forms;

namespace EnglishAutomationApp.Views.Pages
{
    public partial class AchievementsUserControl : UserControl
    {
        public AchievementsUserControl()
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
            titleLabel.Text = "🏆 Achievements";
            titleLabel.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(102, 126, 234);
            titleLabel.Location = new Point(30, 20);
            titleLabel.Size = new Size(500, 40);

            // Content Panel
            var contentPanel = new Panel();
            contentPanel.BackColor = Color.White;
            contentPanel.Location = new Point(30, 80);
            contentPanel.Size = new Size(740, 500);

            var contentLabel = new Label();
            contentLabel.Text = "Achievement system will be implemented here.\n\n" +
                               "Features to include:\n" +
                               "• Learning milestones\n" +
                               "• Badge collection\n" +
                               "• Streak tracking\n" +
                               "• Leaderboards\n" +
                               "• Reward system";
            contentLabel.Font = new Font("Segoe UI", 12);
            contentLabel.Location = new Point(30, 30);
            contentLabel.Size = new Size(680, 440);

            contentPanel.Controls.Add(contentLabel);

            // Add controls to UserControl
            this.Controls.Add(titleLabel);
            this.Controls.Add(contentPanel);

            this.ResumeLayout(false);
        }
    }
}
