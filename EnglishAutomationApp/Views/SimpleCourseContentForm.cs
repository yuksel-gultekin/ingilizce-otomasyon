using System;
using System.Drawing;
using System.Windows.Forms;
using EnglishAutomationApp.Models;
using EnglishAutomationApp.Helpers;

namespace EnglishAutomationApp.Views
{
    public partial class SimpleCourseContentForm : Form
    {
        private readonly Course course;
        private readonly bool isEnglish;
        private Panel mainPanel = null!;
        private Label titleLabel = null!;
        private Label levelLabel = null!;
        private Label typeLabel = null!;
        private RichTextBox contentTextBox = null!;
        private Button startButton = null!;
        private Button closeButton = null!;

        public SimpleCourseContentForm(Course selectedCourse, bool english = true)
        {
            course = selectedCourse;
            isEnglish = english;
            InitializeComponent();
            LoadContent();
        }

        private void InitializeComponent()
        {
            // Form properties
            this.Text = isEnglish ? $"Course: {course.Title}" : $"Kurs: {course.Title}";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = ModernUIHelper.Colors.Background;
            this.Font = ModernUIHelper.Fonts.Body;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            CreateControls();
        }

        private void CreateControls()
        {
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30),
                BackColor = ModernUIHelper.Colors.Background
            };

