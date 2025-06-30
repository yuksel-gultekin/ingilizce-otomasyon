using System;
using System.Drawing;
using System.Windows.Forms;
using EnglishAutomationApp.Helpers;
using EnglishAutomationApp.Models;
using EnglishAutomationApp.Services;

namespace EnglishAutomationApp.Views.Pages
{
    public partial class AddEditWordForm : Form
    {
        private VocabularyWord? editingWord;
        private TextBox englishWordTextBox = null!;
        private TextBox turkishMeaningTextBox = null!;
        private TextBox pronunciationTextBox = null!;
        private TextBox exampleSentenceTextBox = null!;
        private TextBox exampleSentenceTurkishTextBox = null!;
        private ComboBox difficultyComboBox = null!;
        private ComboBox partOfSpeechComboBox = null!;
        private TextBox categoryTextBox = null!;
        private Button saveButton = null!;
        private Button cancelButton = null!;

        public AddEditWordForm(VocabularyWord? word = null)
        {
            editingWord = word;
            InitializeComponent();
            
            if (editingWord != null)
            {
                LoadWordData();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = editingWord == null ? "Add New Word" : "Edit Word";
            this.Size = new Size(600, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            ModernUIHelper.ApplyModernFormStyle(this);

            CreateFormControls();

            this.ResumeLayout(false);
        }

        private void CreateFormControls()
        {
            var yPosition = 30;
            var labelWidth = 160;
            var textBoxWidth = 350;
            var spacing = 45;

            // English Word
            var englishWordLabel = ModernUIHelper.CreateBodyText("English Word:");
            englishWordLabel.Location = new Point(30, yPosition);
            englishWordLabel.Width = labelWidth;

            englishWordTextBox = ModernUIHelper.CreateModernTextBox();
            englishWordTextBox.Location = new Point(200, yPosition);
            englishWordTextBox.Width = textBoxWidth;

            yPosition += spacing;

            // Turkish Meaning
            var turkishMeaningLabel = ModernUIHelper.CreateBodyText("Turkish Meaning:");
            turkishMeaningLabel.Location = new Point(30, yPosition);
            turkishMeaningLabel.Width = labelWidth;

            turkishMeaningTextBox = ModernUIHelper.CreateModernTextBox();
            turkishMeaningTextBox.Location = new Point(200, yPosition);
            turkishMeaningTextBox.Width = textBoxWidth;

            yPosition += spacing;

            // Pronunciation
            var pronunciationLabel = ModernUIHelper.CreateBodyText("Pronunciation:");
            pronunciationLabel.Location = new Point(30, yPosition);
            pronunciationLabel.Width = labelWidth;

            pronunciationTextBox = ModernUIHelper.CreateModernTextBox();
            pronunciationTextBox.Location = new Point(200, yPosition);
            pronunciationTextBox.Width = textBoxWidth;

            yPosition += spacing;

            // Example Sentence
            var exampleSentenceLabel = ModernUIHelper.CreateBodyText("Example Sentence:");
            exampleSentenceLabel.Location = new Point(30, yPosition);
            exampleSentenceLabel.Width = labelWidth;

            exampleSentenceTextBox = new TextBox
            {
                Font = ModernUIHelper.Fonts.Body,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = ModernUIHelper.Colors.Surface,
                ForeColor = ModernUIHelper.Colors.TextPrimary,
                Location = new Point(200, yPosition),
                Width = textBoxWidth,
                Height = 70,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            yPosition += 80;

            // Example Sentence Turkish
            var exampleSentenceTurkishLabel = ModernUIHelper.CreateBodyText("Turkish Translation:");
            exampleSentenceTurkishLabel.Location = new Point(30, yPosition);
            exampleSentenceTurkishLabel.Width = labelWidth;

            exampleSentenceTurkishTextBox = new TextBox
            {
                Font = ModernUIHelper.Fonts.Body,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = ModernUIHelper.Colors.Surface,
                ForeColor = ModernUIHelper.Colors.TextPrimary,
                Location = new Point(200, yPosition),
                Width = textBoxWidth,
                Height = 70,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            yPosition += 80;

            // Difficulty
            var difficultyLabel = ModernUIHelper.CreateBodyText("Difficulty:");
            difficultyLabel.Location = new Point(30, yPosition);
            difficultyLabel.Width = labelWidth;

            difficultyComboBox = new ComboBox
            {
                Font = ModernUIHelper.Fonts.Body,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(200, yPosition),
                Width = 180
            };
            difficultyComboBox.Items.AddRange(new[] { "Beginner", "Intermediate", "Advanced" });
            difficultyComboBox.SelectedIndex = 0;

            yPosition += spacing;

            // Part of Speech
            var partOfSpeechLabel = ModernUIHelper.CreateBodyText("Part of Speech:");
            partOfSpeechLabel.Location = new Point(30, yPosition);
            partOfSpeechLabel.Width = labelWidth;

            partOfSpeechComboBox = new ComboBox
            {
                Font = ModernUIHelper.Fonts.Body,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(200, yPosition),
                Width = 180
            };
            partOfSpeechComboBox.Items.AddRange(new[] { "Noun", "Verb", "Adjective", "Adverb", "Pronoun", "Preposition", "Conjunction", "Interjection" });
            partOfSpeechComboBox.SelectedIndex = 0;

            yPosition += spacing;

            // Category
            var categoryLabel = ModernUIHelper.CreateBodyText("Category:");
            categoryLabel.Location = new Point(30, yPosition);
            categoryLabel.Width = labelWidth;

            categoryTextBox = ModernUIHelper.CreateModernTextBox();
            categoryTextBox.Location = new Point(200, yPosition);
            categoryTextBox.Width = textBoxWidth;

            yPosition += spacing + 30;

            // Buttons
            saveButton = ModernUIHelper.CreateLargeButton("💾 Save", ModernUIHelper.Colors.Primary);
            saveButton.Location = new Point(320, yPosition);
            saveButton.Click += SaveButton_Click;

            cancelButton = ModernUIHelper.CreateLargeButton("❌ Cancel", ModernUIHelper.Colors.TextMuted);
            cancelButton.Location = new Point(490, yPosition);
            cancelButton.Click += CancelButton_Click;

            // Add all controls to form
            this.Controls.AddRange(new Control[]
            {
                englishWordLabel, englishWordTextBox,
                turkishMeaningLabel, turkishMeaningTextBox,
                pronunciationLabel, pronunciationTextBox,
                exampleSentenceLabel, exampleSentenceTextBox,
                exampleSentenceTurkishLabel, exampleSentenceTurkishTextBox,
                difficultyLabel, difficultyComboBox,
                partOfSpeechLabel, partOfSpeechComboBox,
                categoryLabel, categoryTextBox,
                saveButton, cancelButton
            });
        }

        private void LoadWordData()
        {
            if (editingWord == null) return;

            englishWordTextBox.Text = editingWord.EnglishWord;
            turkishMeaningTextBox.Text = editingWord.TurkishMeaning;
            pronunciationTextBox.Text = editingWord.Pronunciation ?? "";
            exampleSentenceTextBox.Text = editingWord.ExampleSentence ?? "";
            exampleSentenceTurkishTextBox.Text = editingWord.ExampleSentenceTurkish ?? "";
            difficultyComboBox.SelectedItem = editingWord.DifficultyText;
            partOfSpeechComboBox.SelectedItem = editingWord.PartOfSpeechText;
            categoryTextBox.Text = editingWord.Category ?? "";
        }

        private async void SaveButton_Click(object? sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                var word = editingWord ?? new VocabularyWord();
                
                word.EnglishWord = englishWordTextBox.Text.Trim();
                word.TurkishMeaning = turkishMeaningTextBox.Text.Trim();
                word.Pronunciation = string.IsNullOrWhiteSpace(pronunciationTextBox.Text) ? null : pronunciationTextBox.Text.Trim();
                word.ExampleSentence = string.IsNullOrWhiteSpace(exampleSentenceTextBox.Text) ? null : exampleSentenceTextBox.Text.Trim();
                word.ExampleSentenceTurkish = string.IsNullOrWhiteSpace(exampleSentenceTurkishTextBox.Text) ? null : exampleSentenceTurkishTextBox.Text.Trim();
                word.Difficulty = Enum.Parse<WordDifficulty>(difficultyComboBox.SelectedItem.ToString());
                word.PartOfSpeech = Enum.Parse<PartOfSpeech>(partOfSpeechComboBox.SelectedItem.ToString());
                word.Category = string.IsNullOrWhiteSpace(categoryTextBox.Text) ? null : categoryTextBox.Text.Trim();

                if (editingWord == null)
                {
                    await VocabularyService.AddWordAsync(word);
                }
                else
                {
                    await VocabularyService.UpdateWordAsync(word);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving word: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(englishWordTextBox.Text))
            {
                MessageBox.Show("English word is required.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                englishWordTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(turkishMeaningTextBox.Text))
            {
                MessageBox.Show("Turkish meaning is required.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                turkishMeaningTextBox.Focus();
                return false;
            }

            return true;
        }
    }
}
