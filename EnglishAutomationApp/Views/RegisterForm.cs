using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnglishAutomationApp.Services;

namespace EnglishAutomationApp.Views
{
    public partial class RegisterForm : Form
    {
        private TextBox firstNameTextBox;
        private TextBox lastNameTextBox;
        private TextBox emailTextBox;
        private TextBox passwordTextBox;
        private TextBox confirmPasswordTextBox;
        private Button registerButton;
        private Button cancelButton;
        private Label messageLabel;
        private Label titleLabel;

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Register - English Automation Platform";
            this.Size = new Size(450, 650);
            this.StartPosition = FormStartPosition.CenterOwner;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Title Label
            titleLabel = new Label();
            titleLabel.Text = "🚀 Create New Account";
            titleLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(102, 126, 234);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Location = new Point(50, 30);
            titleLabel.Size = new Size(350, 40);

            // Main Panel
            var mainPanel = new Panel();
            mainPanel.BackColor = Color.White;
            mainPanel.Location = new Point(50, 90);
            mainPanel.Size = new Size(350, 480);

            // First Name
            var firstNameLabel = new Label();
            firstNameLabel.Text = "First Name:";
            firstNameLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            firstNameLabel.Location = new Point(30, 30);
            firstNameLabel.Size = new Size(100, 25);

            firstNameTextBox = new TextBox();
            firstNameTextBox.Font = new Font("Segoe UI", 12);
            firstNameTextBox.Location = new Point(30, 55);
            firstNameTextBox.Size = new Size(290, 30);

            // Last Name
            var lastNameLabel = new Label();
            lastNameLabel.Text = "Last Name:";
            lastNameLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lastNameLabel.Location = new Point(30, 95);
            lastNameLabel.Size = new Size(100, 25);

            lastNameTextBox = new TextBox();
            lastNameTextBox.Font = new Font("Segoe UI", 12);
            lastNameTextBox.Location = new Point(30, 120);
            lastNameTextBox.Size = new Size(290, 30);

            // Email
            var emailLabel = new Label();
            emailLabel.Text = "Email:";
            emailLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            emailLabel.Location = new Point(30, 160);
            emailLabel.Size = new Size(100, 25);

            emailTextBox = new TextBox();
            emailTextBox.Font = new Font("Segoe UI", 12);
            emailTextBox.Location = new Point(30, 185);
            emailTextBox.Size = new Size(290, 30);

            // Password
            var passwordLabel = new Label();
            passwordLabel.Text = "Password:";
            passwordLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            passwordLabel.Location = new Point(30, 225);
            passwordLabel.Size = new Size(100, 25);

            passwordTextBox = new TextBox();
            passwordTextBox.Font = new Font("Segoe UI", 12);
            passwordTextBox.Location = new Point(30, 250);
            passwordTextBox.Size = new Size(290, 30);
            passwordTextBox.UseSystemPasswordChar = true;

            // Confirm Password
            var confirmPasswordLabel = new Label();
            confirmPasswordLabel.Text = "Confirm Password:";
            confirmPasswordLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            confirmPasswordLabel.Location = new Point(30, 290);
            confirmPasswordLabel.Size = new Size(150, 25);

            confirmPasswordTextBox = new TextBox();
            confirmPasswordTextBox.Font = new Font("Segoe UI", 12);
            confirmPasswordTextBox.Location = new Point(30, 315);
            confirmPasswordTextBox.Size = new Size(290, 30);
            confirmPasswordTextBox.UseSystemPasswordChar = true;

            // Register Button
            registerButton = new Button();
            registerButton.Text = "🎯 Create Account";
            registerButton.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            registerButton.BackColor = Color.FromArgb(102, 126, 234);
            registerButton.ForeColor = Color.White;
            registerButton.FlatStyle = FlatStyle.Flat;
            registerButton.FlatAppearance.BorderSize = 0;
            registerButton.Location = new Point(30, 365);
            registerButton.Size = new Size(140, 40);
            registerButton.Click += RegisterButton_Click;