            // Course Title
            titleLabel = new Label
            {
                Text = course.Title,
                Font = new Font(ModernUIHelper.Fonts.Body.FontFamily, 18, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.TextPrimary,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            // Level Badge
            levelLabel = new Label
            {
                Text = isEnglish ? $"Level: {course.LevelText}" : $"Seviye: {GetTurkishLevel(course.Level)}",
                Font = ModernUIHelper.Fonts.Body,
                ForeColor = Color.White,
                BackColor = GetLevelColor(course.Level),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 40),
                Size = new Size(120, 30),
                Padding = new Padding(8)
            };

            // Type Badge
            typeLabel = new Label
            {
                Text = isEnglish ? $"Type: {course.TypeText}" : $"Tür: {GetTurkishCourseType(course.Type)}",
                Font = ModernUIHelper.Fonts.Body,
                ForeColor = Color.White,
                BackColor = ModernUIHelper.Colors.Primary,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(130, 40),
                Size = new Size(120, 30),
                Padding = new Padding(8)
            };

            // Content TextBox
            contentTextBox = new RichTextBox
            {
                Location = new Point(0, 90),
                Size = new Size(740, 350),
                Font = ModernUIHelper.Fonts.Body,
                BackColor = Color.White,
                ForeColor = ModernUIHelper.Colors.TextPrimary,
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            // Start Button
            startButton = new Button
            {
                Text = isEnglish ? "Start Learning" : "Öğrenmeye Başla",
                Font = ModernUIHelper.Fonts.Body,
                BackColor = ModernUIHelper.Colors.Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(0, 460),
                Size = new Size(150, 40),
                Cursor = Cursors.Hand
            };
            startButton.FlatAppearance.BorderSize = 0;
            startButton.Click += StartButton_Click;

            // Close Button
            closeButton = new Button
            {
                Text = isEnglish ? "Close" : "Kapat",
                Font = ModernUIHelper.Fonts.Body,
                BackColor = ModernUIHelper.Colors.Secondary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(160, 460),
                Size = new Size(100, 40),
                Cursor = Cursors.Hand
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => this.Close();

            // Add controls to main panel
            mainPanel.Controls.AddRange(new Control[]
            {
                titleLabel, levelLabel, typeLabel, contentTextBox, startButton, closeButton
            });

            // Add main panel to form
            this.Controls.Add(mainPanel);
        }

        private void LoadContent()
        {
            var content = GenerateCourseContent();
            contentTextBox.Text = content;
        }

        private string GenerateCourseContent()
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

Course Content Overview:
This course will help you improve your English skills through structured lessons and practical exercises.

What you'll learn:
- Essential vocabulary and phrases related to {course.TypeText.ToLower()}
- Grammar fundamentals and rules
- Practical communication skills
- Real-world applications and examples
- Interactive exercises and practice sessions

Course Structure:
1. Introduction and basics
2. Core concepts and theory
3. Practical exercises
4. Advanced topics
5. Review and assessment

Learning Objectives:
By the end of this course, you will be able to:
- Understand key concepts in {course.TypeText.ToLower()}
- Apply learned skills in real situations
- Communicate more effectively in English
- Build confidence in your language abilities

Ready to start your learning journey? Click 'Start Learning' to begin with interactive lessons and exercises!

Note: This is a comprehensive course designed to take you from your current level to the next. Take your time and practice regularly for best results.";
            }
            else
            {
                return $@"{course.Title} Kursuna Hoş Geldiniz!

Kurs Açıklaması:
{course.Description}

Kurs Detayları:
• Seviye: {GetTurkishLevel(course.Level)}
• Tür: {GetTurkishCourseType(course.Type)}
• Süre: {course.EstimatedDurationMinutes} dakika
• Ön Koşullar: {(string.IsNullOrEmpty(course.Prerequisites) ? "Yok" : course.Prerequisites)}

Kurs İçeriği Özeti:
Bu kurs, yapılandırılmış dersler ve pratik alıştırmalar aracılığıyla İngilizce becerilerinizi geliştirmenize yardımcı olacaktır.

Öğrenecekleriniz:
- {GetTurkishCourseType(course.Type).ToLower()} ile ilgili temel kelime ve ifadeler
- Gramer temelleri ve kuralları
- Pratik iletişim becerileri
- Gerçek dünya uygulamaları ve örnekleri
- İnteraktif alıştırmalar ve pratik oturumları

Kurs Yapısı:
1. Giriş ve temel bilgiler
2. Temel kavramlar ve teori
3. Pratik alıştırmalar
4. İleri düzey konular
5. Gözden geçirme ve değerlendirme

Öğrenme Hedefleri:
Bu kursun sonunda şunları yapabileceksiniz:
- {GetTurkishCourseType(course.Type).ToLower()} alanındaki temel kavramları anlama
- Öğrenilen becerileri gerçek durumlarda uygulama
- İngilizce'de daha etkili iletişim kurma
- Dil becerilerinizde güven oluşturma

Öğrenme yolculuğunuza başlamaya hazır mısınız? İnteraktif dersler ve alıştırmalarla başlamak için 'Öğrenmeye Başla' butonuna tıklayın!

Not: Bu, sizi mevcut seviyenizden bir sonraki seviyeye taşımak için tasarlanmış kapsamlı bir kurstır. En iyi sonuçlar için zaman ayırın ve düzenli olarak pratik yapın.";
            }
        }

        private void StartButton_Click(object? sender, EventArgs e)
        {
            var message = isEnglish
                ? $"Course '{course.Title}' has been started!\n\nThis is a demo version. In the full version, you would access:\n\n• Interactive lessons\n• Quizzes and exercises\n• Progress tracking\n• Certificates\n• Audio pronunciation guides\n\nKeep practicing to improve your English skills!"
                : $"'{course.Title}' kursu başlatıldı!\n\nBu bir demo versiyonudur. Tam versiyonda şunlara erişebilirsiniz:\n\n• İnteraktif dersler\n• Quizler ve alıştırmalar\n• İlerleme takibi\n• Sertifikalar\n• Sesli telaffuz rehberleri\n\nİngilizce becerilerinizi geliştirmek için pratik yapmaya devam edin!";

            var title = isEnglish ? "Course Started!" : "Kurs Başlatıldı!";

            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private Color GetLevelColor(CourseLevel level)
        {
            return level switch
            {
                CourseLevel.Beginner => Color.FromArgb(76, 175, 80),    // Green
                CourseLevel.Intermediate => Color.FromArgb(255, 152, 0), // Orange
                CourseLevel.Advanced => Color.FromArgb(244, 67, 54),     // Red
                _ => Color.Gray
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
    }
}
