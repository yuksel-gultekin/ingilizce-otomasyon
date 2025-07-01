using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnglishAutomationApp.Data;
using EnglishAutomationApp.Models;
using EnglishAutomationApp.Views;
using EnglishAutomationApp.Services;
using EnglishAutomationApp.Helpers;

namespace EnglishAutomationApp.Views.Pages
{
    public partial class CoursesUserControl : UserControl
    {
        private Panel headerPanel = null!;
        private Panel toolbarPanel = null!;
        private Panel contentPanel = null!;
        private FlowLayoutPanel coursesPanel = null!;
        private ComboBox levelFilterComboBox = null!;
        private ComboBox typeFilterComboBox = null!;
        private TextBox searchTextBox = null!;
        private Label statsLabel = null!;

        private List<Course> allCourses = new List<Course>();
        private List<Course> filteredCourses = new List<Course>();
        private bool isEnglish = true;

        public CoursesUserControl()
        {
            InitializeComponent();
            LoadCoursesAsync();
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
                BackColor = ModernUIHelper.Colors.Surface,
                Padding = new Padding(ModernUIHelper.Spacing.Large)
            };

            var titleLabel = ModernUIHelper.CreateHeading("📚 English Learning Courses", 2);
            titleLabel.Location = new Point(ModernUIHelper.Spacing.Large, ModernUIHelper.Spacing.Medium);

            statsLabel = ModernUIHelper.CreateBodyText("Loading courses...", true);
            statsLabel.Location = new Point(ModernUIHelper.Spacing.Large, 50);

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(statsLabel);
        }

        private void CreateToolbarPanel()
        {
            toolbarPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = ModernUIHelper.Colors.SurfaceVariant,
                Padding = new Padding(ModernUIHelper.Spacing.Large, ModernUIHelper.Spacing.Small,
                                    ModernUIHelper.Spacing.Large, ModernUIHelper.Spacing.Small)
            };

            // Search box
            searchTextBox = ModernUIHelper.CreateModernTextBox("Search courses...");
            searchTextBox.Location = new Point(ModernUIHelper.Spacing.Large, ModernUIHelper.Spacing.Small);
            searchTextBox.Width = 200;
            searchTextBox.TextChanged += (s, e) => ApplyFilters();

            // Level filter
            levelFilterComboBox = new ComboBox
            {
                Font = ModernUIHelper.Fonts.Body,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(240, ModernUIHelper.Spacing.Small),
                Width = 120
            };
            levelFilterComboBox.Items.AddRange(new[] { "All Levels", "Beginner", "Intermediate", "Advanced" });
            levelFilterComboBox.SelectedIndex = 0;
            levelFilterComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Type filter
            typeFilterComboBox = new ComboBox
            {
                Font = ModernUIHelper.Fonts.Body,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(370, ModernUIHelper.Spacing.Small),
                Width = 120
            };
            typeFilterComboBox.Items.AddRange(new[] { "All Types", "Grammar", "Vocabulary", "Speaking", "Writing", "Listening", "Pronunciation" });
            typeFilterComboBox.SelectedIndex = 0;
            typeFilterComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            toolbarPanel.Controls.AddRange(new Control[]
            {
                searchTextBox, levelFilterComboBox, typeFilterComboBox
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

            coursesPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = true,
                BackColor = ModernUIHelper.Colors.Background,
                Padding = new Padding(0, 0, 20, 0) // Right padding for scrollbar
            };

            contentPanel.Controls.Add(coursesPanel);
        }

