using System;
using System.Drawing;
using System.Windows.Forms;
using EnglishAutomationApp.Helpers;
using EnglishAutomationApp.Models;

namespace EnglishAutomationApp.Views.Pages
{
    public partial class CourseContentForm : Form
    {
        private Course course;
        private bool isEnglish;

        private Panel headerPanel;
        private Panel contentPanel;
        private Panel navigationPanel;
        private RichTextBox contentTextBox;
        private Button previousButton;
        private Button nextButton;
        private Button completeButton;
        private Label progressLabel;
        private ProgressBar progressBar;

        private int currentLessonIndex = 0;
        private string[] lessons;

        public CourseContentForm(Course selectedCourse, bool english = true)
        {
            // PARAMETRELERİ İLK BAŞTA ATA
            course = selectedCourse;
            isEnglish = english;

            // FORM TASARIMI OLUŞTUR
            InitializeComponent();

            // MODERN TASARIM STİLİNİ UYGULA
            ModernUIHelper.ApplyModernFormStyle(this);

            // DİNAMİK BAŞLIK AYARLA
            this.Text = isEnglish ? $"Course: {course.Title}" : $"Kurs: {course.Title}";

            // KURS İÇERİĞİNİ YÜKLE
            LoadCourseContent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.WindowState = FormWindowState.Maximized;

            CreateHeaderPanel();
            CreateContentPanel();
            CreateNavigationPanel();

            this.Controls.Add(navigationPanel);
            this.Controls.Add(contentPanel);
            this.Controls.Add(headerPanel);

            this.ResumeLayout(false);
        }

        private void CreateHeaderPanel()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = ModernUIHelper.Colors.Primary,
                Padding = new Padding(ModernUIHelper.Spacing.Large)
            };

            var titleLabel = ModernUIHelper.CreateHeading(course.Title, 2);
            titleLabel.ForeColor = Color.White;
            titleLabel.Location = new Point(ModernUIHelper.Spacing.Large, ModernUIHelper.Spacing.Medium);

            var descriptionLabel = ModernUIHelper.CreateBodyText(course.Description);
            descriptionLabel.ForeColor = Color.White;
            descriptionLabel.Location = new Point(ModernUIHelper.Spacing.Large, 50);
            descriptionLabel.Width = 600;

            progressBar = new ProgressBar
            {
                Location = new Point(ModernUIHelper.Spacing.Large, 75),
                Width = 300,
                Height = 10,
                Style = ProgressBarStyle.Continuous,
                ForeColor = ModernUIHelper.Colors.Success
            };

            progressLabel = new Label
            {
                Font = ModernUIHelper.Fonts.Small,
                ForeColor = Color.White,
                Location = new Point(350, 73),
                AutoSize = true
            };

