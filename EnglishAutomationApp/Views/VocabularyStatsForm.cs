using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnglishAutomationApp.Models;
using EnglishAutomationApp.Services;
using EnglishAutomationApp.Helpers;

namespace EnglishAutomationApp.Views
{
    public partial class VocabularyStatsForm : Form
    {
        private readonly int userId;
        private readonly bool isEnglish;
        
        // UI Controls
        private Label titleLabel = null!;
        private Panel statsPanel = null!;
        private Button closeButton = null!;

        public VocabularyStatsForm(int userId, bool isEnglish = true)
        {
            this.userId = userId;
            this.isEnglish = isEnglish;
            InitializeComponent();
            SetupUI();
            LoadStatsAsync();
        }

        private void InitializeComponent()
        {
            this.Text = isEnglish ? "Vocabulary Statistics" : "Kelime İstatistikleri";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = ModernUIHelper.Colors.Background;
        }

        private void SetupUI()
        {
            // Title
            titleLabel = new Label
            {
                Text = isEnglish ? "📊 Vocabulary Statistics" : "📊 Kelime İstatistikleri",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Primary,
                AutoSize = true,
                Location = new Point(30, 30)
            };

            // Stats Panel
            statsPanel = new Panel
            {
                Size = new Size(540, 350),
                Location = new Point(30, 80),
                BackColor = Color.Transparent,
                AutoScroll = true
            };

            // Close Button
            closeButton = ModernUIHelper.CreateButton(
                isEnglish ? "Close" : "Kapat", 
                ModernUIHelper.Colors.Secondary
            );
            closeButton.Size = new Size(100, 40);
            closeButton.Location = new Point(470, 440);
            closeButton.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] 
            { 
                titleLabel, statsPanel, closeButton 
            });
        }

        private async void LoadStatsAsync()
        {
            try
            {
                // Get user statistics
                var userStats = await VocabularyService.GetUserStatsAsync(userId);
                var userVocabulary = await VocabularyService.GetUserVocabulariesAsync(userId);
                var allWords = await VocabularyService.GetAllWordsAsync();

                // Calculate statistics
                var totalWords = allWords.Count;
                var learnedWords = userStats.GetValueOrDefault("TotalWordsLearned", 0);
                var masteredWords = userStats.GetValueOrDefault("MasteredWords", 0);
                var currentStreak = userStats.GetValueOrDefault("CurrentStreak", 0);
                
                var totalAttempts = userVocabulary.Sum(uv => uv.TotalAttempts);
                var totalCorrect = userVocabulary.Sum(uv => uv.CorrectAnswers);
                var accuracy = totalAttempts > 0 ? (totalCorrect * 100 / totalAttempts) : 0;

                // Group by difficulty
                var beginnerWords = userVocabulary.Count(uv => 
                    allWords.Any(w => w.Id == uv.VocabularyWordId && w.Difficulty == DifficultyLevel.Beginner));
                var intermediateWords = userVocabulary.Count(uv => 
                    allWords.Any(w => w.Id == uv.VocabularyWordId && w.Difficulty == DifficultyLevel.Intermediate));
                var advancedWords = userVocabulary.Count(uv => 
                    allWords.Any(w => w.Id == uv.VocabularyWordId && w.Difficulty == DifficultyLevel.Advanced));

                // Group by mastery level
                var masteryLevel1 = userVocabulary.Count(uv => uv.MasteryLevel == 1);
                var masteryLevel2 = userVocabulary.Count(uv => uv.MasteryLevel == 2);
                var masteryLevel3 = userVocabulary.Count(uv => uv.MasteryLevel == 3);
                var masteryLevel4 = userVocabulary.Count(uv => uv.MasteryLevel == 4);
                var masteryLevel5 = userVocabulary.Count(uv => uv.MasteryLevel == 5);

                // Create stats cards
                CreateStatsCards(totalWords, learnedWords, masteredWords, currentStreak, accuracy,
                    beginnerWords, intermediateWords, advancedWords,
                    masteryLevel1, masteryLevel2, masteryLevel3, masteryLevel4, masteryLevel5);
            }
            catch (Exception ex)
            {
                var message = isEnglish 
                    ? $"Error loading statistics: {ex.Message}" 
                    : $"İstatistikler yüklenirken hata: {ex.Message}";
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateStatsCards(int totalWords, int learnedWords, int masteredWords, int currentStreak, int accuracy,
            int beginnerWords, int intermediateWords, int advancedWords,
            int level1, int level2, int level3, int level4, int level5)
        {
            statsPanel.Controls.Clear();
            int yPosition = 10;

            // Overall Statistics Card
            var overallCard = CreateStatsCard(
                isEnglish ? "📈 Overall Statistics" : "📈 Genel İstatistikler",
                new Dictionary<string, string>
                {
                    { isEnglish ? "Total Words in Database" : "Veritabanındaki Toplam Kelime", totalWords.ToString() },
                    { isEnglish ? "Words You've Learned" : "Öğrendiğiniz Kelimeler", learnedWords.ToString() },
                    { isEnglish ? "Mastered Words" : "Ustalaştığınız Kelimeler", masteredWords.ToString() },
                    { isEnglish ? "Current Streak" : "Mevcut Seri", $"{currentStreak} " + (isEnglish ? "days" : "gün") },
                    { isEnglish ? "Overall Accuracy" : "Genel Doğruluk", $"{accuracy}%" }
                },
                yPosition
            );
            statsPanel.Controls.Add(overallCard);
            yPosition += overallCard.Height + 15;

            // Difficulty Level Statistics Card
            var difficultyCard = CreateStatsCard(
                isEnglish ? "🎯 By Difficulty Level" : "🎯 Zorluk Seviyesine Göre",
                new Dictionary<string, string>
                {
                    { isEnglish ? "Beginner Words" : "Başlangıç Kelimeleri", beginnerWords.ToString() },
                    { isEnglish ? "Intermediate Words" : "Orta Seviye Kelimeler", intermediateWords.ToString() },
                    { isEnglish ? "Advanced Words" : "İleri Seviye Kelimeler", advancedWords.ToString() }
                },
                yPosition
            );
            statsPanel.Controls.Add(difficultyCard);
            yPosition += difficultyCard.Height + 15;

            // Mastery Level Statistics Card
            var masteryCard = CreateStatsCard(
                isEnglish ? "⭐ Mastery Levels" : "⭐ Ustalık Seviyeleri",
                new Dictionary<string, string>
                {
                    { isEnglish ? "Level 1 (Learning)" : "Seviye 1 (Öğreniyor)", level1.ToString() },
                    { isEnglish ? "Level 2 (Familiar)" : "Seviye 2 (Tanıdık)", level2.ToString() },
                    { isEnglish ? "Level 3 (Known)" : "Seviye 3 (Bilinen)", level3.ToString() },
                    { isEnglish ? "Level 4 (Good)" : "Seviye 4 (İyi)", level4.ToString() },
                    { isEnglish ? "Level 5 (Mastered)" : "Seviye 5 (Ustalaşmış)", level5.ToString() }
                },
                yPosition
            );
            statsPanel.Controls.Add(masteryCard);
        }

        private Panel CreateStatsCard(string title, Dictionary<string, string> stats, int yPosition)
        {
            var card = new Panel
            {
                Size = new Size(520, 40 + (stats.Count * 25) + 20),
                Location = new Point(10, yPosition),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            ModernUIHelper.ApplyRoundedCorners(card, 8);
            ModernUIHelper.ApplyShadow(card);

            // Title
            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Primary,
                Location = new Point(15, 15),
                AutoSize = true
            };
            card.Controls.Add(titleLabel);

            // Stats
            int statY = 45;
            foreach (var stat in stats)
            {
                var statLabel = new Label
                {
                    Text = $"{stat.Key}: {stat.Value}",
                    Font = new Font("Segoe UI", 10),
                    ForeColor = ModernUIHelper.Colors.TextSecondary,
                    Location = new Point(25, statY),
                    AutoSize = true
                };
                card.Controls.Add(statLabel);
                statY += 25;
            }

            return card;
        }
    }
}
