using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnglishAutomationApp.Helpers;
using EnglishAutomationApp.Models;
using EnglishAutomationApp.Services;
using EnglishAutomationApp.Views;

namespace EnglishAutomationApp.Views.Pages
{
    public partial class AdminVocabularyControl : UserControl
    {
        private Panel headerPanel = null!;
        private Panel toolbarPanel = null!;
        private Panel contentPanel = null!;
        private DataGridView wordsDataGridView = null!;
        private Button addWordButton = null!;
        private Button refreshButton = null!;
        private TextBox searchTextBox = null!;
        private ComboBox categoryFilter = null!;
        private ComboBox difficultyFilter = null!;
        private Label statsLabel = null!;

        private List<VocabularyWord> allWords = new List<VocabularyWord>();
        private bool isEnglish = true;

        public AdminVocabularyControl()
        {
            InitializeComponent();
            LoadWordsAsync();
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
                BackColor = ModernUIHelper.Colors.Primary,
                Padding = new Padding(ModernUIHelper.Spacing.Large)
            };

            var titleLabel = ModernUIHelper.CreateHeading("📖 Vocabulary Management", 2);
            titleLabel.ForeColor = Color.White;
            titleLabel.Location = new Point(ModernUIHelper.Spacing.Large, ModernUIHelper.Spacing.Medium);

            statsLabel = new Label
            {
                Font = ModernUIHelper.Fonts.Body,
                ForeColor = Color.FromArgb(200, 200, 255),
                Location = new Point(ModernUIHelper.Spacing.Large, 45),
                Size = new Size(600, 20),
                Text = "Loading statistics..."
            };

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(statsLabel);
        }

        private void CreateToolbarPanel()
        {
            toolbarPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = ModernUIHelper.Colors.Surface,
                Padding = new Padding(ModernUIHelper.Spacing.Large, ModernUIHelper.Spacing.Medium)
            };

            // Search textbox
            searchTextBox = ModernUIHelper.CreateModernTextBox("Search words...");
            searchTextBox.Location = new Point(20, 20);
            searchTextBox.Size = new Size(200, 30);
            searchTextBox.TextChanged += SearchTextBox_TextChanged;

            // Category filter
            categoryFilter = new ComboBox
            {
                Font = ModernUIHelper.Fonts.Body,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(240, 20),
                Size = new Size(120, 30)
            };
            categoryFilter.Items.AddRange(new[] { "All Categories", "General", "Business", "Academic", "Travel", "Technology" });
            categoryFilter.SelectedIndex = 0;
            categoryFilter.SelectedIndexChanged += CategoryFilter_SelectedIndexChanged;

            // Difficulty filter
            difficultyFilter = new ComboBox
            {
                Font = ModernUIHelper.Fonts.Body,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(380, 20),
                Size = new Size(120, 30)
            };
            difficultyFilter.Items.AddRange(new[] { "All Levels", "Beginner", "Intermediate", "Advanced" });
            difficultyFilter.SelectedIndex = 0;
            difficultyFilter.SelectedIndexChanged += DifficultyFilter_SelectedIndexChanged;

            // Add word button
            addWordButton = ModernUIHelper.CreateIconButton("+ Add Word", "📝", ModernUIHelper.Colors.Primary, 130);
            addWordButton.Location = new Point(520, 20);
            addWordButton.Click += AddWordButton_Click;

            // Refresh button
            refreshButton = ModernUIHelper.CreateIconButton("🔄 Refresh", "↻", ModernUIHelper.Colors.Secondary, 120);
            refreshButton.Location = new Point(670, 20);
            refreshButton.Click += RefreshButton_Click;

