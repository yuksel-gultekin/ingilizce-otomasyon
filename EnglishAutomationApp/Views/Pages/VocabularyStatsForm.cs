using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnglishAutomationApp.Helpers;
using EnglishAutomationApp.Models;
using EnglishAutomationApp.Services;

namespace EnglishAutomationApp.Views.Pages
{
    public partial class VocabularyStatsForm : Form
    {
        private int userId;
        private Panel headerPanel = null!;
        private Panel statsPanel = null!;
        private Panel weakWordsPanel = null!;
        private FlowLayoutPanel statsCardsPanel = null!;
        private FlowLayoutPanel weakWordsCardsPanel = null!;

        public VocabularyStatsForm(int userId)
        {
            this.userId = userId;
            InitializeComponent();
            LoadStatsAsync();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Vocabulary Statistics";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            ModernUIHelper.ApplyModernFormStyle(this);

            CreateHeaderPanel();
            CreateStatsPanel();
            CreateWeakWordsPanel();

            // Add controls to form
            this.Controls.Add(weakWordsPanel);
            this.Controls.Add(statsPanel);
            this.Controls.Add(headerPanel);

            this.ResumeLayout(false);
        }

        private void CreateHeaderPanel()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = ModernUIHelper.Colors.Primary,
                Padding = new Padding(ModernUIHelper.Spacing.Large)
            };

            var titleLabel = ModernUIHelper.CreateHeading("📊 Your Vocabulary Statistics", 2);
            titleLabel.ForeColor = Color.White;
            titleLabel.Location = new Point(ModernUIHelper.Spacing.Large, ModernUIHelper.Spacing.Medium);

