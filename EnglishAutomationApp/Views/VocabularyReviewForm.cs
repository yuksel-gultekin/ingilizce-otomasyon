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
    public partial class VocabularyReviewForm : Form
    {
        private readonly List<VocabularyWord> wordsToReview;
        private readonly bool isEnglish;
        private int currentWordIndex = 0;
        private int correctAnswers = 0;
        private int totalAnswers = 0;
        
        // UI Controls
        private Label titleLabel = null!;
        private Label progressLabel = null!;
        private Label wordLabel = null!;
        private Label meaningLabel = null!;
        private Button correctButton = null!;
        private Button wrongButton = null!;
        private Button nextButton = null!;
        private Button finishButton = null!;
        private Panel cardPanel = null!;
        private Panel buttonPanel = null!;

        public VocabularyReviewForm(List<VocabularyWord> words, bool isEnglish = true)
        {
            this.wordsToReview = words;
            this.isEnglish = isEnglish;
            InitializeComponent();
            SetupUI();
            ShowCurrentWord();
        }

        private void InitializeComponent()
        {
            this.Text = isEnglish ? "Vocabulary Review" : "Kelime İncelemesi";
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
                Text = isEnglish ? "Vocabulary Review" : "Kelime İncelemesi",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Primary,
                AutoSize = true,
                Location = new Point(30, 30)
            };

            // Progress
            progressLabel = new Label
            {
                Font = new Font("Segoe UI", 10),
                ForeColor = ModernUIHelper.Colors.TextMuted,
                AutoSize = true,
                Location = new Point(30, 70)
            };

            // Card Panel
            cardPanel = new Panel
            {
                Size = new Size(540, 250),
                Location = new Point(30, 110),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            ModernUIHelper.ApplyRoundedCorners(cardPanel, 10);
            ModernUIHelper.ApplyShadow(cardPanel);

            // Word Label
            wordLabel = new Label
            {
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Primary,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(500, 80),
                Location = new Point(20, 20)
            };

            // Meaning Label
            meaningLabel = new Label
            {
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Primary,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(500, 80),
                Location = new Point(20, 120),
                Visible = false
            };

            cardPanel.Controls.AddRange(new Control[] { wordLabel, meaningLabel });

            // Button Panel
            buttonPanel = new Panel
            {
                Size = new Size(540, 80),
                Location = new Point(30, 380),
                BackColor = Color.Transparent
            };

            // Correct Button
            correctButton = ModernUIHelper.CreateButton(
                isEnglish ? "✓ Correct" : "✓ Doğru", 
                ModernUIHelper.Colors.Success
            );
            correctButton.Size = new Size(120, 50);
            correctButton.Location = new Point(100, 15);
            correctButton.Click += CorrectButton_Click;

            // Wrong Button
            wrongButton = ModernUIHelper.CreateButton(
                isEnglish ? "✗ Wrong" : "✗ Yanlış", 
                ModernUIHelper.Colors.Error
            );
            wrongButton.Size = new Size(120, 50);
            wrongButton.Location = new Point(240, 15);
            wrongButton.Click += WrongButton_Click;

            // Next Button
            nextButton = ModernUIHelper.CreateButton(
                isEnglish ? "Next →" : "Sonraki →", 
                ModernUIHelper.Colors.Primary
            );
            nextButton.Size = new Size(100, 50);
            nextButton.Location = new Point(380, 15);
            nextButton.Visible = false;
            nextButton.Click += NextButton_Click;

            // Finish Button
            finishButton = ModernUIHelper.CreateButton(
                isEnglish ? "Finish" : "Bitir", 
                ModernUIHelper.Colors.Secondary
            );
            finishButton.Size = new Size(100, 50);
            finishButton.Location = new Point(380, 15);
            finishButton.Visible = false;
            finishButton.Click += FinishButton_Click;

            buttonPanel.Controls.AddRange(new Control[] 
            { 
                correctButton, wrongButton, nextButton, finishButton 
            });

            this.Controls.AddRange(new Control[] 
            { 
                titleLabel, progressLabel, cardPanel, buttonPanel 
            });
        }

        private void ShowCurrentWord()
        {
            if (currentWordIndex >= wordsToReview.Count)
            {
                ShowResults();
                return;
            }

            var currentWord = wordsToReview[currentWordIndex];
            
            // Update progress
            progressLabel.Text = isEnglish 
                ? $"Word {currentWordIndex + 1} of {wordsToReview.Count}"
                : $"Kelime {currentWordIndex + 1} / {wordsToReview.Count}";

            // Show word
            wordLabel.Text = currentWord.EnglishWord;
            meaningLabel.Text = currentWord.TurkishMeaning;
            meaningLabel.Visible = false;

            // Reset buttons
            correctButton.Visible = true;
            wrongButton.Visible = true;
            nextButton.Visible = false;
            finishButton.Visible = false;
        }

        private void CorrectButton_Click(object? sender, EventArgs e)
        {
            ProcessAnswer(true);
        }

        private void WrongButton_Click(object? sender, EventArgs e)
        {
            ProcessAnswer(false);
        }

        private async void ProcessAnswer(bool isCorrect)
        {
            totalAnswers++;
            if (isCorrect) correctAnswers++;

            // Show meaning
            meaningLabel.Visible = true;

            // Hide answer buttons
            correctButton.Visible = false;
            wrongButton.Visible = false;

            // Show next/finish button
            if (currentWordIndex < wordsToReview.Count - 1)
            {
                nextButton.Visible = true;
            }
            else
            {
                finishButton.Visible = true;
            }

            // Update user vocabulary
            if (AuthenticationService.CurrentUser != null)
            {
                try
                {
                    await VocabularyService.AddOrUpdateUserVocabularyAsync(
                        AuthenticationService.CurrentUser.Id,
                        wordsToReview[currentWordIndex].Id,
                        isCorrect
                    );
                }
                catch (Exception ex)
                {
                    // Log error but don't interrupt the review
                    Console.WriteLine($"Error updating user vocabulary: {ex.Message}");
                }
            }
        }

        private void NextButton_Click(object? sender, EventArgs e)
        {
            currentWordIndex++;
            ShowCurrentWord();
        }

        private void FinishButton_Click(object? sender, EventArgs e)
        {
            ShowResults();
        }

        private void ShowResults()
        {
            var percentage = totalAnswers > 0 ? (correctAnswers * 100 / totalAnswers) : 0;
            
            var message = isEnglish
                ? $"Review Complete!\n\nCorrect: {correctAnswers}/{totalAnswers}\nAccuracy: {percentage}%\n\nKeep practicing to improve your vocabulary!"
                : $"İnceleme Tamamlandı!\n\nDoğru: {correctAnswers}/{totalAnswers}\nDoğruluk: {percentage}%\n\nKelime dağarcığınızı geliştirmek için pratik yapmaya devam edin!";

            var title = isEnglish ? "Review Results" : "İnceleme Sonuçları";

            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