            toolbarPanel.Controls.Add(searchTextBox);
            toolbarPanel.Controls.Add(categoryFilter);
            toolbarPanel.Controls.Add(difficultyFilter);
            toolbarPanel.Controls.Add(addWordButton);
            toolbarPanel.Controls.Add(refreshButton);
        }

        private void CreateContentPanel()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ModernUIHelper.Colors.Background,
                Padding = new Padding(ModernUIHelper.Spacing.Large)
            };

            // Words DataGridView
            wordsDataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = ModernUIHelper.Colors.Surface,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                GridColor = ModernUIHelper.Colors.SurfaceVariant,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false
            };

            // Apply modern styling to DataGridView
            wordsDataGridView.DefaultCellStyle.BackColor = ModernUIHelper.Colors.Surface;
            wordsDataGridView.DefaultCellStyle.ForeColor = ModernUIHelper.Colors.TextPrimary;
            wordsDataGridView.DefaultCellStyle.SelectionBackColor = ModernUIHelper.Colors.Primary;
            wordsDataGridView.DefaultCellStyle.SelectionForeColor = Color.White;
            wordsDataGridView.ColumnHeadersDefaultCellStyle.BackColor = ModernUIHelper.Colors.SurfaceVariant;
            wordsDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = ModernUIHelper.Colors.TextPrimary;
            wordsDataGridView.ColumnHeadersDefaultCellStyle.Font = ModernUIHelper.Fonts.Body;
            wordsDataGridView.EnableHeadersVisualStyles = false;

            contentPanel.Controls.Add(wordsDataGridView);
        }

        private async void LoadWordsAsync()
        {
            try
            {
                allWords = await VocabularyService.GetAllWordsAsync();
                UpdateStatsLabel();
                SetupDataGridView();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                var message = isEnglish ? $"Error loading words: {ex.Message}" : $"Kelimeler yüklenirken hata: {ex.Message}";
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateStatsLabel()
        {
            var totalWords = allWords.Count;
            var beginnerWords = allWords.Count(w => w.Difficulty == WordDifficulty.Beginner);
            var intermediateWords = allWords.Count(w => w.Difficulty == WordDifficulty.Intermediate);
            var advancedWords = allWords.Count(w => w.Difficulty == WordDifficulty.Advanced);

            if (isEnglish)
            {
                statsLabel.Text = $"Total Words: {totalWords} | Beginner: {beginnerWords} | Intermediate: {intermediateWords} | Advanced: {advancedWords}";
            }
            else
            {
                statsLabel.Text = $"Toplam Kelime: {totalWords} | Başlangıç: {beginnerWords} | Orta: {intermediateWords} | İleri: {advancedWords}";
            }
        }

        private void SetupDataGridView()
        {
            wordsDataGridView.Columns.Clear();

            // Add columns
            wordsDataGridView.Columns.Add("Id", "ID");
            wordsDataGridView.Columns.Add("EnglishWord", isEnglish ? "English Word" : "İngilizce Kelime");
            wordsDataGridView.Columns.Add("TurkishMeaning", isEnglish ? "Turkish Meaning" : "Türkçe Anlamı");
            wordsDataGridView.Columns.Add("Pronunciation", isEnglish ? "Pronunciation" : "Telaffuz");
            wordsDataGridView.Columns.Add("Category", isEnglish ? "Category" : "Kategori");
            wordsDataGridView.Columns.Add("Difficulty", isEnglish ? "Difficulty" : "Zorluk");
            wordsDataGridView.Columns.Add("CreatedDate", isEnglish ? "Created Date" : "Oluşturma Tarihi");

            // Set column widths
            wordsDataGridView.Columns["Id"].Width = 50;
            wordsDataGridView.Columns["EnglishWord"].Width = 150;
            wordsDataGridView.Columns["TurkishMeaning"].Width = 150;
            wordsDataGridView.Columns["Pronunciation"].Width = 120;
            wordsDataGridView.Columns["Category"].Width = 100;
            wordsDataGridView.Columns["Difficulty"].Width = 100;
            wordsDataGridView.Columns["CreatedDate"].Width = 120;

            // Add action buttons
            var editColumn = new DataGridViewButtonColumn
            {
                Name = "Edit",
                HeaderText = isEnglish ? "Edit" : "Düzenle",
                Text = isEnglish ? "Edit" : "Düzenle",
                UseColumnTextForButtonValue = true,
                Width = 80
            };
            wordsDataGridView.Columns.Add(editColumn);

            var deleteColumn = new DataGridViewButtonColumn
            {
                Name = "Delete",
                HeaderText = isEnglish ? "Delete" : "Sil",
                Text = isEnglish ? "Delete" : "Sil",
                UseColumnTextForButtonValue = true,
                Width = 80
            };
            wordsDataGridView.Columns.Add(deleteColumn);

            // Handle button clicks
            wordsDataGridView.CellClick += WordsDataGridView_CellClick;
        }

        private void DisplayWords(List<VocabularyWord> words)
        {
            wordsDataGridView.Rows.Clear();
            foreach (var word in words)
            {
                wordsDataGridView.Rows.Add(
                    word.Id,
                    word.EnglishWord,
                    word.TurkishMeaning,
                    word.Pronunciation ?? "",
                    word.Category ?? "General",
                    word.DifficultyText,
                    word.CreatedDate.ToString("yyyy-MM-dd")
                );
            }
        }

        private void ApplyFilters()
        {
            var filteredWords = allWords.AsEnumerable();

            // Search filter
            if (!string.IsNullOrWhiteSpace(searchTextBox.Text))
            {
                var searchTerm = searchTextBox.Text.ToLower();
                filteredWords = filteredWords.Where(w =>
                    w.EnglishWord.ToLower().Contains(searchTerm) ||
                    w.TurkishMeaning.ToLower().Contains(searchTerm));
            }

            // Category filter
            if (categoryFilter.SelectedIndex > 0)
            {
                var selectedCategory = categoryFilter.SelectedItem.ToString();
                filteredWords = filteredWords.Where(w => w.Category == selectedCategory);
            }

            // Difficulty filter
            if (difficultyFilter.SelectedIndex > 0)
            {
                var selectedDifficulty = difficultyFilter.SelectedItem.ToString();
                filteredWords = filteredWords.Where(w => w.DifficultyText == selectedDifficulty);
            }

            DisplayWords(filteredWords.ToList());
        }

        private void SearchTextBox_TextChanged(object? sender, EventArgs e)
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

        private void AddWordButton_Click(object? sender, EventArgs e)
        {
            var addWordForm = new AddEditWordForm();
            if (addWordForm.ShowDialog() == DialogResult.OK)
            {
                LoadWordsAsync(); // Refresh the list
            }
        }

        private void RefreshButton_Click(object? sender, EventArgs e)
        {
            LoadWordsAsync();
        }

        private async void WordsDataGridView_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var wordId = Convert.ToInt32(wordsDataGridView.Rows[e.RowIndex].Cells["Id"].Value);
            var word = allWords.FirstOrDefault(w => w.Id == wordId);
            if (word == null) return;

            if (e.ColumnIndex == wordsDataGridView.Columns["Edit"].Index)
            {
                // Edit word
                var editForm = new AddEditWordForm(word);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadWordsAsync(); // Refresh the list
                }
            }
            else if (e.ColumnIndex == wordsDataGridView.Columns["Delete"].Index)
            {
                // Delete word
                var message = isEnglish
                    ? $"Are you sure you want to delete the word '{word.EnglishWord}'?"
                    : $"'{word.EnglishWord}' kelimesini silmek istediğinizden emin misiniz?";
                var title = isEnglish ? "Confirm Delete" : "Silmeyi Onayla";

                var result = MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        await VocabularyService.DeleteWordAsync(wordId);
                        LoadWordsAsync(); // Refresh the list

                        var successMessage = isEnglish ? "Word deleted successfully." : "Kelime başarıyla silindi.";
                        MessageBox.Show(successMessage, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        var errorMessage = isEnglish
                            ? $"Error deleting word: {ex.Message}"
                            : $"Kelime silinirken hata: {ex.Message}";
                        MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void UpdateLanguage()
        {
            if (isEnglish)
            {
                // English
                searchTextBox.PlaceholderText = "Search words...";
                addWordButton.Text = "+ Add Word";
                refreshButton.Text = "🔄 Refresh";

                // Update filter items
                categoryFilter.Items.Clear();
                categoryFilter.Items.AddRange(new[] { "All Categories", "General", "Business", "Academic", "Travel", "Technology" });
                categoryFilter.SelectedIndex = 0;

                difficultyFilter.Items.Clear();
                difficultyFilter.Items.AddRange(new[] { "All Levels", "Beginner", "Intermediate", "Advanced" });
                difficultyFilter.SelectedIndex = 0;
            }
            else
            {
                // Turkish
                searchTextBox.PlaceholderText = "Kelime ara...";
                addWordButton.Text = "+ Kelime Ekle";
                refreshButton.Text = "🔄 Yenile";

                // Update filter items
                categoryFilter.Items.Clear();
                categoryFilter.Items.AddRange(new[] { "Tüm Kategoriler", "Genel", "İş", "Akademik", "Seyahat", "Teknoloji" });
                categoryFilter.SelectedIndex = 0;

                difficultyFilter.Items.Clear();
                difficultyFilter.Items.AddRange(new[] { "Tüm Seviyeler", "Başlangıç", "Orta", "İleri" });
                difficultyFilter.SelectedIndex = 0;
            }

            // Update header title
            var titleLabel = headerPanel.Controls.OfType<Label>().FirstOrDefault();
            if (titleLabel != null)
            {
                titleLabel.Text = isEnglish ? "📖 Vocabulary Management" : "📖 Kelime Yönetimi";
            }

            // Refresh data grid view and stats
            UpdateStatsLabel();
            SetupDataGridView();
            ApplyFilters();
        }
    }
}
