using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnglishAutomationApp.Data;
using EnglishAutomationApp.Models;

namespace EnglishAutomationApp.Forms
{
    public partial class AddStudentForm : Form
    {
        private TextBox emailTextBox = null!;
        private TextBox firstNameTextBox = null!;
        private TextBox lastNameTextBox = null!;
        private TextBox passwordTextBox = null!;
        private TextBox confirmPasswordTextBox = null!;
        private CheckBox isActiveCheckBox = null!;
        private Button saveButton = null!;
        private Button cancelButton = null!;

        public AddStudentForm()
        {
            InitializeComponent();
            SetupModernDesign();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Yeni Öğrenci Ekle";
            this.Size = new Size(450, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.Font = new Font("Segoe UI", 10F);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Create main panel
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30),
                BackColor = Color.Transparent
            };

            // Title label
            var titleLabel = new Label
            {
                Text = "Yeni Öğrenci Bilgileri",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(0, 0)
            };

            // Email
            var emailLabel = CreateLabel("E-posta Adresi:");
            emailTextBox = CreateTextBox();
            emailLabel.Location = new Point(0, 60);
            emailTextBox.Location = new Point(0, 85);

            // First Name
            var firstNameLabel = CreateLabel("Ad:");
            firstNameTextBox = CreateTextBox();
            firstNameLabel.Location = new Point(0, 130);
            firstNameTextBox.Location = new Point(0, 155);

            // Last Name
            var lastNameLabel = CreateLabel("Soyad:");
            lastNameTextBox = CreateTextBox();
            lastNameLabel.Location = new Point(0, 200);
            lastNameTextBox.Location = new Point(0, 225);

            // Password
            var passwordLabel = CreateLabel("Şifre:");
            passwordTextBox = CreateTextBox();
            passwordTextBox.UseSystemPasswordChar = true;
            passwordLabel.Location = new Point(0, 270);
            passwordTextBox.Location = new Point(0, 295);

            // Confirm Password
            var confirmPasswordLabel = CreateLabel("Şifre Tekrar:");
            confirmPasswordTextBox = CreateTextBox();
            confirmPasswordTextBox.UseSystemPasswordChar = true;
            confirmPasswordLabel.Location = new Point(0, 340);
            confirmPasswordTextBox.Location = new Point(0, 365);

            // Is Active checkbox
            isActiveCheckBox = new CheckBox
            {
                Text = "Aktif kullanıcı",
                Checked = true,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(0, 410),
                AutoSize = true
            };

            // Buttons
            var buttonPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Bottom,
                BackColor = Color.Transparent
            };

            saveButton = CreateModernButton("Kaydet", Color.FromArgb(34, 197, 94));
            cancelButton = CreateModernButton("İptal", Color.FromArgb(107, 114, 128));

            saveButton.Location = new Point(0, 5);
            cancelButton.Location = new Point(140, 5);

            saveButton.Click += SaveButton_Click;
            cancelButton.Click += CancelButton_Click;

            buttonPanel.Controls.AddRange(new Control[] { saveButton, cancelButton });

            // Add all controls to main panel
            mainPanel.Controls.AddRange(new Control[] {
                titleLabel,
                emailLabel, emailTextBox,
                firstNameLabel, firstNameTextBox,
                lastNameLabel, lastNameTextBox,
                passwordLabel, passwordTextBox,
                confirmPasswordLabel, confirmPasswordTextBox,
                isActiveCheckBox
            });

            this.Controls.Add(mainPanel);
            this.Controls.Add(buttonPanel);
            this.ResumeLayout(false);
        }

        private Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                AutoSize = true
            };
        }

        private TextBox CreateTextBox()
        {
            return new TextBox
            {
                Size = new Size(350, 30),
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(51, 65, 85)
            };
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
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backgroundColor, 0.1f);
            button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backgroundColor, 0.1f);

            return button;
        }

        private void SetupModernDesign()
        {
            // Additional modern styling can be added here
        }

        private async void SaveButton_Click(object? sender, EventArgs e)
        {
            try
            {
                // Validate input
                if (!ValidateInput())
                    return;

                // Create new user
                var newUser = new User
                {
                    Email = emailTextBox.Text.Trim(),
                    FirstName = firstNameTextBox.Text.Trim(),
                    LastName = lastNameTextBox.Text.Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordTextBox.Text),
                    Role = "User",
                    IsActive = isActiveCheckBox.Checked,
                    CreatedDate = DateTime.Now
                };

                // Save to database
                var success = await AccessDatabaseHelper.CreateUserAsync(newUser);

                if (success)
                {
                    MessageBox.Show("Öğrenci başarıyla eklendi!", "Başarılı",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Öğrenci eklenirken bir hata oluştu.", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool ValidateInput()
        {
            // Email validation
            if (string.IsNullOrWhiteSpace(emailTextBox.Text))
            {
                MessageBox.Show("E-posta adresi gereklidir.", "Doğrulama Hatası", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                emailTextBox.Focus();
                return false;
            }

            if (!IsValidEmail(emailTextBox.Text))
            {
                MessageBox.Show("Geçerli bir e-posta adresi giriniz.", "Doğrulama Hatası", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                emailTextBox.Focus();
                return false;
            }

            // First name validation
            if (string.IsNullOrWhiteSpace(firstNameTextBox.Text))
            {
                MessageBox.Show("Ad gereklidir.", "Doğrulama Hatası", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                firstNameTextBox.Focus();
                return false;
            }

            // Last name validation
            if (string.IsNullOrWhiteSpace(lastNameTextBox.Text))
            {
                MessageBox.Show("Soyad gereklidir.", "Doğrulama Hatası", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lastNameTextBox.Focus();
                return false;
            }

            // Password validation
            if (string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                MessageBox.Show("Şifre gereklidir.", "Doğrulama Hatası", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                passwordTextBox.Focus();
                return false;
            }

            if (passwordTextBox.Text.Length < 6)
            {
                MessageBox.Show("Şifre en az 6 karakter olmalıdır.", "Doğrulama Hatası", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                passwordTextBox.Focus();
                return false;
            }

            // Confirm password validation
            if (passwordTextBox.Text != confirmPasswordTextBox.Text)
            {
                MessageBox.Show("Şifreler eşleşmiyor.", "Doğrulama Hatası", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                confirmPasswordTextBox.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
