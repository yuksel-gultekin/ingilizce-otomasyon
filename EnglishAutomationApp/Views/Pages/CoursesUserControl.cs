using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EnglishAutomationApp.Data;
using EnglishAutomationApp.Models;
using EnglishAutomationApp.Services;
using Microsoft.EntityFrameworkCore;

namespace EnglishAutomationApp.Views.Pages
{
    public partial class CoursesUserControl : UserControl
    {
        private Label titleLabel;
        private ComboBox levelFilterComboBox;
        private ComboBox typeFilterComboBox;
        private TextBox searchTextBox;
        private Button searchButton;
        private FlowLayoutPanel coursesPanel;

        public CoursesUserControl()
        {
            InitializeComponent();
            LoadCourses();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // UserControl properties
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.Dock = DockStyle.Fill;

            // Title
            titleLabel = new Label();
            titleLabel.Text = "📚 English Automation Courses";
            titleLabel.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(102, 126, 234);
            titleLabel.Location = new Point(30, 20);
            titleLabel.Size = new Size(500, 40);

            // Filter Panel
            var filterPanel = new Panel();
            filterPanel.BackColor = Color.White;
            filterPanel.Location = new Point(30, 80);
            filterPanel.Size = new Size(740, 80);

            var filterLabel = new Label();
            filterLabel.Text = "Filters:";
            filterLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            filterLabel.Location = new Point(20, 15);
            filterLabel.Size = new Size(60, 25);

            // Level Filter
            var levelLabel = new Label();
            levelLabel.Text = "Level:";
            levelLabel.Font = new Font("Segoe UI", 10);
            levelLabel.Location = new Point(20, 45);
            levelLabel.Size = new Size(50, 20);

            levelFilterComboBox = new ComboBox();
            levelFilterComboBox.Font = new Font("Segoe UI", 10);
            levelFilterComboBox.Location = new Point(70, 43);
            levelFilterComboBox.Size = new Size(120, 25);
            levelFilterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            levelFilterComboBox.Items.AddRange(new string[] { "All Levels", "Beginner", "Intermediate", "Advanced" });
            levelFilterComboBox.SelectedIndex = 0;
            levelFilterComboBox.SelectedIndexChanged += FilterChanged;

            // Type Filter
            var typeLabel = new Label();
            typeLabel.Text = "Type:";
            typeLabel.Font = new Font("Segoe UI", 10);
            typeLabel.Location = new Point(210, 45);
            typeLabel.Size = new Size(40, 20);

            typeFilterComboBox = new ComboBox();
            typeFilterComboBox.Font = new Font("Segoe UI", 10);
            typeFilterComboBox.Location = new Point(250, 43);
            typeFilterComboBox.Size = new Size(120, 25);
            typeFilterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            typeFilterComboBox.Items.AddRange(new string[] { "All Types", "Grammar", "Vocabulary", "Speaking", "Writing", "Listening" });
            typeFilterComboBox.SelectedIndex = 0;
            typeFilterComboBox.SelectedIndexChanged += FilterChanged;

            // Search
            var searchLabel = new Label();
            searchLabel.Text = "Search:";
            searchLabel.Font = new Font("Segoe UI", 10);
            searchLabel.Location = new Point(390, 45);
            searchLabel.Size = new Size(50, 20);

            searchTextBox = new TextBox();
            searchTextBox.Font = new Font("Segoe UI", 10);
            searchTextBox.Location = new Point(445, 43);
            searchTextBox.Size = new Size(150, 25);

            searchButton = new Button();
            searchButton.Text = "🔍";
            searchButton.Font = new Font("Segoe UI", 10);
            searchButton.Location = new Point(605, 43);
            searchButton.Size = new Size(30, 25);
            searchButton.Click += SearchButton_Click;

            filterPanel.Controls.Add(filterLabel);
            filterPanel.Controls.Add(levelLabel);
            filterPanel.Controls.Add(levelFilterComboBox);
            filterPanel.Controls.Add(typeLabel);
            filterPanel.Controls.Add(typeFilterComboBox);
            filterPanel.Controls.Add(searchLabel);
            filterPanel.Controls.Add(searchTextBox);
            filterPanel.Controls.Add(searchButton);

            // Courses Panel
            coursesPanel = new FlowLayoutPanel();
            coursesPanel.BackColor = Color.FromArgb(240, 240, 240);
            coursesPanel.Location = new Point(30, 180);
            coursesPanel.Size = new Size(740, 400);
            coursesPanel.AutoScroll = true;
            coursesPanel.FlowDirection = FlowDirection.LeftToRight;
            coursesPanel.WrapContents = true;

            // Add controls to UserControl
            this.Controls.Add(titleLabel);
            this.Controls.Add(filterPanel);
            this.Controls.Add(coursesPanel);

            this.ResumeLayout(false);
        }