        private async void LoadCoursesAsync()
        {
            try
            {
                allCourses = await Data.AccessDatabaseHelper.GetAllCoursesAsync();

                // Apply filters to show all courses initially
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading courses: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateStatsLabel()
        {
            var totalCourses = allCourses.Count;
            var displayedCourses = filteredCourses.Count;

            if (isEnglish)
            {
                statsLabel.Text = $"Total: {totalCourses} courses | Showing: {displayedCourses} courses | All courses are FREE!";
            }
            else
            {
                statsLabel.Text = $"Toplam: {totalCourses} kurs | Gösterilen: {displayedCourses} kurs | Tüm kurslar ÜCRETSİZ!";
            }
        }

        private void ApplyFilters()
        {
            var searchTerm = searchTextBox.Text.ToLower();
            var selectedLevel = levelFilterComboBox.SelectedItem?.ToString();
            var selectedType = typeFilterComboBox.SelectedItem?.ToString();

            filteredCourses = allCourses.Where(course =>
            {
                // Search filter
                var matchesSearch = string.IsNullOrEmpty(searchTerm) ||
                    course.Title.ToLower().Contains(searchTerm) ||
                    course.Description.ToLower().Contains(searchTerm);

                // Level filter
                var matchesLevel = selectedLevel == "All Levels" ||
                    selectedLevel == "Tüm Seviyeler" ||
                    course.LevelText == selectedLevel;

                // Type filter
                var matchesType = selectedType == "All Types" ||
                    selectedType == "Tüm Türler" ||
                    course.TypeText == selectedType;

                return matchesSearch && matchesLevel && matchesType;
            }).ToList();

            UpdateStatsLabel();
            DisplayCourses();
        }

        private void DisplayCourses()
        {
            coursesPanel.Controls.Clear();

            if (!filteredCourses.Any())
            {
                var noCoursesLabel = ModernUIHelper.CreateBodyText("No courses found matching your criteria.", true);
                noCoursesLabel.TextAlign = ContentAlignment.MiddleCenter;
                noCoursesLabel.Size = new Size(700, 50);
                coursesPanel.Controls.Add(noCoursesLabel);
                return;
            }

            foreach (var course in filteredCourses)
            {
                var courseCard = CreateCourseCard(course);
                coursesPanel.Controls.Add(courseCard);
            }
        }

        private Panel CreateCourseCard(Course course)
        {
            var card = ModernUIHelper.CreateCard(ModernUIHelper.Spacing.Large);
            card.Size = new Size(280, 320);
            card.Margin = new Padding(ModernUIHelper.Spacing.Medium);

            // Course Title
            var titleLabel = ModernUIHelper.CreateHeading(course.Title, 4);
            titleLabel.Location = new Point(0, 0);
            titleLabel.Width = card.Width - 40;

            // Course Level Badge (bigger size)
            var levelLabel = new Label
            {
                Text = course.LevelText,
                Font = ModernUIHelper.Fonts.Body,
                ForeColor = Color.White,
                BackColor = GetLevelColor(course.Level),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 35),
                Size = new Size(90, 30),
                Padding = new Padding(6)
            };

            // Course Type Badge (bigger size)
            var typeLabel = new Label
            {
                Text = course.TypeText,
                Font = ModernUIHelper.Fonts.Body,
                ForeColor = ModernUIHelper.Colors.TextSecondary,
                BackColor = ModernUIHelper.Colors.SurfaceVariant,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(100, 35),
                Size = new Size(100, 30),
                Padding = new Padding(6)
            };

            // Course Description
            var descriptionLabel = ModernUIHelper.CreateBodyText(
                course.Description.Length > 120 ?
                course.Description.Substring(0, 120) + "..." :
                course.Description, true);
            descriptionLabel.Location = new Point(0, 70);
            descriptionLabel.Width = card.Width - 40;
            descriptionLabel.Height = 80;

            // Duration
            var durationLabel = ModernUIHelper.CreateBodyText($"⏱️ {course.EstimatedDurationMinutes} minutes", true);
            durationLabel.Location = new Point(0, 160);

            // Prerequisites
            if (!string.IsNullOrEmpty(course.Prerequisites))
            {
                var prereqLabel = ModernUIHelper.CreateBodyText($"📋 {course.Prerequisites}", true);
                prereqLabel.Location = new Point(0, 180);
                prereqLabel.Width = card.Width - 40;
                prereqLabel.Height = 40;
                card.Controls.Add(prereqLabel);
            }

            // FREE Badge (bigger size)
            var freeText = isEnglish ? "FREE" : "ÜCRETSİZ";
            var freeLabel = new Label
            {
                Text = freeText,
                Font = ModernUIHelper.Fonts.BodyBold,
                ForeColor = Color.White,
                BackColor = ModernUIHelper.Colors.Success,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 230),
                Size = new Size(isEnglish ? 80 : 100, 35),
                Padding = new Padding(6)
            };

            // Start Button - Basit button oluştur
            var buttonText = isEnglish ? "Start Learning" : "Öğrenmeye Başla";
            var startButton = new Button
            {
                Text = buttonText,
                BackColor = Color.Blue,
                ForeColor = Color.White,
                Location = new Point(0, card.Height - 60),
                Size = new Size(card.Width - 40, 40),
                Tag = course,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            startButton.FlatAppearance.BorderSize = 0;
            startButton.Click += StartCourseButton_Click;

            card.Controls.AddRange(new Control[]
            {
                titleLabel, levelLabel, typeLabel, descriptionLabel, durationLabel,
                freeLabel, startButton
            });

            return card;
        }

        private Color GetLevelColor(CourseLevel level)
        {
            return level switch
            {
                CourseLevel.Beginner => ModernUIHelper.Colors.Success,
                CourseLevel.Intermediate => ModernUIHelper.Colors.Warning,
                CourseLevel.Advanced => ModernUIHelper.Colors.Error,
                _ => ModernUIHelper.Colors.TextMuted
            };
        }

        private void StartCourseButton_Click(object? sender, EventArgs e)
        {
            var button = sender as Button;
            var course = button?.Tag as Course;

            if (course == null)
            {
                MessageBox.Show("Course not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Basit kurs bilgilerini göster
            var title = isEnglish ? "Course Content" : "Kurs İçeriği";
            var message = isEnglish ?
                $"📚 Course: {course.Title}\n\n" +
                $"📊 Level: {course.LevelText}\n" +
                $"🎯 Type: {course.TypeText}\n" +
                $"⏱️ Duration: {course.EstimatedDurationMinutes} minutes\n" +
                $"💰 Price: {course.PriceText}\n\n" +
                $"📝 Description:\n{course.Description}\n\n" +
                $"✅ This course is now available for learning!\n\n" +
                $"🚀 Click OK to start your learning journey!" :
                $"📚 Kurs: {course.Title}\n\n" +
                $"📊 Seviye: {GetTurkishLevel(course.Level)}\n" +
                $"🎯 Tür: {GetTurkishCourseType(course.Type)}\n" +
                $"⏱️ Süre: {course.EstimatedDurationMinutes} dakika\n" +
                $"💰 Fiyat: {(course.Price == 0 ? "ÜCRETSİZ" : course.Price + " TL")}\n\n" +
                $"📝 Açıklama:\n{course.Description}\n\n" +
                $"✅ Bu kurs artık öğrenime hazır!\n\n" +
                $"🚀 Öğrenme yolculuğunuza başlamak için Tamam'a tıklayın!";

            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string GetTurkishLevel(CourseLevel level)
        {
            return level switch
            {
                CourseLevel.Beginner => "Başlangıç",
                CourseLevel.Intermediate => "Orta",
                CourseLevel.Advanced => "İleri",
                _ => level.ToString()
            };
        }

        private string GetTurkishCourseType(CourseType type)
        {
            return type switch
            {
                CourseType.Grammar => "Gramer",
                CourseType.Vocabulary => "Kelime",
                CourseType.Speaking => "Konuşma",
                CourseType.Listening => "Dinleme",
                CourseType.Reading => "Okuma",
                CourseType.Writing => "Yazma",
                CourseType.Pronunciation => "Telaffuz",
                _ => type.ToString()
            };
        }

        private void UpdateLanguage()
        {
            if (isEnglish)
            {
                // English
                var titleLabel = headerPanel.Controls.OfType<Label>().FirstOrDefault();
                if (titleLabel != null) titleLabel.Text = "📚 English Learning Courses";

                searchTextBox.PlaceholderText = "Search courses...";

                // Update level filter
                if (levelFilterComboBox.Items.Count > 0)
                {
                    var selectedLevel = levelFilterComboBox.SelectedIndex;
                    levelFilterComboBox.Items.Clear();
                    levelFilterComboBox.Items.AddRange(new[] { "All Levels", "Beginner", "Intermediate", "Advanced" });
                    levelFilterComboBox.SelectedIndex = selectedLevel >= 0 ? selectedLevel : 0;
                }

                // Update type filter
                if (typeFilterComboBox.Items.Count > 0)
                {
                    var selectedType = typeFilterComboBox.SelectedIndex;
                    typeFilterComboBox.Items.Clear();
                    typeFilterComboBox.Items.AddRange(new[] { "All Types", "Grammar", "Vocabulary", "Speaking", "Writing", "Listening", "Pronunciation" });
                    typeFilterComboBox.SelectedIndex = selectedType >= 0 ? selectedType : 0;
                }
            }
            else
            {
                // Turkish
                var titleLabel = headerPanel.Controls.OfType<Label>().FirstOrDefault();
                if (titleLabel != null) titleLabel.Text = "📚 İngilizce Öğrenme Kursları";

                searchTextBox.PlaceholderText = "Kurs ara...";

                // Update level filter
                if (levelFilterComboBox.Items.Count > 0)
                {
                    var selectedLevel = levelFilterComboBox.SelectedIndex;
                    levelFilterComboBox.Items.Clear();
                    levelFilterComboBox.Items.AddRange(new[] { "Tüm Seviyeler", "Başlangıç", "Orta", "İleri" });
                    levelFilterComboBox.SelectedIndex = selectedLevel >= 0 ? selectedLevel : 0;
                }

                // Update type filter
                if (typeFilterComboBox.Items.Count > 0)
                {
                    var selectedType = typeFilterComboBox.SelectedIndex;
                    typeFilterComboBox.Items.Clear();
                    typeFilterComboBox.Items.AddRange(new[] { "Tüm Türler", "Gramer", "Kelime", "Konuşma", "Yazma", "Dinleme", "Telaffuz" });
                    typeFilterComboBox.SelectedIndex = selectedType >= 0 ? selectedType : 0;
                }
            }

            // Update stats label
            UpdateStatsLabel();

            // Refresh course display to update button texts
            DisplayCourses();
        }
    }
}