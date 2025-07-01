using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnglishAutomationApp.Data;
using EnglishAutomationApp.Models;

namespace EnglishAutomationApp.Views
{
    public partial class EditUserForm : Form
    {
        private TextBox emailTextBox = null!;
        private TextBox firstNameTextBox = null!;
        private TextBox lastNameTextBox = null!;
        private ComboBox roleComboBox = null!;
        private CheckBox isActiveCheckBox = null!;
        private CheckBox isAdminCheckBox = null!;
        private Button saveButton = null!;
        private Button cancelButton = null!;
        private Label messageLabel = null!;
        
        private User userToEdit;
        
        // Language support
        private bool isEnglish = true;

        public EditUserForm(User user, bool english = true)
        {
            userToEdit = user;
            isEnglish = english;
            InitializeComponent();
            LoadUserData();
            UpdateLanguage();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Edit User - Admin Panel";
            this.Size = new Size(450, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(15, 23, 42); // Slate-900

            // Title
            var titleLabel = new Label();
            titleLabel.Text = isEnglish ? "✏️ Edit User" : "✏️ Kullanıcı Düzenle";
            titleLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Location = new Point(20, 20);
            titleLabel.Size = new Size(410, 40);

            // Main Panel
            var mainPanel = new Panel();
            mainPanel.BackColor = Color.FromArgb(30, 41, 59); // Slate-800
            mainPanel.Location = new Point(20, 70);
            mainPanel.Size = new Size(410, 370);

            int yPos = 20;

            // Email
            var emailLabel = CreateLabel(isEnglish ? "EMAIL ADDRESS" : "E-POSTA ADRESİ", yPos);
            emailTextBox = CreateTextBox(yPos + 20);
            yPos += 60;

            // First Name
            var firstNameLabel = CreateLabel(isEnglish ? "FIRST NAME" : "AD", yPos);
            firstNameTextBox = CreateTextBox(yPos + 20);
            yPos += 60;

            // Last Name
            var lastNameLabel = CreateLabel(isEnglish ? "LAST NAME" : "SOYAD", yPos);
            lastNameTextBox = CreateTextBox(yPos + 20);
            yPos += 60;

            // Role
            var roleLabel = CreateLabel(isEnglish ? "ROLE" : "ROL", yPos);
            roleComboBox = new ComboBox();
            roleComboBox.Items.AddRange(new[] { "User", "Admin" });
            roleComboBox.SelectedIndex = 0;
            roleComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            roleComboBox.Font = new Font("Segoe UI", 11);
            roleComboBox.BackColor = Color.FromArgb(51, 65, 85);
            roleComboBox.ForeColor = Color.White;
            roleComboBox.Location = new Point(30, yPos + 20);
            roleComboBox.Size = new Size(350, 30);
            yPos += 60;

            // Checkboxes
            isActiveCheckBox = new CheckBox();
            isActiveCheckBox.Text = isEnglish ? "Active User" : "Aktif Kullanıcı";
            isActiveCheckBox.Font = new Font("Segoe UI", 11);
            isActiveCheckBox.ForeColor = Color.White;
            isActiveCheckBox.Location = new Point(30, yPos);
            isActiveCheckBox.Size = new Size(150, 25);

            isAdminCheckBox = new CheckBox();
            isAdminCheckBox.Text = isEnglish ? "Admin Privileges" : "Yönetici Yetkisi";
            isAdminCheckBox.Font = new Font("Segoe UI", 11);
            isAdminCheckBox.ForeColor = Color.White;
            isAdminCheckBox.Location = new Point(200, yPos);
            isAdminCheckBox.Size = new Size(150, 25);
            yPos += 40;

            // Buttons
            saveButton = new Button();
            saveButton.Text = isEnglish ? "💾 Save Changes" : "💾 Değişiklikleri Kaydet";
            saveButton.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            saveButton.BackColor = Color.FromArgb(34, 197, 94); // Green-500
            saveButton.ForeColor = Color.White;
            saveButton.FlatStyle = FlatStyle.Flat;
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.Location = new Point(30, yPos);
            saveButton.Size = new Size(150, 40);
            saveButton.Cursor = Cursors.Hand;
            saveButton.Click += SaveButton_Click;

            cancelButton = new Button();
            cancelButton.Text = isEnglish ? "❌ Cancel" : "❌ İptal";
            cancelButton.Font = new Font("Segoe UI", 11);
            cancelButton.BackColor = Color.Transparent;
            cancelButton.ForeColor = Color.FromArgb(148, 163, 184);
            cancelButton.FlatStyle = FlatStyle.Flat;
            cancelButton.FlatAppearance.BorderSize = 1;
            cancelButton.FlatAppearance.BorderColor = Color.FromArgb(148, 163, 184);
            cancelButton.Location = new Point(200, yPos);
            cancelButton.Size = new Size(150, 40);
            cancelButton.Cursor = Cursors.Hand;
            cancelButton.Click += CancelButton_Click;
            yPos += 50;

            // Message Label
            messageLabel = new Label();
            messageLabel.Font = new Font("Segoe UI", 10);
            messageLabel.Location = new Point(30, yPos);
            messageLabel.Size = new Size(350, 30);
            messageLabel.TextAlign = ContentAlignment.MiddleCenter;

            // Add controls to main panel
            mainPanel.Controls.Add(emailLabel);
            mainPanel.Controls.Add(emailTextBox);
            mainPanel.Controls.Add(firstNameLabel);
            mainPanel.Controls.Add(firstNameTextBox);
            mainPanel.Controls.Add(lastNameLabel);
            mainPanel.Controls.Add(lastNameTextBox);
            mainPanel.Controls.Add(roleLabel);
            mainPanel.Controls.Add(roleComboBox);
            mainPanel.Controls.Add(isActiveCheckBox);
            mainPanel.Controls.Add(isAdminCheckBox);
            mainPanel.Controls.Add(saveButton);
            mainPanel.Controls.Add(cancelButton);
            mainPanel.Controls.Add(messageLabel);

            // Add controls to form
            this.Controls.Add(titleLabel);
            this.Controls.Add(mainPanel);

            this.ResumeLayout(false);
        }

        private Label CreateLabel(string text, int y)
        {
            var label = new Label();
            label.Text = text;
            label.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            label.ForeColor = Color.FromArgb(148, 163, 184);
            label.Location = new Point(30, y);
            label.Size = new Size(350, 20);
            return label;
        }

        private TextBox CreateTextBox(int y)
        {
            var textBox = new TextBox();
            textBox.Font = new Font("Segoe UI", 11);
            textBox.Location = new Point(30, y);
            textBox.Size = new Size(350, 30);
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.BackColor = Color.FromArgb(51, 65, 85);
            textBox.ForeColor = Color.White;
            return textBox;
        }

        private void LoadUserData()
        {
            emailTextBox.Text = userToEdit.Email;
            firstNameTextBox.Text = userToEdit.FirstName ?? "";
            lastNameTextBox.Text = userToEdit.LastName ?? "";
            roleComboBox.SelectedItem = userToEdit.Role;
            isActiveCheckBox.Checked = userToEdit.IsActive;
            isAdminCheckBox.Checked = userToEdit.IsAdmin;
        }

        private async void SaveButton_Click(object? sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            saveButton.Enabled = false;
            saveButton.Text = isEnglish ? "Saving..." : "Kaydediliyor...";

            try
            {
                // Update user object
                userToEdit.Email = emailTextBox.Text.Trim().ToLower();
                userToEdit.FirstName = string.IsNullOrWhiteSpace(firstNameTextBox.Text) ? null : firstNameTextBox.Text.Trim();
                userToEdit.LastName = string.IsNullOrWhiteSpace(lastNameTextBox.Text) ? null : lastNameTextBox.Text.Trim();
                userToEdit.Role = roleComboBox.SelectedItem?.ToString() ?? "User";
                userToEdit.IsActive = isActiveCheckBox.Checked;
                userToEdit.IsAdmin = isAdminCheckBox.Checked;

                var success = await AccessDatabaseHelper.UpdateUserAsync(userToEdit);

                if (success)
                {
                    var message = isEnglish ? "User updated successfully!" : "Kullanıcı başarıyla güncellendi!";
                    ShowMessage(message, Color.FromArgb(34, 197, 94));
                    await Task.Delay(1500);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    var message = isEnglish ? "Failed to update user. Please try again." : "Kullanıcı güncellenemedi. Lütfen tekrar deneyin.";
                    ShowMessage(message, Color.FromArgb(248, 113, 113));
                }
            }
            catch (Exception ex)
            {
                var message = isEnglish ? $"Error: {ex.Message}" : $"Hata: {ex.Message}";
                ShowMessage(message, Color.FromArgb(248, 113, 113));
            }
            finally
            {
                saveButton.Enabled = true;
                saveButton.Text = isEnglish ? "💾 Save Changes" : "💾 Değişiklikleri Kaydet";
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(emailTextBox.Text))
            {
                var message = isEnglish ? "Email address is required." : "E-posta adresi gereklidir.";
                ShowMessage(message, Color.FromArgb(248, 113, 113));
                emailTextBox.Focus();
                return false;
            }

            return true;
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ShowMessage(string message, Color color)
        {
            messageLabel.Text = message;
            messageLabel.ForeColor = color;
        }

        private void UpdateLanguage()
        {
            if (isEnglish)
            {
                // English
                this.Text = "Edit User - Admin Panel";
            }
            else
            {
                // Turkish
                this.Text = "Kullanıcı Düzenle - Yönetici Paneli";
            }
        }
    }
}