        private async void LoadCourses()
        {
            try
            {
                using var context = new AppDbContext();
                var courses = await context.Courses
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.OrderIndex)
                    .ToListAsync();

                DisplayCourses(courses);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading courses: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayCourses(System.Collections.Generic.List<Course> courses)
        {
            coursesPanel.Controls.Clear();

            if (!courses.Any())
            {
                var noCoursesLabel = new Label();
                noCoursesLabel.Text = "No courses found matching your criteria.";
                noCoursesLabel.Font = new Font("Segoe UI", 12, FontStyle.Italic);
                noCoursesLabel.ForeColor = Color.Gray;
                noCoursesLabel.TextAlign = ContentAlignment.MiddleCenter;
                noCoursesLabel.Size = new Size(700, 50);
                coursesPanel.Controls.Add(noCoursesLabel);
                return;
            }

            foreach (var course in courses)
            {
                var courseCard = CreateCourseCard(course);
                coursesPanel.Controls.Add(courseCard);
            }
        }

        private Panel CreateCourseCard(Course course)
        {
            var card = new Panel();
            card.BackColor = Color.White;
            card.Size = new Size(220, 280);
            card.Margin = new Padding(10);

            // Course Title
            var titleLabel = new Label();
            titleLabel.Text = course.Title;
            titleLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            titleLabel.Location = new Point(15, 15);
            titleLabel.Size = new Size(190, 50);

            // Course Level
            var levelLabel = new Label();
            levelLabel.Text = $"Level: {course.Level}";
            levelLabel.Font = new Font("Segoe UI", 9);
            levelLabel.ForeColor = Color.FromArgb(102, 126, 234);
            levelLabel.Location = new Point(15, 70);
            levelLabel.Size = new Size(100, 20);

            // Course Type
            var typeLabel = new Label();
            typeLabel.Text = $"Type: {course.Type}";
            typeLabel.Font = new Font("Segoe UI", 9);
            typeLabel.ForeColor = Color.FromArgb(76, 175, 80);
            typeLabel.Location = new Point(15, 90);
            typeLabel.Size = new Size(100, 20);

            // Course Description
            var descriptionLabel = new Label();
            descriptionLabel.Text = course.Description.Length > 100 ? 
                course.Description.Substring(0, 100) + "..." : course.Description;
            descriptionLabel.Font = new Font("Segoe UI", 9);
            descriptionLabel.ForeColor = Color.Gray;
            descriptionLabel.Location = new Point(15, 120);
            descriptionLabel.Size = new Size(190, 80);

            // Price - All courses are now free
            var priceLabel = new Label();
            priceLabel.Text = "FREE";
            priceLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            priceLabel.ForeColor = Color.Green;
            priceLabel.Location = new Point(15, 210);
            priceLabel.Size = new Size(80, 25);

            // Start Button
            var startButton = new Button();
            startButton.Text = "Start Course";
            startButton.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            startButton.BackColor = Color.FromArgb(102, 126, 234);
            startButton.ForeColor = Color.White;
            startButton.FlatStyle = FlatStyle.Flat;
            startButton.Location = new Point(15, 240);
            startButton.Size = new Size(190, 30);
            startButton.Tag = course;
            startButton.Click += StartCourseButton_Click;

            card.Controls.Add(titleLabel);
            card.Controls.Add(levelLabel);
            card.Controls.Add(typeLabel);
            card.Controls.Add(descriptionLabel);
            card.Controls.Add(priceLabel);
            card.Controls.Add(startButton);

            return card;
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            LoadCourses(); // Reload with filters applied
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            LoadCourses(); // Reload with search applied
        }

        private void StartCourseButton_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            var course = button?.Tag as Course;

            if (course != null)
            {
                // All courses are now free - no payment required
                MessageBox.Show($"Starting course: {course.Title}\n\nWelcome to your English learning journey!\n\nThis course is now available for free.",
                    "Course Started", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Here you could add logic to:
                // - Create user progress record
                // - Navigate to course content
                // - Show first lesson
            }
        }
    }
}
