using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnglishAutomationApp.Data;
using EnglishAutomationApp.Models;

namespace EnglishAutomationApp.Forms
{
    public partial class AdminPanelForm : Form
    {
        private User _currentUser;
        private TabControl tabControl = null!;
        private TabPage studentsTab = null!;
        private TabPage coursesTab = null!;
        private TabPage vocabularyTab = null!;
        private TabPage reportsTab = null!;

        // Students Tab Controls
        private DataGridView studentsGrid = null!;
        private Button addStudentBtn = null!;
        private Button editStudentBtn = null!;
        private Button deleteStudentBtn = null!;

        // Courses Tab Controls
        private DataGridView coursesGrid = null!;
        private Button addCourseBtn = null!;
        private Button editCourseBtn = null!;
        private Button deleteCourseBtn = null!;

        // Vocabulary Tab Controls
        private DataGridView vocabularyGrid = null!;
        private Button addVocabularyBtn = null!;
        private Button editVocabularyBtn = null!;
        private Button deleteVocabularyBtn = null!;

        public AdminPanelForm(User currentUser)
        {
            _currentUser = currentUser;
            InitializeComponent();
            SetupModernDesign();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Admin Panel - English Automation";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.Font = new Font("Segoe UI", 10F);

            // Create main tab control
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                Padding = new Point(20, 8)
            };

            // Create tabs
            CreateStudentsTab();
            CreateCoursesTab();
            CreateVocabularyTab();
            CreateReportsTab();

            // Add tabs to control
            tabControl.TabPages.Add(studentsTab);
            tabControl.TabPages.Add(coursesTab);
            tabControl.TabPages.Add(vocabularyTab);
            tabControl.TabPages.Add(reportsTab);

            this.Controls.Add(tabControl);
            this.ResumeLayout(false);
        }

        private void CreateStudentsTab()
        {
            studentsTab = new TabPage("Öğrenci Yönetimi");
            studentsTab.BackColor = Color.FromArgb(240, 244, 248);
            studentsTab.Padding = new Padding(20);

            // Create panel for buttons
            var buttonPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = Color.Transparent
            };

            addStudentBtn = CreateModernButton("Yeni Öğrenci", Color.FromArgb(34, 197, 94));
            editStudentBtn = CreateModernButton("Düzenle", Color.FromArgb(59, 130, 246));
            deleteStudentBtn = CreateModernButton("Sil", Color.FromArgb(239, 68, 68));

            addStudentBtn.Location = new Point(0, 10);
            editStudentBtn.Location = new Point(140, 10);
            deleteStudentBtn.Location = new Point(280, 10);

            addStudentBtn.Click += AddStudentBtn_Click;
            editStudentBtn.Click += EditStudentBtn_Click;
            deleteStudentBtn.Click += DeleteStudentBtn_Click;

            buttonPanel.Controls.AddRange(new Control[] { addStudentBtn, editStudentBtn, deleteStudentBtn });

