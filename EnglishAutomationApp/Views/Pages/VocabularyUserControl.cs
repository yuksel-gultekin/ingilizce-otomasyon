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
    public partial class VocabularyUserControl : UserControl
    {
        private Panel headerPanel = null!;
        private Panel toolbarPanel = null!;
        private Panel contentPanel = null!;
        private FlowLayoutPanel wordsPanel = null!;
        private TextBox searchBox = null!;
        private ComboBox categoryFilter = null!;
        private ComboBox difficultyFilter = null!;
        private Button addWordButton = null!;
        private Label statsLabel = null!;

        private List<VocabularyWord> allWords = new List<VocabularyWord>();
        private List<VocabularyWord> filteredWords = new List<VocabularyWord>();
        private bool isEnglish = true;

        public VocabularyUserControl()
        {
            InitializeComponent();
            this.Load += VocabularyUserControl_Load;
        }

        private async void VocabularyUserControl_Load(object? sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        public void SetLanguage(bool english)
        {
            isEnglish = english;
            UpdateLanguage();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // UserControl properties
            this.Size = new Size(800, 600);
            this.BackColor = ModernUIHelper.Colors.Background;
            this.Dock = DockStyle.Fill;

            CreateHeaderPanel();
            CreateToolbarPanel();
            CreateContentPanel();

            // Add controls to UserControl
            this.Controls.Add(contentPanel);
            this.Controls.Add(toolbarPanel);
            this.Controls.Add(headerPanel);

            this.ResumeLayout(false);
        }

        private void CreateHeaderPanel()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = ModernUIHelper.Colors.Surface,
                Padding = new Padding(ModernUIHelper.Spacing.Large)
            };

            var titleLabel = ModernUIHelper.CreateHeading("📖 Vocabulary Learning", 2);
            titleLabel.Location = new Point(ModernUIHelper.Spacing.Large, ModernUIHelper.Spacing.Medium);

            statsLabel = ModernUIHelper.CreateBodyText("Loading...", true);
            statsLabel.Location = new Point(ModernUIHelper.Spacing.Large, 50);

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(statsLabel);
        }

        private void CreateToolbarPanel()
        {
            toolbarPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100, // Increased height for two rows
                BackColor = ModernUIHelper.Colors.SurfaceVariant,
                Padding = new Padding(ModernUIHelper.Spacing.Large, ModernUIHelper.Spacing.Small,
                                    ModernUIHelper.Spacing.Large, ModernUIHelper.Spacing.Small)
            };

            // First row - Search and filters
            // Search box
            searchBox = ModernUIHelper.CreateModernTextBox("Search words...");
            searchBox.Location = new Point(ModernUIHelper.Spacing.Large, ModernUIHelper.Spacing.Small);
            searchBox.Width = 200;
            searchBox.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            searchBox.TextChanged += SearchBox_TextChanged;

            // Category filter
            categoryFilter = new ComboBox
            {
                Font = ModernUIHelper.Fonts.Body,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(240, ModernUIHelper.Spacing.Small),
                Width = 120,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            categoryFilter.Items.Add("All Categories");
            categoryFilter.SelectedIndex = 0;
            categoryFilter.SelectedIndexChanged += CategoryFilter_SelectedIndexChanged;

            // Difficulty filter
            difficultyFilter = new ComboBox
            {
                Font = ModernUIHelper.Fonts.Body,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(370, ModernUIHelper.Spacing.Small),
                Width = 120,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            difficultyFilter.Items.AddRange(new[] { "All Levels", "Beginner", "Intermediate", "Advanced" });
            difficultyFilter.SelectedIndex = 0;
            difficultyFilter.SelectedIndexChanged += DifficultyFilter_SelectedIndexChanged;

            // Second row - Action buttons (improved spacing and sizing)
            // Add word button
            addWordButton = ModernUIHelper.CreateIconButton("+ Add Word", "➕", ModernUIHelper.Colors.Secondary, 130);
            addWordButton.Location = new Point(ModernUIHelper.Spacing.Large, 50);
            addWordButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            addWordButton.Click += AddWordButton_Click;



            // Review button
            var reviewButton = ModernUIHelper.CreateIconButton("🔄 Review", "📝", ModernUIHelper.Colors.Secondary, 130);
            reviewButton.Location = new Point(170, 50);
            reviewButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            reviewButton.Click += ReviewButton_Click;

            // Stats button
            var statsButton = ModernUIHelper.CreateIconButton("📊 Stats", "📈", ModernUIHelper.Colors.Warning, 120);
            statsButton.Location = new Point(320, 50);
            statsButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            statsButton.Click += StatsButton_Click;

            toolbarPanel.Controls.AddRange(new Control[]
            {
                searchBox, categoryFilter, difficultyFilter, addWordButton, reviewButton, statsButton
            });
        }

        private void CreateContentPanel()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ModernUIHelper.Colors.Background,
                Padding = new Padding(ModernUIHelper.Spacing.Large),
                AutoScroll = true
            };

            wordsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = ModernUIHelper.Colors.Background
            };

            contentPanel.Controls.Add(wordsPanel);
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // Initialize database first
                await Data.AccessDatabaseHelper.InitializeDatabaseAsync();

                // Load all words
                allWords = await VocabularyService.GetAllWordsAsync();
                filteredWords = allWords.ToList();

                // Load categories
                var categories = await VocabularyService.GetCategoriesAsync();
                categoryFilter.Items.Clear();
                categoryFilter.Items.Add("All Categories");
                categoryFilter.Items.AddRange(categories.ToArray());
                categoryFilter.SelectedIndex = 0;

                // Ensure difficulty filter is properly set
                if (difficultyFilter.SelectedIndex < 0)
                {
                    difficultyFilter.SelectedIndex = 0;
                }

                // Force UI update on main thread
                this.Invoke((MethodInvoker)delegate
                {
                    DisplayWords();
                    UpdateStatsLabel();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading vocabulary data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void UpdateStatsLabel()
        {
            var totalWords = allWords.Count;
            var displayedWords = filteredWords.Count;

            if (AuthenticationService.CurrentUser != null)
            {
                try
                {
                    var userStats = await VocabularyService.GetUserStatsAsync(AuthenticationService.CurrentUser.Id);
                    var learnedWords = userStats.GetValueOrDefault("TotalWordsLearned", 0);
                    var masteredWords = userStats.GetValueOrDefault("MasteredWords", 0);
                    var currentStreak = userStats.GetValueOrDefault("CurrentStreak", 0);

                    if (isEnglish)
                    {
                        statsLabel.Text = $"Total: {totalWords} words | Showing: {displayedWords} | " +
                                         $"Learned: {learnedWords} | Mastered: {masteredWords} | " +
                                         $"Streak: {currentStreak} days 🔥";
                    }
                    else
                    {
                        statsLabel.Text = $"Toplam: {totalWords} kelime | Gösterilen: {displayedWords} | " +
                                         $"Öğrenilen: {learnedWords} | Ustalaşılan: {masteredWords} | " +
                                         $"Seri: {currentStreak} gün 🔥";
                    }
                }
                catch
                {
                    if (isEnglish)
                    {
                        statsLabel.Text = $"Total: {totalWords} words | Showing: {displayedWords} words";
                    }
                    else
                    {
                        statsLabel.Text = $"Toplam: {totalWords} kelime | Gösterilen: {displayedWords} kelime";
                    }
                }
            }
            else
            {
                if (isEnglish)
                {
                    statsLabel.Text = $"Total: {totalWords} words | Showing: {displayedWords} words";
                }
                else
                {
                    statsLabel.Text = $"Toplam: {totalWords} kelime | Gösterilen: {displayedWords} kelime";
                }
            }
        }

        private void DisplayWords()
        {
            wordsPanel.Controls.Clear();

            foreach (var word in filteredWords)
            {
                var wordCard = CreateWordCard(word);
                wordsPanel.Controls.Add(wordCard);
            }

            UpdateStatsLabel();
        }

        private Panel CreateWordCard(VocabularyWord word)
        {
            var card = ModernUIHelper.CreateCard(ModernUIHelper.Spacing.Large);
            card.Width = wordsPanel.Width - 50;
            card.Height = 120;
            card.Margin = new Padding(0, 0, 0, ModernUIHelper.Spacing.Medium);

            // Word and meaning
            var wordLabel = ModernUIHelper.CreateHeading(word.EnglishWord, 3);
            wordLabel.Location = new Point(0, 0);

            var meaningLabel = ModernUIHelper.CreateBodyText(word.TurkishMeaning);
            meaningLabel.Location = new Point(0, 30);

            // Pronunciation
            if (!string.IsNullOrEmpty(word.Pronunciation))
            {
                var pronunciationLabel = ModernUIHelper.CreateBodyText(word.Pronunciation, true);
                pronunciationLabel.Location = new Point(0, 55);
                card.Controls.Add(pronunciationLabel);
            }

            // Example sentence
            if (!string.IsNullOrEmpty(word.ExampleSentence))
            {
                var exampleLabel = ModernUIHelper.CreateBodyText($"Example: {word.ExampleSentence}", true);
                exampleLabel.Location = new Point(0, 75);
                exampleLabel.Width = card.Width - 200;
                card.Controls.Add(exampleLabel);
            }

            // Difficulty and category badges (bigger size)
            var difficultyLabel = new Label
            {
                Text = word.DifficultyText,
                Font = ModernUIHelper.Fonts.Body,
                ForeColor = Color.White,
                BackColor = GetDifficultyColor(word.Difficulty),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(card.Width - 220, 10),
                Size = new Size(110, 35),
                Padding = new Padding(8),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            var categoryLabel = new Label
            {
                Text = word.Category ?? "General",
                Font = ModernUIHelper.Fonts.Body,
                ForeColor = ModernUIHelper.Colors.TextSecondary,
                BackColor = ModernUIHelper.Colors.SurfaceVariant,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(card.Width - 100, 10),
                Size = new Size(110, 35),
                Padding = new Padding(8),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // Edit button
            var editButton = ModernUIHelper.CreateSmallButton("Edit", ModernUIHelper.Colors.Primary);
            editButton.Location = new Point(card.Width - 170, card.Height - 40);
            editButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            editButton.Click += (s, e) => EditWord(word);

            // Delete button
            var deleteButton = ModernUIHelper.CreateSmallButton("Delete", ModernUIHelper.Colors.Error);
            deleteButton.Location = new Point(card.Width - 85, card.Height - 40);
            deleteButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            deleteButton.Click += (s, e) => DeleteWord(word);

            card.Controls.AddRange(new Control[]
            {
                wordLabel, meaningLabel, difficultyLabel, categoryLabel, editButton, deleteButton
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

        private void SearchBox_TextChanged(object? sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void CategoryFilter_SelectedIndexChanged(object? sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void DifficultyFilter_SelectedIndexChanged(object? sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (allWords == null || allWords.Count == 0)
            {
                filteredWords = new List<VocabularyWord>();
                DisplayWords();
                return;
            }

            var searchTerm = searchBox?.Text?.ToLower() ?? "";
            var selectedCategory = categoryFilter?.SelectedItem?.ToString() ?? "All Categories";
            var selectedDifficulty = difficultyFilter?.SelectedItem?.ToString() ?? "All Levels";

            filteredWords = allWords.Where(word =>
            {
                // Search filter
                var matchesSearch = string.IsNullOrEmpty(searchTerm) ||
                    word.EnglishWord.ToLower().Contains(searchTerm) ||
                    word.TurkishMeaning.ToLower().Contains(searchTerm) ||
                    (word.ExampleSentence?.ToLower().Contains(searchTerm) ?? false);

                // Category filter
                var matchesCategory = selectedCategory == "All Categories" ||
                    selectedCategory == "Tüm Kategoriler" ||
                    word.Category == selectedCategory;

                // Difficulty filter
                var matchesDifficulty = selectedDifficulty == "All Levels" ||
                    selectedDifficulty == "Tüm Seviyeler" ||
                    word.DifficultyText == selectedDifficulty;

                return matchesSearch && matchesCategory && matchesDifficulty;
            }).ToList();

            DisplayWords();
        }

        private async void AddWordButton_Click(object? sender, EventArgs e)
        {
            var addWordForm = new AddEditWordForm();
            if (addWordForm.ShowDialog() == DialogResult.OK)
            {
                await LoadDataAsync();
            }
        }



        private void EditWord(VocabularyWord word)
        {
            var editForm = new AddEditWordForm(word);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadDataAsync();
            }
        }

        private async void DeleteWord(VocabularyWord word)
        {
            var result = MessageBox.Show($"Are you sure you want to delete the word '{word.EnglishWord}'?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    await VocabularyService.DeleteWordAsync(word.Id);
                    LoadDataAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting word: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void ReviewButton_Click(object? sender, EventArgs e)
        {
            if (AuthenticationService.CurrentUser == null)
            {
                MessageBox.Show("Please login to use the review feature.", "Login Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var wordsForReview = await VocabularyService.GetWordsForReviewAsync(AuthenticationService.CurrentUser.Id);

                if (wordsForReview.Any())
                {
                    MessageBox.Show("Study Mode feature has been removed.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Great job! No words need review right now. Come back later!",
                        "No Review Needed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading review words: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StatsButton_Click(object? sender, EventArgs e)
        {
            if (AuthenticationService.CurrentUser == null)
            {
                MessageBox.Show("Please login to view your statistics.", "Login Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show("Vocabulary Stats feature has been removed.", "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateLanguage()
        {
            if (isEnglish)
            {
                // English
                var titleLabel = headerPanel.Controls.OfType<Label>().FirstOrDefault();
                if (titleLabel != null) titleLabel.Text = "📖 Vocabulary Learning";

                searchBox.PlaceholderText = "Search words...";
                addWordButton.Text = "+ Add Word";

                // Update filter items
                if (categoryFilter.Items.Count > 0)
                {
                    var selectedCategory = categoryFilter.SelectedItem?.ToString();
                    categoryFilter.Items[0] = "All Categories";
                    if (selectedCategory == "Tüm Kategoriler") categoryFilter.SelectedIndex = 0;
                }

                if (difficultyFilter.Items.Count > 0)
                {
                    var selectedDifficulty = difficultyFilter.SelectedIndex;
                    difficultyFilter.Items.Clear();
                    difficultyFilter.Items.AddRange(new[] { "All Levels", "Beginner", "Intermediate", "Advanced" });
                    difficultyFilter.SelectedIndex = selectedDifficulty >= 0 ? selectedDifficulty : 0;
                }
            }
            else
            {
                // Turkish
                var titleLabel = headerPanel.Controls.OfType<Label>().FirstOrDefault();
                if (titleLabel != null) titleLabel.Text = "📖 Kelime Öğrenme";

                searchBox.PlaceholderText = "Kelime ara...";
                addWordButton.Text = "+ Kelime Ekle";

                // Update filter items
                if (categoryFilter.Items.Count > 0)
                {
                    var selectedCategory = categoryFilter.SelectedItem?.ToString();
                    categoryFilter.Items[0] = "Tüm Kategoriler";
                    if (selectedCategory == "All Categories") categoryFilter.SelectedIndex = 0;
                }

                if (difficultyFilter.Items.Count > 0)
                {
                    var selectedDifficulty = difficultyFilter.SelectedIndex;
                    difficultyFilter.Items.Clear();
                    difficultyFilter.Items.AddRange(new[] { "Tüm Seviyeler", "Başlangıç", "Orta", "İleri" });
                    difficultyFilter.SelectedIndex = selectedDifficulty >= 0 ? selectedDifficulty : 0;
                }
            }

            // Update stats label
            UpdateStatsLabel();
        }
    }
}