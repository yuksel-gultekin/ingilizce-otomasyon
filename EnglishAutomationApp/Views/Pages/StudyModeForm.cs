using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EnglishAutomationApp.Helpers;
using EnglishAutomationApp.Models;
using EnglishAutomationApp.Services;

namespace EnglishAutomationApp.Views.Pages
{
    public partial class StudyModeForm : Form
    {
        private List<VocabularyWord> words;
        private int currentWordIndex = 0;
        private int correctAnswers = 0;
        private bool showingAnswer = false;

        private Panel headerPanel;
        private Panel cardPanel;
        private Panel buttonPanel;
        private Label progressLabel;
        private Label scoreLabel;
        private Label wordLabel;
        private Label meaningLabel;
        private Label pronunciationLabel;
        private Label exampleLabel;
        private Button showAnswerButton;
        private Button correctButton;
        private Button incorrectButton;
        private Button nextButton;
        private Button finishButton;

        public StudyModeForm(List<VocabularyWord> studyWords)
        {
            words = studyWords.OrderBy(x => Guid.NewGuid()).ToList(); // Shuffle words
            InitializeComponent();
            ShowCurrentWord();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Study Mode - Vocabulary Practice";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            ModernUIHelper.ApplyModernFormStyle(this);

            CreateHeaderPanel();
            CreateCardPanel();
            CreateButtonPanel();

            // Add controls to form
            this.Controls.Add(buttonPanel);
            this.Controls.Add(cardPanel);
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

            var titleLabel = ModernUIHelper.CreateHeading("📚 Study Mode", 3);
            titleLabel.ForeColor = Color.White;
            titleLabel.Location = new Point(ModernUIHelper.Spacing.Large, ModernUIHelper.Spacing.Small);

            progressLabel = new Label
            {
                Font = ModernUIHelper.Fonts.Body,
                ForeColor = Color.White,
                Location = new Point(200, ModernUIHelper.Spacing.Small),
                AutoSize = true
            };

            scoreLabel = new Label
            {
                Font = ModernUIHelper.Fonts.Body,
                ForeColor = Color.White,
                Location = new Point(350, ModernUIHelper.Spacing.Small),
                AutoSize = true
            };

            headerPanel.Controls.AddRange(new Control[] { titleLabel, progressLabel, scoreLabel });
        }

        private void CreateCardPanel()
        {
            cardPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ModernUIHelper.Colors.Background,
                Padding = new Padding(ModernUIHelper.Spacing.Large)
            };

            var card = ModernUIHelper.CreateCard(ModernUIHelper.Spacing.XLarge);
            card.Dock = DockStyle.Fill;

            // Word display
            wordLabel = ModernUIHelper.CreateHeading("", 1);
            wordLabel.Location = new Point(0, 20);
            wordLabel.Width = card.Width - 100;
            wordLabel.TextAlign = ContentAlignment.MiddleCenter;
            wordLabel.Dock = DockStyle.Top;

            // Pronunciation
            pronunciationLabel = ModernUIHelper.CreateBodyText("", true);
            pronunciationLabel.Location = new Point(0, 80);
            pronunciationLabel.Width = card.Width - 100;
            pronunciationLabel.TextAlign = ContentAlignment.MiddleCenter;
            pronunciationLabel.Dock = DockStyle.Top;

            // Meaning (initially hidden)
            meaningLabel = ModernUIHelper.CreateHeading("", 3);
            meaningLabel.ForeColor = ModernUIHelper.Colors.Secondary;
            meaningLabel.Location = new Point(0, 140);
            meaningLabel.Width = card.Width - 100;
            meaningLabel.TextAlign = ContentAlignment.MiddleCenter;
            meaningLabel.Visible = false;
            meaningLabel.Dock = DockStyle.Top;

            // Example (initially hidden)
            exampleLabel = ModernUIHelper.CreateBodyText("", true);
            exampleLabel.Location = new Point(0, 200);
            exampleLabel.Width = card.Width - 100;
            exampleLabel.TextAlign = ContentAlignment.MiddleCenter;
            exampleLabel.Visible = false;
            exampleLabel.Dock = DockStyle.Top;

            card.Controls.AddRange(new Control[] { exampleLabel, meaningLabel, pronunciationLabel, wordLabel });
            cardPanel.Controls.Add(card);
        }

        private void CreateButtonPanel()
        {
            buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = ModernUIHelper.Colors.SurfaceVariant,
                Padding = new Padding(ModernUIHelper.Spacing.Large)
            };

            // Show Answer button
            showAnswerButton = ModernUIHelper.CreateModernButton("Show Answer", ModernUIHelper.Colors.Primary);
            showAnswerButton.Location = new Point(50, ModernUIHelper.Spacing.Medium);
            showAnswerButton.Width = 120;
            showAnswerButton.Click += ShowAnswerButton_Click;