            // Create DataGridView for students
            studentsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(248, 250, 252),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    SelectionBackColor = Color.FromArgb(248, 250, 252)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(51, 65, 85),
                    SelectionBackColor = Color.FromArgb(219, 234, 254),
                    SelectionForeColor = Color.FromArgb(30, 64, 175)
                },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false
            };

            studentsTab.Controls.Add(studentsGrid);
            studentsTab.Controls.Add(buttonPanel);
        }

        private void CreateCoursesTab()
        {
            coursesTab = new TabPage("Kurs Yönetimi");
            coursesTab.BackColor = Color.FromArgb(240, 244, 248);
            coursesTab.Padding = new Padding(20);

            // Create panel for buttons
            var buttonPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = Color.Transparent
            };

            addCourseBtn = CreateModernButton("Yeni Kurs", Color.FromArgb(34, 197, 94));
            editCourseBtn = CreateModernButton("Düzenle", Color.FromArgb(59, 130, 246));
            deleteCourseBtn = CreateModernButton("Sil", Color.FromArgb(239, 68, 68));

            addCourseBtn.Location = new Point(0, 10);
            editCourseBtn.Location = new Point(140, 10);
            deleteCourseBtn.Location = new Point(280, 10);

            addCourseBtn.Click += AddCourseBtn_Click;
            editCourseBtn.Click += EditCourseBtn_Click;
            deleteCourseBtn.Click += DeleteCourseBtn_Click;

            buttonPanel.Controls.AddRange(new Control[] { addCourseBtn, editCourseBtn, deleteCourseBtn });

            // Create DataGridView for courses
            coursesGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(248, 250, 252),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    SelectionBackColor = Color.FromArgb(248, 250, 252)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(51, 65, 85),
                    SelectionBackColor = Color.FromArgb(219, 234, 254),
                    SelectionForeColor = Color.FromArgb(30, 64, 175)
                },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false
            };

            coursesTab.Controls.Add(coursesGrid);
            coursesTab.Controls.Add(buttonPanel);
        }

        private void CreateVocabularyTab()
        {
            vocabularyTab = new TabPage("Kelime Yönetimi");
            vocabularyTab.BackColor = Color.FromArgb(240, 244, 248);
            vocabularyTab.Padding = new Padding(20);

            // Create panel for buttons
            var buttonPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = Color.Transparent
            };

            addVocabularyBtn = CreateModernButton("Yeni Kelime", Color.FromArgb(34, 197, 94));
            editVocabularyBtn = CreateModernButton("Düzenle", Color.FromArgb(59, 130, 246));
            deleteVocabularyBtn = CreateModernButton("Sil", Color.FromArgb(239, 68, 68));

            addVocabularyBtn.Location = new Point(0, 10);
            editVocabularyBtn.Location = new Point(140, 10);
            deleteVocabularyBtn.Location = new Point(280, 10);

            addVocabularyBtn.Click += AddVocabularyBtn_Click;
            editVocabularyBtn.Click += EditVocabularyBtn_Click;
            deleteVocabularyBtn.Click += DeleteVocabularyBtn_Click;

            buttonPanel.Controls.AddRange(new Control[] { addVocabularyBtn, editVocabularyBtn, deleteVocabularyBtn });

            // Create DataGridView for vocabulary
            vocabularyGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(248, 250, 252),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    SelectionBackColor = Color.FromArgb(248, 250, 252)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(51, 65, 85),
                    SelectionBackColor = Color.FromArgb(219, 234, 254),
                    SelectionForeColor = Color.FromArgb(30, 64, 175)
                },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false
            };

            vocabularyTab.Controls.Add(vocabularyGrid);
            vocabularyTab.Controls.Add(buttonPanel);
        }

        private void CreateReportsTab()
        {
            reportsTab = new TabPage("Raporlar");
            reportsTab.BackColor = Color.FromArgb(240, 244, 248);
            reportsTab.Padding = new Padding(20);

            var label = new Label
            {
                Text = "Raporlar bölümü yakında eklenecek...",
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.FromArgb(71, 85, 105),
                AutoSize = true,
                Location = new Point(50, 50)
            };

            reportsTab.Controls.Add(label);
        }

        private Button CreateModernButton(string text, Color backgroundColor)
        {
            var button = new Button
            {
                Text = text,
                Size = new Size(120, 40),
                BackColor = backgroundColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backgroundColor, 0.1f);
            button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backgroundColor, 0.1f);

            return button;
        }

        private void SetupModernDesign()
        {
            // Apply modern styling to tab control
            tabControl.Appearance = TabAppearance.FlatButtons;
            tabControl.SizeMode = TabSizeMode.Fixed;
            tabControl.ItemSize = new Size(150, 40);
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // Load students data
                await LoadStudentsAsync();
                
                // Load courses data
                await LoadCoursesAsync();
                
                // Load vocabulary data
                await LoadVocabularyAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri yükleme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadStudentsAsync()
        {
            var students = await AccessDatabaseHelper.GetAllUsersAsync();

            studentsGrid?.Columns.Clear();
            studentsGrid?.Columns.Add("Id", "ID");
            studentsGrid?.Columns.Add("Email", "E-posta");
            studentsGrid?.Columns.Add("FirstName", "Ad");
            studentsGrid?.Columns.Add("LastName", "Soyad");
            studentsGrid?.Columns.Add("CreatedDate", "Kayıt Tarihi");
            studentsGrid?.Columns.Add("IsActive", "Aktif");

            if (studentsGrid?.Columns["Id"] != null)
                studentsGrid.Columns["Id"].Visible = false;
            if (studentsGrid?.Columns["CreatedDate"] != null)
                studentsGrid.Columns["CreatedDate"].DefaultCellStyle.Format = "dd.MM.yyyy";

            foreach (var student in students)
            {
                studentsGrid?.Rows.Add(
                    student.Id,
                    student.Email ?? "",
                    student.FirstName ?? "",
                    student.LastName ?? "",
                    student.CreatedDate,
                    student.IsActive ? "Evet" : "Hayır"
                );
            }
        }

        private async Task LoadCoursesAsync()
        {
            var courses = await AccessDatabaseHelper.GetAllCoursesAsync();
            
            coursesGrid?.Columns.Clear();
            coursesGrid?.Columns.Add("Id", "ID");
            coursesGrid?.Columns.Add("Title", "Başlık");
            coursesGrid?.Columns.Add("Description", "Açıklama");
            coursesGrid?.Columns.Add("Level", "Seviye");
            coursesGrid?.Columns.Add("Type", "Tür");
            coursesGrid?.Columns.Add("Price", "Fiyat");
            coursesGrid?.Columns.Add("IsActive", "Aktif");

            if (coursesGrid?.Columns["Id"] != null)
                coursesGrid.Columns["Id"].Visible = false;

            foreach (var course in courses)
            {
                coursesGrid?.Rows.Add(
                    course.Id,
                    course.Title ?? "",
                    course.Description ?? "",
                    course.LevelText,
                    course.TypeText,
                    course.PriceText,
                    course.IsActive ? "Evet" : "Hayır"
                );
            }
        }

        private async Task LoadVocabularyAsync()
        {
            var words = await AccessDatabaseHelper.GetAllVocabularyWordsAsync();
            
            vocabularyGrid?.Columns.Clear();
            vocabularyGrid?.Columns.Add("Id", "ID");
            vocabularyGrid?.Columns.Add("EnglishWord", "İngilizce");
            vocabularyGrid?.Columns.Add("TurkishMeaning", "Türkçe");
            vocabularyGrid?.Columns.Add("Pronunciation", "Telaffuz");
            vocabularyGrid?.Columns.Add("Difficulty", "Zorluk");
            vocabularyGrid?.Columns.Add("PartOfSpeech", "Kelime Türü");
            vocabularyGrid?.Columns.Add("Category", "Kategori");

            if (vocabularyGrid?.Columns["Id"] != null)
                vocabularyGrid.Columns["Id"].Visible = false;

            foreach (var word in words)
            {
                vocabularyGrid?.Rows.Add(
                    word.Id,
                    word.EnglishWord ?? "",
                    word.TurkishMeaning ?? "",
                    word.Pronunciation ?? "",
                    word.DifficultyText,
                    word.PartOfSpeechText,
                    word.Category ?? ""
                );
            }
        }

        // Event handlers for buttons
        private async void AddStudentBtn_Click(object? sender, EventArgs e)
        {
            var addStudentForm = new AddStudentForm();
            if (addStudentForm.ShowDialog() == DialogResult.OK)
            {
                await LoadStudentsAsync();
            }
        }

        private void EditStudentBtn_Click(object? sender, EventArgs e)
        {
            if (studentsGrid.SelectedRows.Count > 0)
            {
                // Implementation for editing student
                MessageBox.Show("Öğrenci düzenleme özelliği yakında eklenecek.", "Bilgi");
            }
        }

        private void DeleteStudentBtn_Click(object? sender, EventArgs e)
        {
            if (studentsGrid.SelectedRows.Count > 0)
            {
                var result = MessageBox.Show("Seçili öğrenciyi silmek istediğinizden emin misiniz?",
                    "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Implementation for deleting student
                    MessageBox.Show("Öğrenci silme özelliği yakında eklenecek.", "Bilgi");
                }
            }
        }

        private void AddCourseBtn_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Kurs ekleme özelliği yakında eklenecek.", "Bilgi");
        }

        private void EditCourseBtn_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Kurs düzenleme özelliği yakında eklenecek.", "Bilgi");
        }

        private void DeleteCourseBtn_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Kurs silme özelliği yakında eklenecek.", "Bilgi");
        }

        private void AddVocabularyBtn_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Kelime ekleme özelliği yakında eklenecek.", "Bilgi");
        }

        private void EditVocabularyBtn_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Kelime düzenleme özelliği yakında eklenecek.", "Bilgi");
        }

        private void DeleteVocabularyBtn_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Kelime silme özelliği yakında eklenecek.", "Bilgi");
        }
    }
}
