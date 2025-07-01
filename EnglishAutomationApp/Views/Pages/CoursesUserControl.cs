using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnglishAutomationApp.Data;
using EnglishAutomationApp.Models;
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

        public CoursesUserControl()
        {
            InitializeComponent();
            LoadCoursesAsync();
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

                filteredCourses = allCourses.ToList();
                UpdateStatsLabel();
                DisplayCourses();
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
            statsLabel.Text = $"Total: {totalCourses} courses | Showing: {displayedCourses} courses | All courses are FREE!";
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
                    course.LevelText == selectedLevel;

                // Type filter
                var matchesType = selectedType == "All Types" ||
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

            // Course Level Badge
            var levelLabel = new Label
            {
                Text = course.LevelText,
                Font = ModernUIHelper.Fonts.Small,
                ForeColor = Color.White,
                BackColor = GetLevelColor(course.Level),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 35),
                Size = new Size(70, 20),
                Padding = new Padding(4)
            };

            // Course Type Badge
            var typeLabel = new Label
            {
                Text = course.TypeText,
                Font = ModernUIHelper.Fonts.Small,
                ForeColor = ModernUIHelper.Colors.TextSecondary,
                BackColor = ModernUIHelper.Colors.SurfaceVariant,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(80, 35),
                Size = new Size(80, 20),
                Padding = new Padding(4)
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

            // FREE Badge
            var freeLabel = new Label
            {
                Text = "FREE",
                Font = ModernUIHelper.Fonts.BodyBold,
                ForeColor = Color.White,
                BackColor = ModernUIHelper.Colors.Success,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 230),
                Size = new Size(60, 25),
                Padding = new Padding(4)
            };

            // Start Button
            var startButton = ModernUIHelper.CreateLargeButton("Start Learning", ModernUIHelper.Colors.Primary);
            startButton.Location = new Point(0, card.Height - 60);
            startButton.Width = card.Width - 40;
            startButton.Tag = course;
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

            if (course != null)
            {
                // Show course content form
                var courseContentForm = new CourseContentForm(course);
                courseContentForm.ShowDialog();
            }
        }
    }
} 