            // Correct button (initially hidden)
            correctButton = ModernUIHelper.CreateModernButton("✓ Correct", ModernUIHelper.Colors.Success);
            correctButton.Location = new Point(180, ModernUIHelper.Spacing.Medium);
            correctButton.Width = 100;
            correctButton.Visible = false;
            correctButton.Click += CorrectButton_Click;

            // Incorrect button (initially hidden)
            incorrectButton = ModernUIHelper.CreateModernButton("✗ Incorrect", ModernUIHelper.Colors.Error);
            incorrectButton.Location = new Point(290, ModernUIHelper.Spacing.Medium);
            incorrectButton.Width = 100;
            incorrectButton.Visible = false;
            incorrectButton.Click += IncorrectButton_Click;

            // Next button (initially hidden)
            nextButton = ModernUIHelper.CreateModernButton("Next →", ModernUIHelper.Colors.Primary);
            nextButton.Location = new Point(400, ModernUIHelper.Spacing.Medium);
            nextButton.Width = 80;
            nextButton.Visible = false;
            nextButton.Click += NextButton_Click;

            // Finish button (initially hidden)
            finishButton = ModernUIHelper.CreateModernButton("Finish", ModernUIHelper.Colors.Secondary);
            finishButton.Location = new Point(490, ModernUIHelper.Spacing.Medium);
            finishButton.Width = 80;
            finishButton.Visible = false;
            finishButton.Click += FinishButton_Click;

            buttonPanel.Controls.AddRange(new Control[] 
            { 
                showAnswerButton, correctButton, incorrectButton, nextButton, finishButton 
            });
        }

        private void ShowCurrentWord()
        {
            if (currentWordIndex >= words.Count)
            {
                ShowResults();
                return;
            }

            var word = words[currentWordIndex];
            
            // Update progress
            progressLabel.Text = $"Word {currentWordIndex + 1} of {words.Count}";
            scoreLabel.Text = $"Score: {correctAnswers}/{currentWordIndex}";

            // Show word
            wordLabel.Text = word.EnglishWord;
            pronunciationLabel.Text = word.Pronunciation ?? "";
            meaningLabel.Text = word.TurkishMeaning;
            
            var exampleText = "";
            if (!string.IsNullOrEmpty(word.ExampleSentence))
            {
                exampleText = $"Example: {word.ExampleSentence}";
                if (!string.IsNullOrEmpty(word.ExampleSentenceTurkish))
                {
                    exampleText += $"\n{word.ExampleSentenceTurkish}";
                }
            }
            exampleLabel.Text = exampleText;

            // Reset UI state
            showingAnswer = false;
            meaningLabel.Visible = false;
            exampleLabel.Visible = false;
            showAnswerButton.Visible = true;
            correctButton.Visible = false;
            incorrectButton.Visible = false;
            nextButton.Visible = false;
            finishButton.Visible = false;
        }

        private void ShowAnswerButton_Click(object sender, EventArgs e)
        {
            showingAnswer = true;
            meaningLabel.Visible = true;
            exampleLabel.Visible = true;
            showAnswerButton.Visible = false;
            correctButton.Visible = true;
            incorrectButton.Visible = true;
        }

        private async void CorrectButton_Click(object sender, EventArgs e)
        {
            correctAnswers++;
            await RecordAnswer(true);
            ShowNextButtons();
        }

        private async void IncorrectButton_Click(object sender, EventArgs e)
        {
            await RecordAnswer(false);
            ShowNextButtons();
        }

        private async System.Threading.Tasks.Task RecordAnswer(bool isCorrect)
        {
            if (AuthenticationService.CurrentUser != null)
            {
                try
                {
                    var word = words[currentWordIndex];
                    await VocabularyService.AddOrUpdateUserVocabularyAsync(
                        AuthenticationService.CurrentUser.Id, word.Id, isCorrect);
                }
                catch (Exception ex)
                {
                    // Log error but don't interrupt study session
                    System.Diagnostics.Debug.WriteLine($"Error recording answer: {ex.Message}");
                }
            }
        }

        private void ShowNextButtons()
        {
            correctButton.Visible = false;
            incorrectButton.Visible = false;
            
            if (currentWordIndex < words.Count - 1)
            {
                nextButton.Visible = true;
            }
            else
            {
                finishButton.Visible = true;
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            currentWordIndex++;
            ShowCurrentWord();
        }

        private void FinishButton_Click(object sender, EventArgs e)
        {
            ShowResults();
        }

        private void ShowResults()
        {
            var percentage = words.Count > 0 ? (double)correctAnswers / words.Count * 100 : 0;
            var message = $"Study session completed!\n\n" +
                         $"Words studied: {words.Count}\n" +
                         $"Correct answers: {correctAnswers}\n" +
                         $"Accuracy: {percentage:F1}%\n\n" +
                         $"Great job! Keep practicing to improve your vocabulary.";

            MessageBox.Show(message, "Study Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }
}
