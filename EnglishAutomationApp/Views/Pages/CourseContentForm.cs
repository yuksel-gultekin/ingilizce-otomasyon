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

        public CourseContentForm(Course selectedCourse)
        {
            course = selectedCourse;
            InitializeComponent();
            LoadCourseContent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = $"Course: {course.Title}";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.WindowState = FormWindowState.Maximized;
            ModernUIHelper.ApplyModernFormStyle(this);

            CreateHeaderPanel();
            CreateContentPanel();
            CreateNavigationPanel();

            // Add controls to form
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

            // Progress bar
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

            previousButton = ModernUIHelper.CreateModernButton("← Previous", ModernUIHelper.Colors.TextMuted);
            previousButton.Location = new Point(ModernUIHelper.Spacing.Large, ModernUIHelper.Spacing.Medium);
            previousButton.Width = 120;
            previousButton.Click += PreviousButton_Click;

            nextButton = ModernUIHelper.CreateModernButton("Next →", ModernUIHelper.Colors.Primary);
            nextButton.Location = new Point(200, ModernUIHelper.Spacing.Medium);
            nextButton.Width = 120;
            nextButton.Click += NextButton_Click;

            completeButton = ModernUIHelper.CreateModernButton("Complete Course", ModernUIHelper.Colors.Success);
            completeButton.Location = new Point(350, ModernUIHelper.Spacing.Medium);
            completeButton.Width = 150;
            completeButton.Visible = false;
            completeButton.Click += CompleteButton_Click;

            navigationPanel.Controls.AddRange(new Control[] 
            { 
                previousButton, nextButton, completeButton 
            });
        }

        private void LoadCourseContent()
        {
            // Generate sample lessons based on course type
            lessons = GenerateLessons();
            UpdateProgress();
            ShowCurrentLesson();
        }

        private string[] GenerateLessons()
        {
            return course.Type switch
            {
                CourseType.Grammar => new[]
                {
                    GenerateGrammarLesson1(),
                    GenerateGrammarLesson2(),
                    GenerateGrammarLesson3(),
                    "Congratulations! You have completed the Basic English Grammar course. You've learned about sentence structure, verb tenses, and common grammar rules."
                },
                CourseType.Vocabulary => new[]
                {
                    GenerateVocabularyLesson1(),
                    GenerateVocabularyLesson2(),
                    GenerateVocabularyLesson3(),
                    "Excellent work! You have completed the Daily Life Vocabulary course. You've learned essential words for everyday communication."
                },
                CourseType.Speaking => new[]
                {
                    GenerateSpeakingLesson1(),
                    GenerateSpeakingLesson2(),
                    GenerateSpeakingLesson3(),
                    "Outstanding! You have completed the English Speaking Practice course. Your pronunciation and speaking confidence have improved significantly."
                },
                _ => new[]
                {
                    "Welcome to this English course!",
                    "This is lesson 2 of your course.",
                    "This is lesson 3 of your course.",
                    "Congratulations! You have completed this course."
                }
            };
        }

        private string GenerateGrammarLesson1()
        {
            return @"LESSON 1: SENTENCE STRUCTURE

Welcome to Basic English Grammar!

In this lesson, we'll learn about the fundamental structure of English sentences.

BASIC SENTENCE STRUCTURE:
Subject + Verb + Object

Examples:
• I (subject) eat (verb) apples (object).
• She (subject) reads (verb) books (object).
• They (subject) play (verb) football (object).

TYPES OF SENTENCES:
1. Declarative: Makes a statement
   Example: The cat is sleeping.

2. Interrogative: Asks a question
   Example: Where is the cat?

3. Imperative: Gives a command
   Example: Close the door.

4. Exclamatory: Shows strong emotion
   Example: What a beautiful day!

PRACTICE:
Try to identify the subject, verb, and object in these sentences:
1. John drives a car.
2. Mary loves chocolate.
3. Students study English.

Remember: Every sentence needs at least a subject and a verb to be complete!";
        }

        private string GenerateGrammarLesson2()
        {
            return @"LESSON 2: VERB TENSES

Understanding when actions happen is crucial in English!

PRESENT TENSE:
• Simple Present: I work every day.
• Present Continuous: I am working now.
• Present Perfect: I have worked here for 5 years.

PAST TENSE:
• Simple Past: I worked yesterday.
• Past Continuous: I was working when you called.
• Past Perfect: I had worked before you arrived.

FUTURE TENSE:
• Simple Future: I will work tomorrow.
• Future Continuous: I will be working at 3 PM.
• Future Perfect: I will have worked 8 hours by evening.

EXAMPLES IN CONTEXT:
• Present: ""I eat breakfast every morning.""
• Past: ""I ate breakfast an hour ago.""
• Future: ""I will eat breakfast tomorrow.""

PRACTICE:
Change these sentences to different tenses:
1. She walks to school. (Past)
2. They played football. (Future)
3. We will study English. (Present)

Remember: The verb tense tells us WHEN the action happens!";
        }

        private string GenerateGrammarLesson3()
        {
            return @"LESSON 3: ARTICLES AND PREPOSITIONS

Let's learn about small but important words!

ARTICLES:
• A/An (indefinite articles)
  - Use 'a' before consonant sounds: a book, a car
  - Use 'an' before vowel sounds: an apple, an hour

• The (definite article)
  - Use when talking about specific things: the book on the table

COMMON PREPOSITIONS:
• Time: at, on, in
  - at 3 o'clock
  - on Monday
  - in January

• Place: at, on, in, under, over, between
  - at home
  - on the table
  - in the box

• Movement: to, from, into, out of
  - go to school
  - come from work

EXAMPLES:
• ""I have a cat. The cat is sleeping on the sofa.""
• ""She goes to work at 8 o'clock in the morning.""
• ""The book is between the lamp and the computer.""

PRACTICE:
Fill in the blanks:
1. I saw ___ elephant ___ the zoo.
2. She lives ___ London ___ England.
3. The meeting is ___ Monday ___ 2 PM.

These small words make a big difference in meaning!";
        }

        private string GenerateVocabularyLesson1()
        {
            return @"LESSON 1: FAMILY AND RELATIONSHIPS

Let's learn essential vocabulary for talking about family!

IMMEDIATE FAMILY:
• Father/Dad - baba
• Mother/Mom - anne
• Son - oğul
• Daughter - kız
• Brother - erkek kardeş
• Sister - kız kardeş
• Husband - koca/eş
• Wife - karı/eş

EXTENDED FAMILY:
• Grandfather - büyükbaba
• Grandmother - büyükanne
• Uncle - amca/dayı
• Aunt - teyze/hala
• Cousin - kuzen
• Nephew - erkek yeğen
• Niece - kız yeğen

USEFUL PHRASES:
• ""This is my family."" - Bu benim ailem.
• ""I have two brothers."" - İki erkek kardeşim var.
• ""My sister is older than me."" - Kız kardeşim benden büyük.
• ""We live with our grandparents."" - Büyükanne ve büyükbabamızla yaşıyoruz.

PRACTICE SENTENCES:
1. My father works in a bank.
2. Her daughter is a doctor.
3. Our grandmother makes delicious cookies.
4. His uncle lives in America.

Try using these words to describe your own family!";
        }

        private string GenerateVocabularyLesson2()
        {
            return @"LESSON 2: DAILY ACTIVITIES

Learn vocabulary for everyday actions and routines!

MORNING ACTIVITIES:
• Wake up - uyanmak
• Get up - kalkmak
• Brush teeth - diş fırçalamak
• Take a shower - duş almak
• Get dressed - giyinmek
• Have breakfast - kahvaltı yapmak

WORK/SCHOOL ACTIVITIES:
• Go to work/school - işe/okula gitmek
• Study - çalışmak
• Have a meeting - toplantı yapmak
• Take notes - not almak
• Have lunch - öğle yemeği yemek

EVENING ACTIVITIES:
• Come home - eve gelmek
• Cook dinner - akşam yemeği pişirmek
• Watch TV - televizyon izlemek
• Read a book - kitap okumak
• Go to bed - yatmaya gitmek

TIME EXPRESSIONS:
• In the morning - sabahleyin
• In the afternoon - öğleden sonra
• In the evening - akşamleyin
• At night - geceleyin

EXAMPLE DAILY ROUTINE:
""I wake up at 7 AM. I brush my teeth and take a shower. Then I have breakfast and go to work. I come home at 6 PM, cook dinner, and watch TV. I go to bed at 10 PM.""

Practice describing your daily routine using these words!";
        }

        private string GenerateVocabularyLesson3()
        {
            return @"LESSON 3: FOOD AND DRINKS

Essential vocabulary for meals and dining!

MEALS:
• Breakfast - kahvaltı
• Lunch - öğle yemeği
• Dinner - akşam yemeği
• Snack - atıştırmalık

COMMON FOODS:
• Bread - ekmek
• Rice - pirinç
• Pasta - makarna
• Chicken - tavuk
• Fish - balık
• Vegetables - sebzeler
• Fruits - meyveler
• Cheese - peynir
• Eggs - yumurta

DRINKS:
• Water - su
• Coffee - kahve
• Tea - çay
• Juice - meyve suyu
• Milk - süt
• Soda - gazoz

COOKING VERBS:
• Cook - pişirmek
• Bake - fırında pişirmek
• Fry - kızartmak
• Boil - kaynatmak
• Grill - ızgara yapmak

RESTAURANT PHRASES:
• ""I'd like to order..."" - Sipariş vermek istiyorum...
• ""Can I have the menu?"" - Menüyü alabilir miyim?
• ""The bill, please."" - Hesap, lütfen.
• ""This is delicious!"" - Bu çok lezzetli!

PRACTICE:
1. What do you usually have for breakfast?
2. What's your favorite food?
3. Do you like to cook?

Food vocabulary is essential for daily communication!";
        }

        private string GenerateSpeakingLesson1()
        {
            return @"LESSON 1: PRONUNCIATION BASICS

Let's improve your English pronunciation!

VOWEL SOUNDS:
English has many vowel sounds. Practice these:

• /i:/ as in 'see' - uzun i sesi
• /ɪ/ as in 'sit' - kısa i sesi
• /e/ as in 'bed' - e sesi
• /æ/ as in 'cat' - geniş a sesi
• /ɑ:/ as in 'car' - uzun a sesi

CONSONANT SOUNDS:
Pay attention to these challenging sounds:

• /θ/ as in 'think' - th sesi (dişsiz)
• /ð/ as in 'this' - th sesi (dişli)
• /ʃ/ as in 'ship' - ş sesi
• /ʒ/ as in 'measure' - j sesi
• /tʃ/ as in 'chair' - ç sesi

WORD STRESS:
English words have stressed syllables:
• PHO-to (first syllable stressed)
• com-PU-ter (second syllable stressed)
• in-for-MA-tion (third syllable stressed)

PRACTICE WORDS:
1. Three /θri:/ - Üç
2. Brother /ˈbrʌðər/ - Erkek kardeş
3. School /sku:l/ - Okul
4. Beautiful /ˈbju:tɪfəl/ - Güzel

TIPS:
• Listen to native speakers
• Record yourself speaking
• Practice tongue twisters
• Don't be afraid to make mistakes!

Remember: Good pronunciation helps people understand you better!";
        }

        private string GenerateSpeakingLesson2()
        {
            return @"LESSON 2: CONVERSATION STARTERS

Learn how to start and maintain conversations!

GREETINGS:
• Formal: ""Good morning/afternoon/evening""
• Informal: ""Hi/Hello/Hey""
• ""How are you?"" - Nasılsın?
• ""How's it going?"" - Nasıl gidiyor?
• ""What's up?"" - Ne haber?

INTRODUCING YOURSELF:
• ""My name is..."" - Benim adım...
• ""I'm from..."" - Ben ... 'liyim
• ""I work as a..."" - ... olarak çalışıyorum
• ""Nice to meet you!"" - Tanıştığımıza memnun oldum!

SMALL TALK TOPICS:
• Weather: ""Nice weather today, isn't it?""
• Work: ""What do you do for work?""
• Hobbies: ""What do you like to do in your free time?""
• Travel: ""Have you been to...?""

ASKING QUESTIONS:
• ""Where are you from?""
• ""What brings you here?""
• ""Do you come here often?""
• ""Have you tried...?""

KEEPING CONVERSATION GOING:
• ""That's interesting!""
• ""Tell me more about that.""
• ""Really? What happened next?""
• ""I see what you mean.""

ENDING CONVERSATIONS:
• ""It was nice talking to you.""
• ""I should get going.""
• ""See you later!""
• ""Take care!""

Practice these phrases with friends or in front of a mirror!";
        }

        private string GenerateSpeakingLesson3()
        {
            return @"LESSON 3: EXPRESSING OPINIONS AND FEELINGS

Learn to share your thoughts and emotions in English!

EXPRESSING OPINIONS:
• ""I think..."" - Bence...
• ""In my opinion..."" - Benim görüşüme göre...
• ""I believe..."" - İnanıyorum ki...
• ""It seems to me..."" - Bana öyle geliyor ki...
• ""From my point of view..."" - Benim açımdan...

AGREEING:
• ""I agree."" - Katılıyorum.
• ""That's right."" - Doğru.
• ""Exactly!"" - Kesinlikle!
• ""I couldn't agree more."" - Daha fazla katılamazdım.

DISAGREEING POLITELY:
• ""I'm not sure about that."" - Bundan emin değilim.
• ""I see your point, but..."" - Demek istediğini anlıyorum ama...
• ""I have a different opinion."" - Farklı bir görüşüm var.
• ""I respectfully disagree."" - Saygıyla katılmıyorum.

EXPRESSING FEELINGS:
• Happy: ""I'm delighted/thrilled/overjoyed""
• Sad: ""I'm disappointed/upset/heartbroken""
• Angry: ""I'm frustrated/annoyed/furious""
• Surprised: ""I'm amazed/shocked/astonished""
• Worried: ""I'm concerned/anxious/nervous""

ASKING FOR OPINIONS:
• ""What do you think about...?""
• ""How do you feel about...?""
• ""What's your take on...?""
• ""Do you have any thoughts on...?""

PRACTICE SCENARIOS:
1. Discuss your favorite movie
2. Talk about a recent news event
3. Share your opinion about social media
4. Describe how you feel about learning English

Remember: Everyone's opinion matters, and it's okay to disagree respectfully!";
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

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            if (currentLessonIndex > 0)
            {
                currentLessonIndex--;
                UpdateProgress();
                ShowCurrentLesson();
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (currentLessonIndex < lessons.Length - 1)
            {
                currentLessonIndex++;
                UpdateProgress();
                ShowCurrentLesson();
            }
        }

        private void CompleteButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                $"Congratulations! You have completed '{course.Title}'!\n\n" +
                "Would you like to see your certificate?",
                "Course Completed!",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                ShowCertificate();
            }

            this.Close();
        }

        private void ShowCertificate()
        {
            var certificateForm = new Form
            {
                Text = "Certificate of Completion",
                Size = new Size(600, 400),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White
            };

            var certificateText = $@"
🎓 CERTIFICATE OF COMPLETION 🎓

This certifies that you have successfully completed:

{course.Title}

Course Type: {course.TypeText}
Level: {course.LevelText}
Date: {DateTime.Now:MMMM dd, yyyy}

Congratulations on your achievement!
Keep up the great work in your English learning journey!

English Learning Platform";

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
    }
}