            // Cancel Button
            cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.Font = new Font("Segoe UI", 10);
            cancelButton.BackColor = Color.Gray;
            cancelButton.ForeColor = Color.White;
            cancelButton.FlatStyle = FlatStyle.Flat;
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Location = new Point(180, 365);
            cancelButton.Size = new Size(140, 40);
            cancelButton.Click += CancelButton_Click;

            // Message Label
            messageLabel = new Label();
            messageLabel.Font = new Font("Segoe UI", 9);
            messageLabel.Location = new Point(30, 415);
            messageLabel.Size = new Size(290, 50);
            messageLabel.TextAlign = ContentAlignment.MiddleCenter;

            // Add controls to main panel
            mainPanel.Controls.Add(firstNameLabel);
            mainPanel.Controls.Add(firstNameTextBox);
            mainPanel.Controls.Add(lastNameLabel);
            mainPanel.Controls.Add(lastNameTextBox);
            mainPanel.Controls.Add(emailLabel);
            mainPanel.Controls.Add(emailTextBox);
            mainPanel.Controls.Add(passwordLabel);
            mainPanel.Controls.Add(passwordTextBox);
            mainPanel.Controls.Add(confirmPasswordLabel);
            mainPanel.Controls.Add(confirmPasswordTextBox);
            mainPanel.Controls.Add(registerButton);
            mainPanel.Controls.Add(cancelButton);
            mainPanel.Controls.Add(messageLabel);

            // Add controls to form
            this.Controls.Add(titleLabel);
            this.Controls.Add(mainPanel);

            this.ResumeLayout(false);
        }

        private async void RegisterButton_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            registerButton.Enabled = false;
            registerButton.Text = "Creating account...";

            try
            {
                var result = await AuthenticationService.RegisterAsync(
                    emailTextBox.Text.Trim(),
                    passwordTextBox.Text,
                    firstNameTextBox.Text.Trim(),
                    lastNameTextBox.Text.Trim());

                if (result.Success)
                {
                    ShowMessage(result.Message, Color.Green);
                    
                    // Wait 2 seconds and close form
                    await Task.Delay(2000);
                    this.Close();
                }
                else
                {
                    ShowMessage(result.Message, Color.Red);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Unexpected error: {ex.Message}", Color.Red);
            }
            finally
            {
                registerButton.Enabled = true;
                registerButton.Text = "🎯 Create Account";
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool ValidateForm()
        {
            // First name check
            if (string.IsNullOrWhiteSpace(firstNameTextBox.Text))
            {
                ShowMessage("Please enter your first name.", Color.Red);
                firstNameTextBox.Focus();
                return false;
            }

            // Last name check
            if (string.IsNullOrWhiteSpace(lastNameTextBox.Text))
            {
                ShowMessage("Please enter your last name.", Color.Red);
                lastNameTextBox.Focus();
                return false;
            }

            // Email check
            if (string.IsNullOrWhiteSpace(emailTextBox.Text))
            {
                ShowMessage("Please enter your email address.", Color.Red);
                emailTextBox.Focus();
                return false;
            }

            if (!IsValidEmail(emailTextBox.Text))
            {
                ShowMessage("Please enter a valid email address.", Color.Red);
                emailTextBox.Focus();
                return false;
            }

            // Password check
            if (string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                ShowMessage("Please enter your password.", Color.Red);
                passwordTextBox.Focus();
                return false;
            }

            if (passwordTextBox.Text.Length < 6)
            {
                ShowMessage("Password must be at least 6 characters long.", Color.Red);
                passwordTextBox.Focus();
                return false;
            }

            // Confirm password check
            if (passwordTextBox.Text != confirmPasswordTextBox.Text)
            {
                ShowMessage("Passwords do not match.", Color.Red);
                confirmPasswordTextBox.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return emailRegex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private void ShowMessage(string message, Color color)
        {
            messageLabel.Text = message;
            messageLabel.ForeColor = color;
        }
    }
}