            headerPanel.Controls.AddRange(new Control[]
            {
                titleLabel, descriptionLabel, progressBar, progressLabel
            });
        }

        private void CreateContentPanel()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ModernUIHelper.Colors.Background,
                Padding = new Padding(ModernUIHelper.Spacing.Large)
            };

            var contentCard = ModernUIHelper.CreateCard(ModernUIHelper.Spacing.Large);
            contentCard.Dock = DockStyle.Fill;

            contentTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = ModernUIHelper.Fonts.Body,
                BackColor = ModernUIHelper.Colors.Surface,
                ForeColor = ModernUIHelper.Colors.TextPrimary,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                Padding = new Padding(ModernUIHelper.Spacing.Medium)
            };

            contentCard.Controls.Add(contentTextBox);
            contentPanel.Controls.Add(contentCard);
        }

        private void CreateNavigationPanel()
        {
            navigationPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = ModernUIHelper.Colors.SurfaceVariant,
                Padding = new Padding(ModernUIHelper.Spacing.Large)
            };

            previousButton = ModernUIHelper.CreateIconButton(isEnglish ? "Previous" : "Önceki", "←", ModernUIHelper.Colors.TextMuted, 140);
            previousButton.Location = new Point(ModernUIHelper.Spacing.Large, ModernUIHelper.Spacing.Medium);
            previousButton.Click += PreviousButton_Click;

            nextButton = ModernUIHelper.CreateIconButton(isEnglish ? "Next" : "Sonraki", "→", ModernUIHelper.Colors.Primary, 140);
            nextButton.Location = new Point(200, ModernUIHelper.Spacing.Medium);
            nextButton.Click += NextButton_Click;

            completeButton = ModernUIHelper.CreateIconButton(isEnglish ? "Complete Course" : "Kursu Tamamla", "🎓", ModernUIHelper.Colors.Success, 180);
            completeButton.Location = new Point(350, ModernUIHelper.Spacing.Medium);
            completeButton.Visible = false;
            completeButton.Click += CompleteButton_Click;

            navigationPanel.Controls.AddRange(new Control[]
            {
                previousButton, nextButton, completeButton
            });
        }

        private void LoadCourseContent()
        {
            lessons = GenerateLessons();
            UpdateProgress();
            ShowCurrentLesson();
        }

        private void UpdateProgress()
        {
            progressBar.Maximum = lessons.Length;
            progressBar.Value = currentLessonIndex + 1;
            progressLabel.Text = $"Lesson {currentLessonIndex + 1} of {lessons.Length}";
        }

        private void ShowCurrentLesson()
        {
            if (currentLessonIndex < lessons.Length)
            {
                contentTextBox.Text = lessons[currentLessonIndex];
                previousButton.Enabled = currentLessonIndex > 0;
                nextButton.Visible = currentLessonIndex < lessons.Length - 1;
                completeButton.Visible = currentLessonIndex == lessons.Length - 1;
            }
        }

        private void PreviousButton_Click(object? sender, EventArgs e)
        {
            if (currentLessonIndex > 0)
            {
                currentLessonIndex--;
                UpdateProgress();
                ShowCurrentLesson();
            }
        }

        private void NextButton_Click(object? sender, EventArgs e)
        {
            if (currentLessonIndex < lessons.Length - 1)
            {
                currentLessonIndex++;
                UpdateProgress();
                ShowCurrentLesson();
            }
        }

        private void CompleteButton_Click(object? sender, EventArgs e)
        {
            var message = isEnglish
                ? $"Congratulations! You have completed '{course.Title}'!\n\nWould you like to see your certificate?"
                : $"Tebrikler! '{course.Title}' kursunu tamamladınız!\n\nSertifikanızı görmek ister misiniz?";

            var title = isEnglish ? "Course Completed!" : "Kurs Tamamlandı!";

            var result = MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
                ShowCertificate();

            this.Close();
        }

        private void ShowCertificate()
        {
            var certificateTitle = isEnglish ? "Certificate of Completion" : "Tamamlama Sertifikası";
            var certificateForm = new Form
            {
                Text = certificateTitle,
                Size = new Size(600, 400),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White
            };

            var certificateText = isEnglish ? $@"
🎓 CERTIFICATE OF COMPLETION 🎓

This certifies that you have successfully completed:

{course.Title}

Course Type: {course.TypeText}
Level: {course.LevelText}
Date: {DateTime.Now:MMMM dd, yyyy}

Congratulations on your achievement!
Keep up the great work in your English learning journey!

English Learning Platform"
                : $@"
🎓 TAMAMLAMA SERTİFİKASI 🎓

Bu sertifika aşağıdaki kursu başarıyla tamamladığınızı onaylar:

{course.Title}

Kurs Türü: {GetTurkishCourseType(course.Type)}
Seviye: {GetTurkishLevel(course.Level)}
Tarih: {DateTime.Now:dd MMMM yyyy}

Başarınız için tebrikler!
İngilizce öğrenme yolculuğunuzda harika işler çıkarıyorsunuz!

İngilizce Öğrenme Platformu";

            var label = new Label
            {
                Text = certificateText,
                Font = new Font("Segoe UI", 12),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = ModernUIHelper.Colors.TextPrimary
            };

            certificateForm.Controls.Add(label);
            certificateForm.ShowDialog();
        }

        private string GetTurkishCourseType(CourseType type)
        {
            return type switch
            {
                CourseType.Grammar => "Gramer",
                CourseType.Vocabulary => "Kelime",
                CourseType.Speaking => "Konuşma",
                _ => type.ToString()
            };
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

        // Senin GenerateLessons metodun buraya aynen gelecek.
    }
}