            headerPanel.Controls.Add(titleLabel);
        }

        private void CreateStatsPanel()
        {
            statsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 200,
                BackColor = ModernUIHelper.Colors.Background,
                Padding = new Padding(ModernUIHelper.Spacing.Large)
            };

            var statsTitle = ModernUIHelper.CreateHeading("Overall Progress", 3);
            statsTitle.Location = new Point(0, 0);

            statsCardsPanel = new FlowLayoutPanel
            {
                Location = new Point(0, 40),
                Size = new Size(statsPanel.Width - 40, 140),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                BackColor = ModernUIHelper.Colors.Background
            };

            statsPanel.Controls.Add(statsTitle);
            statsPanel.Controls.Add(statsCardsPanel);
        }

        private void CreateWeakWordsPanel()
        {
            weakWordsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ModernUIHelper.Colors.Background,
                Padding = new Padding(ModernUIHelper.Spacing.Large)
            };

            var weakWordsTitle = ModernUIHelper.CreateHeading("Words That Need Practice", 3);
            weakWordsTitle.Location = new Point(0, 0);

            weakWordsCardsPanel = new FlowLayoutPanel
            {
                Location = new Point(0, 40),
                Size = new Size(weakWordsPanel.Width - 40, weakWordsPanel.Height - 80),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = ModernUIHelper.Colors.Background
            };

            weakWordsPanel.Controls.Add(weakWordsTitle);
            weakWordsPanel.Controls.Add(weakWordsCardsPanel);
        }

        private async void LoadStatsAsync()
        {
            try
            {
                // Load user statistics
                var stats = await VocabularyService.GetUserStatsAsync(userId);
                DisplayStats(stats);

                // Load weak words
                var weakWords = await VocabularyService.GetWeakWordsAsync(userId);
                DisplayWeakWords(weakWords);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading statistics: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayStats(Dictionary<string, int> stats)
        {
            statsCardsPanel.Controls.Clear();

            // Total Words Learned
            var totalCard = CreateStatCard("📚 Total Learned", 
                stats.GetValueOrDefault("TotalWordsLearned", 0).ToString(), 
                ModernUIHelper.Colors.Primary);
            statsCardsPanel.Controls.Add(totalCard);

            // Mastered Words
            var masteredCard = CreateStatCard("🏆 Mastered", 
                stats.GetValueOrDefault("MasteredWords", 0).ToString(), 
                ModernUIHelper.Colors.Success);
            statsCardsPanel.Controls.Add(masteredCard);

            // Words in Progress
            var progressCard = CreateStatCard("📖 In Progress", 
                stats.GetValueOrDefault("WordsInProgress", 0).ToString(), 
                ModernUIHelper.Colors.Warning);
            statsCardsPanel.Controls.Add(progressCard);

            // Total Reviews
            var reviewsCard = CreateStatCard("🔄 Total Reviews", 
                stats.GetValueOrDefault("TotalReviews", 0).ToString(), 
                ModernUIHelper.Colors.Secondary);
            statsCardsPanel.Controls.Add(reviewsCard);

            // Average Accuracy
            var accuracyCard = CreateStatCard("🎯 Accuracy", 
                $"{stats.GetValueOrDefault("AverageAccuracy", 0)}%", 
                ModernUIHelper.Colors.Primary);
            statsCardsPanel.Controls.Add(accuracyCard);

            // Current Streak
            var streakCard = CreateStatCard("🔥 Current Streak", 
                $"{stats.GetValueOrDefault("CurrentStreak", 0)} days", 
                ModernUIHelper.Colors.Error);
            statsCardsPanel.Controls.Add(streakCard);
        }

        private Panel CreateStatCard(string title, string value, Color color)
        {
            var card = ModernUIHelper.CreateCard(ModernUIHelper.Spacing.Medium);
            card.Size = new Size(120, 80);
            card.Margin = new Padding(ModernUIHelper.Spacing.Small);

            var titleLabel = new Label
            {
                Text = title,
                Font = ModernUIHelper.Fonts.Small,
                ForeColor = ModernUIHelper.Colors.TextSecondary,
                Location = new Point(ModernUIHelper.Spacing.Small, ModernUIHelper.Spacing.Small),
                Size = new Size(card.Width - 20, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = ModernUIHelper.Fonts.Heading3,
                ForeColor = color,
                Location = new Point(ModernUIHelper.Spacing.Small, 30),
                Size = new Size(card.Width - 20, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            card.Controls.Add(titleLabel);
            card.Controls.Add(valueLabel);

            return card;
        }

        private void DisplayWeakWords(List<VocabularyWord> weakWords)
        {
            weakWordsCardsPanel.Controls.Clear();

            if (!weakWords.Any())
            {
                var noWordsLabel = ModernUIHelper.CreateBodyText("Great job! No words need extra practice right now.", true);
                noWordsLabel.TextAlign = ContentAlignment.MiddleCenter;
                noWordsLabel.Size = new Size(400, 50);
                weakWordsCardsPanel.Controls.Add(noWordsLabel);
                return;
            }

            foreach (var word in weakWords)
            {
                var wordCard = CreateWeakWordCard(word);
                weakWordsCardsPanel.Controls.Add(wordCard);
            }
        }

        private Panel CreateWeakWordCard(VocabularyWord word)
        {
            var card = ModernUIHelper.CreateCard(ModernUIHelper.Spacing.Medium);
            card.Size = new Size(weakWordsCardsPanel.Width - 40, 80);
            card.Margin = new Padding(0, 0, 0, ModernUIHelper.Spacing.Small);

            // Word and meaning
            var wordLabel = ModernUIHelper.CreateHeading(word.EnglishWord, 4);
            wordLabel.Location = new Point(0, 0);
            wordLabel.Width = 200;

            var meaningLabel = ModernUIHelper.CreateBodyText(word.TurkishMeaning);
            meaningLabel.Location = new Point(0, 25);
            meaningLabel.Width = 200;

            // Difficulty badge
            var difficultyLabel = new Label
            {
                Text = word.DifficultyText,
                Font = ModernUIHelper.Fonts.Small,
                ForeColor = Color.White,
                BackColor = GetDifficultyColor(word.Difficulty),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(220, 10),
                Size = new Size(70, 20),
                Padding = new Padding(4)
            };

            // Practice button
            var practiceButton = ModernUIHelper.CreateIconButton("Practice", "📝", ModernUIHelper.Colors.Primary, 100, 32);
            practiceButton.Location = new Point(card.Width - 110, 25);
            practiceButton.Tag = word;
            practiceButton.Click += PracticeButton_Click;

            card.Controls.AddRange(new Control[] 
            { 
                wordLabel, meaningLabel, difficultyLabel, practiceButton 
            });

            return card;
        }

        private Color GetDifficultyColor(WordDifficulty difficulty)
        {
            return difficulty switch
            {
                WordDifficulty.Beginner => ModernUIHelper.Colors.Success,
                WordDifficulty.Intermediate => ModernUIHelper.Colors.Warning,
                WordDifficulty.Advanced => ModernUIHelper.Colors.Error,
                _ => ModernUIHelper.Colors.TextMuted
            };
        }

        private void PracticeButton_Click(object? sender, EventArgs e)
        {
            var button = sender as Button;
            var word = button?.Tag as VocabularyWord;
            
            if (word != null)
            {
                var practiceWords = new List<VocabularyWord> { word };
                var studyForm = new StudyModeForm(practiceWords, isReviewMode: true);
                studyForm.ShowDialog();
                
                // Refresh stats after practice
                LoadStatsAsync();
            }
        }
    }
}
