using System;
using System.Drawing;
using System.Windows.Forms;
using EnglishAutomationApp.Helpers;
using EnglishAutomationApp.Models;

namespace EnglishAutomationApp.Views
{
    public class CourseContentForm : Form
    {
        private readonly Course course;
        private bool isEnglish = true;

        public CourseContentForm(Course course)
        {
            this.course = course;
            InitializeComponent();
            LoadCourseContent();
        }

        private void InitializeComponent()
        {
            this.Text = isEnglish ? "Course Content" : "Kurs İçeriği";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = ModernUIHelper.Colors.Background;
            this.Font = ModernUIHelper.Fonts.Body;

            CreateControls();
        }

        private void CreateControls()
        {
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30),
                BackColor = ModernUIHelper.Colors.Background
            };

            var titleLabel = new Label
            {
                Text = course.Title,
                Font = ModernUIHelper.Fonts.Title,
                ForeColor = ModernUIHelper.Colors.TextPrimary,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            var descriptionLabel = new Label
            {
                Text = course.Description,
                Font = ModernUIHelper.Fonts.Body,
                ForeColor = ModernUIHelper.Colors.TextSecondary,
                AutoSize = true,
                Location = new Point(0, 50),
                MaximumSize = new Size(800, 0)
            };

            var contentTextBox = new RichTextBox
            {
                Text = GenerateDefaultContent(),
                Font = ModernUIHelper.Fonts.Body,
                BackColor = Color.White,
                ForeColor = ModernUIHelper.Colors.TextPrimary,
                Location = new Point(0, 100),
                Size = new Size(800, 400),
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            var buttonPanel = new Panel
            {
                Location = new Point(0, 520),
                Size = new Size(800, 60),
                BackColor = ModernUIHelper.Colors.Background
            };

            var startButton = ModernUIHelper.CreateLargeButton(
                isEnglish ? "Start Course" : "Kursu Başlat", 
                ModernUIHelper.Colors.Primary
            );
            startButton.Location = new Point(0, 10);
            startButton.Size = new Size(150, 40);
            startButton.Click += StartButton_Click;

            var closeButton = ModernUIHelper.CreateLargeButton(
                isEnglish ? "Close" : "Kapat", 
                ModernUIHelper.Colors.Secondary
            );
            closeButton.Location = new Point(170, 10);
            closeButton.Size = new Size(100, 40);
            closeButton.Click += (s, e) => this.Close();

            buttonPanel.Controls.AddRange(new Control[] { startButton, closeButton });
            mainPanel.Controls.AddRange(new Control[] { titleLabel, descriptionLabel, contentTextBox, buttonPanel });
            this.Controls.Add(mainPanel);
        }

        private void LoadCourseContent()
        {
            if (string.IsNullOrEmpty(course.Content))
            {
                course.Content = GenerateDefaultContent();
            }
        }

        private string GenerateDefaultContent()
        {
            if (isEnglish)
            {
                return $@"Welcome to {course.Title}!

Course Description:
{course.Description}

Course Details:
• Level: {course.LevelText}
• Type: {course.TypeText}
• Duration: {course.EstimatedDurationMinutes} minutes
• Prerequisites: {(string.IsNullOrEmpty(course.Prerequisites) ? "None" : course.Prerequisites)}

Course Content:
This course will help you improve your English skills through structured lessons and practical exercises. 

What you'll learn:
- Essential vocabulary and phrases
- Grammar fundamentals
- Practical communication skills
- Real-world applications

Ready to start your learning journey? Click 'Start Course' to begin!";
            }
            else
            {
                return $@"{course.Title} Kursuna Hoş Geldiniz!

Kurs Açıklaması:
{course.Description}

Kurs Detayları:
• Seviye: {course.LevelText}
• Tür: {course.TypeText}
• Süre: {course.EstimatedDurationMinutes} dakika
• Ön Koşullar: {(string.IsNullOrEmpty(course.Prerequisites) ? "Yok" : course.Prerequisites)}

Kurs İçeriği:
Bu kurs yapılandırılmış dersler ve pratik alıştırmalar aracılığıyla İngilizce becerilerinizi geliştirmenize yardımcı olacak.

Öğrenecekleriniz:
- Temel kelime ve ifadeler
- Gramer temelleri
- Pratik iletişim becerileri
- Gerçek dünya uygulamaları

Öğrenme yolculuğunuza başlamaya hazır mısınız? Başlamak için 'Kursu Başlat' butonuna tıklayın!";
            }
        }

        private void StartButton_Click(object? sender, EventArgs e)
        {
            var message = isEnglish 
                ? $"Course '{course.Title}' has been started!\n\nThis is a demo version. In the full version, you would access interactive lessons, quizzes, and progress tracking."
                : $"'{course.Title}' kursu başlatıldı!\n\nBu bir demo versiyonudur. Tam versiyonda interaktif dersler, quizler ve ilerleme takibi erişilebilir olacaktır.";
                
            MessageBox.Show(message, 
                isEnglish ? "Course Started" : "Kurs Başlatıldı", 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Information);
        }

        public void SetLanguage(bool english)
        {
            isEnglish = english;
            this.Text = isEnglish ? "Course Content" : "Kurs İçeriği";

            this.Controls.Clear();
            CreateControls();
            LoadCourseContent();
        }
    }
}
